using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

#region ���ʹ� �ѹ��� �ִϸ��̼� ����
[System.Serializable]
public class DicEnemyAnimator : SerializableDictionary<int, RuntimeAnimatorController> { }
#endregion

public class EnemyFSM : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public EnemyStateMachine m_stateMachine { get; private set; }

    public PhotonView m_pv;
    public Animator m_anim;
    public bool m_bSelected;
    public Vector3 m_vMovePos;
    public float m_fMoveTime;
    public GameObject m_goShadow;

    public InGamePlayerController m_selectedPlayer;
    public List<bool> m_listAtkSuccess = new List<bool>();

    #region ���ʹ� ����
    //[SerializeField] List<GameObject> m_listEnemy_1;

    //public Dictionary<int, List<GameObject>> m_dicEnemyForms = new Dictionary<int, List<GameObject>>();
    public DicEnemyAnimator m_dicEnemyAnim;
    public ENEMYSTATS m_sCurStats;
    #endregion

    #region ���� ����
    public GameObject m_originObj;
    public GameObject m_ragdollObj;
    public Rigidbody[] m_arrRigid;
    #endregion

    #region ����(���ۺ�) ����
    public BuffManager m_buffManager;
    #endregion

    public List<Transform> m_listEffectPos = new List<Transform>();

    public SKILLINFO m_sSelectedSkill = new SKILLINFO();

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

    public void InitFormInfo()
    {
        //m_dicEnemyForms.Add(1, m_listEnemy_1);
    }


    [PunRPC]
    public void InitEnemyInfo(int idx)
    {
        //foreach(var go in m_dicEnemyForms[idx])
        //{
        //    go.SetActive(true);
        //}

        m_originObj.SetActive(true);

        Transform[] arrChild = m_originObj.GetComponentsInChildren<Transform>(includeInactive: true);

        foreach (var go in arrChild)
        {
            if (go.gameObject.name == idx.ToString())
            {
                go.gameObject.SetActive(true);
            }
        }

        Rigidbody[] arrRb = m_ragdollObj.GetComponentsInChildren<Rigidbody>();

        foreach (var rigid in arrRb)
        {
            rigid.isKinematic = false;
            rigid.useGravity = true;
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }

        m_ragdollObj.SetActive(false);

        m_anim.runtimeAnimatorController = m_dicEnemyAnim[idx];

        var info = GoogleSheetManager.m_enemyInfo[idx - 1];
        
        m_sCurStats.fSize = float.Parse(info["SIZE"].ToString());
        transform.localScale = new Vector3(m_sCurStats.fSize, m_sCurStats.fSize, m_sCurStats.fSize);

        m_sCurStats.INDEX = int.Parse(info["INDEX"].ToString());
        m_sCurStats.HP = int.Parse(info["HP"].ToString());
        m_sCurStats.Physical_POW = int.Parse(info["PHYSICALPOW"].ToString());
        m_sCurStats.Magic_POW = int.Parse(info["MAGICPOW"].ToString());
        m_sCurStats.Physical_DEF = int.Parse(info["PHYSICALDEF"].ToString());
        m_sCurStats.Magic_DEF = int.Parse(info["MAGICDEF"].ToString());
        m_sCurStats.MENT = int.Parse(info["MENT"].ToString());
        m_sCurStats.SPD = float.Parse(info["SPD"].ToString());

        m_sCurStats.maxHp = m_sCurStats.HP * (int)CONVERT_STATS.HP_MP;
        m_sCurStats.curHp = m_sCurStats.maxHp;

        m_sCurStats.name = info["NAME"].ToString();
        m_sCurStats.listSkills = new List<SKILLINFO>();

        gameObject.SetActive(true);

        BATTLEINFO sInfo = new BATTLEINFO();
        sInfo.go = this.gameObject;
        sInfo.SPD = m_sCurStats.SPD;
        sInfo.bUser = false;
        sInfo.iRegistrationOrder = 0;

        BattleManager.m_listBattleInfo.Add(sInfo);

        m_goShadow.SetActive(true);

        //���ʹ� ��ų���� �Է�
        SKILLINFO skill = new SKILLINFO();

        foreach(var skillInfo in GoogleSheetManager.m_enemySkillInfo)
        {
            int enemyType = int.Parse(skillInfo["ENEMYTYPE"].ToString());

            if(m_sCurStats.INDEX == enemyType)
            {
                skill.name = skillInfo["NAME"].ToString();
                skill.idx = int.Parse(skillInfo["INDEX"].ToString());
                skill.iAttackType = int.Parse(skillInfo["ATTACKTYPE"].ToString());
                skill.iSkillType = int.Parse(skillInfo["SKILLTYPE"].ToString());
                skill.fAttackPow = float.Parse(skillInfo["ATTACK"].ToString());
                skill.fAttackProb = float.Parse(skillInfo["ATTACKPROB"].ToString());
                skill.fHitTime = float.Parse(skillInfo["HITTIME"].ToString());
                skill.iProbCnt = int.Parse(skillInfo["PROBCOUNT"].ToString());
                skill.iAtkEffectIdx = int.Parse(skillInfo["ATTACKEFFECT"].ToString());
                skill.iHitEffectIdx = int.Parse(skillInfo["HITEFFECT"].ToString());
                skill.iAttackPosIdx = int.Parse(skillInfo["ATTACKPOS"].ToString());
                skill.bHoming = int.Parse(skillInfo["HOMING"].ToString()) != 0;
                skill.iMaxCoolTime = int.Parse(skillInfo["COOLTIME"].ToString());
                skill.iCurCoolTime = int.Parse(skillInfo["COOLTIME"].ToString());
                skill.fSelectWeight = float.Parse(skillInfo["SELECTWEIGHT"].ToString());

                m_sCurStats.listSkills.Add(skill);
            }
        }

        InitStateMachine();
    }

    //�����ִ� ������Ʈ�� ��Ȱ��ȭ��Ų��.
    public void InActiveObj()
    {
        Transform[] arrChild = m_originObj.GetComponentsInChildren<Transform>(includeInactive: true);

        foreach (var go in arrChild)
        {
            if (go.gameObject.name == m_sCurStats.INDEX.ToString())
            {
                go.gameObject.SetActive(false);
            }
        }

        arrChild = m_ragdollObj.GetComponentsInChildren<Transform>(includeInactive: true);

        foreach (var go in arrChild)
        {
            if (go.gameObject.name == m_sCurStats.INDEX.ToString())
            {
                go.gameObject.SetActive(false);
            }
        }
    }

    void InitStateMachine()
    {
        m_stateMachine = new EnemyStateMachine(ENEMYSTATE.IDLE, new Enemy_Idle(this));
        m_stateMachine.AddState(ENEMYSTATE.MOVE, new Enemy_Move(this));
        m_stateMachine.AddState(ENEMYSTATE.MELEE_ATTACK, new Enemy_MeleeAttack(this));
        m_stateMachine.AddState(ENEMYSTATE.ATTACK_MOVE, new Enemy_AttackMove(this));
        m_stateMachine.AddState(ENEMYSTATE.BACK_MOVE, new Enemy_BackMove(this));
        m_stateMachine.AddState(ENEMYSTATE.HIT_MOVE, new Enemy_Hit_Move(this));
        m_stateMachine.AddState(ENEMYSTATE.HIT, new Enemy_Hit(this));
        m_stateMachine.AddState(ENEMYSTATE.DEATH, new Enemy_Death(this));
        m_stateMachine.AddState(ENEMYSTATE.BLOCK, new Enemy_Block(this));
        m_stateMachine.AddState(ENEMYSTATE.DODGE_MOVE, new Enemy_Dodge_Move(this));
        m_stateMachine.AddState(ENEMYSTATE.DODGE, new Enemy_Dodge(this));
        m_stateMachine.AddState(ENEMYSTATE.RANGED_ATTACK, new Enemy_RangedAttack(this));

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
    public void AttackHitRPC(int enemyIdx, int iAtk, bool isCritcal, bool isFinishAtk)
    {
        var uiInfo = GameManager.m_curUI.GetComponent<UIManagerInGame>().m_arrEnemyInfo[enemyIdx];

        m_sCurStats.curHp -= iAtk;

        if (m_sCurStats.curHp < 0) m_sCurStats.curHp = 0;

        if (isCritcal)
            BattleManager.ShakeCamera(0.5f, 0.08f);
        else
            BattleManager.ShakeCamera(3.5f, 0.08f);

        UIManager.CountText(uiInfo.curHpText, m_sCurStats.curHp);

        //ü�µ� ����ϰ�
        //0 �����ϰ�� �����ǰ� �ϱ�
        //ũ��Ƽ�� ������ ���ε� �ޱ�
        //���⼭ �ڱ� ü�� UI �����ϰ�
        RefreshHp(uiInfo);

        if (m_sCurStats.curHp <= 0 )
        {
            if(m_stateMachine.currentState?.GetType() == m_stateMachine.GetState(ENEMYSTATE.DEATH).GetType())
            {
                //Debug.Log("�̹� ���� ���¾�");
                uiInfo.gameObject.SetActive(false);
                return;
            }

            uiInfo.gameObject.SetActive(false);
            m_stateMachine.ChangeState(ENEMYSTATE.DEATH);
            return;
        }

        if(m_pv.IsMine)
        {
            if (isFinishAtk)
                m_stateMachine.ChangeState(ENEMYSTATE.HIT_MOVE);
            else
                m_stateMachine.ChangeState(ENEMYSTATE.HIT);
        }
    }

    void RefreshHp(EnemyUIInfo info)
    {
        float value = (float)m_sCurStats.curHp / (float)m_sCurStats.maxHp;
        info.sliderHp.value = value;
        StartCoroutine(HpMotion(info, value));
    }

    IEnumerator HpMotion(EnemyUIInfo info, float value)
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
    }

    public void RefreshInfo(bool bInit)
    {
        m_pv.RPC("RefreshInfoRPC", RpcTarget.All, bInit);
    }

    [PunRPC]
    public void RefreshInfoRPC(bool bInit)
    {
        if (m_ragdollObj.activeSelf) return;

        int idx = GameManager.gm.m_listEnemyInfo.IndexOf(this);

        //���⼭ ��ġ ����
        ENEMYSTATS tempStats = new ENEMYSTATS();

        tempStats = m_sCurStats;

        tempStats.Physical_POW = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, m_sCurStats.Physical_POW));
        tempStats.Physical_DEF = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, m_sCurStats.Physical_DEF));
        tempStats.Magic_POW = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.MAGICPOWER, m_sCurStats.Magic_POW));
        tempStats.Magic_DEF = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.MAGICDEFENSE, m_sCurStats.Magic_DEF));
        tempStats.SPD = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.SPD, m_sCurStats.SPD));

        GameManager.m_curUI.GetComponent<UIManagerInGame>().RefreshEnemyHud(idx, tempStats, bInit);
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

        RefreshInfo(false);
    }

    [PunRPC]
    public void AttackBlockRPC(int playerIdx)
    {
        BattleManager.ShakeCamera(0.5f, 0.08f);

        if (m_pv.IsMine)
            m_stateMachine.ChangeState(ENEMYSTATE.BLOCK);
    }

    [PunRPC]
    public void AttackDodgeRPC(int playerIdx, bool isFinishAtk)
    {
        if (!m_pv.IsMine) return;

        if (isFinishAtk)
            m_stateMachine.ChangeState(ENEMYSTATE.DODGE_MOVE);
        else
            m_stateMachine.ChangeState(ENEMYSTATE.DODGE);
    }

    public SKILLINFO PickSkill(List<SKILLINFO> listSkills)
    {
        float total = 0f;
        List<SKILLINFO> listAvailableSkill = new();

        if (listSkills.Count == 1) return listSkills[0];

        for (int i = 0; i < listSkills.Count; i++)
        {
            var skill = listSkills[i];

            float w = skill.fSelectWeight;

            //Debug.Log("�� ��ų ( " + skill.name + " ) �� �־�. ��Ÿ�� : " + skill.iCurCoolTime);

            if (skill.iCurCoolTime > 0) w = 0f;
            else
            {
                skill.iCurCoolTime = 0;
                listAvailableSkill.Add(skill);
            }

            skill.iCurCoolTime--;
            m_sCurStats.listSkills[i] = skill;

            total += w;
        }

        float pick = UnityEngine.Random.Range(0f, total);
        float cumulative = 0f;

        //debug ��
        foreach (var skill in listAvailableSkill)
        {
            //Debug.Log("������ �� �ִ� ��ų : " + skill.name);
        }

        for (int i = 0; i < listAvailableSkill.Count; i++)
        {
            var p = listAvailableSkill[i];

            cumulative += p.fSelectWeight;

            if (pick <= cumulative)
            {
                p.iCurCoolTime = p.iMaxCoolTime;
                //Debug.Log("�� ��ų�� ���� �ƾ� : " + p.name);

                int idx = m_sCurStats.listSkills.FindIndex(b => b.name == p.name);
                m_sCurStats.listSkills[idx] = p;

                return p;
            }
        }

        return listSkills[0];
    }

    //animation clip -> �ش� �����ӿ� event���� �Լ��� ȣ����
    public void AttackCheck(int hitIdx)
    {
        //������ ���θ� ���� ����
        if (!m_pv.IsMine) return;

        bool bFinishAttack = Convert.ToBoolean(hitIdx);
        var skill = m_sSelectedSkill;

        ATTACKRESULT eResult = (ATTACKRESULT)skill.iAttackType;

        switch (eResult)
        {
            case ATTACKRESULT.PHYSICAL_ATTACK:

                //�̹� �׾��� ���
                if (m_selectedPlayer.m_ragdollObj.activeSelf) return;

                int atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, m_sCurStats.Physical_POW));
                int playerDef = Mathf.RoundToInt(m_selectedPlayer.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, m_selectedPlayer.m_sCurStats.Physical_DEF));

                AttackProcess(skill, eResult, false, atkPow, playerDef, m_selectedPlayer, bFinishAttack);

                break;

            case ATTACKRESULT.MAGIC_ATTACK:

                //�̹� �׾��� ���
                if (m_selectedPlayer.m_ragdollObj.activeSelf) return;

                atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.MAGICPOWER, m_sCurStats.Magic_POW));
                playerDef = Mathf.RoundToInt(m_selectedPlayer.m_buffManager.GetBufferdStatus(STATTYPE.MAGICDEFENSE, m_selectedPlayer.m_sCurStats.Magic_DEF));
                AttackProcess(skill, eResult, false, atkPow, playerDef, m_selectedPlayer, bFinishAttack);

                break;

            case ATTACKRESULT.PHYSICAL_PIERCING_ATTACK:

                //�̹� �׾��� ���
                if (m_selectedPlayer.m_ragdollObj.activeSelf) return;

                atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, m_sCurStats.Physical_POW));
                playerDef = Mathf.RoundToInt(m_selectedPlayer.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, m_selectedPlayer.m_sCurStats.Physical_DEF));

                AttackProcess(skill, eResult, true, atkPow, playerDef, m_selectedPlayer, bFinishAttack);

                break;

            case ATTACKRESULT.MAGIC_PIERCING_ATTACK:

                //�̹� �׾��� ���
                if (m_selectedPlayer.m_ragdollObj.activeSelf) return;

                atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.MAGICPOWER, m_sCurStats.Magic_POW));
                playerDef = Mathf.RoundToInt(m_selectedPlayer.m_buffManager.GetBufferdStatus(STATTYPE.MAGICDEFENSE, m_selectedPlayer.m_sCurStats.Magic_DEF));
                AttackProcess(skill, eResult, true, m_sCurStats.Magic_POW, m_selectedPlayer.m_sCurStats.Magic_DEF, m_selectedPlayer, bFinishAttack);

                break;

            case ATTACKRESULT.PHYSICAL_AREA_ATTACK:

                //���Ⱑ ����
                foreach (var player in GameManager.gm.m_listPlayerInfo)
                {
                    //�̹� �׾��� ���
                    if (player.m_ragdollObj.activeSelf) continue;

                    atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, m_sCurStats.Physical_POW));
                    playerDef = Mathf.RoundToInt(m_selectedPlayer.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, player.m_sCurStats.Physical_DEF));

                    AttackProcess(skill, eResult, false, atkPow, playerDef, player, bFinishAttack);
                }

                break;

            case ATTACKRESULT.MAGIC_AREA_ATTACK:

                //���Ⱑ ����
                foreach (var player in GameManager.gm.m_listPlayerInfo)
                {
                    //�̹� �׾��� ���
                    if (player.m_ragdollObj.activeSelf) continue;

                    atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.MAGICPOWER, m_sCurStats.Magic_POW));
                    playerDef = Mathf.RoundToInt(m_selectedPlayer.m_buffManager.GetBufferdStatus(STATTYPE.MAGICDEFENSE, player.m_sCurStats.Magic_DEF));
                    AttackProcess(skill, eResult, false, atkPow, playerDef, player, bFinishAttack);
                }

                break;

            case ATTACKRESULT.SHIELD_SOLO_STRIKE:

                //�̹� �׾��� ���
                if (m_selectedPlayer.m_ragdollObj.activeSelf) return;

                atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, m_sCurStats.Physical_POW));
                playerDef = Mathf.RoundToInt(m_selectedPlayer.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, m_selectedPlayer.m_sCurStats.Physical_DEF));
                AttackProcess(skill, eResult, false, atkPow, playerDef, m_selectedPlayer, bFinishAttack);

                break;

            case ATTACKRESULT.SHIELD_AREA_STRIKE:

                foreach (var player in GameManager.gm.m_listPlayerInfo)
                {
                    //�̹� �׾��� ���
                    if (player.m_ragdollObj.activeSelf) continue;

                    atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, m_sCurStats.Physical_POW));
                    playerDef = Mathf.RoundToInt(m_selectedPlayer.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, player.m_sCurStats.Physical_DEF));
                    AttackProcess(skill, eResult, false, atkPow, playerDef, player, bFinishAttack);
                }

                break;

            case ATTACKRESULT.PHYSICAL_DEF_DEBUFF_SOLO:

                //�̹� �׾��� ���
                if (m_selectedPlayer.m_ragdollObj.activeSelf) return;

                atkPow = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, m_sCurStats.Physical_POW));
                playerDef = Mathf.RoundToInt(m_selectedPlayer.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, m_selectedPlayer.m_sCurStats.Physical_DEF));
                AttackProcess(skill, eResult, false, atkPow, playerDef, m_selectedPlayer, bFinishAttack);

                break;
        }
    }


    void AttackProcess(SKILLINFO skill, ATTACKRESULT eResult, bool bPierce, int iAtkPow, int iDef, InGamePlayerController playerInfo, bool isFinishAtk)
    {
        float atkPower = 0;
        int damage = 0;
        float enemySpd = m_buffManager.GetBufferdStatus(STATTYPE.SPD, m_sCurStats.SPD);
        int playerSpd = Mathf.RoundToInt(playerInfo.m_buffManager.GetBufferdStatus(STATTYPE.SPD, playerInfo.m_sCurStats.SPD));
        int enemyDef = Mathf.RoundToInt(m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALDEFENSE, m_sCurStats.Physical_DEF)) * (int)CONVERT_STATS.DEF;
        bool isCritical = AttackWeight.GetCritChance(m_listAtkSuccess, enemySpd,
                                                    playerSpd);
        iDef *= (int)CONVERT_STATS.DEF;

        atkPower = (iAtkPow * (int)CONVERT_STATS.ATK) * skill.fAttackPow;


        damage = AttackWeight.AttackDamageCal(m_listAtkSuccess, Mathf.RoundToInt(atkPower), isCritical);

        if (!bPierce)
            damage -= iDef;

        int idx = GameManager.gm.m_listPlayerInfo.IndexOf(playerInfo);

        bool bMiss = false;
        bool bHit = false;

        //� �����̵� ������ ����Ʈ�� �����ǰ� �ؾ��Ѵ�.

        if (damage <= 0)
        {
            playerInfo.m_pv.RPC("AttackBlockRPC", RpcTarget.All, idx);
            eResult = ATTACKRESULT.BLOCK;
        }

        else
        {
            bMiss = AttackWeight.MissCal(m_listAtkSuccess, enemySpd, (float)playerSpd);

            if (bMiss)
            {
                playerInfo.m_pv.RPC("AttackDodgeRPC", RpcTarget.All, idx, isFinishAtk);
                eResult = ATTACKRESULT.MISS;
            }

            else
            {
                playerInfo.m_pv.RPC("AttackHitRPC", RpcTarget.All, idx, damage, isCritical, isFinishAtk);
                bHit = true;
            }
        }

        //����� ���°� ��������
        //ũ��Ƽ�� �Ǵ�, �̽��Ǵ�, ���ݰ�� �Ǵܵ��� �ٽ� �����Ѿߵ�
        Vector3 vOwner = gameObject.transform.position;
        var vOwnerAttackPos = m_listEffectPos[skill.iAttackPosIdx];
        Vector3 vTarget = playerInfo.gameObject.transform.position + new Vector3(0, 1.5f, 0);
        bool bFaint = false;

        //� �����̵� ������ ����Ʈ�� �����ǰ� �ؾ��Ѵ�.
        if (skill.bHoming)
            NetworkManager.nm.PV.RPC("PlayEffect", RpcTarget.All, skill.iAtkEffectIdx, vOwnerAttackPos.position, vTarget, skill.bHoming, 0.1f);
        else
            NetworkManager.nm.PV.RPC("PlayEffect", RpcTarget.All, skill.iAtkEffectIdx, vOwnerAttackPos.position, vOwnerAttackPos.position, false, 1.0f);

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
                bFaint = AttackWeight.GetShieldStrike(m_listAtkSuccess, enemyDef,
                                                            iDef);

                if (bFaint && !bMiss)
                {
                    //��Ʈ���� �� ȿ��
                    if (bHit) NetworkManager.nm.PV.RPC("PlayEffect", RpcTarget.All, (int)EFFECTTYPE.SHINING, vOwner, vTarget, false, 1.0f);

                    //���⼭ ���� ��Ű���
                    //������ ���̰� , ���������� �ε����� �� �� ����
                    int playerIdx = GameManager.gm.m_listPlayerInfo.IndexOf(playerInfo);

                    //���ʹ� �ε��� �ѱ��, ����� �������ƴ϶� ���� �˷���
                    NetworkManager.nm.PV.RPC("BattleFaintTimelineRPC", RpcTarget.All, playerIdx, true);
                }

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
                    //ĳ�������� ����� �߰�������ϰ�

                    BuffInfo buff = new BuffInfo(BUFFTYPE.PHYSICAL_DEFENSE_DEBUFF, skill.iMaxCoolTime, skill.fAttackPow);

                    m_selectedPlayer.ApplyBuff(buff);

                    //UI�� �����ؾߵ�

                    m_selectedPlayer.RefreshInfo(idx, false);
                }

                break;

            case ATTACKRESULT.MISS:


                break;

            case ATTACKRESULT.BLOCK:



                break;
        }

        if (!bFaint && !bMiss)
        {
            //�ǰ��� ����Ʈ�� �����Ѵ�.
            //� �����̵� ������ ����Ʈ�� �����ǰ� �ؾ��Ѵ�.
            NetworkManager.nm.PV.RPC("PlayEffect", RpcTarget.All, skill.iHitEffectIdx, vOwnerAttackPos.position, vTarget, false, 1.0f);
        }

        NetworkManager.nm.PV.RPC("EnemyAttackDamageUI", RpcTarget.All, damage, idx, (int)eResult);

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       
    }
}
