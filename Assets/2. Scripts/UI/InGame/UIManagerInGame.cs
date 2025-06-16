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

    #region 결과 유아이 정리
    public GameObject m_goRewardPanel;
    public List<GameObject> m_listRewardCard;
    #endregion

    #region 유아이 정보
    public UserUIInfo[] m_arrUserInfo;
    public EnemyUIInfo[] m_arrEnemyInfo;
    public CanvasInfo[] m_arrUserCanvas;
    public CanvasInfo[] m_arrEnemyCanvas;
    #endregion

    #region 준비단계 UI 정리
    public CheckControl m_checkControl;
    #endregion

    #region 유저 공격선택 UI 정리
    public Image[] m_arrUserAttackCheckImg;
    public Text m_txtTitle, m_txtTargetType, m_txtAtkPow, m_txtAtkProb, m_txtConsumeMp;
    public Image m_imgAttribute;
    #endregion

    //여긴 아이템사용하는곳
    [SerializeField] List<BattleSlotInfo> m_listBattleInvenSlot;
    [SerializeField] GameObject m_goUseButton, m_goGiveButton;
    List<ITEMINFO> m_curListInven;
    public int m_curInvenPage, m_maxInvenPage;
    public Text m_txtInvenPage;
    //선택된 아이템 정보 표시
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
        //테스트용
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
        //이때 정보전달해야되겠다. 현재 선택되어 있는 기본공격, 스킬, 아이템 등
        var listSkill = GameManager.gm.m_inGamePlayer.m_sCurStats.listSkills;

        if (BattleManager.m_iCurSelectedSkill > listSkill.Count) BattleManager.m_iCurSelectedSkill = 0;

        int idx = BattleManager.m_iCurSelectedSkill;

        foreach(var suc in m_arrUserAttackCheckImg)
        {
            suc.gameObject.SetActive(false);
        }

        //상점 인덱스를 선택한거면
        if(idx >= listSkill.Count)
        {
            //인벤토리 최신화
            RefreshInventory();

            m_goBattleSelectPanel.transform.Find("InfoPanel").gameObject.SetActive(false);

            m_goBattleSelectPanel.transform.Find("InvenPanel").gameObject.SetActive(true);

            m_goBattleSelectComplete.SetActive(false);
        }

        //스킬 선택한 사항이면
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
                    m_txtTargetType.text = "물리 단일";

                    atkPow = Mathf.RoundToInt(((float)GameManager.gm.m_inGamePlayer.m_sCurStats.Physical_POW * 5) * listSkill[idx].fAttackPow);
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.red;
                    break;

                case ATTACKTYPE.MAGIC_SINGLE:
                    m_txtTargetType.text = "마법 단일";

                    atkPow = Mathf.RoundToInt(((float)GameManager.gm.m_inGamePlayer.m_sCurStats.Magic_POW * 5) * listSkill[idx].fAttackPow);
                    m_txtAtkPow.text = atkPow.ToString();
                    m_txtAtkPow.color = Color.blue;
                    break;

                case ATTACKTYPE.COMPLEX_SINGLE:
                    m_txtTargetType.text = "복합 단일";
                    break;
            }

            m_txtAtkProb.text = (listSkill[idx].fAttackProb * 100).ToString() +
                "%";

            m_txtConsumeMp.text = listSkill[idx].iConsumeMp.ToString() + " 소모";
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
            UIManager.SystemMsg("적을 다시한번 선택해주세요");
            return;
        }

        Debug.Log("선택 완료");

        m_goBattleSelectComplete.SetActive(false);
        m_goBattleSelectPanel.SetActive(false);

        InGamePlayerController player = BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<InGamePlayerController>();

        player.m_listAtkSuccess.Clear();

        var skill = GameManager.gm.m_inGamePlayer.m_sCurStats.listSkills[BattleManager.m_iCurSelectedSkill];

        //플레이어가 어떤 공격을 선택했는지를 알아야됨
        //공격정보를 담을게 필요함.

        //int attackCheckTime = BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<InGamePlayerController>().m_sCurStats.횟수;

        //int attackCheckTime = BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<InGamePlayerController>().m_sCurStats.확률;

        int attackCheckTime = skill.iProbCnt;

        List<float> listAttackProbs = new List<float>();
        List<bool> listPenets = new List<bool>();

        for (int i = 0; i < attackCheckTime; i++)
        {
            listAttackProbs.Add(skill.fAttackProb);
            listPenets.Add(false);
        }

        //배틀상태 배틀로 넘긴다
        //배틀상태 배틀로 넘기고 배틀상태에서 전투가 이루어지도록한다.
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

    #region 에너미가 공격대상 설정하는 함수들
    public void EnemyAttackSelect(EnemyFSM enemy)
    {
        //공격대상랜덤설정

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

        //배틀상태 배틀로 넘긴다
        //배틀상태 배틀로 넘기고 배틀상태에서 전투가 이루어지도록한다.
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

    #region 준비 및 정비하는 단계 함수
    public void OnCheckPanel()
    {
        m_checkControl.gameObject.SetActive(true);
    }
    #endregion

    #region 아이템 선택하는 곳 함수
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

        //이름 띄우기
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

        //얘는 특정 조건을 달성되면 활성화되도록 함
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
