using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime=.4f;
    private bool skillUsed;


    private float defaultGravity;
    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        defaultGravity = player.rb.gravityScale;

        skillUsed = false;
        stateTimer = flyTime;
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();

        player.rb.gravityScale = defaultGravity;
        player.fx.MakeTransparent(false);
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0,15);

        if (stateTimer < 0)
        { 
            rb.velocity = new Vector2(0,- .1f);

            if (!skillUsed) 
            {
                //Debug.Log("CAST BLACKHOLE");
                if(player.skill.blackhole.CanUseSkill())   //右下角骷髅头上添加了两个qte按键标记?   需要去黑洞预制体，将hotkeyprefab重新添加一下
                    skillUsed = true;
            }
        }

        //WE exit state in blackhole skills controller when all of the attacks are over
        if (player.skill.blackhole.SkillCompleted())
            stateMachine.ChangeState(player.airState);
    }
}
