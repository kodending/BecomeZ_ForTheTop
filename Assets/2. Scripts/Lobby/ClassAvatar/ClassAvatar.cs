using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassAvatar : MonoBehaviourPunCallbacks
{
    [SerializeField] Animator m_anim;
    [SerializeField] List<GameObject> m_listKnightForm;
    [SerializeField] List<GameObject> m_listDarkKnightForm;

    public Dictionary<CLASSTYPE, List<GameObject>> m_dicClassForm = new Dictionary<CLASSTYPE, List<GameObject>>();
    public DicClassAnimator m_dicClassAnim;

    void InitClassFormInfo()
    {
        if (m_dicClassForm.Count > 0) m_dicClassForm.Clear();

        m_dicClassForm.Add(CLASSTYPE.KNIGHT, m_listKnightForm);
        m_dicClassForm.Add(CLASSTYPE.DARKKNIGHT, m_listDarkKnightForm);
    }

    public void ActiveForm(CLASSTYPE eType, bool bActive)
    {
        if(m_dicClassForm.Count <= 0)
        {
            InitClassFormInfo();
        }

        foreach (var form in m_dicClassForm[eType])
        {
            form.SetActive(bActive);
        }

        if (!bActive) return;

        m_anim.runtimeAnimatorController = m_dicClassAnim[eType];
    }
}
