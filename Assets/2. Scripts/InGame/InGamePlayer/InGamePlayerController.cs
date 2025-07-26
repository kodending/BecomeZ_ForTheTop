using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    #region Ŭ���� ����
    //[SerializeField] List<GameObject> m_listKnightForm;
    //[SerializeField] List<GameObject> m_listDarkKnightForm;
    //
    //public Dictionary<CLASSTYPE, List<GameObject>> m_dicClassForm = new Dictionary<CLASSTYPE, List<GameObject>>();
    public CLASSTYPE m_eCurClass;
    //public DicClassAnimator m_dicClassAnim;
    public CLASSSTATS m_sCurStats = new CLASSSTATS();
    public int m_classIdx; // ���� ���޿�

    //25.06.20 ����
    public DicClassIdleAnimClip m_dicClassIdleAnimClip;
    #endregion

    public Color m_myColor;

    public EnemyFSM m_enemySelected;

    public List<bool> m_listAtkSuccess = new List<bool>();

    public GameObject m_goShadow;

    public Vector3 m_vMovePos;
    public float m_fMoveTime;

    //������ ������ ����ϱ�
    public bool m_isWaitingNext = false;

    int m_iBattleIdx = 0;

    public List<Transform> m_listEffectPos = new List<Transform>();

    #region ���� ����
    public GameObject m_originObj;
    public GameObject m_ragdollObj;
    public Rigidbody[] m_arrRigid;
    #endregion

    #region ����(���ۺ�) ����
    public BuffManager m_buffManager;
    #endregion

    private void OnAnimatorMove()
    {
        if (!m_pv.IsMine) return;

        m_stateMachine.currentState?.OnAnimatorMove();
    }

    private void Update()
    {
        if (!m_pv.IsMine) return;

        m_stateMachine.currentState?.OnUpdateState();
    }

    private void FixedUpdate()
    {
        if (!m_pv.IsMine) return;

        m_stateMachine.currentState?.OnFixedUpdateState();
    }

    public void InitClassFormInfo()
    {
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

    public void RefreshInfo(int playerIdx, bool bInit)
    {
        CLASSSTATS tempStats = new CLASSSTATS();

        tempStats = m_sCurStats;

        tempStats.Physical_POW = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, m_sCurStats.Physical_POW));
        tempStats.Physical_DEF = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, m_sCurStats.Physical_DEF));
        tempStats.Magic_POW = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.MAGICPOWER, m_sCurStats.Magic_POW));
        tempStats.Magic_DEF = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.MAGICDEFENSE, m_sCurStats.Magic_DEF));
        tempStats.SPD = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.SPD, m_sCurStats.SPD));

        string json = JsonUtility.ToJson(tempStats);
        m_pv.RPC("RefreshUserInfoRPC", RpcTarget.All, playerIdx, json, bInit);
    }

    [PunRPC]
    void RefreshUserInfoRPC(int userIdx, string json, bool bInit)
    {
        m_myColor = BattleManager.bm.m_listUserColors[userIdx];

        var stats = JsonUtility.FromJson<CLASSSTATS>(json);
        //����� �߰� ���� �����ϸ� �ɵ�
        //�� ����� ĭ�� �� ������ �����ϰڼ�
        RefreshUIInfo(userIdx, stats, bInit);
    }

    void RefreshUIInfo(int userIdx, CLASSSTATS tempStats, bool bInit)
    {
        //���⼭ ��ġ ��ġ
        GameManager.m_curUI.GetComponent<UIManagerInGame>().RefreshUserHud(userIdx, tempStats, bInit);
    }

    void InitStateMachine()
    {
        m_stateMachine = new PlayerStateMachine(PLAYERSTATE.IDLE, new Player_Idle(this));
        m_stateMachine.AddState(PLAYERSTATE.MOVE, new Player_Move(this));
        m_stateMachine.AddState(PLAYERSTATE.MELEE_ATTACk, new Player_MeleeAttack(this));
        m_stateMachine.AddState(PLAYERSTATE.ATTACK_MOVE, new Player_AttackMove(this));
        m_stateMachine.AddState(PLAYERSTATE.BACK_MOVE, new Player_BackMove(this));
        m_stateMachine.AddState(PLAYERSTATE.HIT_MOVE, new Player_Hit_Move(this));
        m_stateMachine.AddState(PLAYERSTATE.DEATH, new Player_Death(this));
        m_stateMachine.AddState(PLAYERSTATE.RETURN_MOVE, new Player_ReturnMove(this));
        m_stateMachine.AddState(PLAYERSTATE.DODGE_MOVE, new Player_Dodge_Move(this));
        m_stateMachine.AddState(PLAYERSTATE.BLOCK, new Player_Block(this));
        m_stateMachine.AddState(PLAYERSTATE.RANGED_ATTACk, new Player_RangedAttack(this));
        m_stateMachine.AddState(PLAYERSTATE.HIT, new Player_Hit(this));
        m_stateMachine.AddState(PLAYERSTATE.DODGE, new Player_Dodge(this));

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
    public void AttackHitRPC(int playerIdx, int iAtk, bool isCritcal, bool isFinishAtk)
    {
        var uiInfo = GameManager.m_curUI.GetComponent<UIManagerInGame>().m_arrUserInfo[playerIdx];

        var player = GameManager.gm.m_listPlayerInfo[playerIdx];

        player.m_sCurStats.curHp -= iAtk;

        if (player.m_sCurStats.curHp < 0) m_sCurStats.curHp = 0;

        if (isCritcal)
            BattleManager.ShakeCamera(0.5f, 0.08f);
        else
            BattleManager.ShakeCamera(3.5f, 0.08f);

        UIManager.CountText(uiInfo.curHpText, player.m_sCurStats.curHp);

        //ü�µ� ����ϰ�
        //0 �����ϰ�� �����ǰ� �ϱ�
        //ũ��Ƽ�� ������ ���ε� �ޱ�
        //���⼭ �ڱ� ü�� UI �����ϰ�
        RefreshHp(uiInfo);

        if (m_sCurStats.curHp <= 0)
        {
            if(m_pv.IsMine)
            {
                if (m_stateMachine.currentState?.GetType() == m_stateMachine.GetState(PLAYERSTATE.DEATH).GetType())
                {
                    Debug.Log("�̹� ���� ���¾�");
                    return;
                }
                m_stateMachine.ChangeState(PLAYERSTATE.DEATH);
            }
            return;
        }

        if (m_pv.IsMine)
        {
            if(isFinishAtk)
                m_stateMachine.ChangeState(PLAYERSTATE.HIT_MOVE);
            else
                m_stateMachine.ChangeState(PLAYERSTATE.HIT);
        }
    }

    public void ApplyBuff(BuffInfo buff)
    {
        string json = JsonUtility.ToJson(buff);
        m_pv.RPC("ApplyBuffRPC", RpcTarget.All, json);
    }

    [PunRPC]
    void ApplyBuffRPC(string json)
    {
        var buff = JsonUtility.FromJson<BuffInfo>(json);
        m_buffManager.ApplyBuff(buff);
    }

    public void TickTurnBuffCheck()
    {
        m_pv.RPC("TickTurnRPC", RpcTarget.All);
    }

    [PunRPC]
    void TickTurnRPC()
    {
        m_buffManager.TickTurn();
        int idx = GameManager.gm.m_listPlayerInfo.IndexOf(this);
        RefreshInfo(idx, false);
    }

    public IEnumerator UseConsumeItem(int invenIdx, ITEMINFO item, bool isBattle = false)
    {
        int randStat = 0;

        STATTYPE statType = STATTYPE.PHYSICALPOWER;

        //�� ĳ���Ϳ� �����ؾ��ϰ� UI���� �ٷ� ����ǵ��� �ؾߵ�
        switch ((CONSUMETYPE)item.iType)
        {
            //ü�� ������ ���
            case CONSUMETYPE.HP_POTION:

                if(m_sCurStats.curHp >= m_sCurStats.maxHp)
                {
                    UIManager.SystemMsg("ü���� ���� �� �ֽ��ϴ�");
                }

                //�ܲ� ����
                else
                {
                    if (!isBattle)
                    {
                        InvenManager.RemoveItem(CheckControl.cc.m_curPage, invenIdx, item);
                        CheckControl.RefreshInventory();
                    }

                    else
                    {
                        var uiInGame = GameManager.m_curUI.GetComponent<UIManagerInGame>();
                        int curPage = uiInGame.m_curInvenPage;
                        InvenManager.RemoveItem(curPage, invenIdx, item);
                        uiInGame.RefreshInventory();
                        uiInGame.InitBattleInventory();
                    }

                    yield return new WaitForSeconds(0.5f);

                    m_sCurStats.curHp += (int)item.fPoint;

                    if (m_sCurStats.curHp > m_sCurStats.maxHp) m_sCurStats.curHp = m_sCurStats.maxHp;

                    UIManager.SystemMsg("ü���� ȸ�� �Ǿ����ϴ�");
                }

                break;

            //���� ������ ���
            case CONSUMETYPE.MP_POTION:

                if (m_sCurStats.curMp >= m_sCurStats.maxMp)
                {
                    UIManager.SystemMsg("������ ���� �� �ֽ��ϴ�");
                }

                else
                {
                    //�ܲ� ����

                    if (!isBattle)
                    {
                        InvenManager.RemoveItem(CheckControl.cc.m_curPage, invenIdx, item);
                        CheckControl.RefreshInventory();
                    }

                    else
                    {
                        var uiInGame = GameManager.m_curUI.GetComponent<UIManagerInGame>();
                        int curPage = uiInGame.m_curInvenPage;
                        InvenManager.RemoveItem(curPage, invenIdx, item);
                        uiInGame.RefreshInventory();
                        uiInGame.InitBattleInventory();
                    }

                    yield return new WaitForSeconds(0.5f);

                    m_sCurStats.curMp += (int)item.fPoint;

                    if (m_sCurStats.curMp > m_sCurStats.maxMp) m_sCurStats.curMp = m_sCurStats.maxMp;

                    UIManager.SystemMsg("������ ȸ�� �Ǿ����ϴ�");
                }


                break;

            //�ɷ�ġ ������ ���
            case CONSUMETYPE.RANDOM_STAT_POT:
                
                int statMaxPoint = (int)item.fPoint;

                randStat = UnityEngine.Random.Range(1, statMaxPoint + 1);

                statType = (STATTYPE)UnityEngine.Random.Range((int)STATTYPE.PHYSICALPOWER, (int)STATTYPE.INSIGHT);

                switch (statType)
                {
                    case STATTYPE.PHYSICALPOWER:

                        m_sCurStats.Physical_POW += randStat;
                        UIManager.SystemMsg("�������ݷ� " + randStat.ToString() + " ���");

                        break;
                    case STATTYPE.MAGICPOWER:

                        m_sCurStats.Magic_POW += randStat;
                        UIManager.SystemMsg("�������ݷ� " + randStat.ToString() + " ���");

                        break;
                    case STATTYPE.PHYSICALDEFENSE:

                        m_sCurStats.Physical_DEF += randStat;
                        UIManager.SystemMsg("�������� " + randStat.ToString() + " ���");

                        break;
                    case STATTYPE.MAGICDEFENSE:

                        m_sCurStats.Magic_DEF += randStat;
                        UIManager.SystemMsg("�������� " + randStat.ToString() + " ���");

                        break;
                    case STATTYPE.MENT:

                        m_sCurStats.MENT += randStat;

                        int mana = randStat * (int)CONVERT_STATS.HP_MP;

                        float rateMp = (float)m_sCurStats.curMp / (float)m_sCurStats.maxMp;
                        int curMp = Mathf.RoundToInt(mana * rateMp);

                        m_sCurStats.maxMp += mana;
                        m_sCurStats.curMp += curMp;

                        UIManager.SystemMsg("���� " + mana.ToString() + " ���");

                        break;
                    case STATTYPE.SPD:

                        m_sCurStats.SPD += randStat;
                        UIManager.SystemMsg("���ǵ� " + randStat.ToString() + " ���");

                        break;
                    case STATTYPE.HP:

                        m_sCurStats.HP += randStat;

                        int hp = randStat * (int)CONVERT_STATS.HP_MP;

                        float rateHp = (float)m_sCurStats.curHp / (float)m_sCurStats.maxHp;
                        int curHp = Mathf.RoundToInt(hp * rateHp);

                        m_sCurStats.maxHp += hp;
                        m_sCurStats.curHp += curHp;

                        UIManager.SystemMsg("ü�� " + hp.ToString() + " ���");

                        break;
                }

                InvenManager.RemoveItem(CheckControl.cc.m_curPage, invenIdx, item);
                CheckControl.RefreshInventory();

                yield return new WaitForSeconds(0.5f);

            break;
        }

        //�κ��丮 ���� �� �� ���� UI�� ������
        if(!isBattle)
            CheckControl.RefreshStatInfo();

        m_pv.RPC("RestoreRPC", RpcTarget.All, item.iType, (int)statType, randStat);
    }

    [PunRPC]
    public void RestoreRPC(int itemType, int statType, int randStat)
    {
        //�÷��̾� �ε����� �˾ƾߵ�.
        int playerIdx = 0;

        foreach (var player in GameManager.gm.m_listPlayerInfo)
        {
            if (player.m_pv.Owner.NickName == m_pv.Owner.NickName)
            {
                playerIdx = GameManager.gm.m_listPlayerInfo.IndexOf(player);
                break;
            }
        }

        //int playerIdx = GameManager.gm.m_listPlayerInfo.IndexOf(this);

        var uiInfo = GameManager.m_curUI.GetComponent<UIManagerInGame>().m_arrUserInfo[playerIdx];

        switch ((CONSUMETYPE)itemType)
        {
            case CONSUMETYPE.HP_POTION:

                UIManager.CountText(uiInfo.curHpText, m_sCurStats.curHp);
                RefreshHp(uiInfo);

                break;

            //���� ������ ���
            case CONSUMETYPE.MP_POTION:

                RefreshMp(uiInfo);

                break;

            //�ɷ�ġ ������ ���
            case CONSUMETYPE.RANDOM_STAT_POT:

                switch ((STATTYPE)statType)
                {
                    case STATTYPE.PHYSICALPOWER:
                        UIManager.CountText(uiInfo.curPhysicalPowText, m_sCurStats.Physical_POW);
                        break;
                    case STATTYPE.MAGICPOWER:
                        UIManager.CountText(uiInfo.curMagicPowText, m_sCurStats.Magic_POW);
                        break;
                    case STATTYPE.PHYSICALDEFENSE:
                        UIManager.CountText(uiInfo.curPhysicalDefText, m_sCurStats.Physical_DEF);
                        break;
                    case STATTYPE.MAGICDEFENSE:
                        UIManager.CountText(uiInfo.curMagicDefText, m_sCurStats.Magic_DEF);
                        break;
                    case STATTYPE.MENT:
                        uiInfo.curMaxMpText.text = m_sCurStats.curMp.ToString() + " / " + m_sCurStats.maxMp.ToString();
                        break;
                    case STATTYPE.SPD:
                        UIManager.CountText(uiInfo.curSpeedText, m_sCurStats.SPD);
                        break;
                    case STATTYPE.HP:
                        uiInfo.curMaxHpText.text = m_sCurStats.curHp.ToString() + " / " + m_sCurStats.maxHp.ToString();
                        break;
                }

                break;
        }
    }

    public void ConsumeMP(int iConsumeMP, int playerIdx)
    {
        m_pv.RPC("ConsumeMPRPC", RpcTarget.All, iConsumeMP, playerIdx);
    }

    [PunRPC]
    public void ConsumeMPRPC(int iConsumeMP, int playerIdx)
    {
        m_sCurStats.curMp -= iConsumeMP;
        
        var uiInfo = GameManager.m_curUI.GetComponent<UIManagerInGame>().m_arrUserInfo[playerIdx];

        RefreshMp(uiInfo);
    }

    void RefreshMp(UserUIInfo info)
    {
        float value = (float)m_sCurStats.curMp / (float)m_sCurStats.maxMp;
        info.sliderMp.value = value;
        StartCoroutine(MpMotion(info, value));
    }

    IEnumerator MpMotion(UserUIInfo info, float value)
    {
        float delta = 0;
        float duration = 1.0f;

        while (delta <= duration)
        {
            float t = delta / duration;
            //t = Mathf.Sin((t * Mathf.PI) / 2);
            t = t * t * t * t * t;

            info.sliderMpBack.value = Mathf.Lerp(info.sliderMpBack.value, value, t);

            delta += Time.deltaTime;
            yield return null;
        }

        info.sliderMpBack.value = value;

        info.curMaxMpText.text = m_sCurStats.curMp.ToString() + " / " + m_sCurStats.maxMp.ToString();
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
        if(m_pv.IsMine)
            m_stateMachine.ChangeState(PLAYERSTATE.BLOCK);
    }

    [PunRPC]
    public void AttackDodgeRPC(int playerIdx, bool isFinishAtk)
    {
        if (!m_pv.IsMine) return;

        if (isFinishAtk)    
            m_stateMachine.ChangeState(PLAYERSTATE.DODGE_MOVE);
        else
            m_stateMachine.ChangeState(PLAYERSTATE.DODGE);
    }

    public void ControlWaitingNext(bool bCheck)
    {
        m_pv.RPC("ControlWaitingNextRPC", RpcTarget.All, bCheck);
    }

    [PunRPC]
    void ControlWaitingNextRPC(bool bCheck)
    {
        m_isWaitingNext = bCheck;
    }

    void InitClass(CLASSTYPE eType)
    {
        //foreach (var form in m_dicClassForm[eType])
        //{
        //    form.SetActive(true);
        //}

        Transform[] arrChild = m_originObj.GetComponentsInChildren<Transform>(includeInactive: true);

        int changeIdx = (int)eType + 1;

        foreach (var go in arrChild)
        {
            if (go.gameObject.name == changeIdx.ToString())
            {
                go.gameObject.SetActive(true);
            }
        }

        //IsMine�϶�
        if (m_pv.IsMine)
        {
            m_sCurStats = UserDataManager.LoadMyUserInfo(m_pv.Owner.NickName);

            if (m_sCurStats.name == null)
            {
                var info = GoogleSheetManager.m_classInfo[(int)eType];

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
                m_sCurStats.maxHp = m_sCurStats.HP * (int)CONVERT_STATS.HP_MP;
                m_sCurStats.curHp = m_sCurStats.maxHp;
                m_sCurStats.maxMp = m_sCurStats.MENT * (int)CONVERT_STATS.HP_MP;
                m_sCurStats.curMp = m_sCurStats.maxMp;
                m_sCurStats.eClass = eType;
                m_sCurStats.listSkills = new List<SKILLINFO>();

                SKILLINFO skill = new SKILLINFO();

                foreach (var skillInfo in GoogleSheetManager.m_skillInfo)
                {
                    int skillIdx = int.Parse(skillInfo["INDEX"].ToString());
                    var classes = GoogleSheetManager.ParseAvailableClassType(skillInfo["CLASSTYPE"].ToString());
                    int classIdx = (int)m_sCurStats.eClass;

                    if (skillIdx == 0 && classes.Contains(classIdx.ToString()))
                    {
                        skill.idx = skillIdx;
                        skill.name = skillInfo["NAME"].ToString();
                        skill.engName = skillInfo["ENGNAME"].ToString();
                        skill.listAvailClasses = classes;
                        skill.iAttackType = int.Parse(skillInfo["ATTACKTYPE"].ToString());
                        skill.iSkillType = int.Parse(skillInfo["SKILLTYPE"].ToString());
                        skill.fAttackPow = float.Parse(skillInfo["ATTACK"].ToString());
                        skill.fAttackProb = float.Parse(skillInfo["ATTACKPROB"].ToString());
                        skill.iConsumeMp = int.Parse(skillInfo["CONSUMEMP"].ToString());
                        skill.fHitTime = float.Parse(skillInfo["HITTIME"].ToString());
                        skill.iProbCnt = int.Parse(skillInfo["PROBCOUNT"].ToString());
                        skill.iMaxCoolTime = int.Parse(skillInfo["COOLTIME"].ToString());
                        skill.iCurCoolTime = int.Parse(skillInfo["COOLTIME"].ToString());
                        skill.iAtkEffectIdx = int.Parse(skillInfo["ATTACKEFFECT"].ToString());
                        skill.iHitEffectIdx = int.Parse(skillInfo["HITEFFECT"].ToString());
                        skill.iAttackPosIdx = int.Parse(skillInfo["ATTACKPOS"].ToString());
                        skill.bHoming = int.Parse(skillInfo["HOMING"].ToString()) != 0;

                        m_sCurStats.listSkills.Add(skill);

                        break;
                    }
                }

                m_iBattleIdx = GameManager.gm.m_BattlePosIdx;
                string json = JsonUtility.ToJson(m_sCurStats);
                m_pv.RPC("SaveUserInfoRPC", RpcTarget.All, m_iBattleIdx, json);
            }
        }

        //IsMine �ƴ� ��
        else
        {
            m_sCurStats = UserDataManager.LoadMyUserInfo(m_pv.Owner.NickName);

            if (m_sCurStats.name == null)
            {
                var info = GoogleSheetManager.m_classInfo[(int)eType];

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
                m_sCurStats.maxHp = m_sCurStats.HP * (int)CONVERT_STATS.HP_MP;
                m_sCurStats.curHp = m_sCurStats.maxHp;
                m_sCurStats.maxMp = m_sCurStats.MENT * (int)CONVERT_STATS.HP_MP;
                m_sCurStats.curMp = m_sCurStats.maxMp;
                m_sCurStats.eClass = eType;
                m_sCurStats.listSkills = new List<SKILLINFO>();

                SKILLINFO skill = new SKILLINFO();

                foreach (var skillInfo in GoogleSheetManager.m_skillInfo)
                {
                    int skillIdx = int.Parse(skillInfo["INDEX"].ToString());
                    var classes = GoogleSheetManager.ParseAvailableClassType(skillInfo["CLASSTYPE"].ToString());
                    int classIdx = (int)m_sCurStats.eClass;

                    if (skillIdx == 0 && classes.Contains(classIdx.ToString()))
                    {
                        skill.idx = skillIdx;
                        skill.name = skillInfo["NAME"].ToString();
                        skill.engName = skillInfo["ENGNAME"].ToString();
                        skill.listAvailClasses = classes;
                        skill.iAttackType = int.Parse(skillInfo["ATTACKTYPE"].ToString());
                        skill.iSkillType = int.Parse(skillInfo["SKILLTYPE"].ToString());
                        skill.fAttackPow = float.Parse(skillInfo["ATTACK"].ToString());
                        skill.fAttackProb = float.Parse(skillInfo["ATTACKPROB"].ToString());
                        skill.iConsumeMp = int.Parse(skillInfo["CONSUMEMP"].ToString());
                        skill.fHitTime = float.Parse(skillInfo["HITTIME"].ToString());
                        skill.iProbCnt = int.Parse(skillInfo["PROBCOUNT"].ToString());
                        skill.iMaxCoolTime = int.Parse(skillInfo["COOLTIME"].ToString());
                        skill.iCurCoolTime = int.Parse(skillInfo["COOLTIME"].ToString());
                        skill.iAtkEffectIdx = int.Parse(skillInfo["ATTACKEFFECT"].ToString());
                        skill.iHitEffectIdx = int.Parse(skillInfo["HITEFFECT"].ToString());
                        skill.iAttackPosIdx = int.Parse(skillInfo["ATTACKPOS"].ToString());
                        skill.bHoming = int.Parse(skillInfo["HOMING"].ToString()) != 0;

                        m_sCurStats.listSkills.Add(skill);

                        break;
                    }
                }
            }
        }

        var overrideCtrl = new AnimatorOverrideController(m_anim.runtimeAnimatorController);
        overrideCtrl["IdleA"] = m_dicClassIdleAnimClip[eType];

        m_anim.runtimeAnimatorController = overrideCtrl;

        m_eCurClass = eType;

        BATTLEINFO sInfo = new BATTLEINFO();
        sInfo.go = this.gameObject;
        sInfo.SPD = (float)m_sCurStats.SPD;
        //
        sInfo.bUser = true;
        sInfo.iRegistrationOrder = 0;

        BattleManager.m_listBattleInfo.Add(sInfo);
    }

    [PunRPC]
    public void SaveUserInfoRPC(int battleIdx, string json)
    {
        m_sCurStats = JsonUtility.FromJson<CLASSSTATS>(json);
        UserDataManager.SaveMyUserInfo(battleIdx, m_sCurStats);
    }

    [PunRPC]
    public void ClearUserInfoRPC(int battleIdx)
    {
        UserDataManager.ClearMyUserInfo(battleIdx);
    }

    [PunRPC]
    public void AddBattleListRPC(int classIdx)
    {
        InitClass((CLASSTYPE)classIdx);
    }

    public void OnBattleSelectEnemy(EnemyFSM enemy)
    {
        m_enemySelected = enemy;
    }

    public void AttackCheck(int hitIdx)
    {
        if (!m_pv.IsMine) return;

        bool bFinishAttack = Convert.ToBoolean(hitIdx);
        var curSkillInfo = m_sCurStats.listSkills[BattleManager.m_iCurSelectedSkill];

        //ũ��Ƽ�� ���, ������ ��� �ؿ��� �� ���� ���Ǿ �Ѿ�ߵ�..��..
        //��� ȣ���ϰ� �����ϴ°� ������?
        ATTACKRESULT eResult = (ATTACKRESULT)curSkillInfo.iAttackType;

        switch(eResult)
        {
            case ATTACKRESULT.PHYSICAL_ATTACK:

                //�̹� �׾��� ���
                if (m_enemySelected.m_ragdollObj.activeSelf) return;

                int atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, m_sCurStats.Physical_POW));
                int enemyDef = Mathf.RoundToInt(m_enemySelected.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, m_enemySelected.m_sCurStats.Physical_DEF));
                AttackProcess(curSkillInfo, eResult, false, atkPow, enemyDef, m_enemySelected, bFinishAttack);

                break;

            case ATTACKRESULT.MAGIC_ATTACK:

                //�̹� �׾��� ���
                if (m_enemySelected.m_ragdollObj.activeSelf) return;

                atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.MAGICPOWER, m_sCurStats.Magic_POW));
                enemyDef = Mathf.RoundToInt(m_enemySelected.m_buffManager.GetBufferdStatus(STATTYPE.MAGICDEFENSE, m_enemySelected.m_sCurStats.Magic_DEF));
                AttackProcess(curSkillInfo, eResult, false, atkPow, enemyDef, m_enemySelected, bFinishAttack);

                break;

            case ATTACKRESULT.PHYSICAL_PIERCING_ATTACK:

                //�̹� �׾��� ���
                if (m_enemySelected.m_ragdollObj.activeSelf) return;

                atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, m_sCurStats.Physical_POW));
                enemyDef = Mathf.RoundToInt(m_enemySelected.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, m_enemySelected.m_sCurStats.Physical_DEF));
                AttackProcess(curSkillInfo, eResult, true, atkPow, enemyDef, m_enemySelected, bFinishAttack);

                break;

            case ATTACKRESULT.MAGIC_PIERCING_ATTACK:

                //�̹� �׾��� ���
                if (m_enemySelected.m_ragdollObj.activeSelf) return;

                atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.MAGICPOWER, m_sCurStats.Magic_POW));
                enemyDef = Mathf.RoundToInt(m_enemySelected.m_buffManager.GetBufferdStatus(STATTYPE.MAGICDEFENSE, m_enemySelected.m_sCurStats.Magic_DEF));
                AttackProcess(curSkillInfo, eResult, true, atkPow, enemyDef, m_enemySelected, bFinishAttack);

                break;

            case ATTACKRESULT.PHYSICAL_AREA_ATTACK:

                //���Ⱑ ����
                foreach(var enemy in GameManager.gm.m_listEnemyInfo)
                {
                    if (enemy.m_ragdollObj.activeSelf) continue;

                    atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, m_sCurStats.Physical_POW));
                    enemyDef = Mathf.RoundToInt(m_enemySelected.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, enemy.m_sCurStats.Physical_DEF));
                    AttackProcess(curSkillInfo, eResult, false, atkPow, enemyDef, enemy, bFinishAttack);
                }

                break;

            case ATTACKRESULT.MAGIC_AREA_ATTACK:

                //���Ⱑ ����
                foreach (var enemy in GameManager.gm.m_listEnemyInfo)
                {
                    if (enemy.m_ragdollObj.activeSelf) continue;

                    atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.MAGICPOWER, m_sCurStats.Magic_POW));
                    enemyDef = Mathf.RoundToInt(m_enemySelected.m_buffManager.GetBufferdStatus(STATTYPE.MAGICDEFENSE, enemy.m_sCurStats.Magic_DEF));
                    AttackProcess(curSkillInfo, eResult, false, atkPow, enemyDef, enemy, bFinishAttack);
                }


                break;

            case ATTACKRESULT.SHIELD_SOLO_STRIKE:

                //�̹� �׾��� ���
                if (m_enemySelected.m_ragdollObj.activeSelf) return;

                atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, m_sCurStats.Physical_POW));
                enemyDef = Mathf.RoundToInt(m_enemySelected.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, m_enemySelected.m_sCurStats.Physical_DEF));
                AttackProcess(curSkillInfo, eResult, false, atkPow, enemyDef, m_enemySelected, bFinishAttack);

                break;

            case ATTACKRESULT.SHIELD_AREA_STRIKE:

                foreach (var enemy in GameManager.gm.m_listEnemyInfo)
                {
                    if (enemy.m_ragdollObj.activeSelf) continue;

                    atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, m_sCurStats.Physical_POW));
                    enemyDef = Mathf.RoundToInt(m_enemySelected.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, enemy.m_sCurStats.Physical_DEF));
                    AttackProcess(curSkillInfo, eResult, false, atkPow, enemyDef, enemy, bFinishAttack);
                }

                break;

            case ATTACKRESULT.PHYSICAL_DEF_DEBUFF_SOLO:

                //�̹� �׾��� ���
                if (m_enemySelected.m_ragdollObj.activeSelf) return;

                atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, m_sCurStats.Physical_POW));
                enemyDef = Mathf.RoundToInt(m_enemySelected.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, m_enemySelected.m_sCurStats.Physical_DEF));
                AttackProcess(curSkillInfo, eResult, false, atkPow, enemyDef, m_enemySelected, bFinishAttack);

                break;
        }
    }

    void AttackProcess(SKILLINFO curSkillInfo, ATTACKRESULT eResult, bool bPierce,int iAtkPow, int iDef, EnemyFSM enemyInfo, bool isFinishAtk)
    {
        float atkPower = 0;
        int damage = 0;
        int playerSpd = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.SPD, m_sCurStats.SPD));
        float enemySpd = m_enemySelected.m_buffManager.GetBufferdStatus(STATTYPE.SPD, enemyInfo.m_sCurStats.SPD);
        int playerDef = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, m_sCurStats.Physical_DEF)) * (int)CONVERT_STATS.DEF;
        bool isCritical = false;
        iDef *= (int)CONVERT_STATS.DEF;

        isCritical = AttackWeight.GetCritChance(m_listAtkSuccess, playerSpd,
                           enemySpd);

        atkPower = (iAtkPow * (int)CONVERT_STATS.ATK) * curSkillInfo.fAttackPow;

        damage = AttackWeight.AttackDamageCal(m_listAtkSuccess, Mathf.RoundToInt(atkPower), isCritical);

        if(!bPierce)
            damage -= iDef;

        int idx = GameManager.gm.m_listEnemyInfo.IndexOf(enemyInfo);

        bool bMiss = false;
        bool bHit = false;

        if (damage <= 0)
        {
            enemyInfo.m_pv.RPC("AttackBlockRPC", RpcTarget.All, idx);
            eResult = ATTACKRESULT.BLOCK;
        }

        else
        {
            bMiss = AttackWeight.MissCal(m_listAtkSuccess, (float)playerSpd, enemySpd);

            if (bMiss)
            {
                enemyInfo.m_pv.RPC("AttackDodgeRPC", RpcTarget.All, idx, isFinishAtk);
                eResult = ATTACKRESULT.MISS;
            }

            else
            {
                enemyInfo.m_pv.RPC("AttackHitRPC", RpcTarget.All, idx, damage, isCritical, isFinishAtk);
                bHit = true;
            }
        }

        //����� ���°� ��������
        //ũ��Ƽ�� �Ǵ�, �̽��Ǵ�, ���ݰ�� �Ǵܵ��� �ٽ� �����Ѿߵ�
        Vector3 vOwner = gameObject.transform.position;
        var vOwnerAttackPos = m_listEffectPos[curSkillInfo.iAttackPosIdx];
        Vector3 vTarget = enemyInfo.gameObject.transform.position + new Vector3(0, 1.5f, 0);
        bool bFaint = false;

        //� �����̵� ������ ����Ʈ�� �����ǰ� �ؾ��Ѵ�.
        if (curSkillInfo.bHoming)
            NetworkManager.nm.PV.RPC("PlayEffect", RpcTarget.All, curSkillInfo.iAtkEffectIdx, vOwnerAttackPos.position, vTarget, curSkillInfo.bHoming, 0.1f);
        else
            NetworkManager.nm.PV.RPC("PlayEffect", RpcTarget.All, curSkillInfo.iAtkEffectIdx, vOwnerAttackPos.position, vOwnerAttackPos.position, false, 1.0f);

        switch (eResult)
        {
            case ATTACKRESULT.PHYSICAL_ATTACK:
            case ATTACKRESULT.MAGIC_ATTACK:
            case ATTACKRESULT.PHYSICAL_PIERCING_ATTACK:
            case ATTACKRESULT.MAGIC_PIERCING_ATTACK:
            case ATTACKRESULT.PHYSICAL_AREA_ATTACK:
            case ATTACKRESULT.MAGIC_AREA_ATTACK:
                break;
            case ATTACKRESULT.SHIELD_SOLO_STRIKE:
            case ATTACKRESULT.SHIELD_AREA_STRIKE:

                isCritical = false;

                //���� �̽����µ� ���� �ȵȴ�.
                bFaint = AttackWeight.GetShieldStrike(m_listAtkSuccess, playerDef,
                                                            iDef);
                if (bFaint && !bMiss)
                {
                    if (bHit) NetworkManager.nm.PV.RPC("PlayEffect", RpcTarget.All, (int)EFFECTTYPE.SHINING, vOwner, vTarget, false, 1.0f);

                    //���⼭ ���� ��Ű���
                    //������ ���̰� , ���������� �ε����� �� �� ����
                    int enemyIdx = GameManager.gm.m_listEnemyInfo.IndexOf(enemyInfo);

                    //���ʹ� �ε��� �ѱ��, ����� �������ƴ϶� ���� �˷���
                    NetworkManager.nm.PV.RPC("BattleFaintTimelineRPC", RpcTarget.All, enemyIdx, false);
                }

                //Debug.Log("���彺Ʈ����ũ �ߴ���");

                break;

            case ATTACKRESULT.PHYSICAL_DEF_DEBUFF_SOLO:

                bool fullSuc = true;

                //ĳ�������� ����� �߰�������ϰ�
                foreach (var suc in m_listAtkSuccess)
                {
                    if (!suc) fullSuc = false;
                }

                if (fullSuc)
                {
                    BuffInfo buff = new BuffInfo(BUFFTYPE.PHYSICAL_DEFENSE_DEBUFF, curSkillInfo.iMaxCoolTime, curSkillInfo.fAttackPow);

                    m_enemySelected.ApplyBuff(buff);

                    //UI�� �����ؾߵ�

                    m_enemySelected.RefreshInfo(false);
                }

                break;

            case ATTACKRESULT.MISS:


                break;

            case ATTACKRESULT.BLOCK:



                break;
        }

        if(!bFaint && !bMiss)
        {
            //�ǰ��� ����Ʈ�� �����Ѵ�.
            //� �����̵� ������ ����Ʈ�� �����ǰ� �ؾ��Ѵ�.
            NetworkManager.nm.PV.RPC("PlayEffect", RpcTarget.All, curSkillInfo.iHitEffectIdx, vOwnerAttackPos.position, vTarget, false, 1.0f);
        }

        NetworkManager.nm.PV.RPC("UserAttackDamageUI", RpcTarget.All, damage, idx, (int)eResult);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_classIdx);
            stream.SendNext(m_iBattleIdx);
        }

        else
        {
            m_classIdx = (int)stream.ReceiveNext();
            m_iBattleIdx = (int)stream.ReceiveNext();
        }
    }
}
