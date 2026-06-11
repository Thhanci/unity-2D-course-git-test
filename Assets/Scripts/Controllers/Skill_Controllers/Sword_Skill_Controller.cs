using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    //private float returntimerx=0;   //个人额外设置,飞剑飞回来时间
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate=true;//默认default:0
    private bool isReturning;

    private float freezeTimeDuration;
    private float returnSpeed = 12;                 //把剑丢入虚空，飞不回来，是因为returnSpeed太小，重力太大，飞回来的力比重力小，就一直往下掉了，调大就行

    [Header("Pierce info")]
    private float pierceAmount;

    [Header("Bounce info")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget; 
    private int targetIndex;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private float spinDirection;

    private void Awake()  //Start()Awake()   //和玩家碰撞的可以把预制体的图层设置成enemy  //Awake = 出生就做事，Start = 睡醒才做事，飞剑出生就要飞，所以用 Awake！
    {
        //Awake	对象创建后立即执行	获取组件、初始化
       // Start 对象创建后下一帧执行  依赖其他对象的逻辑
       anim = GetComponentInChildren<Animator>();
        rb= GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void SetupSword(Vector2 _dir,float _gravityScale,Player _player,float _freezeTimeDuration,float _returnSpeed)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed=_returnSpeed;

        rb.velocity=_dir;
        rb.gravityScale=_gravityScale;

        if (pierceAmount<=0) 
            anim.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);//Mathf.Clamp(被限制的值, 最小值, 最大值);
                                                          //// 速度 5 → 变成 1（向右旋转）// 速度 -3 → 变成 -1（向左旋转）// 速度 0.5 → 变成 0.5（轻微向右旋转）

        Invoke("DestroyMe", 7);//飞出去7秒自动销毁剑
    }

    public void SetupBounce(bool _isBouncing,int _amountOfBounces,float _bounceSpeed)   // public 方法，可以改 private 变量
    {
        isBouncing= _isBouncing;
        bounceAmount = _amountOfBounces;
        bounceSpeed = _bounceSpeed;

        enemyTarget = new List<Transform>();   //声明了变量，但没有分配内存空间，默认是 null。,故而添加本行
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount= _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning,float _maxTravelDistance,float _spinDuration,float _hitCooldown)
    {
        isSpinning= _isSpinning;
        maxTravelDistance= _maxTravelDistance;
        spinDuration= _spinDuration;
        hitCooldown= _hitCooldown;
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;//冻结刚体的所有运动（位置和旋转都不能变）
        //rb.isKinematic = false;
        transform.parent = null;   //物体的父物体    此脚本在sword下，故脚本挂在哪个物体上，transform 就代表那个物体。  //让物体从父物体中脱离，成为场景根目录下的独立物体。  //剑从敌人身上脱离   //MoveTowards是每帧执行一次，没重力快
        isReturning = true;
    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;        //unity2022版本不要直接在预制体的对象上面做更改，建议直接对文件夹里的预制体做更改，或者右键预制件对象->预制件->在上下文中打开资产，然后在弹出来的界面更改，不然会有无法套用更改的bug

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);    //物体的世界坐标位置

            if (Vector2.Distance(transform.position, player.transform.position) < 1)
                player.CatchTheSword();

        }

        BounceLogic();

        SpinLogic();

       //CheckDestroyOutOfBounds();//额外添加  //剑掉入虚空（超出范围）,则销毁剑

    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)  //抛出最远距离，停止位移，开始旋转计时
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }


                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);//Transform[] enemyTarget   数组
                                                                                               //在 2D 物理世界中，以某个点为圆心、指定半径画一个圆，找出所有与这个圆重叠的碰撞体,并存入 colliders 数组中
                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                    }
                }

            }

        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    /*

    private void CheckDestroyOutOfBounds()
    {
        //5. Improving sword throwing(2)  不早说   //算了还是销毁吧
        if (Vector2.Distance(transform.position, player.transform.position) > 200f)  //掉入虚空就自己回来（划掉），别回来了 销毁得了
        {
            Destroy(player.sword);
            //player.CatchTheSword();

            //returnSpeed = 6000;
            //returntimerx += Time.deltaTime;
            //transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * returntimerx);
            //Debug.Log(returnSpeed * returntimerx);
            //if (Vector2.Distance(transform.position, player.transform.position) < 20)
            //    player.ClearTheSword();
        }

        //// 超过一定距离自动返回
        //if (Vector2.Distance(transform.position, player.transform.position) > 30f)
        //{
        //    ReturnSword();
        //}
    }


    */

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)           //在敌人中间弹跳
        {
            //Debug.Log("Bouncing");
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());

                //enemyTarget[targetIndex].GetComponent<Enemy>().Damage();
                //enemyTarget[targetIndex].GetComponent<Enemy>().StartCoroutine("FreezeTimerFor",freezeTimeDuration);

                targetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)  //命中后贴到敌人enemy身上     //(Collider2D collision) Unity 自动传入的碰到的物体  //只在碰撞发生时执行（不在固定顺序里）
    {                                           //OnTriggerEnter2D 勾选unity面板里collider里的 Is Trigger触发,如果另一个碰撞器2D进入了触发器，则调用OnTriggerEnter2D(仅限2D物理）    
        if (isReturning)   //召回剑的时候就不算攻击了
            return;

        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            SwordSkillDamage(enemy);
        }

        // collision.GetComponent<Enemy>()?.Damage();  //问号（?.）是空条件运算符   如果左边不是 null，就执行右边的方法；如果是 null，就跳过，不报错。

        SetupTargetsForBounce(collision);

        StuckInto(collision);

    }
    private void SwordSkillDamage(Enemy enemy)
    {
        //enemy.DamageEffect();
        player.stats.DoDamage(enemy.GetComponent<CharacterStats>());
        enemy.StartCoroutine("FreezeTimerFor", freezeTimeDuration);  //类比玉米射手仍黄油
    }

    private void SetupTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);//Transform[] enemyTarget   数组
                                                                                            //注意这个Physics2D.OverlapCircleAll()函数返回的collider数组是按照目标的Z轴高度从小到大排序的，
                                                                                            //也就是说可能这个返回的数组里面排第一的可能并不是离你最近的敌人,
                                                                                            //所以在这里最好先对返回的colliders数组做一下自定义排序，根据与玩家的距离从小到大排序

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);   //enemyTarget = [敌人A.transform, 敌人B.transform, 敌人C.transform]
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)   //贴在敌人身上
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTarget.Count>0)
            return;
        
        //停止剑旋转的动画，并黏在敌人身上
        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}



/*
enemyTarget (List)的常用属性和方法列表  public List<transform> enemyTarget=[(0,0),(1,1)];         列表可变，数组固定，写法都是 [] 不是 <>！
常用属性
属性	作用	例子
.Count	元素个数	enemyTarget.Count
.Capacity	容量大小	enemyTarget.Capacity
常用方法
方法	              作用	                例子
.Add(item)	          添加元素	            enemyTarget.Add(enemy)
.Remove(item)	      删除指定元素	          enemyTarget.Remove(enemy)
.RemoveAt(index)	  删除指定位置	        enemyTarget.RemoveAt(0)
.Clear()	          清空所有	              enemyTarget.Clear()
.Contains(item)  	  是否包含	             enemyTarget.Contains(enemy)
.IndexOf(item)	      查找索引	             enemyTarget.IndexOf(enemy)
.Insert(index, item)  插入到指定位置	    enemyTarget.Insert(0, enemy)
.Sort()	              排序	                 enemyTarget.Sort()
.Reverse()	           反转顺序	                enemyTarget.Reverse() 

 */


/*
51:  enemyTarget = new List<Transform>();      7. Setting sword type
1.  public 字段在 Inspector 中可见，允许你在编辑器中手动为列表添加元素。
2.  当你首次在 Inspector 中点击“+”号添加元素时，Unity 会自动将该 List 字段从 null 初始化为一个空列表，然后添加你指定的元素。
3.  这个过程是由 Unity 序列化系统在编辑时完成的，而不是在代码运行时。
    而private 字段默认不在 Inspector 中显示，因此你无法通过点击“+”号来触发 Unity 的自动初始化，它在代码运行前始终为 null。
    最佳实践是无论变量是 public 还是 private，都应主动初始化，明确控制其状态：
    private List<Transform> enemyTarget = new List<Transform>();

 */

/*
（?.）是空条件运算符      ?. = 有东西才做，没东西就跳过
如果左边不是 null，就执行右边的方法；如果是 null，就跳过，不报错。
 
// 没有 ? 的写法
if (collision.GetComponent<Enemy>() != null)
{
    collision.GetComponent<Enemy>().Damage();
}

// 有 ? 的写法（简洁版）
collision.GetComponent<Enemy>()?.Damage(); 

 */