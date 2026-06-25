 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity  //定义class类Player
{
    [Header("Attack details")]
    public Vector2[] attackMovement;// public float[] attackMovement;//数组
    public float counterAttackDuration = .2f;


    public bool isBusy { get; private set; }

    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;
    public float swordReturnImpact;
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("Dash info")]
    //[SerializeField] private float dashCooldown;
    //private float dashUsageTimer;
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;
    public float dashDir { get; private set; }


    public SkillMananger skill {  get; private set; }

    public GameObject sword{ get; private set; }//如果想允许子类修改// 用 protected set    public GameObject sword { get; protected set; }   

    #region States
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }

    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlide {  get; private set; }
    public PlayerWallJumpState wallJump { get; private set; }

    public PlayerDashState dashState { get; private set; }
    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }

    public PlayerAimSwordState aimSword {  get; private set; }
    public PlayerCatchSwordState catchSword { get; private set; }
    public PlayerBlackholeState blackHole { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this,stateMachine,"Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this,stateMachine,"WallSlide");
        wallJump = new PlayerWallJumpState(this,stateMachine,"Jump");

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack"); //对应animator里的bool变量Attack
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSword = new PlayerAimSwordState(this, stateMachine, "AimSword");//类，状态机，animator动画参数名
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHole = new PlayerBlackholeState(this, stateMachine, "Jump");

        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }
    protected override void Start()
    {
        base.Start();
   
        skill=SkillMananger.instance;

        stateMachine.Initialize(idleState);         //初始状态是idleState

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }



    protected override void Update()
    { 
        base .Update();

        stateMachine.currentState.Update();

        

        CheckForDashInput();
        //timer-= Time.deltaTime;  //deltatime两帧之间的时间

        if (Input.GetKeyDown(KeyCode.F))
            skill.crystal.CanUseSkill();  // skill=SkillMananger.instance;

        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            //Debug.Log("use flask");
            Inventory.instance.UseFlask();
        }

    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        //base.SlowEntityBy(_slowPercentage, _slowDuration);
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        jumpForce =jumpForce * (1 - _slowPercentage);
        dashSpeed=dashSpeed * (1 - _slowPercentage);
        anim.speed=anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed=defaultDashSpeed;
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        //sword = null; // 关键：回收时立刻置空，不然有可能极少的概率出现第二把无法回收的剑。
        stateMachine.ChangeState(catchSword);
       Destroy(sword); //Destroy(sword, 2f);            // 2秒后销毁 sword

    }

    //public void ExitBlackHoleAbility()
    //{
    //    stateMachine.ChangeState(airState);
    //}

    public IEnumerator BusyFor(float _seconds)//IEnumerator（枚举器）coroutine协程
    {
        isBusy = true;// 1. 先设置忙碌
        // Debug.Log("IS BUSY");
       
        yield return new WaitForSeconds(_seconds);//yield return = 按暂停        WaitForSeconds(2) = 暂停2秒         2秒后自动继续播放后面的内容

        // Debug.Log("NOT BUSY");
        isBusy = false;// 3. 2秒后才执行这句
    }

    public void AnimationTrigger()=>stateMachine.currentState.AnimationFinishTrigger();

    private void CheckForDashInput()
    {
        if (IsWallDetected())          //贴墙的时候不许冲刺
            return;

        //dashUsageTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillMananger.instance.dash.CanUseSkill()) 
        {
           // dashUsageTimer = dashCooldown;
            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
                dashDir = facingDir;

            stateMachine.ChangeState(dashState);
        }
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }





}
