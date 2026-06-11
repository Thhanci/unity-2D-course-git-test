using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : MonoBehaviour  //封装(写说明书)/继承（基类/子类）/多态（同一个方法不同使用[public virtual]）     //jumpforce 12->16   attackDistance 1.5->2.5   CM vcam1 1.4->0.2
{

    #region Components
    public Animator anim { get; private set; }  //anim = GetComponentInChildren<Animator>();
    public Rigidbody2D rb { get; private set; }
    public EntityFX fx {  get; private set; }
    public SpriteRenderer sr {get; private set;  }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    #endregion

    [Header("Knocked info")]
    [SerializeField] protected Vector2 knockbackDirection;//击退速度/击退力度
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;

    [Header("Collision info")]   //[UnitHeaderInspectable("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;

    [SerializeField] protected Transform groundCheck;   //Alt+shift多选行改关键字
    [SerializeField] protected float groundCheckDistance;  //public>protected>private
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    public int facingDir { get; private set; } = 1;//->dashtime 有初值=1才能冲刺
    protected bool facingRight = true;

    public System.Action onFlipped;

    protected virtual void Awake()
    { 
    
    }

    protected virtual void Start()
    {
        sr=GetComponent<SpriteRenderer>();
        fx=GetComponent<EntityFX>();   //fx=GetComponentInChildren<EntityFX>();//赋值：fx = 获取挂在这个对象上的 EntityFX 组件
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<CharacterStats>();
        cd=GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    { 
    
    }

    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)
    { 
    
    }
    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    public virtual void DamageImpact() => StartCoroutine("HitKnockback");

    /*
    public virtual void DamageImpact()
    {
        //fx.StartCoroutine("FlashFX");// 出错一次FlashFX与FlashFx
        StartCoroutine("HitKnockback");

        //Debug.Log(gameObject.name + " was damaged"); //this.gameObject.name
    }

    */
    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        rb.velocity = new Vector2(knockbackDirection.x*-facingDir, knockbackDirection.y);

        yield return new WaitForSeconds(knockbackDuration);
        isKnocked=false;
    }


    #region Velocity
    public void SetZeroVelocity()
    {
        if (isKnocked)
            return;

        rb.velocity=new Vector2(0,0);
    }


    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion

    #region Collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region Flip
    public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        if(onFlipped!=null)
            onFlipped();
    }
    public virtual void FlipController(float _x)   //virtual就是让同一个函数名，在不同类里有不同的实现代码！
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }
    #endregion



    public virtual void Die()
    { 
    
    }

}




/*
 默认顺序规则
因素	说明
脚本组件顺序	同一物体上，从上到下执行
物体加载顺序	按 Hierarchy 中物体的先后顺序
预制体实例化顺序	先实例化的先执行

Hierarchy 窗口：
├── Player        ← 第1个执行
├── Enemy         ← 第2个执行
└── Manager       ← 第3个执行

每个物体上的多个脚本：
Player 物体：
├── PlayerMovement ← 先执行
└── PlayerHealth   ← 后执行

1.菜单栏 → Edit → Project Settings
2.选择 Script Execution Order
3.点击 + 添加脚本
4.设置数值（越小越先执行）
 */

/*
 
Unity 引擎
    ↓
第1步：调用所有脚本的 Awake()（所有对象，只执行1次）
    ↓
第2步：调用所有脚本的 Start()（所有对象，只执行1次）
    ↓
第3步：进入游戏循环，每帧执行：
    ├── 调用所有脚本的 Update()（按某种顺序）
    ├── 调用所有脚本的 Update()
    └── ...
    ↓
第4步：重复第3步
 */

/*
方法5：检查是否在别的场景
打开 Project 窗口

在 Assets/Scenes 文件夹下找其他场景文件

双击打开试试

预防措施
操作	说明
经常保存	Ctrl + S 养成习惯
使用版本控制	Git + 定期提交
备份场景	另存为 SceneName_Backup.unity
开启自动保存	Edit → Project Settings → Editor → Auto Save



场景文件里保存了什么scene
内容	说明
物体信息	Hierarchy 里的所有物体（位置、旋转、缩放）
组件数据	挂载的脚本、参数值、材质、动画等
引用关系	物体之间的父子关系、脚本间的引用
场景设置	光照、音效、渲染设置等
 */

/*
 剑击中骷髅，骷髅会反转：估计是跳起来判定了一次地面，由于没接触判定为没路，就反转了，解决方法：enemy_skeleton wall checkdistance 0.4->1
 */


/*
 
前移后移代码块 Tab   shift+Tab

Alt+shift多选行改关键字         按住alt+shift，从左上选中到右下
 */

/*
 Invoke("方法名", 延迟秒数);   //延迟一段时间后调用指定的方法。
 */