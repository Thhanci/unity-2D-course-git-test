using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAttackState : EnemyState   //攻击
{
    private Enemy_Skeleton enemy;

    public SkeletonAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy=_enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeAttacked=Time.time;
    }

    public override void Update()
    {
        base.Update();

        enemy.SetZeroVelocity();//停下攻击，但是被击飞的时候，skeleton攻击有霸体 故而回去修改public void SetZeroVelocity() => rb.velocity = new Vector2(0, 0);
        //或修改这里：if（isKnocked），如果被击中，return退出,中断Attackstate攻击状态

        //Debug.Log(triggerCalled);

        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }
}
