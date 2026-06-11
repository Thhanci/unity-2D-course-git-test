using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();//private Animator anim = GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();
    private Player player;

    private float crystalExistTimer;

    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed=5;

    private Transform closestTarget;
    [SerializeField] private LayerMask whatIsEnemy;//whatIsEnemy 是一个 LayerMask（图层遮罩），用来过滤要检测的物体。

    public void SetupCrystal(float _crystalDuration,bool _canExplode,bool _canMove,float _moveSpeed,Transform _closestTarget,Player _player)
    {
        player = _player;
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
    }

    public void ChooseRandomEnemy()
    {
        float radius = SkillMananger.instance.blackhole.GetBlackholeRadius();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius,whatIsEnemy);//只检测"Enemy"图层的物体

        if (colliders.Length>0)
            closestTarget=colliders[Random.Range(0,colliders.Length)].transform;
    }

    private void Update()
    {
        crystalExistTimer -= Time.deltaTime;

        if (crystalExistTimer < 0)
        {
            FinishCrystal();
        }

        if (canMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, closestTarget.position) < 1) 
            {
                FinishCrystal();
                canMove = false;
            }
        }

        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,cd.radius);//是OverlapCircleAll而不是OverCircle

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null) 
            {
                //hit.GetComponent<Enemy>().DamageEffect();//水晶爆炸伤害
                player.stats.DoMagicalDamage(hit.GetComponent<CharacterStats>());
            }
        }
    }

    public void FinishCrystal()//水晶爆炸
    {
        if (canExplode) 
        { 
            canGrow = true;
            anim.SetTrigger("Explode");//水晶爆炸
        }
        else
            SelfDestroy();
    }

    public void SelfDestroy() => Destroy(gameObject);
}


/*
 
写法	                                                               类型	      执行时机
private Animator anim => GetComponent<Animator>();	                   只读属性	   每次访问时调用 GetComponent
private Animator anim; void Awake(){anim = GetComponent<Animator>();}	 字段	   赋值一次，之后直接用
 */