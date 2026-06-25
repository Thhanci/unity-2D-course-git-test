using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thunder strike effect", menuName = "Data/Item effect/Thunder strike")]

public class ThunderStrike_Effect : ItemEffect   //Tip:以后要改名就去Unity文件夹里改，同名的文件都改一遍，再新命名
{
    [SerializeField] private GameObject thunderStrikePrefab;
 

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        //base.ExecuteEffect();
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab,_enemyPosition.position,Quaternion.identity);
        Destroy(newThunderStrike,1 );

        //TOOD:setup new thunder strike
    }

}
