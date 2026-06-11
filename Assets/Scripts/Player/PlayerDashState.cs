using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)//animBoolName为unity里animator的Dash
    {

    }

    public override void Enter()
    {
        base.Enter();

        player.skill.clone.CreateCloneOnDashStart();
        //player.skill.clone.CreateClone(player.transform, Vector3.zero); //SkillMananger.instance.clone.CreateClone(player.transform);  //这里别忘了给之前的dash技能创造的残影也加上offset参数，不然会报错

        stateTimer = player.dashDuration;
    }

    public override void Exit()
    {
        base.Exit();

        player.skill.clone.CreateCloneOnDashOver();
        player.SetVelocity(0, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (!player.IsGroundDetected() && player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlide);

        player.SetVelocity(player.dashSpeed * player.dashDir,0);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
    }
}
