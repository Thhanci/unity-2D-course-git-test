using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockStrikeController : MonoBehaviour
{
    [SerializeField] private CharacterStats targetStats;//因为攻击需要访问目标的血量、防御等属性。
    [SerializeField] private float speed;
    private int damage;

    private Animator anim;
    private bool triggered;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Setup(int _damage, CharacterStats _targetStats)
    { 
        damage=_damage;
        targetStats = _targetStats;
    }

    void Update()
    {
        if (!targetStats)
            return;

        if (triggered)
            return;

        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);//让当前物体以恒定速度向目标物体移动。
        transform.right = transform.position - targetStats.transform.position;//让物体的右方（X轴正方向）指向目标方向。  //transform.position - targetStats.transform.position 计算的是从目标指向当前物体的方向（一个向量）


        if (Vector2.Distance(transform.position, targetStats.transform.position) < .1f)
        {
            anim.transform.localPosition = new Vector3(0,.5f);//动画组件在骷髅头上播放
            anim.transform.localRotation=Quaternion.identity;
            //anim.transform.localScale = new Vector3(333, 333);

            transform.localRotation=Quaternion.identity;
            transform.localScale=new Vector3(3,3);   //雷电放大3倍  //把动画物体和当前物体的旋转都归零，并把当前物体放大3倍

            Invoke("DamageAndSelfDestroy", .2f);
            triggered = true;
            anim.SetTrigger("Hit");
        
        }
    }

    private void DamageAndSelfDestroy()
    {
        targetStats.ApplyShock(true);
        targetStats.TakeDamage(damage);
        Destroy(gameObject, .4f);
    }

}
