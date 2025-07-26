using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class AttackWeight
{
    [Range(0f, 1f)] static float baseDodge = 0.05f;      // �ּ� ȸ�� 5 %
    [Range(0f, 1f)] static float maxDodge = 0.85f;      // ���� 85 %

    static float maxRatio = 4f;   // ���/���� �ӵ��� �� ������ �����ϸ� ����
    static float exponent = 1.5f; // 1 = ����, 2 = ����, 0.5 = ��Ʈ

    static public AnimationCurve ratioSpdCurve =
    AnimationCurve.EaseInOut(0f, 0.05f, 1f, 0.85f);

    static public AnimationCurve absSpdCurve =
        AnimationCurve.EaseInOut(0f, 0.05f, 1f, 0.85f);

    [Range(0f, 1f)] static float weightAbs = 0.65f;   // ���� �� % ����

    static float penaltyScale = 1f;                       // 0 => �г�Ƽ ����, 1 => ��������ŭ 100% ����
    static float speedPenaltyScale = 0.7f;                       // 0 => �г�Ƽ ����, 1 => ��������ŭ 100% ����

    /* �� ���� ���ǵ� Ŀ�� ---------------------------------- */
    [Header("Absolute-Speed (atk)")]
    static float absMin = 60f;                     // ���ϸ� curve X = 0
    static float absMax = 180f;                     // �� �̻��� curve X = 1
    static AnimationCurve absCurve = new AnimationCurve(
        new Keyframe(0f, 0.05f),                    // 5 %
        new Keyframe(1f, 0.50f));                   // 50 %

    /* �� ��� ���ǵ�(����) Ŀ�� ------------------------------ */
    [Header("Relative-Speed (atk / def)")]
    static float relMin = 0.5f;                     // ���� �ӵ�, ����
    static float relMax = 3.0f;                     // 3 �� �ӵ�, ����
    static AnimationCurve relCurve = new AnimationCurve(
        new Keyframe(0f, 0.05f),                    // 5 %
        new Keyframe(1f, 0.90f));                   // 90 %

    /* �� �� Ŀ�긦 ���� ����ġ ------------------------------ */
    [Header("Mix-Ratio")]
    [Range(0f, 1f)]
    static float absSpeedWeight = 0.8f;                  // 0.8 = ���� 80 %, ��� 20 % ���ǵ忡 ���� ����ġ

    [Header("Mix-Ratio")]
    [Range(0f, 1f)]
    static float absDefWeight = 0.2f;                  // 0.8 = ���� 80 %, ��� 20 % ���� ���� ����ġ

    public static bool AttackSuccessCal(float iProb, bool isPenet)
    {
        if (isPenet) return true;

        //���ο� Ȯ�� ����
        iProb = Mathf.Clamp01(iProb);
        bool bSuccess = new bool();

        bSuccess = UnityEngine.Random.value < iProb;

        return bSuccess;
    }

    public static int AttackDamageCal(List<bool> listSuccess, int iAtk, bool bCritcal)
    {
        //������ ����ϴºκ� �ٽ� �������
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

    //������ ���ݷ¿� ������ �ִ� ���ݼ��� ����
    static float GetEffectiveAtkDamage(float AtkDamage, int successHits, int totalHits)
    {
        float ratio = (float)successHits / totalHits;          // 0~1
        float missRatio = 1f - ratio;
        float penalty = missRatio * penaltyScale;               // 0~penaltyScale
        float damage = AtkDamage * (1f - penalty);             // ���� ����

        return Mathf.Max(0, damage);                        // �عٴ� ����
    }


    //������ ���ǵ忡 ������ �ִ� ���ݼ��� ����
    static float GetEffectiveAtkSpd(float AtkSpeed, int successHits, int totalHits)
    {
        float successRatio = Mathf.Clamp01((float)successHits / totalHits);          // 0~1
        float missRatio = 1f - successRatio;
        float penalty = missRatio * speedPenaltyScale;                    // 0~penaltyScale
        float spd = AtkSpeed * (1f - penalty);             // ���� ����

        return Mathf.Max(0, spd);                        // �عٴ� ����
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

        float diffAbs = Mathf.Abs(defSpeed - attSpeed);     // 0~��
        float tAbs = Mathf.Clamp01(diffAbs / 100f);       // 0��100 map
        float pAbs = absSpdCurve.Evaluate(tAbs);

        /* 3) ���� (���� 70 %, ���� 30 %) */
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

        /* ���� ���ǵ� Ȯ�� pAbs */
        float tAbs = Mathf.InverseLerp(absMin, absMax, atkSpd);     // 0~1
        float pAbs = absCurve.Evaluate(Mathf.Clamp01(tAbs));        // 0~1

        /* ���(����) Ȯ�� pRel */
        float ratio = defSpd <= 0f ? relMax : atkSpd / defSpd;      // ���=0 ��ȣ
        float tRel = Mathf.InverseLerp(relMin, relMax, ratio);     // 0~1
        float pRel = relCurve.Evaluate(Mathf.Clamp01(tRel));       // 0~1

        /* ���� ������� �ռ� */
        float pFinal = absSpeedWeight * pAbs            // ���� ��Ʈ
                     + (1f - absSpeedWeight) * pRel;    // ��� ��Ʈ

        float pCrit = Mathf.Clamp01(pFinal);              // 0 �� Ȯ�� �� 1

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

        ///* ���� ���� Ȯ�� pAbs */
        //float tAbs = Mathf.InverseLerp(absMin, absMax, atkDef);     // 0~1
        //float pAbs = absCurve.Evaluate(Mathf.Clamp01(tAbs));        // 0~1

        ///* ���(����) Ȯ�� pRel */
        //float ratio = defDef <= 0f ? relMax : atkDef / defDef;      // ���=0 ��ȣ
        //float tRel = Mathf.InverseLerp(relMin, relMax, ratio);     // 0~1
        //float pRel = relCurve.Evaluate(Mathf.Clamp01(tRel));       // 0~1

        ///* ���� ������� �ռ� */
        //float pFinal = absDefWeight * pAbs            // ���� ��Ʈ
        //             + (1f - absDefWeight) * pRel;    // ��� ��Ʈ

        //float pFaint = Mathf.Clamp01(pFinal);              // 0 �� Ȯ�� �� 1

        //bool faint = UnityEngine.Random.value < pFaint;

        return true;
    }
}
