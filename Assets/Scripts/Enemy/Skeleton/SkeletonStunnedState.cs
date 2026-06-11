using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonStunnedState : EnemyState
{
    private Enemy_Skeleton enemy;
    public SkeletonStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.fx.InvokeRepeating("RedColorBlink", 0, .1f);//(方法名字,开始时间,间隔时间)

        stateTimer = enemy.stunDuration;

        rb.velocity=new Vector2(-enemy.facingDir * enemy.stunDirection.x, enemy.stunDirection.y);
        //enemy.SetVelocity(-enemy.facingDir*enemy.stunDirection.x,enemy.stunDirection.y);   //set Velocity的x速度和filp捆绑了，直接用vector2就不会反转
    }

    public override void Exit()
    {
        base.Exit();

        enemy.fx.Invoke("CancelColorChange", 0); //Invoke("方法", 2f) 2秒后执行一次
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);
    }
}
