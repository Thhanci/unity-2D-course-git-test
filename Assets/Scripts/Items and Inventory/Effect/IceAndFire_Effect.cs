using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice and Fire effect", menuName = "Data/Item effect/Ice and Fire")]

public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private float xVelocity;

    public override void ExecuteEffect(Transform _respawnPosition)
    {
        //base.ExecuteEffect(_respawnPosition);//Tip_correct:  _respawnPosition   _respondPosition

        Player player = PlayerManager.instance.player;//这个 player 是玩家的 Transform 组件，包含了玩家的位置、旋转和缩放信息。

        bool thirdAttack = player.GetComponent<Player>().primaryAttack.comboCounter == 2;

        if (thirdAttack)
        {
            GameObject newIceAndFire = Instantiate(iceAndFirePrefab, _respawnPosition.position,player.transform.rotation);//player.rotation 是玩家的旋转，在 2D 中通常用 transform.eulerAngles 或 Quaternion 来控制。
            //Debug.Log("IceAndFire 已生成，位置：" + _respawnPosition.position);   //public void Effect(Transform _enemyPosition)因为需要enemyPosition，所以空斩没有效果
            newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * player.facingDir, 0); 
        
            Destroy(newIceAndFire,10);
        }

    }
}
