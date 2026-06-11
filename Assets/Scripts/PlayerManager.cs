using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
   public static PlayerManager instance;  //单例 静态变量 
    public Player player;          //定义类，在hierarchy下拖拽赋值  

    private void Awake()   //游戏对象被创建时第一个执行的函数
    {
        if(instance != null)     //Awake只执行一次，在运行前，instance获得第一个gameObjiet（hierarchy下），多了就销毁
            Destroy(instance.gameObject);  //这个instance单例为PlayerManager，多了的单例PlayerManager销毁
        else
            instance = this;
    }

}
