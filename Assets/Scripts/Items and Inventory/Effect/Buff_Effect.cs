using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    strength,
    agility,
    intelligence,//intelegence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightingDamage
}

[CreateAssetMenu(fileName = "Buff effect", menuName = "Data/Item effect/Buff effect")]
//Tip_repair_wait:为什么被攻击一次就触发了两次闪避加成？ 骷髅兵的物理伤害和魔法伤害都调用了TakeDamage()
public class Buff_Effect : ItemEffect
{
    private PlayerStats stats;
    [SerializeField] private StatType buffType;
    [SerializeField] private int buffAmount;
    [SerializeField] private float buffDuration;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        //base.ExecuteEffect(_enemyPosition);

        stats=PlayerManager.instance.player.GetComponent<PlayerStats>();

        stats.IncreaseStatBy(buffAmount,buffDuration,StatToModify());
    }
    private Stat StatToModify()
    {
        
        if (buffType == StatType.strength) return stats.strength;
        else if (buffType == StatType.agility) return stats.agility;
        else if (buffType == StatType.intelligence) return stats.intelligence;
        else if (buffType == StatType.vitality) return stats.vitality;
        else if (buffType == StatType.damage) return stats.damage;
        else if (buffType == StatType.critChance) return stats.critChance;
        else if (buffType == StatType.critPower) return stats.critPower;
        else if (buffType == StatType.health) return stats.maxHealth;
        else if (buffType == StatType.armor) return stats.armor;
        else if (buffType == StatType.evasion) return stats.evasion;
        else if (buffType == StatType.magicRes) return stats.magicResistance;
        else if (buffType == StatType.fireDamage) return stats.fireDamage;
        else if (buffType == StatType.iceDamage) return stats.iceDamage;
        else if (buffType == StatType.lightingDamage) return stats.lightingDamage;

        return null;

    }

}

/*
 

     switch (buffType)
    {
        case StatType.strength: return stats.strength;
        case StatType.agility: return stats.agility;
        case StatType.intelligence: return stats.intelligence;
        case StatType.vitality: return stats.vitality;
        case StatType.damage: return stats.damage;
        case StatType.critChance: return stats.critChance;
        case StatType.critPower: return stats.critPower;
        case StatType.health: return stats.maxHealth;
        case StatType.armor: return stats.armor;
        case StatType.evasion: return stats.evasion;
        case StatType.magicRes: return stats.magicResistance;
        case StatType.fireDamage: return stats.fireDamage;
        case StatType.iceDamage: return stats.iceDamage;
        case StatType.lightningDamage: return stats.lightningDamage;
        default: return null;
    }
 

 */