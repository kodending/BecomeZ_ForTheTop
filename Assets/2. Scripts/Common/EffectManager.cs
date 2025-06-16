using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EffectManager : MonoBehaviourPunCallbacks
{
    public static EffectManager em;

    [SerializeField]
    GameObject m_goEffPrefab;

    Queue<GameObject> m_poolingEffects = new Queue<GameObject>();

    private void Awake()
    {
        em = this;
        DontDestroyOnLoad(em);
    }

    private void Update()
    {
        //타이머 이후 없애기
        //EffectThread();
    }

    //void EffectThread()
    //{
    //    EffectInfo[] arrEffs = GetComponentsInChildren<EffectInfo>();

    //    if(arrEffs != null)
    //    {
    //        foreach(var eff in arrEffs)
    //        {
    //            if (!eff.gameObject.activeSelf) continue;

    //            eff.m_fTimer += Time.deltaTime;

    //            if(eff.m_fTimer > eff.m_fTimerForLim)
    //            {
    //                ReturnEffObj(eff.gameObject);
    //            }

    //            else
    //            {
    //                eff.EffectMove();
    //            }
    //        }
    //    }
    //}

    public GameObject Instantiate(EFFECTTYPE eType, GameObject goOwner, GameObject goTarget, bool bHoming, float fTimer)
    {
        if(m_poolingEffects.Count > 0)
        {
            var go = m_poolingEffects.Dequeue();
            go.GetComponent<EffectInfo>().SetEffectObj(eType, goOwner, goTarget, bHoming, fTimer);
            return go;
        }

        else
        {
            GameObject go = Instantiate(m_goEffPrefab);
            go.transform.SetParent(this.transform);
            go.GetComponent<EffectInfo>().SetEffectObj(eType, goOwner, goTarget, bHoming, fTimer);
            return go;
        }

        return null;
    }

    public void ReturnEffObj(GameObject eff)
    {
        var effInfo = eff.GetComponent<EffectInfo>();

        int iTypeNum = (int)effInfo.m_eEffectType;
        effInfo.m_goCurEffObj = transform.GetChild(iTypeNum).gameObject;
        effInfo.m_goCurEffObj.SetActive(false);
        eff.SetActive(false);
        m_poolingEffects.Enqueue(eff);
    }
}
