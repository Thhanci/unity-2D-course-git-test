using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill : Skill
{
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float cloneCooldown;
    [SerializeField] private float blackholeDuration;
    [Space]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    Blackhole_Skill_Controller currentBlackhole;

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackHole=Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity);

        //Blackhole_Skill_Controller newBlackHoleScript
        currentBlackhole= newBlackHole.GetComponent<Blackhole_Skill_Controller>();// 脚本类型     变量名         赋值   源物体      获取组件方法<脚本类型>()

        currentBlackhole.SetupBlackhole(maxSize,growSpeed,shrinkSpeed,amountOfAttacks,cloneCooldown,blackholeDuration);    //传参 public void SetupBlackhole(float _maxSize,float _growSpeed,float _shrinkSpeed,int _amountOfAttacks,float _cloneAttackCooldown,float _blackholeDuration)
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool SkillCompleted()
    { 
        if(!currentBlackhole)
            return false;

        if (currentBlackhole.playerCanExitState)
        { 
            currentBlackhole = null;
            return true;
        }

        return false;
    }

    public float GetBlackholeRadius()
    {
        return maxSize / 2;
    }

}






/*
 写法	            作用	                                  示例
Quaternion.identity	                无旋转（0度）	         Instantiate(obj, pos, Quaternion.identity)
Quaternion.Euler(x, y, z)	         按欧拉角旋转	          Quaternion.Euler(0, 90, 0) 绕Y轴转90度
Quaternion.LookRotation(Vector3)	 让物体看向某方向	      Quaternion.LookRotation(target.position - transform.position)
Quaternion.Slerp(a, b, t)	         平滑旋转插值	          transform.rotation = Quaternion.Slerp(a, b, 0.5f)
Quaternion.AngleAxis(角度, 轴)	     绕指定轴旋转	           Quaternion.AngleAxis(90, Vector3.up)
 */



/*
 
脚本	                      职责	                         原因
Blackhole_Skill	              技能管理（冷却、消耗、生成）	 继承自 Skill，统一技能逻辑
Blackhole_Skill_Controller	   黑洞具体行为	                 单独控制，可以挂载到预制体上

技能系统只管"怎么放"，控制器管"怎么动"。

脚本	                                 比喻
Blackhole_Skill	                         遥控器（按一下，黑洞就出来）
Blackhole_Skill_Controller	             黑洞本身（怎么长大、怎么吸怪、怎么攻击）

Blackhole_Skill 生成并控制 Blackhole_Skill_Controller，两者通过 playerCanExitState 通信。
 



 Blackhole_Skill                    Blackhole_Skill_Controller
     │                                      │
     │  SetupBlackhole(参数)                │
     ├─────────────────────────────────────→│
     │                                      │
     │                                      │ 执行长大、吸怪、QTE...
     │                                      │
     │  SkillCompleted() 轮询               │
     ├─────────────────────────────────────→│
     │                                      │
     │  ←────── playerCanExitState ────────│
     │                                      │
     │  返回 true，技能冷却                 │

 */