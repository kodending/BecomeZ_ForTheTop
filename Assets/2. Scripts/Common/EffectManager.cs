using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EffectManager : MonoBehaviourPunCallbacks
{
    public static EffectManager em;

    [SerializeField]
    public GameObject m_goEffPrefab;

    static Queue<GameObject> m_poolingEffects = new Queue<GameObject>();

    private void Awake()
    {
        em = this;
        DontDestroyOnLoad(em);
    }

    static public GameObject Instantiate(EFFECTTYPE eType, Vector3 goOwner, Vector3 goTarget, bool bHoming, float fTimer)
    {
        if(m_poolingEffects.Count > 0)
        {
            var go = m_poolingEffects.Dequeue();
            go.GetComponent<EffectInfo>().SetEffectObj(eType, goOwner, goTarget, bHoming, fTimer);
            return go;
        }

        else
        {
            GameObject go = Instantiate(em.m_goEffPrefab);
            go.transform.SetParent(em.transform);
            go.GetComponent<EffectInfo>().SetEffectObj(eType, goOwner, goTarget, bHoming, fTimer);
            return go;
        }
    }

    static public void ReturnEffObj(GameObject eff)
    {
        var effInfo = eff.GetComponent<EffectInfo>();

        effInfo.m_fTimer = 0;
        effInfo.m_fTimerForLim = 0;
        effInfo.m_goCurEffObj.SetActive(false);
        eff.SetActive(false);
        m_poolingEffects.Enqueue(eff);
    }
}
