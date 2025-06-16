using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class InGamePlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public PlayerStateMachine m_stateMachine { get; private set; }

    public NavMeshAgent m_agent;
    public PhotonView m_pv;
    public PlayerCanvas m_playerCanvas;
    public Animator m_anim;

    #region 클래스 정의
    [SerializeField] List<GameObject> m_listKnightForm;
    [SerializeField] List<GameObject> m_listDarkKnightForm;

    public Dictionary<CLASSTYPE, List<GameObject>> m_dicClassForm = new Dictionary<CLASSTYPE, List<GameObject>>();
    public CLASSTYPE m_eCurClass;
    public DicClassAnimator m_dicClassAnim;
    public CLASSSTATS m_sCurStats;
    public int m_classIdx; // 정보 전달용

    public List<Dictionary<string, object>> m_classInfo;
    #endregion

    public Color m_myColor;

    public EnemyFSM m_enemySelected;

    public List<bool> m_listAtkSuccess = new List<bool>();

    public GameObject m_goShadow;

    public Vector3 m_vMovePos;
    public float m_fMoveTime;

    #region 렉돌 정의
    public GameObject m_originObj;
    public GameObject m_ragdollObj;
    public Rigidbody[] m_arrRigid;
    #endregion

    private void Update()
    {
        if(!m_pv.IsMine)
        {
            if(m_classIdx != (int)m_eCurClass)
            {
                InitClass((CLASSTYPE)m_classIdx);
            }
            return;
        }

        m_stateMachine.currentState?.OnUpdateState();
    }

    private void FixedUpdate()
    {
        if (!m_pv.IsMine) return;

        m_stateMachine.currentState?.OnFixedUpdateState();
    }

    public void InitClassFormInfo()
    {
        m_classInfo = CSVManager.instance.m_dicData[LOCALDATALOADTYPE.CLASSINFO].recordDataList;

        if (m_dicClassForm.Count > 0) m_dicClassForm.Clear();

        m_dicClassForm.Add(CLASSTYPE.KNIGHT, m_listKnightForm);
        m_dicClassForm.Add(CLASSTYPE.DARKKNIGHT, m_listDarkKnightForm);
        m_eCurClass = CLASSTYPE._MAX_;
    }

    public void InitInfo()
    {
        InitStateMachine();

        if (!m_pv.IsMine) return;

        GameManager.gm.m_inGamePlayer = this;
        m_playerCanvas.m_imgMine.gameObject.SetActive(true);
        m_pv.Owner.NickName = NetworkManager.nm.m_myPlayFabInfo.DisplayName;
        m_agent.SetDestination(BattleManager.bm.m_trPlayerPos[GameManager.gm.m_BattlePosIdx].position);
        m_classIdx = GameManager.gm.m_SavedClassIdx;
        InitClass((CLASSTYPE)m_classIdx);
    }

    public void RefreshInfo()
    {
        m_pv.RPC("RefreshUserInfoRPC", RpcTarget.All, GameManager.gm.m_BattlePosIdx);
    }

    [PunRPC]
    void RefreshUserInfoRPC(int userIdx)
    {
        m_myColor = BattleManager.bm.m_listUserColors[userIdx];

        //여기로 추가 정보 전달하면 될듯
        //나 몇번쨰 칸에 내 정보를 전달하겠소
        RefreshUIInfo(userIdx);
    }

    void RefreshUIInfo(int userIdx)
    {
        GameManager.m_curUI.GetComponent<UIManagerInGame>().RefreshUserHud(userIdx, m_sCurStats);
    }

    void InitStateMachine()
    {
        m_stateMachine = new PlayerStateMachine(PLAYERSTATE.IDLE, new Player_Idle(this));
        m_stateMachine.AddState(PLAYERSTATE.MOVE, new Player_Move(this));
        m_stateMachine.AddState(PLAYERSTATE.ATTACK, new Player_Attack(this));
        m_stateMachine.AddState(PLAYERSTATE.ATTACK_MOVE, new Player_AttackMove(this));
        m_stateMachine.AddState(PLAYERSTATE.BACK_MOVE, new Player_BackMove(this));
        m_stateMachine.AddState(PLAYERSTATE.HIT, new Player_Hit(this));
        m_stateMachine.AddState(PLAYERSTATE.DEATH, new Player_Death(this));
        m_stateMachine.AddState(PLAYERSTATE.RETURN_MOVE, new Player_ReturnMove(this));
        m_stateMachine.AddState(PLAYERSTATE.DODGE, new Player_Dodge(this));
        m_stateMachine.AddState(PLAYERSTATE.BLOCK, new Player_Block(this));

        m_stateMachine.currentState?.OnEnterState();
    }

    [PunRPC]
    public void AnimTriggerRPC(string animName)
    {
        m_anim.SetTrigger(animName);
    }

    [PunRPC]
    public void AnimBoolRPC(string animName, bool bActive)
    {
        m_anim.SetBool(animName, bActive);
    }

    [PunRPC]
    public void AnimFloatRPC(string animName, float fValue)
    {
        m_anim.SetFloat(animName, fValue);
    }

    [PunRPC]
    public void DoMoveRPC(Vector3 pos, float fDuration, Ease ease)
    {
        transform.DOMove(pos, fDuration).SetEase(ease);
    }

    [PunRPC]
    public void AttackHitRPC(int playerIdx, int iAtk, bool isCritcal)
    {
        var uiInfo = GameManager.m_curUI.GetComponent<UIManagerInGame>().m_arrUserInfo[playerIdx];

        m_sCurStats.curHp -= iAtk;

        if (m_sCurStats.curHp < 0) m_sCurStats.curHp = 0;

        if (isCritcal)
            BattleManager.ShakeCamera(0.5f, 0.08f);
        else
            BattleManager.ShakeCamera(3.5f, 0.08f);

        UIManager.CountText(uiInfo.curHpText, m_sCurStats.curHp);

        //체력도 계산하고
        //0 이하일경우 데스되게 하기
        //크리티컬 들어갔는지 여부도 받기
        //여기서 자기 체력 UI 조절하고
        RefreshHp(uiInfo);

        if (m_sCurStats.curHp <= 0)
        {
            //uiInfo.gameObject.SetActive(false);
            m_stateMachine.ChangeState(PLAYERSTATE.DEATH);
            return;
        }

        m_stateMachine.ChangeState(PLAYERSTATE.HIT);
    }

    void RefreshHp(UserUIInfo info)
    {
        float value = (float)m_sCurStats.curHp / (float)m_sCurStats.maxHp;
        info.sliderHp.value = value;
        StartCoroutine(HpMotion(info, value));
    }

    IEnumerator HpMotion(UserUIInfo info, float value)
    {
        float delta = 0;
        float duration = 1.0f;

        while (delta <= duration)
        {
            float t = delta / duration;
            //t = Mathf.Sin((t * Mathf.PI) / 2);
            t = t * t * t * t * t;

            info.sliderHpBack.value = Mathf.Lerp(info.sliderHpBack.value, value, t);

            delta += Time.deltaTime;
            yield return null;
        }

        info.sliderHpBack.value = value;

        info.curMaxHpText.text = m_sCurStats.curHp.ToString() + " / " + m_sCurStats.maxHp.ToString();
    }

    [PunRPC]
    public void AttackBlockRPC(int playerIdx)
    {
        BattleManager.ShakeCamera(0.5f, 0.08f);
        m_stateMachine.ChangeState(PLAYERSTATE.BLOCK);
    }

    [PunRPC]
    public void AttackDodgeRPC(int playerIdx)
    {
        m_stateMachine.ChangeState(PLAYERSTATE.DODGE);
    }

    void InitClass(CLASSTYPE eType)
    {
        foreach (var form in m_dicClassForm[eType])
        {
            form.SetActive(true);
        }

        m_sCurStats = UserDataManager.LoadUserStats();
        
        if(m_sCurStats.name == null)
        {
            var info = m_classInfo[(int)eType];

            m_sCurStats.HP = int.Parse(info["HP"].ToString());
            m_sCurStats.Physical_POW = int.Parse(info["PHYSICALPOW"].ToString());
            m_sCurStats.Magic_POW = int.Parse(info["MAGICPOW"].ToString());
            m_sCurStats.Physical_DEF = int.Parse(info["PHYSICALDEF"].ToString());
            m_sCurStats.Magic_DEF = int.Parse(info["MAGICDEF"].ToString());
            m_sCurStats.MENT = int.Parse(info["MENT"].ToString());
            m_sCurStats.SPD = int.Parse(info["SPD"].ToString());
            m_sCurStats.INSIGHT = int.Parse(info["INSIGHT"].ToString());
            m_sCurStats.curInsight = m_sCurStats.INSIGHT;
            m_sCurStats.name = m_pv.Owner.NickName;
            m_sCurStats.attribute = BattleManager.bm.m_listAttributeSprites[(int)eType];
            m_sCurStats.maxHp = m_sCurStats.HP * 5;
            m_sCurStats.curHp = m_sCurStats.maxHp;
            m_sCurStats.maxMp = m_sCurStats.MENT * 5;
            m_sCurStats.curMp = m_sCurStats.maxMp;
            m_sCurStats.eClass = eType;
            m_sCurStats.listSkills = new List<SKILLINFO>();

            SKILLINFO skill = new SKILLINFO();

            foreach (var skillInfo in ItemManager.im.m_skillInfo)
            {
                int skillIdx = int.Parse(skillInfo["INDEX"].ToString());
                int classIdx = int.Parse(skillInfo["CLASSTYPE"].ToString());

                if (skillIdx == 0 && classIdx == (int)m_sCurStats.eClass)
                {
                    skill.idx = skillIdx;
                    skill.name = skillInfo["NAME"].ToString();
                    skill.iClassType = classIdx;
                    skill.iAttackType = int.Parse(skillInfo["ATTACKTYPE"].ToString());
                    skill.iSkillType = int.Parse(skillInfo["SKILLTYPE"].ToString());
                    skill.fAttackPow = float.Parse(skillInfo["ATTACK"].ToString());
                    skill.fAttackProb = float.Parse(skillInfo["ATTACKPROB"].ToString());
                    skill.iConsumeMp = int.Parse(skillInfo["CONSUMEMP"].ToString());
                    skill.fHitTime = float.Parse(skillInfo["HITTIME"].ToString());
                    skill.iProbCnt = int.Parse(skillInfo["PROBCOUNT"].ToString());
                    skill.iEffectIdx = int.Parse(skillInfo["EFFECT"].ToString());

                    m_sCurStats.listSkills.Add(skill);
                    break;
                }
            }

            m_anim.runtimeAnimatorController = m_dicClassAnim[eType];

            m_eCurClass = eType;
        }
 

        BATTLEINFO sInfo = new BATTLEINFO();
        sInfo.go = this.gameObject;
        sInfo.SPD = (float)m_sCurStats.SPD;
        sInfo.bUser = true;

        BattleManager.m_listBattleInfo.Add(sInfo);
    }

    public void OnBattleSelectEnemy(EnemyFSM enemy)
    {
        m_enemySelected = enemy;
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
