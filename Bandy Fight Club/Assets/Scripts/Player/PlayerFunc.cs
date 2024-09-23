using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFunc : MonoBehaviour
{
    public Rigidbody2D rb;
    public PlayerStatus playerStatus;
    public PlayerAttack playerAttack;
    public PlayerInputs inputs;
    public GroundChecker groundCheck;

    //side movement
    [SerializeField] float movementSpeed = 14;
    int sideInputDirection;

    [SerializeField] float multAccleration = 0.5f;
    [SerializeField] float flatAccleration = 0.3f;
    float accelerationCurve = 100; //for making acceleration values easier to deal with
    [SerializeField] float currentMultAcceleration;
    [SerializeField] float currentFlatAcceleration;
    float currentAccelerationMult = 1;

    [SerializeField] GameObject accelerationAnimator;

    public float maintainSpeed;
    [SerializeField] float reachMaxSpeedForTimeTimer;

    //ground
    public bool grounded;

    //facing
    public int facing = 1;

    //jumping
    [SerializeField] float jumpHeight = 8;
    [SerializeField] float airJumpHeight = 4;
    [SerializeField] int totalAirJumps = 1;
    int airJumpsLeft = 0;
    [SerializeField] float jumpReleaseFallFactor = 0.85f;
    bool enableJumpReleaseFall;

    [SerializeField] float jumpBufferTime = 0.1f;
    float jumpBufferTimer;

    [SerializeField] GameObject canJumpAnimator;

    //dodge
    public bool isDodging;
    public bool isDodgingBurst;
    [SerializeField] float dodgeBurstSpeed = 40;
    [SerializeField] float dodgeLowerSpeed = 10;
    [SerializeField] float dodgeBurstTime = 0.15f;
    [SerializeField] float dodgeTime = 0.3f;
    public Vector2 dodgeDirection;
    [SerializeField] float dodgeAcceleration = 0.3f;

    [SerializeField] float dodgeBufferTime = 0.1f;
    float dodgeBufferTimer;

    [SerializeField] GameObject canDodgeAnimator;

    //jump dash
    [SerializeField] float dashJumpHeight = 3.5f;
    [SerializeField] float dashJumpSpeed = 24;

    [SerializeField] float dashJumpAccel = 0.35f;
    [SerializeField] float dashJumpAccelTime = 0.4f;

    [SerializeField] float dashJumpGrav = 0.8f;
    [SerializeField] float dashJumpGravTime = 0.6f;

    //grav
    [SerializeField] float baseGravity = 90;
    [SerializeField] float currentGrav;
    float currentGravMult = 1;
    List<float> gravMults = new List<float>();

    void Update()
    {
        //side inputs
        sideInputDirection = 0;
        if (inputs.leftHeld) sideInputDirection--;
        if (inputs.rightHeld) sideInputDirection++;

        if (sideInputDirection != 0) facing = Mathf.RoundToInt(Mathf.Sign(sideInputDirection)); 
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x)*facing,transform.localScale.y,transform.localScale.z);

        //jumping
        UFunc.TickTimer(ref jumpBufferTimer);
        if (inputs.jumpPressed) jumpBufferTimer = jumpBufferTime;

        if (grounded) airJumpsLeft = totalAirJumps;

        bool canJump = true;
        if (playerAttack.isAttacking && !canDodgeAnimator.activeSelf) canJump = false;

        if (canJump && jumpBufferTimer > 0)
        {
            if (!isDodging)
            {
                if (grounded)
                {
                    JumpToHeight(jumpHeight);
                }
                else
                {
                    if (airJumpsLeft > 0)
                    {
                        JumpToHeight(airJumpHeight);
                        airJumpsLeft--;
                    }
                }
            }
            else
            {
                if (grounded) DoDashJump();
            }
        }

        //dodging
        UFunc.TickTimer(ref dodgeBufferTimer);
        if (inputs.dodgePressed) dodgeBufferTimer = dodgeBufferTime;

        bool canDodge = true;
        if (isDodging) canDodge = false;
        if (playerAttack.isAttacking && !canDodgeAnimator.activeSelf) canDodge = false;
        
        if (canDodge && dodgeBufferTimer > 0)
        {
            dodgeBufferTimer = 0;
            int direction = sideInputDirection;
            if (direction == 0) direction = facing;
            StartDodge(new Vector2(direction,0));
        }
    }

    void FixedUpdate()
    {
        grounded = groundCheck.grounded;

        //momentum
        float xVel = rb.velocity.x;
        float yVel = rb.velocity.y;

        float targetSideSpeed = movementSpeed * sideInputDirection;
        if (playerAttack.isAttacking)
        {
            targetSideSpeed = 0;
        }
        if (isDodging)
        {
            targetSideSpeed = dodgeLowerSpeed * dodgeDirection.x;
        }

        //total grav
        currentGrav = baseGravity;
        currentGrav *= currentGravMult;
        foreach (float g in gravMults)
        {
            currentGrav *= g;
        }

        if (!isDodging)
        {
            HandleAcceleration();
            MaintainingSpeed();

            VerticalMovementStuff();
        }
        else
        {
            DodgeMovement();
        }

        xVel = UFunc.RoundToDecimal(xVel, 2); //rounding the xVel to the nearest hundredth 
        if (Mathf.Abs(xVel) < 0.05f) xVel = 0;
        rb.velocity = new Vector2(xVel, yVel);  

        //funcs
        void HandleAcceleration()
        {
            float thisMultAcceleration = 1;
            float thisFlatAcceleration = 1;

            float mAccel = multAccleration;
            if (isDodging) mAccel = dodgeAcceleration;

            thisMultAcceleration = accelerationCurve * (1 - Mathf.Pow(1 - (mAccel * (1 - Mathf.Pow(1 - (1 / accelerationCurve), 1 / Time.fixedDeltaTime))), Time.fixedDeltaTime));
            thisFlatAcceleration = UFunc.FloatToDirection(targetSideSpeed - rb.velocity.x) * Mathf.Min(flatAccleration * movementSpeed, Mathf.Abs(targetSideSpeed - rb.velocity.x)) * Time.fixedDeltaTime; //add flat acceleration, make sure we dont overshoot target speed

            //acceleration mults
            float mult = CalculateAccelerationMultOverride();//currentAccelerationMult;
            if (accelerationAnimator) if (accelerationAnimator.activeSelf) mult *= accelerationAnimator.transform.localPosition.x;
            thisMultAcceleration *= mult;
            thisFlatAcceleration *= mult;

            xVel = rb.velocity.x + thisFlatAcceleration; //add flat acceleration, make sure we dont overshoot target speed

            xVel += ((targetSideSpeed - rb.velocity.x) * thisMultAcceleration); //add mult acceleration
        } 
        void MaintainingSpeed()
        {
            bool inDirectionOfSpeed = UFunc.Sign(sideInputDirection) == UFunc.Sign(maintainSpeed);

            if (inDirectionOfSpeed && maintainSpeed != 0) //if our input direction and maintain direciton are the same
            {
                xVel = maintainSpeed;
            }
            
            if ((grounded || !inDirectionOfSpeed) && rb.velocity.y < 5) //dont dont reset if we're rising
            {
                maintainSpeed = 0;
            }
        }
        void VerticalMovementStuff()
        {
            if (!inputs.jumpHeld && rb.velocity.y > 0 && enableJumpReleaseFall) //when we release jump during a jump, cut upwards speed every frame
            {
                yVel = Mathf.Clamp(rb.velocity.y * Mathf.Pow(jumpReleaseFallFactor, Time.fixedDeltaTime * 50), 0, rb.velocity.y);
            }

            yVel -= currentGrav * Time.fixedDeltaTime;
        }
        void DodgeMovement()
        {
            yVel = 0; 

            if (isDodgingBurst)
            {
                xVel = dodgeBurstSpeed * dodgeDirection.x;
            }
            else
            {
                HandleAcceleration();
            }
        }
    }

    void JumpToHeight(float height)
    {
        Jump(Mathf.Sqrt(2*baseGravity*height) + (baseGravity*0.5f*Time.fixedDeltaTime));
    }
    void JumpToHeight(float height, float gravMult)
    {
        Jump(Mathf.Sqrt(2*baseGravity*gravMult*height) + (baseGravity*gravMult*0.5f*Time.fixedDeltaTime));
    }
    void Jump(float force)
    {
        jumpBufferTimer = 0;
        enableJumpReleaseFall = true;
        rb.velocity = new Vector2(rb.velocity.x, force);
    }

    void StartDodge(Vector2 direction)
    {
        isDodging = true;
        isDodgingBurst = true;

        dodgeDirection = direction;

        CancelInvoke("EndDodge");
        CancelInvoke("EndDodgeBurst");
        Invoke("EndDodgeBurst", dodgeBurstTime);
    }
    void EndDodgeBurst()
    {
        isDodgingBurst = false;

        CancelInvoke("EndDodge");
        CancelInvoke("EndDodgeBurst");
        Invoke("EndDodge", dodgeTime-dodgeBurstTime);
    }   
    public void EndDodge()
    {
        isDodging = false;

        CancelInvoke("EndDodge");
        CancelInvoke("EndDodgeBurst");
    }

    void DoDashJump()
    {
        jumpBufferTimer = 0;
        EndDodge();

        JumpToHeight(dashJumpHeight, dashJumpGrav);

        maintainSpeed = dashJumpSpeed * sideInputDirection;
        rb.velocity = new Vector2(maintainSpeed, rb.velocity.y);

        DoAccelerationMult(dashJumpAccel, dashJumpAccelTime);
        DoGravMult(dashJumpGrav, dashJumpGravTime);
    }

    public void DoAccelerationMult(float mult, float time)
    {
        currentAccelerationMult = mult;

        CancelInvoke("EndAccelerationMult");
        Invoke("EndAccelerationMult", time);
    }
    void EndAccelerationMult()
    {
        CancelInvoke("EndAccelerationMult");
        currentAccelerationMult = 1;
    }

    public virtual float CalculateAccelerationMultOverride()
    {
        return CalculateAccelerationMult();
    }
    public float CalculateAccelerationMult()
    {
        float mult = currentAccelerationMult;
        if (accelerationAnimator) if (accelerationAnimator.activeSelf) mult *= accelerationAnimator.transform.localPosition.x;
        return mult;
    }

    void DoGravMult(float mult, float time)
    {
        currentGravMult = mult;

        CancelInvoke("EndGravMult");
        Invoke("EndGravMult", time);
    }
    void EndGravMult()
    {
        CancelInvoke("EndGravMult");
        currentGravMult = 1;
    }

    public virtual void OnAttack()
    {
        //for overrides and stuff i guess
    }
}
