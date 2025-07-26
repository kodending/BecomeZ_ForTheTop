using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

#region 클래스별 애니메이션 정리
[System.Serializable]
public class DicClassAnimator : SerializableDictionary<CLASSTYPE, RuntimeAnimatorController> { }
#endregion

#region 클래스별 Idle 애니메이션 클립정리
[System.Serializable]
public class DicClassIdleAnimClip : SerializableDictionary<CLASSTYPE, AnimationClip> { }
#endregion

public class LobbyPlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [Tooltip("이동 방향 설정")]
    Vector3 m_vecMove;
    float m_fMoveX = 0.0f;
    float m_fMoveY = 0.0f;
    public float m_fMoveSpeed;

    FixedJoystick m_fixJoy;
    [SerializeField] Animator m_anim;

    public PhotonView m_pv;

    public PlayerCanvas m_playerCanvas;

    #region 클래스 정의하는곳
    //public Dictionary<CLASSTYPE, List<GameObject>> m_dicClassForm = new Dictionary<CLASSTYPE, List<GameObject>>();
    //public DicClassAnimator m_dicClassAnim;

    public List<Dictionary<string, object>> m_classInfo;
    public CLASSSTATS m_sCurStats;
    public CLASSTYPE m_eCurClass;
    public List<int> m_listStats = new List<int>();
    public int m_classIdx; // 정보 전달용

    //25.06.20 적용
    public DicClassIdleAnimClip m_dicClassIdleAnimClip;
    #endregion

    public GameObject m_originObj;

    private void Update()
    {
        if (!m_pv.IsMine)
        {
            if(m_classIdx != (int)m_eCurClass)
            {
                NotMineChangeClass(m_classIdx);
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            m_anim.SetTrigger("Sit");
        }
    }

    private void FixedUpdate()
    {
        if (!m_pv.IsMine) return;

        m_fMoveX = m_fixJoy.Horizontal;
        m_fMoveY = m_fixJoy.Vertical;

        m_vecMove = new Vector3(m_fMoveX, 0, m_fMoveY).normalized;

        m_anim.SetBool("isWalk", m_vecMove != Vector3.zero);

        transform.position += m_vecMove * m_fMoveSpeed * Time.deltaTime;

        transform.LookAt(transform.position + m_vecMove);
    }

    public void InitClassFormInfo()
    {
        m_classInfo = GoogleSheetManager.instance.m_dicGoogleData[GOOGLEDATALOADTYPE.CLASSINFO].recordDataList;

        InitClass(CLASSTYPE.KNIGHT);
    }

    public void InitInfo()
    {
        if (!m_pv.IsMine) return;

        GameManager.gm.m_lobbyPlayer = this;
        m_playerCanvas.m_imgMine.gameObject.SetActive(true);
        m_fixJoy = GameObject.Find("Canvas").transform.Find("Fixed Joystick").GetComponent<FixedJoystick>();
        m_pv.Owner.NickName = NetworkManager.nm.m_myPlayFabInfo.DisplayName;

        m_eCurClass = CLASSTYPE.KNIGHT;
        m_classIdx = (int)m_eCurClass;
        InitClass(m_eCurClass);
    }

    void InitClass(CLASSTYPE eType)
    {
        //foreach (var form in m_dicClassForm[eType])
        //{
        //    form.SetActive(true);
        //}

        Transform[] arrChild = m_originObj.GetComponentsInChildren<Transform>(includeInactive: true);

        int idx = (int)eType + 1;

        foreach (var go in arrChild)
        {
            if (go.gameObject.name == idx.ToString())
            {
                go.gameObject.SetActive(true);
            }
        }

        var info = m_classInfo[(int)eType];

        m_sCurStats.Physical_POW = int.Parse(info["PHYSICALPOW"].ToString());
        m_sCurStats.Magic_POW = int.Parse(info["MAGICPOW"].ToString());
        m_sCurStats.HP = int.Parse(info["HP"].ToString());
        m_sCurStats.MENT = int.Parse(info["MENT"].ToString());
        m_sCurStats.Magic_DEF = int.Parse(info["MAGICDEF"].ToString());
        m_sCurStats.Physical_DEF = int.Parse(info["PHYSICALDEF"].ToString());
        m_sCurStats.SPD = int.Parse(info["SPD"].ToString());
        m_sCurStats.INSIGHT = int.Parse(info["INSIGHT"].ToString());

        m_listStats.Add(m_sCurStats.Physical_POW);
        m_listStats.Add(m_sCurStats.Magic_POW);
        m_listStats.Add(m_sCurStats.HP);
        m_listStats.Add(m_sCurStats.MENT);
        m_listStats.Add(m_sCurStats.Magic_DEF);
        m_listStats.Add(m_sCurStats.Physical_DEF);
        m_listStats.Add(m_sCurStats.SPD);
        m_listStats.Add(m_sCurStats.INSIGHT);

        //m_anim.runtimeAnimatorController = m_dicClassAnim[eType];

        var overrideCtrl = new AnimatorOverrideController(m_anim.runtimeAnimatorController);
        overrideCtrl["IdleA"] = m_dicClassIdleAnimClip[eType];

        m_anim.runtimeAnimatorController = overrideCtrl;
    }

    public void ChangeForm(int idxType)
    {
        Transform[] arrChild = m_originObj.GetComponentsInChildren<Transform>(includeInactive: true);

        int curIdx = (int)m_eCurClass + 1;
        int changeIdx = idxType + 1;

        foreach (var go in arrChild)
        {
            if (go.gameObject.name == curIdx.ToString())
            {
                go.gameObject.SetActive(false);
            }

            if (go.gameObject.name == changeIdx.ToString())
            {
                go.gameObject.SetActive(true);
            }
        }

        //이전 클래스 폼은 비활성화
        //foreach (var form in m_dicClassForm[m_eCurClass])
        //{
        //    form.SetActive(false);
        //}

        GameManager.m_curUI.GetComponent<UIManagerInWait>().m_classAvatar.ActiveForm(m_eCurClass, false);

        ////현재 바뀌라고 하는 폼은 활성화
        //foreach (var form in m_dicClassForm[(CLASSTYPE)idxType])
        //{
        //    form.SetActive(true);
        //}

        GameManager.m_curUI.GetComponent<UIManagerInWait>().m_classAvatar.ActiveForm((CLASSTYPE)idxType, true);

        SetStats(idxType);
        GameManager.m_curUI.GetComponent<UIManagerInWait>().ChangeStatUI();

        //m_anim.runtimeAnimatorController = m_dicClassAnim[(CLASSTYPE)idxType];

        var overrideCtrl = new AnimatorOverrideController(m_anim.runtimeAnimatorController);
        overrideCtrl["IdleA"] = m_dicClassIdleAnimClip[(CLASSTYPE)idxType];

        m_anim.runtimeAnimatorController = overrideCtrl;

        m_eCurClass = (CLASSTYPE)idxType;
        m_classIdx = (int)m_eCurClass;
    }

    void SetStats(int idxType)
    {
        var info = m_classInfo[idxType];

        m_sCurStats.Physical_POW = int.Parse(info["PHYSICALPOW"].ToString());
        m_sCurStats.Magic_POW = int.Parse(info["MAGICPOW"].ToString());
        m_sCurStats.HP = int.Parse(info["HP"].ToString());
        m_sCurStats.MENT = int.Parse(info["MENT"].ToString());
        m_sCurStats.Magic_DEF = int.Parse(info["MAGICDEF"].ToString());
        m_sCurStats.Physical_DEF = int.Parse(info["PHYSICALDEF"].ToString());
        m_sCurStats.SPD = int.Parse(info["SPD"].ToString());
        m_sCurStats.INSIGHT = int.Parse(info["INSIGHT"].ToString());
        GameManager.m_curUI.GetComponent<UIManagerInWait>().m_txtClassName.text = info["CLASS"].ToString();

        if (m_listStats.Count > 0) m_listStats.Clear();

        m_listStats.Add(m_sCurStats.Physical_POW);
        m_listStats.Add(m_sCurStats.Magic_POW);
        m_listStats.Add(m_sCurStats.HP);
        m_listStats.Add(m_sCurStats.MENT);
        m_listStats.Add(m_sCurStats.Magic_DEF);
        m_listStats.Add(m_sCurStats.Physical_DEF);
        m_listStats.Add(m_sCurStats.SPD);
        m_listStats.Add(m_sCurStats.INSIGHT);

        for (int i = 0; i< m_listStats.Count; i++)
        {
            GameManager.m_curUI.GetComponent<UIManagerInWait>().m_arrStatTexts[i].text = m_listStats[i].ToString();
        }
    }

    void NotMineChangeClass(int classIdx)
    {
        Transform[] arrChild = m_originObj.GetComponentsInChildren<Transform>(includeInactive: true);

        int curIdx = (int)m_eCurClass + 1;
        int changeIdx = classIdx + 1;

        foreach (var go in arrChild)
        {
            if (go.gameObject.name == changeIdx.ToString())
            {
                go.gameObject.SetActive(true);
            }

            if (go.gameObject.name == curIdx.ToString())
            {
                go.gameObject.SetActive(false);
            }
        }

        //foreach (var form in m_dicClassForm[m_eCurClass])
        //{
        //    form.SetActive(false);
        //}

        ////현재 바뀌라고 하는 폼은 활성화
        //foreach (var form in m_dicClassForm[(CLASSTYPE)classIdx])
        //{
        //    form.SetActive(true);
        //}

        SetStats(classIdx);

        //m_anim.runtimeAnimatorController = m_dicClassAnim[(CLASSTYPE)classIdx];
        var overrideCtrl = new AnimatorOverrideController(m_anim.runtimeAnimatorController);
        overrideCtrl["IdleA"] = m_dicClassIdleAnimClip[(CLASSTYPE)classIdx];

        m_anim.runtimeAnimatorController = overrideCtrl;

        m_eCurClass = (CLASSTYPE)classIdx;
    }

    [PunRPC]
    public void ClearForm()
    {
        //이전 클래스 폼은 비활성화
        //foreach (var form in m_dicClassForm[m_eCurClass])
        //{
        //    form.SetActive(false);
        //}

        Transform[] arrChild = m_originObj.GetComponentsInChildren<Transform>(includeInactive: true);

        int curIdx = (int)m_eCurClass;

        foreach (var go in arrChild)
        {
            if (go.gameObject.name == curIdx.ToString())
            {
                go.gameObject.SetActive(false);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.m_classIdx);
        }

        else
        {
            m_classIdx = (int)stream.ReceiveNext();
        }
    }
}
