using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ClassAvatar : MonoBehaviourPunCallbacks
{
    [SerializeField] Animator m_anim;
    ////[SerializeField] List<GameObject> m_listKnightForm;
    ////[SerializeField] List<GameObject> m_listDarkKnightForm;

    ////public Dictionary<CLASSTYPE, List<GameObject>> m_dicClassForm = new Dictionary<CLASSTYPE, List<GameObject>>();
    //public DicClassAnimator m_dicClassAnim;

    public GameObject m_originObj;

    //25.06.20 Àû¿ë
    public DicClassIdleAnimClip m_dicClassIdleAnimClip;

    void InitClassFormInfo()
    {

    }

    public void ActiveForm(CLASSTYPE eType, bool bActive)
    {
        //if(m_dicClassForm.Count <= 0)
        //{
        //    InitClassFormInfo();
        //}

        //foreach (var form in m_dicClassForm[eType])
        //{
        //    form.SetActive(bActive);
        //}

        Transform[] arrChild = m_originObj.GetComponentsInChildren<Transform>(includeInactive: true);

        int Idx = (int)eType + 1;

        foreach (var go in arrChild)
        {
            if (go.gameObject.name == Idx.ToString())
            {
                go.gameObject.SetActive(bActive);
            }
        }

        if (!bActive) return;

        //m_anim.runtimeAnimatorController = m_dicClassAnim[eType];

        var overrideCtrl = new AnimatorOverrideController(m_anim.runtimeAnimatorController);
        overrideCtrl["IdleA"] = m_dicClassIdleAnimClip[eType];

        m_anim.runtimeAnimatorController = overrideCtrl;
    }
}
