using UnityEngine;

public class PlayerCounterAttackState : PlayerState   //groundstate keycode.Q
{
    private bool canCreateClone;

    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        canCreateClone = true;
        stateTimer = player.counterAttackDuration;
        player.anim.SetBool("SuccessfulCounterAttack", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);//是OverlapCircleAll而不是OverCircle

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    stateTimer = 10;  //any value bigger than 1   /给你足够的时间完成弹反，并设置SuccessfulCounterAttack为true，退出该状态
                    //计时器大是为了以免反击成功，却因为计时结束退出了反击攻击动画
                    player.anim.SetBool("SuccessfulCounterAttack", true);

                    if (canCreateClone) 
                    {
                        canCreateClone=false;
                        player.skill.clone.CreateCloneOnCounterAttack(hit.transform);
                    }
                }

            }

        }
        if (stateTimer < 0 || triggerCalled)
            stateMachine.ChangeState(player.idleState);


    }
}
