using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill : Skill   //Crystal 结晶，晶体；水晶；水晶玻璃；（钟表的）石英玻璃保护面，表蒙子
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Crystal mirage")]//水晶幻象  mirage n.海市蜃楼；幻想，妄想
    [SerializeField] private bool cloneInsteadOfCryestal;

    [Header("Explosive crystal")]
    [SerializeField] private bool canExplode;

    [Header("Moving crystal")]
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField]private List<GameObject> crystalLeft =new List<GameObject>();

    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiCrystal())
            return;
     
        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        { 
            if(canMoveToEnemy)
                return;

            Vector2 playerPos=player.transform.position;// 1. 保存玩家当前的位置
            player.transform.position = currentCrystal.transform.position;// 2. 把玩家移动到水晶所在的位置
            currentCrystal.transform.position = playerPos;// 3. 把水晶移动到玩家原来的位置（交换完成）

            if (cloneInsteadOfCryestal)
            {
                SkillMananger.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            { 
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();// 4. 结束水晶技能（如果水晶上有这个脚本的话）//?. 是"如果左边不为空，才执行右边"的安全运算符   //string name = player?.gameObject?.name;
            }
        }

    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform),player);
        //currentCrystalScript.ChooseRandomEnemy();
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            //respawn crystal   respawn  vt.复位vi.重生   spawn产卵；引发，促生；大量生产
            if (crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == amountOfStacks) //给“水晶从满变未满”这个动作设置一个计时器，用于后续的自动装填。
                    Invoke("ResetAbility", useTimeWindow);

                cooldown = 0;
                GameObject crystalToSpawn=crystalLeft[crystalLeft.Count-1];
                GameObject newCrystal=Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration,canExplode,canMoveToEnemy,moveSpeed,FindClosestEnemy(newCrystal.transform),player);
                if (crystalLeft.Count <= 0)
                {
                    //cooldown the skill
                    //refill crystals
                    cooldown = multiStackCooldown;
                    RefillCrystal();
                }

             return true;
            }

        }

        return false;
    }

    private void RefillCrystal()
    {
        int amoutToAdd = amountOfStacks - crystalLeft.Count;

        for (int i = 0; i < amoutToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()//在给定的时间窗口（useTimeWindow）后，自动为玩家补充所有水晶
    {
        if (cooldown > 0)
            return;
        cooldownTimer = multiStackCooldown;
        RefillCrystal();
    }

}
