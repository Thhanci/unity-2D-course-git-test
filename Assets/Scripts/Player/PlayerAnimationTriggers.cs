using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();//private 人类 一个人 => GetComponentInParent<人类>();

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);//是OverlapCircleAll而不是OverCircle
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(0, 0), 1);//Collider2D[] colliders = Physics2D.OverlapCircle(player.attackCheck.position, player.attackCheckRadius);



        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null) 
            {
                EnemyStats _target=hit.GetComponent<EnemyStats>();


                player.stats.DoDamage(_target);

                //hit.GetComponent<Enemy>().Damage();
                /*
                hit.GetComponent<CharacterStats>().TakeDamage(player.stats.damage.GetValue());
                Debug.Log(player.stats.damage.GetValue());
                */
            }
        }

    }

    private void ThrowSword()
    {
        SkillMananger.instance.sword.CreateSword();
    }

}
