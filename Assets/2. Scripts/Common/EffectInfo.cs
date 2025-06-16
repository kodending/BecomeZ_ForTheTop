using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInfo : MonoBehaviour
{
    public List<GameObject> m_effObj = new List<GameObject>();

    public EFFECTTYPE m_eEffectType = EFFECTTYPE._MAX_;

    //이펙트를 쐈을때
    //이펙트의 주인이 에너미인지 플레이어인지 알아야한다.
    public bool m_bPlayer;

    //이펙트 주인이 누군지알아야하고
    public GameObject m_goOwner;

    //목표를 추적하는건지 알아야 하고
    public bool m_bHoming;

    //지속인지, 즉각인지 알아야됨.
    //0턴이면 즉각, 1턴 이상부터는 지속으로 본다.
    public int m_keepTime = 0;

    //누구한테 붙은 이펙트인지 알아야 됨.
    public GameObject m_goTarget;

    //타이머 있어야하고
    //public float m_fTimer;

    //남은 타이머시간 알아야하고
    //public float m_fTimerForLim;

    //현재 이펙트가 무엇인지 알아야하고
    public GameObject m_goCurEffObj;


    private void FixedUpdate()
    {
        EffectMove();
    }

    //public void SetEffectObj(EFFECTTYPE eType, GameObject goOwner, Vector3 vPos, bool bHoming, float fTimer)

    public void SetEffectObj(EFFECTTYPE eType, GameObject goOwner, GameObject goTarget, bool bHoming, float fTimer)
    {
        m_eEffectType = eType;
        m_goOwner = goOwner;
        m_bHoming = bHoming;
        m_goTarget = goTarget;
        //m_fTimerForLim = fTimer;
        //m_fTimer = 0;

        if(m_goTarget != null)
        {
            transform.position = m_goTarget.transform.position;
        }

        SetInit();
    }

    public void SetInit()
    {
        int iTypeNum = (int)m_eEffectType;
        m_goCurEffObj = transform.GetChild(iTypeNum).gameObject;
        m_goCurEffObj.SetActive(true);
        gameObject.SetActive(true);
    }

    public void EffectMove()
    {
        if (m_goTarget == null) return;

        if(m_bHoming)
        {
            transform.position = m_goTarget.transform.position;
        }
    }
}
