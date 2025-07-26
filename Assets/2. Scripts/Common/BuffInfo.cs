using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffInfo
{
    public BUFFTYPE eBuffType;
    public int iRemainTurns;
    public float value;

    public BuffInfo(BUFFTYPE eResult, int iTurns, float fValue)
    {
        this.eBuffType = eResult;
        this.iRemainTurns = iTurns;
        this.value = fValue;
    }
}
