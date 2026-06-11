using UnityEngine;


//edit->project setting -> script execution order ->characterstats 300 HealthBar_UI 400提早这两个代码的执行顺序（Awake）
public class CharacterStats : MonoBehaviour//stats 统计（statistics）  
{
    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength;  //n.体力，力量   //1 point increase damage by 1 and crit.power by 1%   //strength又加基伤又加暴击
    public Stat agility;  //n.（动作）敏捷，灵活；（思维）机敏  //1 point increase evasion by 1% and crit.chance by 1%
    public Stat intelligence;   //n.智力，才智；智能  //1 point increase magic damage by 1 and magic resistance by 3
    public Stat vitality;   //n. 活力，热情；生机，生命力  //1 point increase health health by 3 or 5 points

    [Header("Offensive stats")]
    public Stat damage;//一个stat 类 damage下有多种属性 base value ,other value   扩展 Stat 类
    public Stat critChance;    //crit n.暴击   //暴击率
    public Stat critpower;    //    //default value 150%

    [Header("Defensive stats")]
    public Stat maxHealth;  // 最大生命值属性
    public Stat armor;   //armor n.盔甲，甲胄；装甲,防御
    public Stat evasion;  //evasionn.逃脱，躲避；逃避手段，逃避方法；遁词，借口
    public Stat magicResistance;//法抗  //resistancen.反对，抵制；抵抗，反抗；

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited;  //Ignited点燃    //does damage over time
    public bool isChilled;  //使变冷       //reduce armor by 20%
    public bool isShocked;  //使受电击 震惊的  //reduce accuracy by 20%

    [SerializeField] private float ailmentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float igniteDamageCooldown = .3f;
    private float igniteDamagerTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;

    public int currentHealth;// 当前生命值

    public System.Action onHealthChanged;
    protected bool isDead;

    protected virtual void Start()
    {
        critpower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();// 初始化当前血量

        fx = GetComponent<EntityFX>();

        //Debug.Log("Character stats called");
        //example equipt sword with 4 damage
        //damage.AddModifier(4);

    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamagerTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if (isIgnited)
            ApplyIgniteDamage();

    }



    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            //Debug.Log("CRIT HIT");
            totalDamage = CalculateCriticalDamage(totalDamage);
            //Debug.Log("Total crit damage is "+totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);  //物伤

        //if the current weapon has fire damage
        //then
        //DoMagicalDamage(_targetStats);//法伤
    }

    #region Magical damage and ailments

    public virtual void DoMagicalDamage(CharacterStats _targetStats)   //判断法伤类型
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();

        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)   //3个0，没有魔法伤害，下面while死循环
            return;


        AttemptyToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightingDamage);


    }

    private void AttemptyToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)//3个0，没有魔法伤害，while会陷入死循环
        {
            if (Random.value < .5f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
                //Debug.Log("Applied fire");
                return;
            }
            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
                //Debug.Log("Applied ice");
                return;
            }
            if (Random.value < .5f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
                //Debug.Log("Applied lighting");
                return;
            }

        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * .1f));

        _targetStats.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
    }




    public void ApplyAliments(bool _ignite, bool _chill, bool _shock)
    {
        // 只有当前没有任何状态时，才能应用新状态
        // 不允许叠加状态
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        //if (isIgnited || isChilled || isShocked)//有一个状态之后就不再造成新状态了，也就是直接返回
        //    return;

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;

            fx.IgniteFxFor(ailmentsDuration);
        }
        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = ailmentsDuration;

            float slowPercentage = .2f;//速度降低20%(slowPercentage %)

            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);//Tip:由于player有sprite renderer,而其下的animator也有sprite renderer，
                                                                                  //故而删除player里的sprite renderer,2个sprite renderer导致了玩家寒冷时不变色

            fx.ChillFxFor(ailmentsDuration);
        }
        if (_shock && canApplyShock)
        {
            if (isShocked)
            {
                ApplyShock(_shock);

            }
            else
            {
                if (GetComponent<Player>() != null)  //如果不是玩家造成伤害，就不产生闪电
                    return;
                HitNearestTargetWithShockStrike();

            }

        }

    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;

        isShocked = _shock;
        shockedTimer = ailmentsDuration;

        fx.ShockFxFor(ailmentsDuration);
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);  //计算两个点之间的距离。

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }

            }

            if (closestEnemy == null)         //如果附近没有敌人，自己（敌人）生成的闪电就打自己 // Remove this  to disable shock strike on shocked targets
                closestEnemy = transform;

        }

        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrikeController>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());

        }

        //find closest target ,only among the enemier
        //instantiate thunder strike
        //set up thunder strike
    }

    private void ApplyIgniteDamage()
    {
        if (igniteDamagerTimer < 0 && isIgnited)
        {
            //Debug.Log("Take burn damage " + igniteDamage);

            DecreaseHealthBy(igniteDamage);

            if (currentHealth < 0)
                Die();

            igniteDamagerTimer = igniteDamageCooldown;
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;

    #endregion

    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");
        //Debug.Log(_damage);

        if (currentHealth <= 0 && !isDead)// <0
            Die();
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;

        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void Die()
    {
        isDead = true;
        //throw new NotImplementedException();//这是一个占位符，表示"这个方法还没实现，先留在这里，以后再写"。
    }

    #region Stat calculations

    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);//Mathf.Clamp(数值, 最小值, 最大值);把数值限制在最小值和最大值之间。
        return totalDamage;
    }

    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }



    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();//总闪避值

        if (isShocked)
            totalEvasion += 20;//降低命中率 = 增加闪避率

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }

        return false;
    }

    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();//Critical adj.批判的  //暴击率加上敏捷能力的暴击率
        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critpower.GetValue() + strength.GetValue()) * .01f;//暴击伤害倍率
        //Debug.Log("total crit power %"+totalCritPower);
        float critDamage = _damage * totalCritPower;  //暴击伤害
        //Debug.Log("crit damage before round up"+critDamage);

        return Mathf.RoundToInt(critDamage);//Mathf.RoundToInt()把一个浮点数四舍五入转换成整数。
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

    #endregion

}


/*

public bool isIgnited; 初值为false

 操作	哪里修改	什么时候
isIgnited = true	ApplyAliments()	受到火焰伤害
isIgnited = false	Update()	点燃持续时间结束（4秒后）
 */