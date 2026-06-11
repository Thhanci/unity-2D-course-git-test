using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBattleState : EnemyState   //寻敌   //attackDistance太短，会导致player在skeleton脸上不动时，skeleton砍几下就不砍了（player摩擦力太小，被skeleton推着在地上平移）（增加attackDistance）
{
    
    private Transform player;
    private Enemy_Skeleton enemy;//_enemyBase临时参数
    private int moveDir;
    
    
    public SkeletonBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform; //player = GameObject.Find("Player").transform;
        //Debug.Log("I'M in battle state");
    }
    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                //enemy.SetZeroVelocity();
                if (CanAttack())
                    stateMachine.ChangeState(enemy.attackState);
                //Debug.Log("I Attack");
                //enemy.SetZeroVelocity();
                return; //Tip：添加return以解决骷髅抽搐问题
            }
        }
        else
        { 
            if(stateTimer<0 || Vector2.Distance(player.transform.position,enemy.transform.position)>10)
                stateMachine.ChangeState(enemy.idleState);
        }

        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if(player.position.x<enemy.transform.position.x)
            moveDir= -1;

        
        enemy.SetVelocity(enemy.moveSpeed*moveDir,rb.velocity.y);

    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
        { 
            enemy.lastTimeAttacked = Time.time;
            return true;
        }
        //Debug.Log("Attack is on cooldown");
        return false;


    }
}
