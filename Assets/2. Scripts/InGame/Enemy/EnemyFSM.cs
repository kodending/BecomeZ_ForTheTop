using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

#region 에너미 넘버별 애니메이션 정리
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

    #region 에너미 정의
    [SerializeField] List<GameObject> m_listEnemy_1;

    public Dictionary<int, List<GameObject>> m_dicEnemyForms = new Dictionary<int, List<GameObject>>();
    public DicEnemyAnimator m_dicEnemyAnim;
    public List<Dictionary<string, object>> m_enemyInfo;
    public ENEMYSTATS m_sCurStats;
    #endregion

    #region 렉돌 정의
    public GameObject m_originObj;
    public GameObject m_ragdollObj;
    public Rigidbody[] m_arrRigid;
    #endregion

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
        m_dicEnemyForms.Add(1, m_listEnemy_1);

        m_enemyInfo = CSVManager.instance.m_dicData[LOCALDATALOADTYPE.ENEMYINFO].recordDataList;
    }


    [PunRPC]
    public void InitEnemyInfo(int idx, float fsize)
    {
        foreach(var go in m_dicEnemyForms[idx])
        {
            go.SetActive(true);
        }

        m_originObj.SetActive(true);

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
        transform.localScale = new Vector3(fsize, fsize, fsize);

        var info = m_enemyInfo[idx - 1];

        m_sCurStats.INDEX = int.Parse(info["INDEX"].ToString());
        m_sCurStats.HP = int.Parse(info["HP"].ToString());
        m_sCurStats.Physical_POW = int.Parse(info["PHYSICALPOW"].ToString());
        m_sCurStats.Magic_POW = int.Parse(info["MAGICPOW"].ToString());
        m_sCurStats.Physical_DEF = int.Parse(info["PHYSICALDEF"].ToString());
        m_sCurStats.Magic_DEF = int.Parse(info["MAGICDEF"].ToString());
        m_sCurStats.MENT = int.Parse(info["MENT"].ToString());
        m_sCurStats.SPD = float.Parse(info["SPD"].ToString());

        m_sCurStats.maxHp = m_sCurStats.HP * 5;
        m_sCurStats.curHp = m_sCurStats.maxHp;

        m_sCurStats.name = "식인종";
        //m_sCurStats.name = info["NAME"].ToString();

        gameObject.SetActive(true);

        BATTLEINFO sInfo = new BATTLEINFO();
        sInfo.go = this.gameObject;
        sInfo.SPD = m_sCurStats.SPD;
        sInfo.bUser = false;

        BattleManager.m_listBattleInfo.Add(sInfo);

        m_goShadow.SetActive(true);

        InitStateMachine();
    }
    void InitStateMachine()
    {
        m_stateMachine = new EnemyStateMachine(ENEMYSTATE.IDLE, new Enemy_Idle(this));
        m_stateMachine.AddState(ENEMYSTATE.MOVE, new Enemy_Move(this));
        m_stateMachine.AddState(ENEMYSTATE.ATTACK, new Enemy_Attack(this));
        m_stateMachine.AddState(ENEMYSTATE.ATTACK_MOVE, new Enemy_AttackMove(this));
        m_stateMachine.AddState(ENEMYSTATE.BACK_MOVE, new Enemy_BackMove(this));
        m_stateMachine.AddState(ENEMYSTATE.HIT, new Enemy_Hit(this));
        m_stateMachine.AddState(ENEMYSTATE.DEATH, new Enemy_Death(this));
        m_stateMachine.AddState(ENEMYSTATE.BLOCK, new Enemy_Block(this));
        m_stateMachine.AddState(ENEMYSTATE.DODGE, new Enemy_Dodge(this));

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
    public void AttackHitRPC(int enemyIdx, int iAtk, bool isCritcal)
    {
        var uiInfo = GameManager.m_curUI.GetComponent<UIManagerInGame>().m_arrEnemyInfo[enemyIdx];

        m_sCurStats.curHp -= iAtk;

        if(isCritcal)
            BattleManager.ShakeCamera(0.5f, 0.08f);
        else
            BattleManager.ShakeCamera(3.5f, 0.08f);

        if (m_sCurStats.curHp <= 0 )
        {
            uiInfo.gameObject.SetActive(false);
            m_stateMachine.ChangeState(ENEMYSTATE.DEATH);
            return;
        }

        UIManager.CountText(uiInfo.curHpText, m_sCurStats.curHp);

        //체력도 계산하고
        //0 이하일경우 데스되게 하기
        //크리티컬 들어갔는지 여부도 받기
        //여기서 자기 체력 UI 조절하고
        RefreshHp(uiInfo);

        m_stateMachine.ChangeState(ENEMYSTATE.HIT);
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

    [PunRPC]
    public void AttackBlockRPC(int playerIdx)
    {
        BattleManager.ShakeCamera(0.5f, 0.08f);
        m_stateMachine.ChangeState(ENEMYSTATE.BLOCK);
    }

    [PunRPC]
    public void AttackDodgeRPC(int playerIdx)
    {
        m_stateMachine.ChangeState(ENEMYSTATE.DODGE);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       
    }
}
