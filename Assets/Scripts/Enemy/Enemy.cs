using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] protected LayerMask whatIsPlayer;

    [Header("Stunned info")]
    public float stunDuration;   //眩晕时间
    public Vector2 stunDirection;   //眩晕时速度
    protected bool canBeStunned;   //执行动画animation，执行到对应关键帧，执行事件，设置canBeStunned=true/false
    [SerializeField] protected GameObject counterImage;

    [Header("Move info")]
    public float moveSpeed;
    public float idleTime;
    public float battleTime;
    private float defaultMoveSpeed;

    [Header("Attack info")]
    public float attackDistance;
    public float attackCooldown;
    [HideInInspector] public float lastTimeAttacked;


    public EnemyStateMachine stateMachine { get; private set; }
    public string lastAnimBoolName { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();

        defaultMoveSpeed=moveSpeed;
    }

   protected override void Update()
    {
        base.Update();

        //Debug.Log(Time.time);

        stateMachine.currentState.Update();

        //Debug.Log(IsPlayerDetected().collider.gameObject.name+"I SEE");//当 IsPlayerDetected() 没有检测到玩家时，它会返回一个空的RaycastHit2D，此时collider.gameObject 会报空引用错误
        //Debug.Log(IsPlayerDetected().collider);
    }

    public virtual void AssignLastAnimName(string _animBoolName)=>lastAnimBoolName = _animBoolName;

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);

    }
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
    }

    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
        
    }

    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimeCoroutine(_duration));
    //Tip:如何使用protected 或Private,public 一个函数返回就行了

    protected virtual IEnumerator FreezeTimeCoroutine(float _seconds)
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTime(false);
    }


    #region Counter Attack Window
    public virtual void OpenCounterAttackWindow()//执行动画animation，执行到对应关键帧，执行事件，设置canBeStunned=true/false
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }
    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned= false;
        counterImage.SetActive(false);
    }
    #endregion


    public virtual bool CanBeStunned()  //可以弹反
    {
        if(canBeStunned)
        {
            CloseCounterAttackWindow();//结束红框提升
            return true;  //可以弹反
        }

        return false;
    }

    public virtual void AnimationFinishTrigger()=>stateMachine.currentState.AnimationFinishTrigger();
    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position,Vector2.right*facingDir,50,whatIsPlayer);



    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }
}
