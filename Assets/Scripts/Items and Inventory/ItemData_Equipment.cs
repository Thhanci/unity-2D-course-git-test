using System.Collections.Generic;
using UnityEngine;
public enum EquipmentType
{ 
    Weapon,
    Armor,
    Amulet,
    Flask
}
//武器，护甲，护身符，药水

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
//[CreateAssetMenu] 只能用在 class 上，不能用在其他东西上。

public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    public float itemCooldown;
    public ItemEffect[] itemEffect;

    [Header("Major stats")]  //Major 主要的
    public int strength;  //力量
    public int agility;  //敏捷
    public int intelligence;  //智力
    public int vitality;  //活力

    [Header("Offensive stats")]  //Offensive 攻击的
    public int damage;  //伤害
    public int critChance;  //暴击率
    public int critPower;  //暴击伤害

    [Header("Defensive stats")]  //Defensive 防御的
    public int health;  //生命值
    public int armor;  //护甲
    public int evasion;  //闪避
    public int magicResistance;  //魔法抗性

    [Header("Magic stats")]  //Magic 魔法的
    public int fireDamage;  //火焰伤害
    public int iceDamage;  //冰霜伤害
    public int lightingDamage;  //雷电伤害

    [Header("Craft requirements")]
    public List<InventoryItem> craftingMaterials;

    public void Effect(Transform _enemyPosition)//Tip:应用装备特殊效果（闪电，冰与火）
    {
        foreach (var item in itemEffect)
        { 
            item.ExecuteEffect(_enemyPosition);
        }
    }

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.maxHealth.AddModifier(health);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightingDamage.AddModifier(lightingDamage);

    }

    public void RemoveModifiers() 
    {
        //Debug.Log("RemoveModifiers 被调用: " + this.itemName);

        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        playerStats.maxHealth.RemoveModifier(health);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightingDamage.RemoveModifier(lightingDamage);
    }

}


/*
 
        equipmentType 的类型是 EquipmentType（你定义的枚举），
    只能存 Weapon、Armor、Amulet、Flask 四个值之一。

 */