using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState 
{

    protected EnemyStateMachine stateMachine;
    protected Enemy enemyBase;
    protected Rigidbody2D rb;

    private string animBoolName;   // 这里存储着 "idle"之类的东西

    protected float stateTimer;
    protected bool triggerCalled;   //（Attackstate）标记完成攻击状态,攻击完成后回到Battlestate

    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        this.enemyBase = _enemyBase;//this指的是当前正在执行的这个状态对象   /this就是"正在工作的那个状态"  /空闲时：this = 空闲状态对象  /攻击时：this = 攻击状态对象  /受伤时：this = 受伤状态对象
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Enter()
    {
        triggerCalled = false;
        rb = enemyBase.rb;
        enemyBase.anim.SetBool(animBoolName, true);   //"idle" (animBoolName)就像是一个开关，告诉 Animator："现在要播放空闲动画了！"
    }
    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName, false);
        enemyBase.AssignLastAnimName(animBoolName);
    }

    public virtual void AnimationFinishTrigger()
    { 
        triggerCalled = true;
    }

}
