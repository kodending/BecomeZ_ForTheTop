using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWeight
{

    public static bool AttackSuccessCal(float iProb, bool isPenet)
    {
        if (isPenet) return true;

        float totalWeight = 1;

        float randomValue = UnityEngine.Random.value * totalWeight;

        randomValue -= iProb;

        if (randomValue < 0f) return true;

        return false;
    }

    public static int AttackDamageCal(List<bool> listSuccess, int iAtk, bool bCritcal)
    {
        //데미지 계산하는부분 다시 계산하자

        int damage = 0;

        float maxCnt = listSuccess.Count;
        int hitCnt = 0;

        foreach (var suc in listSuccess)
        {
            if (suc) hitCnt++;
        }

        float hitRate = (float)hitCnt / maxCnt;

        damage = Mathf.FloorToInt(hitRate * iAtk);

        if (bCritcal) damage = Mathf.RoundToInt(damage * 1.5f);

        return damage;
    }

    public static bool MissCal(List<bool> listSuccess, float attackerSpeed, float defenderSpeed)
    {
        float maxCnt = listSuccess.Count;
        int hitCnt = 0;

        foreach (var suc in listSuccess)
        {
            if (suc) hitCnt++;
        }

        float hitRate = (float)hitCnt / maxCnt;

        attackerSpeed = attackerSpeed * hitRate;

        float missRate = 0;
        if (Mathf.RoundToInt(attackerSpeed) <= 0)
            missRate = defenderSpeed;
        else
            missRate = defenderSpeed / attackerSpeed;

        float defaultRate = 0.3f;

        float resultRate = defaultRate * missRate;

        float totalWeight = 1;

        float randomValue = UnityEngine.Random.value * totalWeight;

        randomValue -= resultRate;

        if (randomValue < 0f) return true;

        return false;
    }
}
