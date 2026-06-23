using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;

    [Header("Level details")]
    [SerializeField] private int level = 1;

    [Range(0f,1f)]
    [SerializeField] private float percentageModifier = .4f;  //在 Inspector 中会显示为滑动条，值被限制在范围内。

    protected override void Start()
    {
        ApplyLevelModifiers();

        base.Start();

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
    }

    private void ApplyLevelModifiers()
    {
        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);

        Modify(damage);
        Modify(critChance);
        Modify(critPower);

        Modify(maxHealth);
        Modify(armor);
        Modify(evasion);
        Modify(magicResistance);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightingDamage);

    }

    private void Modify(Stat _stat)
    { 
        //level等级控制循环次数，percentage Modifier控制每次循环加的数值
        for(int i = 0; i < level; i++)
        {
            float modifier = _stat.GetValue() * percentageModifier;

            _stat.AddModifier(Mathf.RoundToInt(modifier));  //把浮点数四舍五入转换成整数。round圆形的 四舍五入
        }

    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

        //enemy.DamageEffect();
    }
    protected override void Die()
    {
        if (isDead) return;//Tip:额外添加，解决骷髅反复死亡，另外反复掉落的问题

        base.Die();
        enemy.Die();

        myDropSystem.GenerateDrop();
    }
}

//Tip:冲刺可以超越生死（冲刺阶段被砍死还能动）

/*
 
     1. GetValue() 计算当前值：
       baseValue + modifiers[0] + modifiers[1] + ... = 当前值

    2. 当前值 × percentageModifier = 修饰量

    3. Mathf.RoundToInt(修饰量) 转整数
 
 */


/*
 
    最终血量 = (baseValue + 等级强化modifier + 装备modifier) + vitality × 5
               100    +  10+11+12          +  10          +   10   × 5
             = 100    +  33                +  10          +   50
             = 193

 */