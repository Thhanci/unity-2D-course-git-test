using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{

    private int comboCounter;

    private float lastTimeAttacked;
    private float comboWindow = 2;
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        xInput = 0;//xInput = Input.GetAxisRaw("Horizontal");//(a better choice!)   //we need this to fix bug on attack direction

        if (comboCounter>2 || Time.time>=lastTimeAttacked+comboWindow) //Time.time>=lastTimeAttacked+comboWindow太久没攻击
            comboCounter = 0;

        player.anim.SetInteger("ComboCounter", comboCounter);   //player.anim.SetInteger("Animator里的参数名", 代码里的变量);
        //player.anim.speed = 1.2f;

        float attackDir =player.facingDir;

        if(xInput!=0)                //Attack's direction hot fix(攻击时，如果上一刻xInput=A，面向为D，则有概率向A攻击)[AD{同时摁，最后面向D}要摁的够快就会出现]:xInput重新获取一遍就好了，有bug是因为它的更新在上一次的攻击状态里
            attackDir =xInput;       //xInput的值的刷新是在update不是在enter里面，容易出bug


        player.SetVelocity(player.attackMovement[comboCounter].x *attackDir, player.attackMovement[comboCounter].y);

        stateTimer = .1f;
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .15f);
        //player.anim.speed = 1;

        comboCounter++;
        lastTimeAttacked = Time.time;
       
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            player.SetZeroVelocity();

        if(triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
