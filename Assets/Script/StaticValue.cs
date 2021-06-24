using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticValue
{
    //难度对应的须填数字个数
    public const int Easy = 25;
    public const int Normal = 42;
    public const int Hard = 50;
    public const int Lunatic = 56;
    public const int Empty = 81;
    public const int Load = -1;

    //当前
    public int _Difficult = 25;
    public int _HintCount = 5;

    public string _Filename = "save";
    public static StaticValue staticValue;

    public static StaticValue Get()
    {
        if(staticValue == null)
        {
            staticValue = new StaticValue();
        }
        return staticValue;
    }

}
