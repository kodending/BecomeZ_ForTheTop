using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class EffectInfo : MonoBehaviour
{
    public List<GameObject> m_effObj = new List<GameObject>();

    public EFFECTTYPE m_eEffectType = EFFECTTYPE._MAX_;

    //이펙트를 쐈을때
    //이펙트의 주인이 에너미인지 플레이어인지 알아야한다.
    public bool m_bPlayer;

    //이펙트 주인이 누군지알아야하고
    public Vector3 m_vOwner;

    //목표를 추적하는건지 알아야 하고
    public bool m_bHoming;

    //지속인지, 즉각인지 알아야됨.
    //0턴이면 즉각, 1턴 이상부터는 지속으로 본다.
    public int m_keepTime = 0;

    //누구한테 붙은 이펙트인지 알아야 됨.
    public Vector3 m_vTarget;

    //타이머 있어야하고
    public float m_fTimer;

    //남은 타이머시간 알아야하고
    public float m_fTimerForLim;

    //현재 이펙트가 무엇인지 알아야하고
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
        yield return transform.DOMove(m_vTarget, m_fTimerForLim).SetEase(Ease.Linear); //등속 운동
    }

    IEnumerator EffectTimer(float fTimerLim)
    {
        yield return new WaitForSeconds(fTimerLim);

        EffectManager.ReturnEffObj(gameObject);
    }
}
