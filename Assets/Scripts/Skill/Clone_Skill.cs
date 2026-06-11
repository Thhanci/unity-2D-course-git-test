using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill : Skill   //关联黑洞技能，克隆技能：生成一个克隆体（残影），并设置它的位置、持续时间和是否能攻击
{
    [Header("Clone info")]
    [SerializeField]private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;

    
    [SerializeField] private bool createCloneOnDashStart;//在选中行ctrl +D能快速复制，方便写相同类属性
    [SerializeField] private bool createCloneOnDashOver;
    [SerializeField]private bool canCreateCloneOnCounterAttack;

    [Header("Clone can duplicate")]
    [SerializeField] private bool canDuplicateClone;//Duplicate v.复制，复印；
    [SerializeField] private float chanceToDuplicate;

    [Header("Crystal instead of clone")]
    public bool crystalInsteadOfClone;
    public void CreateClone(Transform _clonePosition,Vector3 _offset)//offset偏移量
    {
        if (crystalInsteadOfClone)            //水晶代替分身
        {
            SkillMananger.instance.crystal.CreateCrystal();
            //SkillMananger.instance.crystal.CurrentCrystalChooseRandomTarget();
            return;
        }

        GameObject newClone=Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Skill_Controller>().
            SetupClone(_clonePosition,cloneDuration,canAttack,_offset,FindClosestEnemy(newClone.transform),canDuplicateClone,chanceToDuplicate,player);
    }
    public void CreateCloneOnDashStart()
    {
        if (createCloneOnDashStart)
            CreateClone(player.transform, Vector3.zero);
    }

    public void CreateCloneOnDashOver()
    {
        if (createCloneOnDashOver)
            CreateClone(player.transform, Vector3.zero);
    }

    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (canCreateCloneOnCounterAttack)       
            StartCoroutine(CreateCloneWithDelay(_enemyTransform,new Vector3(2*player.facingDir,0)));
    }

    private IEnumerator CreateCloneWithDelay(Transform _transform, Vector3 _offset) //numerator分子；计算者；计算器
    { 
        yield return new WaitForSeconds(.4f);
            CreateClone(_transform,_offset);
    }

}
