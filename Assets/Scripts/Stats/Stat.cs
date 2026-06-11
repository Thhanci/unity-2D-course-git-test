using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]    //让自定义的类或结构体可以在 Unity 的 Inspector 窗口中显示和编辑。
public class Stat // 统一管理属性值啊    //封装 = 把复杂的东西装进一个黑盒子里，只给你留几个简单的按钮。   //把数据（变量）和操作（方法）打包在一起，并隐藏内部细节，只暴露必要的接口。
{
    [SerializeField] private int baseValue;

    public List<int>  modifiers;//modifier n.修饰语，修饰成分；更改者，改型者；（遗传）修饰基因
    public int GetValue()  //封装的好处：改内部逻辑，外部代码不用动。
    { 
        int finalValue=baseValue;

        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    public void SetDefaultValue(int _value)
    { 
        baseValue = _value;
    }

    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }
    public void RemoveModifier(int _modifier)
    { 
        modifiers.Remove(_modifier);
    }


}
