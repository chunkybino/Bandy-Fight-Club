using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerFunc : MonoBehaviour
{
    public Rigidbody2D rb;
    public PlayerStatus playerStatus;
    public PlayerAttack playerAttack;
    public PlayerHitbox playerHitbox;
    public PlayerAnimation playerAnimation;
    public PlayerInputs inputs;
    public GroundChecker groundCheck;

    public int team {get{return playerStatus.team.team;}}

    public ICharacterFunc characterFunc;

    public bool allowControl;

    //side movement
    [SerializeField] float movementSpeed = 14;
    [HideInInspector] public int sideInputDirection;
    [HideInInspector] public int verticalInputDirection;

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
    public UnityEvent onGroundLand;

    [SerializeField] float groundedLeewayTime = 0.15f;
    float groundedLeewayTimer;

    //facing
    public int facing = 1;

    //jumping
    [SerializeField] float jumpHeight = 8;
    [SerializeField] float airJumpHeight = 4;
    [SerializeField] int totalAirJumps = 1;
    int airJumpsLeft = 0;
    [SerializeField] float jumpReleaseFallFactor = 0.85f;
    public bool enableJumpReleaseFall;

    [SerializeField] float jumpBufferTime = 0.1f;
    float jumpBufferTimer;

    [SerializeField] GameObject canJumpAnimator;

    //dodge
    public bool isDodging;
    public bool isDodgingBurst;
    public bool doDodgeMovement;
    [SerializeField] float dodgeBurstSpeed = 40;
    [SerializeField] float dodgeLowerSpeed = 10;
    [SerializeField] float dodgeBurstTime = 0.15f;
    [SerializeField] float dodgeTime = 0.3f;
    public Vector2 dodgeDirection;
    [SerializeField] float dodgeAcceleration = 0.3f;

    [SerializeField] float dodgeBufferTime = 0.1f;
    float dodgeBufferTimer;

    [SerializeField] float timeDodging;
    [SerializeField] float timeSinceDodge;

    [SerializeField] float dodgeIntangibilityTime = 0.15f;
    public bool isDodgeIntangible {get{return isDodging && timeDodging < dodgeIntangibilityTime;}}

    [SerializeField] GameObject canDodgeAnimator;

    [SerializeField] float attackOutOfDodgeTime = 0.3f; //how fast we can attack out of a dodge

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

    [SerializeField] GameObject gravMultAnimator;

    //platform
    [SerializeField] float downPlatformBufferTime = 0.1f;
    float downPlatformBufferTimer;

    //blocking
    public bool isBlocking;
    public float timeBlocking;
    [SerializeField] float perfBlockTime = 0.20f;
    public bool isPerfBlocking;

    public bool isLateBlocking;
    [SerializeField] float lateBlockTime = 0.7f;
    [SerializeField] float lateBlockDisadvantageMult = 1.5f;

    [SerializeField] float blockStunTimer;
    public bool isBlockStun;

    [SerializeField] ParticleSystem blockParticles;

    //hit stun
    [SerializeField] float stunTimeNeeded;
    [SerializeField] float currentTimeStunned;
    public bool isStun;
    [SerializeField] float stunSpeedMult = 0.4f;
    [SerializeField] float stunAccelerationMult = 0.2f;

    public bool canBlockOutOfStun;
    [SerializeField] float stunBlockTimeOffest = 0.15f; //we can stun block this musch ealier than we would ussually become un stunned
    [SerializeField] float stunBlockDisadvantageMult = 0f; //affects of disadvantaeg are multipluied by this when calculating for blocking out of stun
    public bool stunBlocking;

    [SerializeField] float blockBreakStunTime = 1f;

    public float disadvantage = 0;
    [SerializeField] float disadvantageHealRate = 0.05f;
    [SerializeField] float perfBlockDisadvantageHeal = 0.05f;
    public float maxDisavantage = 0.5f;
    public float disadvantageStunMult = 1; //when we add disadvantage to stun time, multply by this

    float disadvantageHealDelayTimer;
    [SerializeField] float disadvantageHealDelay = 0.75f;
    
    public void Update()
    {
        //side inputs
        sideInputDirection = 0;
        if (inputs.leftHeld) sideInputDirection--;
        if (inputs.rightHeld) sideInputDirection++;

        //vertical inputs
        verticalInputDirection = 0;
        if (inputs.upHeld) verticalInputDirection++;
        if (inputs.downHeld) verticalInputDirection--;

        bool canChangeFacing = true;
        if (playerAttack.isAttacking && playerAttack.currentAttack.lockFacing) canChangeFacing = false;
        if (canChangeFacing && sideInputDirection != 0) facing = Mathf.RoundToInt(Mathf.Sign(sideInputDirection)); 

        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x)*facing,transform.localScale.y,transform.localScale.z);

        //ground leeway
        if (grounded) groundedLeewayTimer = groundedLeewayTime;
        UFunc.TickTimer(ref groundedLeewayTimer);

        //stun
        UFunc.TickTimer(ref disadvantageHealDelayTimer);
        if (disadvantage > 0 && disadvantageHealDelayTimer == 0) disadvantage = Mathf.Max(disadvantage - (disadvantageHealRate * Time.deltaTime), 0);

        if (isStun)
        {
            isStun = currentTimeStunned < stunTimeNeeded + (disadvantage * disadvantageStunMult);

            canBlockOutOfStun = currentTimeStunned > stunTimeNeeded + (disadvantage*disadvantageStunMult*stunBlockDisadvantageMult) - stunBlockTimeOffest; //doesnt factor in disadvantage;
            currentTimeStunned += Time.deltaTime;
        }
        stunBlocking = isStun && isBlocking;

        //jumping
        UFunc.TickTimer(ref jumpBufferTimer);
        if (inputs.jumpPressed) jumpBufferTimer = jumpBufferTime;

        if (grounded) airJumpsLeft = totalAirJumps;

        bool canJump = GetIsActionable();
        if (playerAttack.isAttacking || isBlockStun)
        {
            if (!canDodgeAnimator.activeSelf) canJump = false;
        }

        if (canJump && jumpBufferTimer > 0)
        {
            if (!isDodging)
            {
                if (grounded || groundedLeewayTimer > 0)
                {
                    JumpToHeight(jumpHeight);
                    groundedLeewayTimer = 0;
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
                if (grounded || groundedLeewayTimer > 0)
                {
                    DoDashJump();
                    groundedLeewayTimer = 0;
                }
            }
        }

        //dodging
        UFunc.TickTimer(ref dodgeBufferTimer);
        if (inputs.dodgePressed) dodgeBufferTimer = dodgeBufferTime;

        bool canDodge = GetIsActionable();
        if (isDodging) canDodge = false;
        if (playerAttack.isAttacking || isBlocking)
        {
            if (!canDodgeAnimator.activeSelf) canDodge = false;
        }

        bool canBlock = true;
        if (isStun && !canBlockOutOfStun) canBlock = false;
        if (isDodging || playerAttack.isAttacking) canBlock = false;
        
        if (dodgeBufferTimer > 0)
        {
            if (sideInputDirection != 0) //dodge
            {
                if (canDodge)
                {
                    dodgeBufferTimer = 0;
                    int direction = sideInputDirection;
                    if (direction == 0) direction = facing;
                    StartDodge(new Vector2(direction,0));
                }
            }
            else //block
            {
                if (canBlock)
                {
                    dodgeBufferTimer = 0;
                    StartBlock();
                }
            }
        }

        if (isDodging)
        {
            timeDodging += Time.deltaTime;
        }
        else
        {
            timeDodging = 0;
        }
        timeSinceDodge += Time.deltaTime;

        //more blocking
        isBlockStun = blockStunTimer > 0;

        if (isBlocking)
        {
            if (!isBlockStun)
            {
                if (!inputs.dodgeHeld) 
                {
                    EndBlock();
                }
            }
            else
            {
                UFunc.TickTimer(ref blockStunTimer);
            }

            isPerfBlocking = timeBlocking < perfBlockTime;
            isLateBlocking = timeBlocking > lateBlockTime;
            timeBlocking += Time.deltaTime;
        }
        else
        {
            isPerfBlocking = false;
            blockStunTimer = 0;
        }

        //droping through platform stuff
        UFunc.TickTimer(ref downPlatformBufferTimer);
        if (inputs.downPressed) downPlatformBufferTimer = downPlatformBufferTime;
        if (downPlatformBufferTimer > 0 && grounded)
        {
            playerHitbox.DropThroughPlatform();
        }
    }

    public void FixedUpdate()
    {
        if (!grounded && groundCheck.grounded) GroundLand(); //check if we land on the ground
        grounded = groundCheck.grounded;

        //momentum
        float xVel = rb.velocity.x;
        float yVel = rb.velocity.y;

        float targetSideSpeed = movementSpeed * sideInputDirection;
        if (isStun) targetSideSpeed *= stunSpeedMult;
        if (playerAttack.isAttacking)
        {
            targetSideSpeed *= playerAttack.attackingSpeedMult; //ussually 0, stop us from moving during attack
        }
        if (isDodging)
        {
            targetSideSpeed = dodgeLowerSpeed * dodgeDirection.x;
        }

        //total grav
        currentGrav = baseGravity;
        currentGrav *= currentGravMult;
        if (gravMultAnimator.activeSelf) currentGrav *= gravMultAnimator.transform.localPosition.x;
        foreach (float g in gravMults)
        {
            currentGrav *= g;
        }

        bool movementDodging = isDodging || doDodgeMovement; //dodging for the sake of movement
        if (movementDodging)
        {
            DodgeMovement();
        }
        else
        {
            HandleAcceleration();
            MaintainingSpeed();

            VerticalMovementStuff();
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
            if (movementDodging) mAccel = dodgeAcceleration;
            if (isStun) mAccel *= stunAccelerationMult;

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

            if (inDirectionOfSpeed && maintainSpeed != 0 && Mathf.Abs(maintainSpeed) > Mathf.Abs(xVel)) //if our input direction and maintain direciton are the same
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

            if (timeSinceDodge <= dodgeBurstTime)
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

    void GroundLand()
    {
        onGroundLand?.Invoke();
    }

    void StartDodge(Vector2 direction)
    {
        isDodging = true;
        isDodgingBurst = true;

        dodgeDirection = direction;

        timeSinceDodge = 0;

        CancelInvoke("EndDodge");
        CancelInvoke("EndDodgeBurst");
        Invoke("EndDodgeBurst", dodgeBurstTime);

        doDodgeMovement = false;
        CancelInvoke("EndDodgeMovement");
        Invoke("EndDodgeMovement", dodgeTime);
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
        isDodgingBurst = false;
        isDodging = false;

        CancelInvoke("EndDodge");
        CancelInvoke("EndDodgeBurst");
    }
    public void EndDodgeMovement()
    {
        doDodgeMovement = false;

        CancelInvoke("EndDodgeMovement");
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

    public void StartBlock()
    {
        isBlocking = true;
        timeBlocking = 0;
        playerAnimation.DoNewBlockAnim();
    }
    public void EndBlock()
    {
        isBlocking = false;
    }

    public void GetBlockStuned(float time)
    {
        blockStunTimer = time;
    }
    public void GetStunned(float time)
    {
        stunTimeNeeded = time;
        isStun = true;
        currentTimeStunned = 0;
    }

    public void GetBlockBroken()
    {
        EndBlock();
        GetStunned(blockBreakStunTime);
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
        if (isDodging) doDodgeMovement = true;
        EndDodge();
    }

    public void OnBlock(float advantageDamage)
    {
        if (isPerfBlocking) //perfect
        {
            //disadvantage = Mathf.Max(disadvantage - perfBlockDisadvantageHeal, 0);
        }
        else
        {
            if (isLateBlocking) advantageDamage *= lateBlockDisadvantageMult;
            AddDisadvantage(advantageDamage);
            disadvantageHealDelayTimer = disadvantageHealDelay;

            if (disadvantage >= maxDisavantage)
            {
                GetBlockBroken();
            }
        }

        Instantiate(blockParticles, transform.position, Quaternion.identity);
    }

    public void AddDisadvantage(float amount)
    {
        disadvantage = Mathf.Min(disadvantage + amount, maxDisavantage);
    }

    public void DisableJumpReleaseFall()
    {
        enableJumpReleaseFall = false;
    }

    public void UpdateFacing()
    {
        if (sideInputDirection != 0) facing = Mathf.RoundToInt(Mathf.Sign(sideInputDirection)); 
    }

    //other script functions, for overriding i guess
    public virtual bool GetIsActionable()
    {
        return !isStun;
    }

    public virtual bool GetCanAttack()
    {
        bool canAttack = !playerAttack.isAttacking;
        if (!GetIsActionable()) canAttack = false;
        if (isDodging && timeDodging < attackOutOfDodgeTime) canAttack = false;
        if (isBlocking) canAttack = false;
        return canAttack;
    }

    public virtual bool GetIsIntangible()
    {
        return isDodgeIntangible;
    }

    public virtual bool GetDisableEntityCollision()
    {
        bool disable = isDodging;
        if (playerAttack.isAttacking) disable = false;
        return disable;
    }

    public virtual bool GetFallThroughPlatform()
    {
        return false;
    }

    public virtual bool GetDoAnimation()
    {
        return !playerAttack.isAttacking;
    }
}
