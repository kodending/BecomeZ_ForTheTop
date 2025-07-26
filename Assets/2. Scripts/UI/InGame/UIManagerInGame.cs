using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerInGame : MonoBehaviourPunCallbacks
{
    [SerializeField] Text m_txtInGame;
    public GameObject m_goTimelineParent;
    [SerializeField] GameObject m_goGameInfoPanel;
    public Transform m_trStartPoint;
    public Transform m_trEndPoint;
    public int m_orderNum;
    [SerializeField] Text m_txtBattleStart;
    public Text m_txtVictory, m_txtDefeat;
    public GameObject m_goBattleSelectPanel;
    public GameObject m_goBattleSelectComplete;
    public GameObject m_goUserAttackSuccessPanel;
    public GameObject m_goEnemyAttackSuccessPanel;
    public Image[] m_arrEnemyAttackCheckImg;
    public Sprite[] m_arrAttackSuccFailSprite;
    public Image m_imgDarkFade;
    public Text m_txtFloor;
    [SerializeField] public Text m_txtNextWaiting;

    #region ��� ������ ����
    public GameObject m_goRewardPanel;
    public List<GameObject> m_listRewardCard;
    #endregion

    #region ������ ����
    public UserUIInfo[] m_arrUserInfo;
    public EnemyUIInfo[] m_arrEnemyInfo;
    public CanvasInfo[] m_arrUserCanvas;
    public CanvasInfo[] m_arrEnemyCanvas;
    #endregion

    #region �غ�ܰ� UI ����
    public CheckControl m_checkControl;
    #endregion

    #region ���� ���ݼ��� UI ����
    public Image[] m_arrUserAttackCheckImg;
    public Text m_txtTitle, m_txtTargetType, m_txtAtkPow, m_txtAtkProb, m_txtConsumeMp;
    public Image m_imgAttribute;
    #endregion

    //���� �����ۻ���ϴ°�
    [SerializeField] List<BattleSlotInfo> m_listBattleInvenSlot;
    [SerializeField] GameObject m_goUseButton, m_goGiveButton;
    List<ITEMINFO> m_curListInven;
    public int m_curInvenPage, m_maxInvenPage;
    public Text m_txtInvenPage;
    //���õ� ������ ���� ǥ��
    [SerializeField] Image m_imgSelectedItem;
    [SerializeField] Text m_txtSelectedItemInfo, m_txtSelectedItemName;
    [SerializeField] GameObject m_goUnavailableText, m_goSelectButton;

    public DamageTextSpawner damageTextSpawner;

    private void Start()
    {
        m_curInvenPage = 1;
        m_maxInvenPage = InvenManager.GetMaxPage();

        //���� �� Ȯ���ϰ� ��Ī �ٲٱ�
        string strFloor = BattleManager.m_iCurFloor.ToString() + "��";
        m_txtInGame.text = strFloor;
        m_txtFloor.text = strFloor;
        UIManager.FadeInOutText(m_txtInGame, 1f);

        StartCoroutine(OnPanel());
    }

    void Update()
    {
        //�׽�Ʈ��
        if (Input.GetKeyDown(KeyCode.R))
        {
            //test
            //OnCheckPanel();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ITEMINFO item = ItemManager.RandomGenerateItem(WEIGHTTYPE.BOX);

            InvenManager.AddItem(item);
        }
    }

    IEnumerator OnPanel()
    {
        yield return new WaitForSeconds(3f);

        UIManager.ShowScale(m_goGameInfoPanel);
        UIManager.ShowScale(m_goTimelineParent);

        int playerIdx = GameManager.gm.m_listPlayerInfo.IndexOf(GameManager.gm.m_inGamePlayer);
        GameManager.gm.m_inGamePlayer.RefreshInfo(playerIdx, true);

        if (PhotonNetwork.IsMasterClient)
        {
            NetworkManager.nm.PV.RPC("InitBattleTimelineRPC", RpcTarget.All);
        }

        for(int i = 0; i < GameManager.gm.m_listEnemyInfo.Count; i++)
        {
            var enemy = GameManager.gm.m_listEnemyInfo[i];
            RefreshEnemyHud(i, enemy.m_sCurStats, true);
        }
       
        yield return new WaitForSeconds(0.5f);

        if (PhotonNetwork.IsMasterClient) StartCoroutine(InstantiateOrderImg());

        yield return new WaitForSeconds(1.0f);

        UIManager.FadeInOutText(m_txtBattleStart, 1f);
        UIManager.FadeInOutText(m_txtBattleStart.transform.Find("Text").GetComponent<Text>(), 1f);

        yield return new WaitForSeconds(2.0f);

        BattleManager.bm.m_stateMachine.ChangeState(BATTLESTATE.SELECT);
    }

    public IEnumerator InstantiateOrderImg()
    {
        var order = PhotonNetwork.Instantiate("INGAMEORDER_OrderImage", m_trStartPoint.position, Quaternion.identity);
        order.GetComponent<OrderInfo>().PV.RPC("InitInfo", RpcTarget.All, BattleManager.m_listBattleTimelineInfo[m_orderNum].bUser, m_orderNum);
        m_orderNum++;

        if (m_orderNum >= 12) yield break;

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(InstantiateOrderImg());
    }

    public void OnBattleSelectPanel()
    {
        //�̶� ���������ؾߵǰڴ�. ���� ���õǾ� �ִ� �⺻����, ��ų, ������ ��
        var listSkill = GameManager.gm.m_inGamePlayer.m_sCurStats.listSkills;

        if (BattleManager.m_iCurSelectedSkill > listSkill.Count) BattleManager.m_iCurSelectedSkill = 0;

        int skillIdx = BattleManager.m_iCurSelectedSkill;

        foreach(var suc in m_arrUserAttackCheckImg)
        {
            suc.gameObject.SetActive(false);
        }

        //���� �ε����� �����ѰŸ�
        if(skillIdx >= listSkill.Count)
        {
            //�κ��丮 �ֽ�ȭ
            RefreshInventory();

            m_goBattleSelectPanel.transform.Find("InfoPanel").gameObject.SetActive(false);

            m_goBattleSelectPanel.transform.Find("InvenPanel").gameObject.SetActive(true);

            m_goBattleSelectComplete.SetActive(false);
            m_goUnavailableText.SetActive(false);
        }

        //��ų ������ �����̸�
        else
        {
            //��ų�� ��밡������ �˾ƾߵ�.
            bool bAvail = AvailableSkillCheck(listSkill[skillIdx]);

            //��밡���ϸ� ��ų Ȱ��ȭ
            m_goUnavailableText.SetActive(!bAvail);

            m_goBattleSelectPanel.transform.Find("InfoPanel").gameObject.SetActive(true);
            m_goBattleSelectComplete.SetActive(bAvail);
            m_goBattleSelectPanel.transform.Find("InvenPanel").gameObject.SetActive(false);

            //�̺κ��� ���޵Ǿ�ߵ�(��ü����)
            for (int i = 0; i < listSkill[skillIdx].iProbCnt; i++)
            {
                m_arrUserAttackCheckImg[i].gameObject.SetActive(true);
            }

            //�� ���� ��ų �ε����� �ٸ���������׵� �����ؾߵ�
            NetworkManager.nm.PV.RPC("CheckSkillProbsRPC", RpcTarget.Others, listSkill[skillIdx].iProbCnt, true);
            //*//
            m_txtTitle.text = listSkill[skillIdx].name;
            int atkPow = 0;

            switch ((ATTACKRESULT)listSkill[skillIdx].iAttackType)
            {
                case ATTACKRESULT.PHYSICAL_ATTACK:
                    m_txtTargetType.text = "���� ����";

                    int buffAtk = Mathf.RoundToInt(GameManager.gm.m_inGamePlayer.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, GameManager.gm.m_inGamePlayer.m_sCurStats.Physical_POW));

                    atkPow = Mathf.RoundToInt(buffAtk * listSkill[skillIdx].fAttackPow) * (int)CONVERT_STATS.ATK;
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.red;

                    BattleManager.m_bSelectedAll = false;
                    break;

                case ATTACKRESULT.MAGIC_ATTACK:
                    m_txtTargetType.text = "���� ����";

                    buffAtk = Mathf.RoundToInt(GameManager.gm.m_inGamePlayer.m_buffManager.GetBufferdStatus(STATTYPE.MAGICPOWER, GameManager.gm.m_inGamePlayer.m_sCurStats.Magic_POW));

                    atkPow = Mathf.RoundToInt(buffAtk * listSkill[skillIdx].fAttackPow) * (int)CONVERT_STATS.ATK;
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.blue;

                    BattleManager.m_bSelectedAll = false;
                    break;

                case ATTACKRESULT.PHYSICAL_PIERCING_ATTACK:
                    m_txtTargetType.text = "���� ����";

                    buffAtk = Mathf.RoundToInt(GameManager.gm.m_inGamePlayer.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, GameManager.gm.m_inGamePlayer.m_sCurStats.Physical_POW));

                    atkPow = Mathf.RoundToInt(buffAtk * listSkill[skillIdx].fAttackPow) * (int)CONVERT_STATS.ATK;
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.red;

                    BattleManager.m_bSelectedAll = false;
                    break;

                case ATTACKRESULT.MAGIC_PIERCING_ATTACK:
                    m_txtTargetType.text = "���� ����";

                    buffAtk = Mathf.RoundToInt(GameManager.gm.m_inGamePlayer.m_buffManager.GetBufferdStatus(STATTYPE.MAGICPOWER, GameManager.gm.m_inGamePlayer.m_sCurStats.Magic_POW));

                    atkPow = Mathf.RoundToInt(buffAtk * listSkill[skillIdx].fAttackPow) * (int)CONVERT_STATS.ATK;
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.blue;

                    BattleManager.m_bSelectedAll = false;

                    break;

                case ATTACKRESULT.PHYSICAL_AREA_ATTACK:
                    m_txtTargetType.text = "���� ����";

                    buffAtk = Mathf.RoundToInt(GameManager.gm.m_inGamePlayer.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, GameManager.gm.m_inGamePlayer.m_sCurStats.Physical_POW));

                    atkPow = Mathf.RoundToInt(buffAtk * listSkill[skillIdx].fAttackPow) * (int)CONVERT_STATS.ATK;
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.red;

                    BattleManager.m_bSelectedAll = true;
                    break;

                case ATTACKRESULT.MAGIC_AREA_ATTACK:
                    m_txtTargetType.text = "���� ����";

                    buffAtk = Mathf.RoundToInt(GameManager.gm.m_inGamePlayer.m_buffManager.GetBufferdStatus(STATTYPE.MAGICPOWER, GameManager.gm.m_inGamePlayer.m_sCurStats.Magic_POW));

                    atkPow = Mathf.RoundToInt(buffAtk * listSkill[skillIdx].fAttackPow) * (int)CONVERT_STATS.ATK;
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.blue;

                    BattleManager.m_bSelectedAll = true;
                    break;

                case ATTACKRESULT.SHIELD_SOLO_STRIKE:
                    m_txtTargetType.text = "���� ����";

                    buffAtk = Mathf.RoundToInt(GameManager.gm.m_inGamePlayer.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, GameManager.gm.m_inGamePlayer.m_sCurStats.Physical_POW));

                    atkPow = Mathf.RoundToInt(buffAtk * listSkill[skillIdx].fAttackPow) * (int)CONVERT_STATS.ATK;
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.red;

                    BattleManager.m_bSelectedAll = false;
                    break;

                case ATTACKRESULT.SHIELD_AREA_STRIKE:
                    m_txtTargetType.text = "���� ����";

                    buffAtk = Mathf.RoundToInt(GameManager.gm.m_inGamePlayer.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, GameManager.gm.m_inGamePlayer.m_sCurStats.Physical_POW));

                    atkPow = Mathf.RoundToInt(buffAtk * listSkill[skillIdx].fAttackPow) * (int)CONVERT_STATS.ATK;
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.red;

                    BattleManager.m_bSelectedAll = true;
                    break;

                case ATTACKRESULT.PHYSICAL_DEF_DEBUFF_SOLO:
                    m_txtTargetType.text = "�������� ����";

                    buffAtk = Mathf.RoundToInt(GameManager.gm.m_inGamePlayer.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, GameManager.gm.m_inGamePlayer.m_sCurStats.Physical_POW));

                    atkPow = Mathf.RoundToInt(buffAtk * listSkill[skillIdx].fAttackPow) * (int)CONVERT_STATS.ATK;
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.red;

                    BattleManager.m_bSelectedAll = false;
                    break;

                case ATTACKRESULT.MAGIC_DEF_DEBUFF_SOLO:
                    m_txtTargetType.text = "�������� ����";

                    buffAtk = Mathf.RoundToInt(GameManager.gm.m_inGamePlayer.m_buffManager.GetBufferdStatus(STATTYPE.MAGICPOWER, GameManager.gm.m_inGamePlayer.m_sCurStats.Magic_POW));

                    atkPow = Mathf.RoundToInt(buffAtk * listSkill[skillIdx].fAttackPow) * (int)CONVERT_STATS.ATK;
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.blue;

                    BattleManager.m_bSelectedAll = false;

                    break;

                case ATTACKRESULT.PHYSICAL_ATK_DEBUFF_SOLO:
                    m_txtTargetType.text = "�������ݷ� ����";

                    buffAtk = Mathf.RoundToInt(GameManager.gm.m_inGamePlayer.m_buffManager.GetBufferdStatus(STATTYPE.PHYSICALPOWER, GameManager.gm.m_inGamePlayer.m_sCurStats.Physical_POW));

                    atkPow = Mathf.RoundToInt(buffAtk * listSkill[skillIdx].fAttackPow) * (int)CONVERT_STATS.ATK;
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.red;
                    BattleManager.m_bSelectedAll = false;
                    break;

                case ATTACKRESULT.MAGIC_ATK_DEBUFF_SOLO:
                    m_txtTargetType.text = "�������ݷ� ����";

                    buffAtk = Mathf.RoundToInt(GameManager.gm.m_inGamePlayer.m_buffManager.GetBufferdStatus(STATTYPE.MAGICPOWER, GameManager.gm.m_inGamePlayer.m_sCurStats.Magic_POW));

                    atkPow = Mathf.RoundToInt(buffAtk * listSkill[skillIdx].fAttackPow) * (int)CONVERT_STATS.ATK;
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.blue;
                    BattleManager.m_bSelectedAll = false;
                    break;
            }

            //����
            var enemy = GameManager.gm.m_listEnemyInfo.Find(b => b.m_bSelected == true);

            if (enemy != null)
            {
                BattleManager.SelectEnemy(GameManager.gm.m_listEnemyInfo.IndexOf(enemy), enemy.gameObject);
            }

            m_txtAtkProb.text = (listSkill[skillIdx].fAttackProb * 100).ToString() +
                "%";

            m_txtConsumeMp.text = listSkill[skillIdx].iConsumeMp.ToString() + " �Ҹ�";
        }


        m_goBattleSelectPanel.SetActive(true);
        //m_goBattleSelectComplete.SetActive(true);
        m_goUserAttackSuccessPanel.SetActive(true);
    }

    bool AvailableSkillCheck(SKILLINFO skill)
    {
        Debug.Log("�������� ��ų �̸� : " + skill.engName);
        //üũüũ (99�� ����)
        if (skill.listAvailClasses.Contains(GameManager.gm.m_inGamePlayer.m_classIdx.ToString()) ||
            skill.listAvailClasses.Contains(99.ToString()))
        {
            return true;
        }

        else return false;
    }

    public void CheckSkillProbs(int probCnt, bool bUser)
    {
        if(bUser)
        {
            foreach (var suc in m_arrUserAttackCheckImg)
            {
                suc.gameObject.SetActive(false);
            }

            for (int i = 0; i < probCnt; i++)
            {
                m_arrUserAttackCheckImg[i].gameObject.SetActive(true);
            }
        }

        else
        {
            foreach (var suc in m_arrEnemyAttackCheckImg)
            {
                suc.gameObject.SetActive(false);
            }

            for (int i = 0; i < probCnt; i++)
            {
                m_arrEnemyAttackCheckImg[i].gameObject.SetActive(true);
            }
        }
    }

    public void OnClickNextSkill()
    {
        BattleManager.m_iCurSelectedSkill++;

        if (BattleManager.m_iCurSelectedSkill > GameManager.gm.m_inGamePlayer.m_sCurStats.listSkills.Count)
            BattleManager.m_iCurSelectedSkill = 0;

        OnBattleSelectPanel();
    }

    public void OnClickPreviousSkill()
    {
        BattleManager.m_iCurSelectedSkill--;

        if (BattleManager.m_iCurSelectedSkill < 0)
            BattleManager.m_iCurSelectedSkill = GameManager.gm.m_inGamePlayer.m_sCurStats.listSkills.Count;

        OnBattleSelectPanel();
    }

    public void OnClickBattleSelectComplete()
    {
        bool isTarget = false;

        foreach (var enemy in GameManager.gm.m_listEnemyInfo)
        {
            if (enemy.m_bSelected)
            {
                isTarget = true;
                BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<InGamePlayerController>().OnBattleSelectEnemy(enemy);
                break;
            }
        }

        if (!isTarget)
        {
            UIManager.SystemMsg("���� �ٽ��ѹ� �������ּ���");
            return;
        }

        InGamePlayerController player = BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<InGamePlayerController>();

        var skill = GameManager.gm.m_inGamePlayer.m_sCurStats.listSkills[BattleManager.m_iCurSelectedSkill];

        //�÷��̾� ���� ���
        int mp = player.m_sCurStats.curMp;

        if(mp - skill.iConsumeMp < 0)
        {
            UIManager.SystemMsg("������ �����մϴ�.");
            return;
        }

        //int playerIdx = GameManager.gm.m_listPlayerInfo.IndexOf(player);

        int playerIdx = 0;

        foreach (var p in GameManager.gm.m_listPlayerInfo)
        {
            if (p.m_pv.Owner.NickName == player.m_pv.Owner.NickName)
            {
                playerIdx = GameManager.gm.m_listPlayerInfo.IndexOf(p);
                break;
            }
        }

        player.ConsumeMP(skill.iConsumeMp, playerIdx);

        m_goBattleSelectComplete.SetActive(false);
        m_goBattleSelectPanel.SetActive(false);

        player.m_listAtkSuccess.Clear();

        //�÷��̾ � ������ �����ߴ����� �˾ƾߵ�
        //���������� ������ �ʿ���.

        int attackCheckTime = skill.iProbCnt;

        List<float> listAttackProbs = new List<float>();
        List<bool> listPenets = new List<bool>();

        for (int i = 0; i < attackCheckTime; i++)
        {
            //�������ε� üũ Penets ��뵵 Ȯ���ؾߵ�

            listAttackProbs.Add(skill.fAttackProb);
            listPenets.Add(false);
        }

        BattleManager.m_bSelectedAll = false;
        var enemyInfo = GameManager.gm.m_listEnemyInfo.Find(b => b.m_bSelected == true);

        if (enemyInfo != null)
        {
            BattleManager.SelectEnemy(GameManager.gm.m_listEnemyInfo.IndexOf(enemyInfo), enemyInfo.gameObject);
        }

        //��Ʋ���� ��Ʋ�� �ѱ��
        //��Ʋ���� ��Ʋ�� �ѱ�� ��Ʋ���¿��� ������ �̷���������Ѵ�.
        StartCoroutine(OnUserAttackAccCheck(attackCheckTime, listAttackProbs, listPenets, player));
    }

    public IEnumerator OnUserAttackAccCheck(int iTime, List<float> listAttackProbs, List<bool> listPenets, InGamePlayerController player)
    {
        if (iTime <= 0)
        {
            BattleManager.bm.m_stateMachine.ChangeState(BATTLESTATE.BATTLE);
            NetworkManager.nm.PV.RPC("InitUserAttackCalHudRPC", RpcTarget.All);

            yield break;
        }

        int idx = listAttackProbs.Count - iTime;
        bool bSuccess = AttackWeight.AttackSuccessCal(listAttackProbs[idx], listPenets[idx]);
        player.m_listAtkSuccess.Add(bSuccess);

        NetworkManager.nm.PV.RPC("UserAttackCalHudRPC", RpcTarget.All, idx, bSuccess);

        iTime -= 1;

        yield return new WaitForSeconds(0.35f);
        StartCoroutine(OnUserAttackAccCheck(iTime, listAttackProbs, listPenets, player));
    }

    public void ApplyUserAttackUI(int idx, bool bSuccess)
    {
        if (!m_goUserAttackSuccessPanel.activeSelf) m_goUserAttackSuccessPanel.SetActive(true);

        if (bSuccess)
        {
            m_arrUserAttackCheckImg[idx].sprite = m_arrAttackSuccFailSprite[1];
            m_arrUserAttackCheckImg[idx].color = Color.blue;
        }

        else
        {
            m_arrUserAttackCheckImg[idx].sprite = m_arrAttackSuccFailSprite[2];
            m_arrUserAttackCheckImg[idx].color = Color.red;
        }
    }

    public void InitUserAttackUI()
    {
        m_goUserAttackSuccessPanel.SetActive(false);

        foreach (var img in m_arrUserAttackCheckImg)
        {
            img.sprite = m_arrAttackSuccFailSprite[0];
            img.color = Color.white;
        }
    }

    #region ���ʹ̰� ���ݴ�� �����ϴ� �Լ���
    public void EnemyAttackSelect(EnemyFSM enemy)
    {
        //���ݴ�󷣴�����

        int idx = Random.Range(0, GameManager.gm.m_listPlayerInfo.Count);

        if (GameManager.gm.m_listPlayerInfo[idx].m_sCurStats.curHp <= 0)
        {
            EnemyAttackSelect(enemy);
            return;
        }

        enemy.m_selectedPlayer = GameManager.gm.m_listPlayerInfo[idx];

        m_goBattleSelectComplete.SetActive(false);
        m_goBattleSelectPanel.SetActive(false);

        enemy.m_listAtkSuccess.Clear();

        foreach (var suc in m_arrEnemyAttackCheckImg)
        {
            suc.gameObject.SetActive(false);
        }

        //���ʹ� ���� ��ų ����
        enemy.m_sSelectedSkill = enemy.PickSkill(enemy.m_sCurStats.listSkills);

        int attackCheckTime = enemy.m_sSelectedSkill.iProbCnt;

        //�̰� ��ü ���ĵǾ����
        for (int i = 0; i < attackCheckTime; i++)
        {
            m_arrEnemyAttackCheckImg[i].gameObject.SetActive(true);
        }

        NetworkManager.nm.PV.RPC("CheckSkillProbsRPC", RpcTarget.Others, attackCheckTime, false);

        List<float> listAttackProbs = new List<float>();
        List<bool> listPenets = new List<bool>();

        for (int i = 0; i < attackCheckTime; i++)
        {
            //����Ȯ���� �����Ϳ����� �����Ų��.
            listAttackProbs.Add(enemy.m_sSelectedSkill.fAttackProb);
            listPenets.Add(false);
        }

        //��Ʋ���� ��Ʋ�� �ѱ��
        //��Ʋ���� ��Ʋ�� �ѱ�� ��Ʋ���¿��� ������ �̷���������Ѵ�.
        StartCoroutine(OnEnemyAttackAccCheck(attackCheckTime, listAttackProbs, listPenets, enemy));
    }

    public IEnumerator OnEnemyAttackAccCheck(int iTime, List<float> listAttackProbs, List<bool> listPenets, EnemyFSM enemy)
    {
        if (iTime <= 0)
        {
            BattleManager.bm.m_stateMachine.ChangeState(BATTLESTATE.BATTLE);
            NetworkManager.nm.PV.RPC("InitEnemyAttackCalHudRPC", RpcTarget.All);

            yield break;
        }

        int idx = listAttackProbs.Count - iTime;
        bool bSuccess = AttackWeight.AttackSuccessCal(listAttackProbs[idx], listPenets[idx]);
        enemy.m_listAtkSuccess.Add(bSuccess);

        NetworkManager.nm.PV.RPC("EnemyAttackCalHudRPC", RpcTarget.All, idx, bSuccess);

        iTime -= 1;

        yield return new WaitForSeconds(0.35f);
        StartCoroutine(OnEnemyAttackAccCheck(iTime, listAttackProbs, listPenets, enemy));
    }

    public void ApplyEnemyAttackUI(int idx, bool bSuccess)
    {
        if (!m_goEnemyAttackSuccessPanel.activeSelf) m_goEnemyAttackSuccessPanel.SetActive(true);

        if (bSuccess)
        {
            m_arrEnemyAttackCheckImg[idx].sprite = m_arrAttackSuccFailSprite[1];
            m_arrEnemyAttackCheckImg[idx].color = Color.blue;
        }

        else
        {
            m_arrEnemyAttackCheckImg[idx].sprite = m_arrAttackSuccFailSprite[2];
            m_arrEnemyAttackCheckImg[idx].color = Color.red;
        }
    }

    public void InitEnemyAttackUI()
    {
        m_goEnemyAttackSuccessPanel.SetActive(false);

        foreach (var img in m_arrEnemyAttackCheckImg)
        {
            img.sprite = m_arrAttackSuccFailSprite[0];
            img.color = Color.white;
        }
    }

    #endregion

    #region �غ� �� �����ϴ� �ܰ� �Լ�
    public void OnCheckPanel()
    {
        m_checkControl.gameObject.SetActive(true);
    }
    #endregion

    #region ������ �����ϴ� �� �Լ�
    public void DeselectInvenSlots()
    {
        foreach (var slot in m_listBattleInvenSlot)
        {
            slot.DeselectSlot();
        }
    }

    public void ActvieBattleItemButton(bool bActive)
    {
        m_goGiveButton.gameObject.SetActive(bActive);
        m_goUseButton.gameObject.SetActive(bActive);
    }

    public void ShowItemInfo(ITEMINFO item)
    {
        if(item.idx == 0)
        {
            m_imgSelectedItem.gameObject.SetActive(false);
            m_txtSelectedItemInfo.gameObject.SetActive(false);
            m_txtSelectedItemName.gameObject.SetActive(false);
            return;
        }

        m_imgSelectedItem.sprite = item.sprite;
        m_imgSelectedItem.gameObject.SetActive(true);

        m_txtSelectedItemInfo.text = item.strExplain;
        m_txtSelectedItemInfo.transform.Find("Text").GetComponent<Text>().text = item.strExplain;

        //�̸� ����
        m_txtSelectedItemName.text = item.strName;
        m_txtSelectedItemName.transform.Find("Text").GetComponent<Text>().text = item.strName;

        m_txtSelectedItemInfo.gameObject.SetActive(true);
        m_txtSelectedItemName.gameObject.SetActive(true);
    }

    public void RefreshInventory()
    {
        m_maxInvenPage = InvenManager.GetMaxPage();
        m_curListInven = InvenManager.GetCurPageItem(m_curInvenPage);

        string txtPage = m_curInvenPage.ToString() + " / " + m_maxInvenPage.ToString();

        m_txtInvenPage.text = txtPage;
        m_txtInvenPage.transform.Find("Text").GetComponent<Text>().text = m_txtInvenPage.text;

        if (m_curListInven == null) return;

        for (int i = 0; i < m_listBattleInvenSlot.Count; i++)
        {
            var slot = m_listBattleInvenSlot[i];

            if (i >= m_curListInven.Count)
            {
                slot.ResetItem();
                continue;
            }

            else
            {
                slot.SetItem(m_curListInven[i]);
            }
        }
    }

    public void OnClickInvenNextPage()
    {
        m_curInvenPage++;

        m_maxInvenPage = InvenManager.GetMaxPage();

        if (m_curInvenPage > m_maxInvenPage) m_curInvenPage = 1;

        RefreshInventory();
    }

    public void OnClickInvenPrePage()
    {
        m_curInvenPage--;

        m_maxInvenPage = InvenManager.GetMaxPage();

        if (m_curInvenPage <= 0) m_curInvenPage = m_maxInvenPage;

        RefreshInventory();
    }

    public void OnClickUseSelectedItem()
    {
        //���õ� �Ҹ�ǰ ������ �˾ƾ��ϰ�
        var invenSlot = GetSelectedBattleInven();
        ITEMINFO item = new ITEMINFO();
        if (invenSlot != null) item = invenSlot.m_item;

        int invenIdx = GetSelectedBattleInvenIndex();

        if (invenSlot.m_item.iType != (int)CONSUMETYPE.MP_POTION && invenSlot.m_item.iType != (int)CONSUMETYPE.HP_POTION)
        {
            UIManager.SystemMsg("�Ҹ�ǰ(����)�� ��� �����մϴ�");
            return;
        }

        //�÷��̾�ȿ��� �ذ��ϵ��� ��.
        //[����]
        //�길�� �������� �����ߵ�.
        StartCoroutine(GameManager.gm.m_inGamePlayer.UseConsumeItem(invenIdx, item, true));
    }

    int GetSelectedBattleInvenIndex()
    {
        int index = 0;

        for (; index < m_listBattleInvenSlot.Count; index++)
        {
            if (m_listBattleInvenSlot[index].IsSelected()) break;
        }

        return index;
    }

    BattleSlotInfo GetSelectedBattleInven()
    {
        BattleSlotInfo slot = null;

        for (int i = 0; i < m_listBattleInvenSlot.Count; i++)
        {
            if (m_listBattleInvenSlot[i].IsSelected())
            {
                slot = m_listBattleInvenSlot[i];
                break;
            }
        }

        return slot;
    }

    public void InitBattleInventory()
    {
        m_imgSelectedItem.gameObject.SetActive(false);
        m_txtSelectedItemInfo.gameObject.SetActive(false);
        m_txtSelectedItemName.gameObject.SetActive(false);

        foreach(var slot in m_listBattleInvenSlot)
        {
            slot.DeselectSlot();
        }

        GameManager.m_curUI.GetComponent<UIManagerInGame>().ActvieBattleItemButton(false);
    }

    #endregion

    public void RefreshUserHud(int userIdx, CLASSSTATS sInfo, bool bInit)
    {
        var ui = m_arrUserInfo[userIdx];

        if(!ui.gameObject.activeSelf && bInit)
            UIManager.ShowScale(ui.gameObject);

        for (int i = 0; i < sInfo.INSIGHT; i++)
        {
            ui.maxInsights[i].SetActive(true);
            ui.curInsights[i].SetActive(true);
        }

        ui.name.text = sInfo.name;

        //��� Ư�� ������ �޼��Ǹ� Ȱ��ȭ�ǵ��� ��
        ui.attributeImg.color = BattleManager.bm.m_listAttributeColors[(int)sInfo.eClass];
        ui.attributeImg.sprite = BattleManager.bm.m_listAttributeSprites[(int)sInfo.eClass];

        ui.curHpText.text = sInfo.curHp.ToString();
        ui.curMaxHpText.text = sInfo.curHp.ToString() + " / " + sInfo.maxHp.ToString();
        ui.curMaxMpText.text = sInfo.curMp.ToString() + " / " + sInfo.maxMp.ToString();
        ui.curPhysicalPowText.text = (sInfo.Physical_POW * (int)CONVERT_STATS.ATK).ToString();
        ui.curMagicPowText.text = (sInfo.Magic_POW * (int)CONVERT_STATS.ATK).ToString();
        ui.curPhysicalDefText.text = (sInfo.Physical_DEF * (int)CONVERT_STATS.DEF).ToString();
        ui.curMagicDefText.text = (sInfo.Magic_DEF * (int)CONVERT_STATS.DEF).ToString();
        ui.curSpeedText.text = sInfo.SPD.ToString();

        ui.sliderHp.value = (float)sInfo.curHp / (float)sInfo.maxHp;
        ui.sliderHpBack.value = ui.sliderHp.value;

        ui.sliderMp.value = (float)sInfo.curMp / (float)sInfo.maxMp;
        ui.sliderMpBack.value = ui.sliderMp.value;

    }

    public void RefreshEnemyHud(int enemyIdx, ENEMYSTATS sInfo, bool bInit)
    {
        var ui = m_arrEnemyInfo[enemyIdx];

        if (!ui.gameObject.activeSelf && bInit)
            UIManager.ShowScale(ui.gameObject);

        ui.name.text = sInfo.name;
        ui.curHpText.text = sInfo.curHp.ToString();
        ui.curPhysicalDefText.text = (sInfo.Physical_DEF * (int)CONVERT_STATS.DEF).ToString();
        ui.curMagicDefText.text = (sInfo.Magic_DEF * (int)CONVERT_STATS.DEF).ToString();

        ui.sliderHp.value = (float)sInfo.curHp / (float)sInfo.maxHp;
    }

    public void ShowVictory()
    {
        foreach(var ui in m_arrUserInfo)
        {
            ui.gameObject.SetActive(false);
        }

        UIManager.FadeInOutText(m_txtVictory, 2f);
        UIManager.FadeInOutText(m_txtVictory.transform.Find("Text").GetComponent<Text>(), 2f);
    }

    public IEnumerator ShowDefeat()
    {
        foreach (var ui in m_arrUserInfo)
        {
            ui.gameObject.SetActive(false);
        }

        UIManager.FadeInOutText(m_txtDefeat, 2f);
        UIManager.FadeInOutText(m_txtDefeat.transform.Find("Text").GetComponent<Text>(), 2f);

        yield return new WaitForSeconds(2f);

        UIManager.FadeInImage(m_imgDarkFade, 1f);

        yield return new WaitForSeconds(1f);

        GameManager.gm.bReturnRoom = true;
        UIManager.um.StartCoroutine(GameManager.ChangeScene("WaitingRoomScene", GAMESTATE.WAITROOM, 2f));
    }
}
