using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class EffectInfo : MonoBehaviour
{
    public List<GameObject> m_effObj = new List<GameObject>();

    public EFFECTTYPE m_eEffectType = EFFECTTYPE._MAX_;

    //����Ʈ�� ������
    //����Ʈ�� ������ ���ʹ����� �÷��̾����� �˾ƾ��Ѵ�.
    public bool m_bPlayer;

    //����Ʈ ������ �������˾ƾ��ϰ�
    public Vector3 m_vOwner;

    //��ǥ�� �����ϴ°��� �˾ƾ� �ϰ�
    public bool m_bHoming;

    //��������, �ﰢ���� �˾ƾߵ�.
    //0���̸� �ﰢ, 1�� �̻���ʹ� �������� ����.
    public int m_keepTime = 0;

    //�������� ���� ����Ʈ���� �˾ƾ� ��.
    public Vector3 m_vTarget;

    //Ÿ�̸� �־���ϰ�
    public float m_fTimer;

    //���� Ÿ�̸ӽð� �˾ƾ��ϰ�
    public float m_fTimerForLim;

    //���� ����Ʈ�� �������� �˾ƾ��ϰ�
    public GameObject m_goCurEffObj;


    //private void FixedUpdate()
    //{
    //    EffectMove();
    //}

    //public void SetEffectObj(EFFECTTYPE eType, GameObject goOwner, Vector3 vPos, bool bHoming, float fTimer)

    public void SetEffectObj(EFFECTTYPE eType, Vector3 vOwner, Vector3 vTarget, bool bHoming, float fTimer)
    {
        m_eEffectType = eType;
        m_vOwner = vOwner;
        m_bHoming = bHoming;
        m_vTarget = vTarget;
        m_fTimerForLim = fTimer;
        m_fTimer = 0;

        if(m_vTarget != null)
        {
            Vector3 pos = m_vTarget;
            m_vTarget = pos;
            transform.position = m_vOwner;
        }

        SetInit();

        StartCoroutine(EffectTimer(m_fTimerForLim));

        if (bHoming) StartCoroutine(EffectMoveTarget());
        else transform.position = m_vTarget;
    }

    public void SetInit()
    {
        int iTypeNum = (int)m_eEffectType;
        m_goCurEffObj = transform.GetChild(iTypeNum).gameObject;
        m_goCurEffObj.SetActive(true);
        gameObject.SetActive(true);
    }

    IEnumerator EffectMoveTarget()
    {
        yield return transform.DOMove(m_vTarget, m_fTimerForLim).SetEase(Ease.Linear); //��� �
    }

    IEnumerator EffectTimer(float fTimerLim)
    {
        yield return new WaitForSeconds(fTimerLim);

        EffectManager.ReturnEffObj(gameObject);
    }
}
