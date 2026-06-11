using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLoosingSpeed;

    //[SerializeField] private float cloneDuration;
    private float cloneTimer;   //cloneTimer = cloneDuration;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private Transform closestEnemy;
    private int facingDir = 1;

    private bool canDuplicateClone;//分身砍到怪就继续生成分身
    private float chanceToDuplicate;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));  //在Update，逐帧减少

            if (sr.color.a <= 0)
                Destroy(gameObject);

        }
    }
    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack, Vector3 _offset, Transform _closestEnemy, bool _canDuplicate, float _chanceToDuplicate,Player _player)
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 3));

        player = _player;
        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;

        canDuplicateClone = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;
        closestEnemy = _closestEnemy;
        FaceClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
        //player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);//是OverlapCircleAll而不是OverCircle
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(0, 0), 1);//Collider2D[] colliders = Physics2D.OverlapCircle(player.attackCheck.position, player.attackCheckRadius);



        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //hit.GetComponent<Enemy>().DamageEffect();
                player.stats.DoDamage(hit.GetComponent<CharacterStats>());

                if (canDuplicateClone)
                {
                    if (Random.Range(0, 100) < chanceToDuplicate)   //反击后，chanceToDuplicate)%的概率触发格挡再次反击   //Random.Range(0, 100)随机生成一个 0 到 99 之间的整数。
                    {
                        SkillMananger.instance.clone.CreateClone(hit.transform, new Vector3(.5f * facingDir, 0));
                    }
                }
            }
        }

    }

    private void FaceClosestTarget()
    {


        if (closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }

        }


    }
}
