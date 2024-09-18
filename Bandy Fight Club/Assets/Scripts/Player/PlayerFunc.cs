using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFunc : MonoBehaviour
{
    public Rigidbody2D rb;
    public PlayerStatus playerStatus;
    public PlayerInputs inputs;
    public GroundChecker groundCheck;

    //side movement
    [SerializeField] float movementSpeed = 10;
    int sideInputDirection;

    [SerializeField] float multAccleration = 0.5f;
    [SerializeField] float flatAccleration = 0.3f;
    float accelerationCurve = 100; //for making acceleration values easier to deal with

    //facing
    public int facing;

    //jumping
    [SerializeField] float jumpHeight = 2.5f;
    [SerializeField] int totalAirJumps = 1;
    int airJumpsLeft = 0;
    [SerializeField] float jumpReleaseFallFactor = 0.9f;
    bool enableJumpReleaseFall;

    [SerializeField] float jumpBufferTime = 0.1f;
    float jumpBufferTimer;

    //dodge
    [SerializeField] bool isDodging;
    [SerializeField] bool isDodgingBurst;
    [SerializeField] float dodgeBurstSpeed = 16;
    [SerializeField] float dodgeLowerSpeed = 9;
    [SerializeField] float dodgeBurstTime = 0.15f;
    [SerializeField] float dodgeTime = 0.5f;
    [SerializeField] Vector2 dodgeDirection;
    [SerializeField] float dodgeAcceleration = 0.1f;

    [SerializeField] float dodgeBufferTime = 0.1f;
    float dodgeBufferTimer;

    //grav
    [SerializeField] float baseGravity = 20;

    void Update()
    {
        bool grounded = groundCheck.grounded;

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

        if (jumpBufferTimer > 0)
        {
            if (grounded)
            {
                JumpToHeight(jumpHeight);
            }
            else
            {
                if (airJumpsLeft > 0) 
                {
                    JumpToHeight(jumpHeight);
                    airJumpsLeft--;
                }
            }
        }

        UFunc.TickTimer(ref dodgeBufferTimer);
        if (inputs.dodgePressed) dodgeBufferTimer = dodgeBufferTime;

        if (!isDodging)
        {
            if (dodgeBufferTimer > 0)
            {
                dodgeBufferTimer = 0;
                int direction = sideInputDirection;
                if (direction == 0) direction = facing;
                StartDodge(new Vector2(direction,0));
            }
        }
    }

    void FixedUpdate()
    {
        //momentum
        float xVel = rb.velocity.x;
        float yVel = rb.velocity.y;

        float targetSideSpeed = movementSpeed * sideInputDirection;
        if (isDodging)
        {
            targetSideSpeed = dodgeLowerSpeed * dodgeDirection.x;
        }

        if (!isDodging)
        {
            HandleAcceleration();
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

            //if (isDodging) thisFlatAcceleration = 0;

            xVel = rb.velocity.x + thisFlatAcceleration; //add flat acceleration, make sure we dont overshoot target speed
            xVel += ((targetSideSpeed - rb.velocity.x) * thisMultAcceleration); //add mult acceleration
        } 
        void VerticalMovementStuff()
        {
            if (!inputs.jumpHeld && rb.velocity.y > 0 && enableJumpReleaseFall) //when we release jump during a jump, cut upwards speed every frame
            {
                yVel = Mathf.Clamp(rb.velocity.y * Mathf.Pow(jumpReleaseFallFactor, Time.fixedDeltaTime * 50), 0, rb.velocity.y);
            }

            yVel -= baseGravity * Time.fixedDeltaTime;
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
                //xVel = (Mathf.Max(Mathf.Abs(xVel) - (dodgeSpeedDecaySpeed*Time.fixedDeltaTime), dodgeLowerSpeed)) * dodgeDirection.x;
            }
        }
    }

    void JumpToHeight(float height)
    {
        Jump(Mathf.Sqrt(2*baseGravity*height) + (baseGravity*0.5f*Time.fixedDeltaTime));
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
    void EndDodge()
    {
        isDodging = false;

        CancelInvoke("EndDodge");
        CancelInvoke("EndDodgeBurst");
    }
}
