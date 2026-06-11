using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        player.skill.sword.DotsActive(true);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .03f);//退出瞄准状态的时候等0.03秒在移动   //默认2f
    }

    public override void Update()
    {
        base.Update();

        //5. Improving sword throwing(1)
        ////额外添加以下1行（瞄准前仍在移动会平移） 
        //rb.velocity = Vector2.zero;
        player.SetZeroVelocity();  //调用类player里的函数使得速度归零


        if (Input.GetKeyUp(KeyCode.Mouse1))
            stateMachine.ChangeState(player.idleState);

        Vector2 mousePosition =Camera.main.ScreenToWorldPoint(Input.mousePosition);    //Input.mousePosition     /   屏幕坐标（像素），和玩家位置不在同一个坐标系   //世界坐标  /    玩家位置的坐标系，需要转换才能比较大小

        if (player.transform.position.x > mousePosition.x && player.facingDir == 1)
            player.Flip();
        else if (player.transform.position.x < mousePosition.x && player.facingDir == -1)
            player.Flip();

        ////异或
        //if ((player.transform.position.x > mousePosition.x) ^ (player.facingDir == -1))
        //    player.Flip();
    }
}
