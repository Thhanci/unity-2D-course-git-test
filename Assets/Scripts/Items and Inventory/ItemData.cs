using UnityEngine;

public enum ItemType
{ 
    Material,
    Equipment
}    //     public enum ItemType {Material,Equipment}  enum（枚举）是定义一组固定选项的类型，让代码更易读、更安全
     //     0 = 材料, 1 = 装备

[CreateAssetMenu(fileName ="New Item Data",menuName ="Data/Item")]
public class ItemData : ScriptableObject  //脚本化对象 //这是一种区别于生命周期的一种类型，一般用来存储一些固定数据
{
    //定义物品的“蓝图”。不挂载到场景，作为配置文件存在 Project 中。
    public ItemType itemType;
    public string itemName;
    public Sprite icon;

    [Range(0,100)] public float dropChance;

}


/*
 
ScriptableObject 是 Unity 中一种特殊的数据容器，用于存储大量数据，不依附于场景中的 GameObject。

 MonoBehaviour	演员（在场景中表演，每场戏都有自己的状态）
ScriptableObject	剧本（不表演，存着角色的设定，所有演员共用同一份剧本）
 */