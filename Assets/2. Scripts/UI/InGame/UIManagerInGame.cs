using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        m_curInvenPage = 1;
        m_maxInvenPage = InvenManager.GetMaxPage();

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

        if (PhotonNetwork.IsMasterClient)
        {
            NetworkManager.nm.PV.RPC("InitBattleTimelineRPC", RpcTarget.All);
        }

        GameManager.gm.m_inGamePlayer.RefreshInfo();

        for(int i = 0; i < GameManager.gm.m_listEnemyInfo.Count; i++)
        {
            var enemy = GameManager.gm.m_listEnemyInfo[i];
            RefreshEnemyHud(i, enemy.m_sCurStats);
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

        int idx = BattleManager.m_iCurSelectedSkill;

        foreach(var suc in m_arrUserAttackCheckImg)
        {
            suc.gameObject.SetActive(false);
        }

        //���� �ε����� �����ѰŸ�
        if(idx >= listSkill.Count)
        {
            //�κ��丮 �ֽ�ȭ
            RefreshInventory();

            m_goBattleSelectPanel.transform.Find("InfoPanel").gameObject.SetActive(false);

            m_goBattleSelectPanel.transform.Find("InvenPanel").gameObject.SetActive(true);

            m_goBattleSelectComplete.SetActive(false);
        }

        //��ų ������ �����̸�
        else
        {
            m_goBattleSelectPanel.transform.Find("InfoPanel").gameObject.SetActive(true);
            m_goBattleSelectComplete.SetActive(true);
            m_goBattleSelectPanel.transform.Find("InvenPanel").gameObject.SetActive(false);

            for (int i = 0; i < listSkill[idx].iProbCnt; i++)
            {
                m_arrUserAttackCheckImg[i].gameObject.SetActive(true);
            }

            m_txtTitle.text = listSkill[idx].name;
            int atkPow = 0;

            switch ((ATTACKTYPE)listSkill[idx].iAttackType)
            {

                case ATTACKTYPE.PHYSICAL_SINGLE:
                    m_txtTargetType.text = "���� ����";

                    atkPow = Mathf.RoundToInt(((float)GameManager.gm.m_inGamePlayer.m_sCurStats.Physical_POW * 5) * listSkill[idx].fAttackPow);
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.red;
                    break;

                case ATTACKTYPE.MAGIC_SINGLE:
                    m_txtTargetType.text = "���� ����";

                    atkPow = Mathf.RoundToInt(((float)GameManager.gm.m_inGamePlayer.m_sCurStats.Magic_POW * 5) * listSkill[idx].fAttackPow);
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.blue;
                    break;

                case ATTACKTYPE.COMPLEX_SINGLE:
                    m_txtTargetType.text = "���� ����";
                    break;
            }

            m_txtAtkProb.text = (listSkill[idx].fAttackProb * 100).ToString() +
                "%";

            m_txtConsumeMp.text = listSkill[idx].iConsumeMp.ToString() + " �Ҹ�";
        }


        m_goBattleSelectPanel.SetActive(true);
        //m_goBattleSelectComplete.SetActive(true);
        m_goUserAttackSuccessPanel.SetActive(true);
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

        Debug.Log("���� �Ϸ�");

        m_goBattleSelectComplete.SetActive(false);
        m_goBattleSelectPanel.SetActive(false);

        InGamePlayerController player = BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<InGamePlayerController>();

        player.m_listAtkSuccess.Clear();

        var skill = GameManager.gm.m_inGamePlayer.m_sCurStats.listSkills[BattleManager.m_iCurSelectedSkill];

        //�÷��̾ � ������ �����ߴ����� �˾ƾߵ�
        //���������� ������ �ʿ���.

        //int attackCheckTime = BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<InGamePlayerController>().m_sCurStats.Ƚ��;

        //int attackCheckTime = BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<InGamePlayerController>().m_sCurStats.Ȯ��;

        int attackCheckTime = skill.iProbCnt;

        List<float> listAttackProbs = new List<float>();
        List<bool> listPenets = new List<bool>();

        for (int i = 0; i < attackCheckTime; i++)
        {
            listAttackProbs.Add(skill.fAttackProb);
            listPenets.Add(false);
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

        yield return new WaitForSeconds(0.5f);
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

        enemy.m_listAtkSuccess.Clear();

        int attackCheckTime = 6;

        List<float> listAttackProbs = new List<float>();
        List<bool> listPenets = new List<bool>();

        for (int i = 0; i < attackCheckTime; i++)
        {
            listAttackProbs.Add(0.5f);
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

        yield return new WaitForSeconds(0.5f);
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

    #endregion

    public void RefreshUserHud(int userIdx, CLASSSTATS sInfo)
    {
        var ui = m_arrUserInfo[userIdx];

        if(!ui.gameObject.activeSelf)
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
        ui.curPhysicalPowText.text = sInfo.Physical_POW.ToString();
        ui.curMagicPowText.text = sInfo.Magic_POW.ToString();
        ui.curPhysicalDefText.text = sInfo.Physical_DEF.ToString();
        ui.curMagicDefText.text = sInfo.Magic_DEF.ToString();
        ui.curSpeedText.text = sInfo.SPD.ToString();

        ui.sliderHp.value = (float)sInfo.curHp / (float)sInfo.maxHp;
        ui.sliderHpBack.value = ui.sliderHp.value;

        ui.sliderMp.value = (float)sInfo.curMp / (float)sInfo.maxMp;

    }

    public void RefreshEnemyHud(int enemyIdx, ENEMYSTATS sInfo)
    {
        var ui = m_arrEnemyInfo[enemyIdx];

        if (!ui.gameObject.activeSelf)
            UIManager.ShowScale(ui.gameObject);

        ui.name.text = sInfo.name;
        ui.curHpText.text = sInfo.curHp.ToString();
        ui.curPhysicalDefText.text = sInfo.Physical_DEF.ToString();
        ui.curMagicDefText.text = sInfo.Magic_DEF.ToString();

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

    public void ShowDefeat()
    {

    }
}
