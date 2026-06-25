using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal effect", menuName = "Data/Item effect/Heal effect")]    //Tip:New Item Data(Heal effect) 是创建时的默认文件名，但它只是一个模板名字。当你真正创建文件时，会被你实际起的名字覆盖。


public class HealEffect : ItemEffect
{
    //player stats

    //how much to heal

    //heal
    [Range(0f, 1f)]  [SerializeField] private float healPercent;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        //base.ExecuteEffect(_enemyPosition);
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue()*healPercent);

        playerStats.IncreaseHealthBy(healAmount);

    }


}
