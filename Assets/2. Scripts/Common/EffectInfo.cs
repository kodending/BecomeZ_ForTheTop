using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInfo : MonoBehaviour
{
    public List<GameObject> m_effObj = new List<GameObject>();

    public EFFECTTYPE m_eEffectType = EFFECTTYPE._MAX_;

    //����Ʈ�� ������
    //����Ʈ�� ������ ���ʹ����� �÷��̾����� �˾ƾ��Ѵ�.
    public bool m_bPlayer;

    //����Ʈ ������ �������˾ƾ��ϰ�
    public GameObject m_goOwner;

    //��ǥ�� �����ϴ°��� �˾ƾ� �ϰ�
    public bool m_bHoming;

    //��������, �ﰢ���� �˾ƾߵ�.
    //0���̸� �ﰢ, 1�� �̻���ʹ� �������� ����.
    public int m_keepTime = 0;

    //�������� ���� ����Ʈ���� �˾ƾ� ��.
    public GameObject m_goTarget;

    //Ÿ�̸� �־���ϰ�
    //public float m_fTimer;

    //���� Ÿ�̸ӽð� �˾ƾ��ϰ�
    //public float m_fTimerForLim;

    //���� ����Ʈ�� �������� �˾ƾ��ϰ�
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
