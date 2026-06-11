using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;

    protected Rigidbody2D rb;

    protected float xInput;
    protected float yInput;
    protected string animBoolName;

    protected float stateTimer;         //默认是0，不断减帧时间deltatime
    protected bool triggerCalled;   //动画完成标志

    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)//构造体
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);
        rb=player.rb;
        triggerCalled = false;   //默认未完成
    }
    public virtual void Update() 
    {
        stateTimer-= Time.deltaTime;
        //Debug.Log(stateTimer);
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        player.anim.SetFloat("yVelocity", rb.velocity.y);
    }

    public virtual void Exit() 
    {
        player.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    { 
        triggerCalled = true;   //调用函数标志：标记为完成动画
    }

}
