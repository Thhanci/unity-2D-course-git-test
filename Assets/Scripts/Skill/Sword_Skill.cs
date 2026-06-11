using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum SwordType   //剑种类  //enum（枚举）是定义一组固定选项的类型，SwordType 就是剑的四种类型。
{                           //enum = 下拉菜单（选一个） struct = 轻量级数据包   class = 完整对象模板
    Regular,   //定期的；有规律的；合格的；整齐的；普通的 //普通剑
    Bounce,     //弹跳
    Pierce,   //穿孔
    Spin,       //高速旋转
}

public class Sword_Skill : Skill   //sword animation里是，常态是sword23的动画
{
    public SwordType swordType = SwordType.Regular;

    [Header("Bounce info")]
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Peirce info")]
    [SerializeField] private int pierceAmout;
    [SerializeField] private float pierceGravity;

    [Header("Spin info")]
    [SerializeField] private float hitCooldown = .35f;
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 1;

    [Header("Skill info")]
    [SerializeField] private GameObject swordPrefab;   // 飞剑预制体
    [SerializeField] private Vector2 launchForce;   // 发射力度（如 (10, 10)）
    [SerializeField] private float swordGravity;   // 重力大小
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    protected Vector2 finalDir;   // 最终发射方向

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;//spaceBeetwenDots   //生成点的间隔
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();

        GenerateDots();

        SetupGravity();//SetupGraivty
    }

    private void SetupGravity()
    {
        if (swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if (swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))   //AimDirection().normalized 获取鼠标方向的单位向量（长度为1）
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);

        if (Input.GetKey(KeyCode.Mouse1))                //每帧执行，最后松开getkeyup了，才执行上一帧    //用于画点，点的位置在DotPosition
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }

    public void CreateSword()
    {
        //第二行获取组件
        //1行初始化sword对象，2行从该对象里找到cotroller，unity里我们将controller挪到了sword上来着因为,3行调用controller内的方法，
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)   //选择if else if 然后按下Alt+enter，可转换为switch case
        {
            //swordGravity=bounceGravity;   // newSwordScript.SetupSword(swordGravity);  // public void CreateSword()  //private值，也被public传递了
            newSwordScript.SetupBounce(true, bounceAmount,bounceSpeed);
        }
        else if (swordType == SwordType.Pierce)
            newSwordScript.SetupPierce(pierceAmout);
        else if (swordType == SwordType.Spin)
            newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration,hitCooldown);


        newSwordScript.SetupSword(finalDir, swordGravity, player,freezeTimeDuration,returnSpeed);

        player.AssignNewSword(newSword);    //你把飞剑存到了 player.sword 里，下次再丢剑时，旧剑还在，但新剑覆盖了引用，导致逻辑出问题。

        DotsActive(false);   //转到public class PlayerAnimationTriggers : MonoBehaviour          // SkillMananger.instance.sword.CreateSword();
    }


    #region Aim region
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;   // 玩家位置
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);   // 鼠标世界坐标
        Vector2 direction = mousePosition - playerPosition;   // 方向 = 鼠标 - 玩家

        return direction;  // 返回从玩家指向鼠标的方向
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenerateDots()   //GenereateDots
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);   //要复制的预制体   生成位置（玩家位置）（生成的初始位置）   	默认旋转（无旋转）   父物体（拖进去的容器）(生成的点存放在哪)
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        //d = vt + 1/2at^2             //(Vector2)player.transform.position  // 转成 Vector2（丢掉 z，只留 x, y）
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }
    #endregion


}



/*
  Input 类常用的东西
名称	         类型	 作用
mousePosition	属性	鼠标屏幕坐标
GetKeyDown()	方法	按键按下
GetKeyUp()	方法	按键抬起
GetAxis()	方法	输入轴（水平/垂直）
GetButton()	方法	按钮输入
anyKeyDown	属性	是否有按键按下
*/

/*
 Camera.main 常用方法
方法	                作用
ScreenToWorldPoint	屏幕坐标 → 世界坐标
WorldToScreenPoint	世界坐标 → 屏幕坐标
ViewportToWorldPoint	视口坐标(0-1) → 世界坐标
transform.position	摄像机的位置
orthographicSize	2D摄像机的大小
*/

/*
 代码	类型	说明
Camera	类	Unity 的摄像机类
main	静态属性	返回场景中Tag为"MainCamera"的摄像机
ScreenToWorldPoint	方法	Camera 类的方法，用 main 返回的对象调用
 */

/*
 Physics2D	Unity 2D 物理系统类
gravity	Physics2D 类的静态属性，代表全局重力
*	乘法运算符
swordGravity	你自己定义的重力倍数
 */


/*
 
游戏启动
    ↓
SkillMananger.Awake() → 单例设置
    ↓
SkillMananger.Start() → 获取 sword = GetComponent<Sword_Skill>()
    ↓
Sword_Skill.Start() → base.Start() → GenerateDots() 生成轨迹点
    ↓
【玩家操作】
    ↓
地面状态按鼠标右键 → 切换到 aimSword 状态
    ↓
aimSword.Enter() → DotsActive(true) 显示轨迹点
    ↓
Sword_Skill.Update() → 按住右键时不断更新点的位置
    ↓
松开鼠标右键
    ↓
Sword_Skill.Update() → KeyUp 计算 finalDir
    ↓
PlayerAimSwordState.Update() → 检测到 KeyUp → 切换回 idleState
    ↓
同时PlayerAnimationTriggers调用 CreateSword() → 发射飞剑 → DotsActive(false) 隐藏点

 */



/*
 游戏开始：
1. SkillMananger.Awake()
2. Player.Awake()（基类 Entity.Awake）
3. Sword_Skill.Start()（继承 Skill.Start）
4. Player.Start()
5. PlayerGroundedState 初始化
   ...
   进入游戏循环

每帧（按住右键时）：
1. Player.Update()
   └── stateMachine.currentState.Update()
       └── PlayerAimSwordState.Update()
           ├── base.Update()（PlayerState.Update）
           │   └── 更新输入、动画参数
           └── 检查 KeyUp
2. Sword_Skill.Update()
   └── 更新轨迹点位置

注意：这是顺序执行，不是同时！
 */