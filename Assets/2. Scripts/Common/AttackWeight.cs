using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class AttackWeight
{
    [Range(0f, 1f)] static float baseDodge = 0.05f;      // 최소 회피 5 %
    [Range(0f, 1f)] static float maxDodge = 0.85f;      // 상한 85 %

    static float maxRatio = 4f;   // 방어/공격 속도가 이 비율에 도달하면 상한
    static float exponent = 1.5f; // 1 = 직선, 2 = 제곱, 0.5 = 루트

    static public AnimationCurve ratioSpdCurve =
    AnimationCurve.EaseInOut(0f, 0.05f, 1f, 0.85f);

    static public AnimationCurve absSpdCurve =
        AnimationCurve.EaseInOut(0f, 0.05f, 1f, 0.85f);

    [Range(0f, 1f)] static float weightAbs = 0.65f;   // 절대 차 % 가중

    static float penaltyScale = 1f;                       // 0 => 패널티 없음, 1 => 성공률만큼 100% 감속
    static float speedPenaltyScale = 0.7f;                       // 0 => 패널티 없음, 1 => 성공률만큼 100% 감속

    /* ① 절대 스피드 커브 ---------------------------------- */
    [Header("Absolute-Speed (atk)")]
    static float absMin = 60f;                     // 이하면 curve X = 0
    static float absMax = 180f;                     // 이 이상은 curve X = 1
    static AnimationCurve absCurve = new AnimationCurve(
        new Keyframe(0f, 0.05f),                    // 5 %
        new Keyframe(1f, 0.50f));                   // 50 %

    /* ② 상대 스피드(비율) 커브 ------------------------------ */
    [Header("Relative-Speed (atk / def)")]
    static float relMin = 0.5f;                     // 절반 속도, 방어력
    static float relMax = 3.0f;                     // 3 배 속도, 방어력
    static AnimationCurve relCurve = new AnimationCurve(
        new Keyframe(0f, 0.05f),                    // 5 %
        new Keyframe(1f, 0.90f));                   // 90 %

    /* ③ 두 커브를 섞는 가중치 ------------------------------ */
    [Header("Mix-Ratio")]
    [Range(0f, 1f)]
    static float absSpeedWeight = 0.8f;                  // 0.8 = 절대 80 %, 상대 20 % 스피드에 대한 가중치

    [Header("Mix-Ratio")]
    [Range(0f, 1f)]
    static float absDefWeight = 0.2f;                  // 0.8 = 절대 80 %, 상대 20 % 방어도에 대한 가중치

    public static bool AttackSuccessCal(float iProb, bool isPenet)
    {
        if (isPenet) return true;

        //새로운 확률 적용
        iProb = Mathf.Clamp01(iProb);
        bool bSuccess = new bool();

        bSuccess = UnityEngine.Random.value < iProb;

        return bSuccess;
    }

    public static int AttackDamageCal(List<bool> listSuccess, int iAtk, bool bCritcal)
    {
        //데미지 계산하는부분 다시 계산하자
        int damage = 0;

        int maxCnt = listSuccess.Count;
        int hitCnt = 0;

        foreach (var suc in listSuccess)
        {
            if (suc) hitCnt++;
        }

        float atkDamageEff = GetEffectiveAtkDamage(iAtk, hitCnt, maxCnt);

        if (bCritcal) damage = Mathf.RoundToInt(atkDamageEff * 1.5f);
        else damage = Mathf.RoundToInt(atkDamageEff);

        return damage;
    }

    public static bool MissCal(List<bool> listSuccess, float attackerSpeed, float defenderSpeed)
    {
        int maxCnt = listSuccess.Count;
        int hitCnt = 0;

        foreach (var suc in listSuccess)
        {
            if (suc) hitCnt++;
        }

        float atkSpdEff = GetEffectiveAtkSpd(attackerSpeed, hitCnt, maxCnt);
        float dodgeP = GetDodgeChance(atkSpdEff, defenderSpeed);

        return UnityEngine.Random.value < dodgeP;
    }

    //공격자 공격력에 영향을 주는 공격성공 개수
    static float GetEffectiveAtkDamage(float AtkDamage, int successHits, int totalHits)
    {
        float ratio = (float)successHits / totalHits;          // 0~1
        float missRatio = 1f - ratio;
        float penalty = missRatio * penaltyScale;               // 0~penaltyScale
        float damage = AtkDamage * (1f - penalty);             // 선형 감소

        return Mathf.Max(0, damage);                        // 밑바닥 보장
    }


    //공격자 스피드에 영향을 주는 공격성공 개수
    static float GetEffectiveAtkSpd(float AtkSpeed, int successHits, int totalHits)
    {
        float successRatio = Mathf.Clamp01((float)successHits / totalHits);          // 0~1
        float missRatio = 1f - successRatio;
        float penalty = missRatio * speedPenaltyScale;                    // 0~penaltyScale
        float spd = AtkSpeed * (1f - penalty);             // 선형 감소

        return Mathf.Max(0, spd);                        // 밑바닥 보장
    }

    static float GetDodgeChance(float attSpeed, float defSpeed)
    {
        float pRatio = baseDodge;

        if(defSpeed > attSpeed)
        {
            float ratio = defSpeed / attSpeed;
            float x = Mathf.Pow(Mathf.Clamp01(ratio / maxRatio), exponent);
            pRatio = ratioSpdCurve.Evaluate(x);
        }

        float diffAbs = Mathf.Abs(defSpeed - attSpeed);     // 0~∞
        float tAbs = Mathf.Clamp01(diffAbs / 100f);       // 0→100 map
        float pAbs = absSpdCurve.Evaluate(tAbs);

        /* 3) 블렌드 (비율 70 %, 절대 30 %) */
        float chance = Mathf.Lerp(pRatio, pAbs, weightAbs);

        return Mathf.Clamp(chance, baseDodge, maxDodge);
    }

    public static bool GetCritChance(List<bool> listSuccess, float atkSpd, float defSpd)
    {
        int maxCnt = listSuccess.Count;
        int hitCnt = 0;

        foreach (var suc in listSuccess)
        {
            if (suc) hitCnt++;
        }

        if (hitCnt < maxCnt) return false;

        /* 절대 스피드 확률 pAbs */
        float tAbs = Mathf.InverseLerp(absMin, absMax, atkSpd);     // 0~1
        float pAbs = absCurve.Evaluate(Mathf.Clamp01(tAbs));        // 0~1

        /* 상대(비율) 확률 pRel */
        float ratio = defSpd <= 0f ? relMax : atkSpd / defSpd;      // 방어=0 보호
        float tRel = Mathf.InverseLerp(relMin, relMax, ratio);     // 0~1
        float pRel = relCurve.Evaluate(Mathf.Clamp01(tRel));       // 0~1

        /* 가중 평균으로 합성 */
        float pFinal = absSpeedWeight * pAbs            // 절대 파트
                     + (1f - absSpeedWeight) * pRel;    // 상대 파트

        float pCrit = Mathf.Clamp01(pFinal);              // 0 ≤ 확률 ≤ 1

        bool crit = UnityEngine.Random.value < pCrit;

        return crit;
    }

    public static bool GetShieldStrike(List<bool> listSuccess, float atkDef, float defDef)
    {
        int maxCnt = listSuccess.Count;
        int hitCnt = 0;

        foreach (var suc in listSuccess)
        {
            if (suc) hitCnt++;
        }

        if (hitCnt < maxCnt) return false;

        ///* 절대 방어력 확률 pAbs */
        //float tAbs = Mathf.InverseLerp(absMin, absMax, atkDef);     // 0~1
        //float pAbs = absCurve.Evaluate(Mathf.Clamp01(tAbs));        // 0~1

        ///* 상대(비율) 확률 pRel */
        //float ratio = defDef <= 0f ? relMax : atkDef / defDef;      // 방어=0 보호
        //float tRel = Mathf.InverseLerp(relMin, relMax, ratio);     // 0~1
        //float pRel = relCurve.Evaluate(Mathf.Clamp01(tRel));       // 0~1

        ///* 가중 평균으로 합성 */
        //float pFinal = absDefWeight * pAbs            // 절대 파트
        //             + (1f - absDefWeight) * pRel;    // 상대 파트

        //float pFaint = Mathf.Clamp01(pFinal);              // 0 ≤ 확률 ≤ 1

        //bool faint = UnityEngine.Random.value < pFaint;

        return true;
    }
}
