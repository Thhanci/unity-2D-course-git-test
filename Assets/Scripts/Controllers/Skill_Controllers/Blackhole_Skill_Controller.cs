using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour    //当你创建第一个 UI 元素（如 Button、Image）时，Unity 会自动生成 EventSystem。
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    private float maxSize;    
    private float growSpeed;
    private float shrinkSpeed;   //Alt + Shift同步改3行
    private float blackholeTimer;

    private bool canGrow=true;
    private bool canShrink;//缩小，减少
    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;
    private bool playerCanDisappear=true;   //playerCanDisapear

    private int amountOfAttacks = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();//private List<Transform> targets = new List<Transform>(); // 私有化变量注意初始化
    private List<GameObject> createHotKey=new List<GameObject>();

    public bool playerCanExitState {  get; private set; }

    public void SetupBlackhole(float _maxSize,float _growSpeed,float _shrinkSpeed,int _amountOfAttacks,float _cloneAttackCooldown,float _blackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks= _amountOfAttacks;
        cloneAttackCooldown= _cloneAttackCooldown;

        blackholeTimer = _blackholeDuration;

        if (SkillMananger.instance.clone.crystalInsteadOfClone)
            playerCanDisappear = false;
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        if (blackholeTimer < 0)
        {
            blackholeTimer = Mathf.Infinity;  //Mathf.Infinity	正无穷

            if (targets.Count > 0)
                ReleaseCloneAttack();
            else
                FinishBlackHoleAbility();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);//transform.localScale = Vector2.Lerp(当前大小, 目标大小, 进度);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0)
            return;

        DestroyHotKeys();
        cloneAttackReleased = true;
        canCreateHotKeys = false;   //按下R键开始攻击后，不在计算新进入的敌人

        if (playerCanDisappear) 
        {
            playerCanDisappear = false;
            PlayerManager.instance.player.fx.MakeTransparent(true);//Transparent透明的，清澈的；
        }
    }

    private void CloneAttackLogic()
    {


        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks >0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;  //在随机选中的敌人位置附近（左边或右边偏移2个单位），生成一个克隆体。

            if (Random.Range(0, 100) > 50)
                xOffset = 2;
            else
                xOffset = -2;

            //Debug.Log(xOffset);

            if (SkillMananger.instance.clone.crystalInsteadOfClone)
            {
                SkillMananger.instance.crystal.CreateCrystal();
                SkillMananger.instance.crystal.CurrentCrystalChooseRandomTarget();
            }
            else 
            {
                SkillMananger.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));  //vectors(x,y)
            }
            
            
            amountOfAttacks--;

            if (amountOfAttacks <= 0)
            {
                Invoke("FinishBlackHoleAbility", 1f);  //延迟一段时间后，调用指定的方法。
            }
        }
    }

    private void FinishBlackHoleAbility()
    {
        DestroyHotKeys();
        playerCanExitState = true;
        canShrink = true;
        cloneAttackReleased = false;
        
        
        //PlayerManager.instance.player.ExitBlackHoleAbility();
    }

    private void DestroyHotKeys()
    { 
        if(createHotKey.Count <= 0)
            return;
        for (int i = 0; i < createHotKey.Count; i++)
        { 
            Destroy(createHotKey[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>()!=null)
            collision.GetComponent<Enemy>().FreezeTime(false);
    }

    //private void OnTriggerExit2D(Collider2D collision) => collision.GetComponent<Enemy>()?.FreezeTime(false);   

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("Not enough hot keys in a key code list!");
            return;
        }

        if(!canCreateHotKeys)
            return;

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);  //quaternion 四元数 默认旋转  //Instantiate(要克隆的物体, 生成位置, 生成旋转, 父物体)
        createHotKey.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];   //  随机选一个按键  //Random.Range(0, count) 返回 0 到 count-1 之间的整数（不包含最大值）
        keyCodeList.Remove(choosenKey);   // 移除已选的，避免重复

        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();//newHotKey	刚生成的按键提示物体

        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);//是collision.transform 而不是transform，否则砍的是文本而不是下面的敌人
    }

    public void AddEnemyTolist(Transform _enemyTransform) => targets.Add(_enemyTransform);

}




/*
 
符号	名称	含义
?.	空条件运算符	左边不为空才执行右边
??	空合并运算符	左边为空就用右边的值


 */