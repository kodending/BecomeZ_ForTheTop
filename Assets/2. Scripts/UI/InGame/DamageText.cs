using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public Text text;
    private Action<GameObject> returnToPool;

    public void Init(int damage, int atkResult, Action<GameObject> returnAction)
    {
        switch ((ATTACKRESULT)atkResult)
        {
            case ATTACKRESULT.PHYSICAL_ATTACK:
            case ATTACKRESULT.MAGIC_ATTACK:
            case ATTACKRESULT.PHYSICAL_PIERCING_ATTACK:
            case ATTACKRESULT.MAGIC_PIERCING_ATTACK:
            case ATTACKRESULT.PHYSICAL_AREA_ATTACK:
            case ATTACKRESULT.MAGIC_AREA_ATTACK:
            case ATTACKRESULT.SHIELD_SOLO_STRIKE:
            case ATTACKRESULT.SHIELD_AREA_STRIKE:
            case ATTACKRESULT.PHYSICAL_DEF_DEBUFF_SOLO:
            case ATTACKRESULT.MAGIC_DEF_DEBUFF_SOLO:
            case ATTACKRESULT.PHYSICAL_ATK_DEBUFF_SOLO:
            case ATTACKRESULT.MAGIC_ATK_DEBUFF_SOLO:

                text.text = damage.ToString();

                break;

            case ATTACKRESULT.MISS:
                text.text = "ȸ��";
                break;

            case ATTACKRESULT.BLOCK:
                text.text = "���";
                break;
        }

        returnToPool = returnAction;
        gameObject.SetActive(true);

        // ��ġ ��¦ ���� �̵� + ���̵� �ƿ�
        transform.DOMove(transform.position + new Vector3(0, 0.8f, 0), 1f).SetEase(Ease.OutCubic);
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = 1f;
        cg.DOFade(0f, 2f).OnComplete(() =>
        {
            returnToPool?.Invoke(gameObject);
        });
    }
    void LateUpdate()
    {
        Vector3 dir = transform.position - Camera.main.transform.position;
        dir.y = 0;  // ���� ȸ�� ����
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
