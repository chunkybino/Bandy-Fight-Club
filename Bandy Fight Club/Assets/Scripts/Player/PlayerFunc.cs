using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerFunc : MonoBehaviour
{
    public GameManager gameManager;
    public Rigidbody2D rb;
    public PlayerStatus playerStatus;
    public PlayerAttack playerAttack;
    public PlayerHitbox playerHitbox;
    public PlayerAnimation playerAnimation;
    public PlayerInputs inputs;
    public GroundChecker groundCheck;
    public KnockbackHandler knockbackHandler;
    public SquashStretcher squasher;

    public int team {get{return playerStatus.team.team;}}

    public ICharacterFunc characterFunc;

    public bool allowControl;

    [SerializeField] List<Vector2> lastFrameVels = new List<Vector2>();
    public float highestRecentYVel {get{
        float high = rb.velocity.y;
        for (int i = 0; i < lastFrameVels.Count; i++)
        {
            if (lastFrameVels[i].y > high) high = lastFrameVels[i].y;
        }
        return high;
    }}
    public float lowestRecentYVel {get {
        float low = rb.velocity.y;
        for (int i = 0; i < lastFrameVels.Count; i++)
        {
            if (lastFrameVels[i].y < low) low = lastFrameVels[i].y;
        }
        return low;
    }}
    [SerializeField] int lastFrameVelCount = 5; //number of frame we keep track of

    [SerializeField] Vector2 lastFramePos;

    //side movement
    [SerializeField] float movementSpeed = 14;
    [HideInInspector] public int sideInputDirection;
    [HideInInspector] public int verticalInputDirection;

    public Vector2 inputVector;

    [SerializeField] float multAccleration = 0.5f;
    [SerializeField] float flatAccleration = 0.3f;
    float accelerationCurve = 100; //for making acceleration values easier to deal with
    [SerializeField] float currentMultAcceleration;
    [SerializeField] float currentFlatAcceleration;
    float currentAccelerationMult = 1;

    [SerializeField] GameObject accelerationAnimator;

    public float maintainSpeed;
    [SerializeField] float reachMaxSpeedForTimeTimer;

    public float targetSideSpeed;

    //ground
    public bool grounded;
    public UnityEvent onGroundLand;

    [SerializeField] SquashData groundLandSquashData;
    [SerializeField] DoubleInverseLerpData groundLandSquashMinMaxVelAmp; //amplitude we get from htting the ground with high velocity, input is y vel, output is amp

    [SerializeField] SquashData deadGroundLandSquashData;
    //[SerializeField] float deadGroundLandBounceForceMult = 0.2f;
    //[SerializeField] float deadGroundLandBounceForceSubtract = 5;

    [SerializeField] float groundedLeewayTime = 0.15f;
    float groundedLeewayTimer;

    //facing
    public int facing = 1;
    int facingBuffer = 1;

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

    [SerializeField] SquashData jumpSquashData;

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
    float dodgeReleaseBufferTimer;
    [SerializeField] float dodgeTapWindow = 0.35f; //how fast we have t release the button after pressing it to dodge
    float dodgeTapTimer;

    [SerializeField] float dodgeCooldownTimer;
    [SerializeField] float currentDodgeCooldownTime;
    [SerializeField] Vector2 dodgeCooldownMinMaxTime = new Vector2(0.25f, 1.5f);
    [SerializeField] float dodgeCooldownAddPerDodge = 0.5f;
    [SerializeField] float dodgeCooldownDecaySpeed = 0.5f;

    [SerializeField] float timeDodging;
    [SerializeField] float timeSinceDodge;

    [SerializeField] float dodgeIntangibilityTime = 0.15f;
    public bool isDodgeIntangible {get{return isDodging && timeDodging < dodgeIntangibilityTime;}}

    [SerializeField] GameObject canDodgeAnimator;

    [SerializeField] float attackOutOfDodgeTime = 0.3f; //how fast we can attack out of a dodge

    [SerializeField] SquashData dodgeSquashData;

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

    [SerializeField] bool isBlockCounter;
    public bool isBlockCounterSuccess {get {return isBlockCounter && blockCounterTimer <= blockCounterTime;}}
    [SerializeField] float blockCounterTimer;
    [SerializeField] float blockCounterTime = 0.15f;
    [SerializeField] float blockCounterLateTime = 0.25f; //how long we're vulnreble for after
    [SerializeField] float blockCounterStunMult = 0.4f; //get block stunnned fr less time if we block counter
    [SerializeField] float blockCounterDisadvantageHeal = 0.1f;

    public float perfBlockDisadvantageReturn = 0.1f; //deal this much disadvantage when attacked
    public float blockCounterDisadvantageReturn = 0.2f;

    public float perfBlockAttackStun = 0.15f;
    public float blockCounterAttackStun = 0.4f;
    public Knockback blockCounterReturnKnockback;

    public Vector2 blockKnockbackMult = new Vector2(0.6f, 0.7f);
    public Vector2 blockCounterKnockbackMult = new Vector2(0.3f, 0.5f);

    [SerializeField] float airBlockSpeedMult = 0.5f;
    [SerializeField] float groundBlockSpeedMult = 0; 

    [SerializeField] float blockSkidTime = 0.2f;
    float blockSkidTimer;
    [SerializeField] float blockSkidMaxFallSpeed = 8f;

    [SerializeField] SquashData blockCounterStartSquash; //squash for when we press the button
    [SerializeField] ParticleSystem blockCounterStartParticles;

    [SerializeField] ParticleSystem blockParticles;
    [SerializeField] ParticleSystem perfBlockParticles;
    [SerializeField] ParticleSystem blockCounterParticles;

    public float blockCounterHitStop = 0.5f;

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
    public float maxDisavantage = 0.5f;
    public float disadvantageStunMult = 1; //when we add disadvantage to stun time, multply by this

    float disadvantageHealDelayTimer;
    [SerializeField] float disadvantageHealDelay = 0.75f;

    //visual
    public PlayerMeshParent playerMesh;

    //other garbage
    //for when you dodge through people, the person dodge thorugh you references these
    public float boundingTopBound {get{return playerHitbox.boundingTopBound;}}
    public float boundingBottomBound {get{return playerHitbox.boundingBottomBound;}}
    
    public void Start()
    {
        gameManager = GameManager.singleton;

        facingBuffer = facing;
    }

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

        inputVector = new Vector2(sideInputDirection, verticalInputDirection);

        bool canChangeFacing = true;
        if (playerAttack.isAttacking && playerAttack.currentAttack.lockFacing) canChangeFacing = false;
        if (isDodging) canChangeFacing = false;
        if (canChangeFacing && sideInputDirection != 0) facing = Mathf.RoundToInt(Mathf.Sign(sideInputDirection)); 
        if (sideInputDirection != 0) facingBuffer = Mathf.RoundToInt(Mathf.Sign(sideInputDirection)); 

        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x)*facing,transform.localScale.y,transform.localScale.z);
        if (playerMesh) playerMesh.UpdateFacing();

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
            bool jumpDash = false;
            if (isDodging) jumpDash = true;
            if (isBlocking && dodgeTapTimer > 0) jumpDash = true;

            if (!jumpDash)
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
                    dodgeReleaseBufferTimer = 0;
                    dodgeTapTimer = 0;
                }
            }
        }

        //dodging
        UFunc.TickTimer(ref dodgeBufferTimer);
        if (inputs.dodgePressed) dodgeBufferTimer = dodgeBufferTime;
        UFunc.TickTimer(ref dodgeReleaseBufferTimer);
        if (inputs.dodgeReleased) dodgeReleaseBufferTimer = dodgeBufferTime;

        UFunc.TickTimer(ref dodgeTapTimer);
        if (inputs.dodgePressed) dodgeTapTimer = dodgeTapWindow;

        bool canDodge = GetIsActionable();
        if (isDodging) canDodge = false;
        if (playerAttack.isAttacking || isBlocking)
        {
            if (!canDodgeAnimator.activeSelf) canDodge = false;
        }
        if (dodgeCooldownTimer > 0) canDodge = false;

        if (!isDodging) UFunc.TickTimer(ref dodgeCooldownTimer);
        if (canDodge) currentDodgeCooldownTime = Mathf.Max(currentDodgeCooldownTime-(dodgeCooldownDecaySpeed*Time.deltaTime),0);

        bool canBlock = true;
        //if (isStun && !canBlockOutOfStun) canBlock = false; ///stun block is not a thing anymore
        if (isDodging || playerAttack.isAttacking) canBlock = false;
        
        if (dodgeBufferTimer > 0)
        {
            /*if (sideInputDirection != 0) //dodge
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
            {*/
            if (canBlock && !isBlocking)
            {
                dodgeBufferTimer = 0;
                StartBlock();
            }
            //}
        }
        if (dodgeReleaseBufferTimer > 0 && dodgeTapTimer > 0)
        {
            if (sideInputDirection != 0 && canDodge) //dodge
            {
                EndBlock();
                dodgeReleaseBufferTimer = 0;
                dodgeTapTimer = 0;
                int direction = sideInputDirection;
                if (direction == 0) direction = facing;
                StartDodge(new Vector2(direction,0));
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
            if (inputs.attackPressed)
            {
                playerAttack.attackBufferTimer = 0;
                DoBlockCounter();
            }

            if (isBlockCounter)
            {
                blockCounterTimer += Time.deltaTime;

                if (blockCounterTimer > blockCounterTime+blockCounterLateTime) isBlockCounter = false;
            }

            if (!isBlockStun && !isBlockCounter)
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

            if (isBlockCounter)
            {
                if (blockCounterTimer < blockCounterTime)
                {
                    isPerfBlocking = true;
                    isLateBlocking = false;
                }
                else //late block counter
                {
                    isPerfBlocking = false;
                    isLateBlocking = true;
                }
            }

            if (isPerfBlocking) knockbackHandler.knockbackResistanceMult = blockKnockbackMult;
            if (isBlockCounterSuccess) knockbackHandler.knockbackResistanceMult = blockCounterKnockbackMult;
        }
        else
        {
            isPerfBlocking = false;
            isBlockCounter = false;
            blockStunTimer = 0;

            knockbackHandler.knockbackResistanceMult = new Vector2(1,1);
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

        targetSideSpeed = movementSpeed * sideInputDirection;
        if (isStun) targetSideSpeed *= stunSpeedMult;
        
        UFunc.TickTimerFixed(ref blockSkidTimer);
        if (isBlocking)
        {
            if (blockSkidTimer == 0)
            {
                if (grounded)
                {
                    targetSideSpeed *= groundBlockSpeedMult; //ground
                }
                else
                {
                    targetSideSpeed *= airBlockSpeedMult; //air
                }
            }
            else //during skid
            {
                yVel = Mathf.Max(yVel, -blockSkidMaxFallSpeed);
            }
        }
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
            CheckForDodgeThroughPlayer();
        }
        else
        {
            HandleAcceleration();
            MaintainingSpeed();

            VerticalMovementStuff();
        }

        xVel = UFunc.RoundToDecimal(xVel, 2); //rounding the xVel to the nearest hundredth 
        if (Mathf.Abs(xVel) < 0.05f) xVel = 0;

        //lastFrameVel = rb.velocity;
        lastFrameVels.Insert(0, rb.velocity);
        if (lastFrameVels.Count > lastFrameVelCount) lastFrameVels.RemoveAt(lastFrameVels.Count-1);

        //last frame pos
        lastFramePos = transform.position;

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

        squasher.DoSquash(jumpSquashData);
        playerAnimation.DoParticles("jump");
    }

    void GroundLand()
    {
        onGroundLand?.Invoke();

        if (!playerStatus.isDead)
        {
            SquashData data = groundLandSquashData.CloneYourself();

            float vel = lowestRecentYVel;//rb.velocity.y;
            //if (lastFrameVels.Count >= 2) vel = lastFrameVels[1].y;
            data.amplitude = groundLandSquashData.amplitude * groundLandSquashMinMaxVelAmp.Lerp(Mathf.Abs(vel));

            squasher.DoSquash(data);
        }
        else //if we dead
        {
            /*float vel = lowestRecentYVel;
            vel = Mathf.Max(Mathf.Abs(vel)-deadGroundLandBounceForceSubtract, 0) * deadGroundLandBounceForceMult;
            rb.velocity = new Vector2(rb.velocity.x, vel);*/

            squasher.DoSquash(deadGroundLandSquashData);
        }

        playerAnimation.OnLand();
    }

    void StartDodge(Vector2 direction)
    {
        isDodging = true;
        isDodgingBurst = true;

        dodgeDirection = direction;

        timeSinceDodge = 0;

        dodgeCooldownTimer = currentDodgeCooldownTime;
        currentDodgeCooldownTime = Mathf.Clamp(currentDodgeCooldownTime+dodgeCooldownAddPerDodge, dodgeCooldownMinMaxTime.x,dodgeCooldownMinMaxTime.y);

        CancelInvoke("EndDodge");
        CancelInvoke("EndDodgeBurst");
        Invoke("EndDodgeBurst", dodgeBurstTime);

        doDodgeMovement = false;
        CancelInvoke("EndDodgeMovement");
        Invoke("EndDodgeMovement", dodgeTime);

        squasher.DoSquash(dodgeSquashData);
        playerAnimation.ExitActionAnim();
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

    void CheckForDodgeThroughPlayer()
    {
        List<Vector2> playerPos = new List<Vector2>();
        List<Vector2> playerVerticalBounds = new List<Vector2>(); //top and botto of the players bounding box
        for (int i = 0; i < gameManager.players.Count; i++) 
        {
            if (gameManager.players[i] == playerStatus) continue;
            playerPos.Add(gameManager.players[i].transform.position);
            playerVerticalBounds.Add(new Vector2(gameManager.players[i].playerFunc.boundingBottomBound, gameManager.players[i].playerFunc.boundingTopBound));
        }

        for (int i = 0; i < playerPos.Count; i++)
        {
            bool lastFrameSide = lastFramePos.x <= playerPos[i].x; //check if ere on different sides of the player this and last frame
            bool currentSide = transform.position.x <= playerPos[i].x;

            bool inBounds = transform.position.y >= playerVerticalBounds[i].x && transform.position.y <= playerVerticalBounds[i].y; //between the top and bottom of the player bounding

            if (lastFrameSide == currentSide || !inBounds) continue;

            DoDodgeTurnAround();
        }
    }
    void DoDodgeTurnAround()
    {
        if (playerAttack.isAttacking) return;
        facing *= -1;
        facingBuffer = facing;
    }

    public void StartBlock()
    {
        isBlocking = true;
        timeBlocking = 0;
        playerAnimation.DoNewBlockAnim();
        blockSkidTimer = blockSkidTime;
    }
    public void EndBlock()
    {
        isBlocking = false;
    }

    public void DoBlockCounter()
    {
        isBlockCounter = true;
        blockCounterTimer = 0;

        squasher.DoSquash(blockCounterStartSquash);
        blockCounterStartParticles.Play();
    }
    public void SuccessfulBlockCounter()
    {
        isBlockCounter = false;
        blockCounterTimer = 0;

        blockStunTimer *= blockCounterStunMult;
 
        disadvantage = Mathf.Max(disadvantage - blockCounterDisadvantageHeal, 0);

        Instantiate(blockCounterParticles, transform.position, Quaternion.identity);
    }

    public void GetBlockStuned(float time)
    {
        blockStunTimer = time;
    }
    public virtual void GetStunned(float time)
    {
        stunTimeNeeded = time;
        isStun = true;
        currentTimeStunned = 0;
        playerAttack.CancelAttack();
        EndBlock();
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

        UpdateFacing();
    }
    public void OnAttackEnd()
    {
        facing = facingBuffer;
    }

    public void OnBlock(float advantageDamage)
    {
        ParticleSystem particle = blockParticles;

        if (isBlockCounterSuccess)
        {
            SuccessfulBlockCounter(); 
        }

        if (isPerfBlocking) //perfect
        {
            particle = perfBlockParticles;
        }
        else
        {
            if (isLateBlocking) advantageDamage *= lateBlockDisadvantageMult;
            AddDisadvantage(advantageDamage);

            if (disadvantage >= maxDisavantage)
            {
                GetBlockBroken();
            }
        }

        Instantiate(particle, transform.position, Quaternion.identity);
    }

    public void AddDisadvantage(float amount)
    {
        disadvantageHealDelayTimer = disadvantageHealDelay;
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
