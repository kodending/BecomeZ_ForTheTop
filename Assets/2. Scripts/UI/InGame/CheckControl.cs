using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

#region �� ���� ���� ��������
[System.Serializable]
public class DicSlotInfo : SerializableDictionary<int, SlotInfo> { }
#endregion

#region ���� UI ���� ����
[System.Serializable]
public class DicStatUI : SerializableDictionary<STATTYPE, GameObject> { }
#endregion

public class CheckControl : MonoBehaviour
{
    public static CheckControl cc;

    public GameObject m_goLeftPanel, m_goRightPanel, m_goReadyPanel;

    [SerializeField] List<SlotInfo> m_listInvenSlot;
    [SerializeField] DicSlotInfo m_dicRuneSlot;

    public Text m_txtSpeed, m_txtInsight, m_txtPhysicalAtk,
                m_txtPhysicalDef, m_txtMagicAtk, m_txtMagicDef, m_txtHp, m_txtMp, m_txtPage;

    public Slider m_sliderHp, m_sliderMp;

    public GameObject m_goUpdownSpeed, m_goUpdownInsight, m_goUpdownPhysicalAtk,
                      m_goUpdownPhysicalDef, m_goUpdownMagicAtk, m_goUpdownMagicDef;

    List<ITEMINFO> m_curListInven = new List<ITEMINFO>();
    Dictionary<int, ITEMINFO> m_curDicEquipment = new Dictionary<int, ITEMINFO>();

    public int m_curPage, m_maxPage;

    //���õ� ������ ���� ǥ��
    [SerializeField] Image m_imgSelectedItem;
    [SerializeField] Text m_txtSelectedItemInfo, m_txtSelectedItemName;

    //������ �� ���� ǥ��
    [SerializeField] Text m_txtSelectedRuneInfo, m_txtSelectedRuneName;
    [SerializeField]
    Text[] m_arrSelectedRuneStats;

    [SerializeField] GameObject m_goEquipButton, m_goDetachButton, m_goGiveButton, m_goUseButton, m_goShiftButton;

    [SerializeField] DicStatUI m_dicStatUI;

    private void Awake()
    {
        cc = this;
    }

    public void onClickReady()
    {
        m_goReadyPanel.SetActive(false);
        this.gameObject.SetActive(false);
        BattleManager.bm.m_stateMachine.ChangeState(BATTLESTATE.NEXTFLOOR);
    }

    public void OnClickCheck()
    {
        m_goReadyPanel.SetActive(false);
        RefreshStatInfo();
        InitInvenPage();
        RefreshInventory();
        RefreshEquipment();

        UIManager.ShowSlideUI(m_goLeftPanel, UIDIRECTION.RIGHT);
        UIManager.ShowSlideUI(m_goRightPanel, UIDIRECTION.LEFT);
    }

    void InitInvenPage()
    {
        m_curPage = 1;
        m_maxPage = InvenManager.GetMaxPage();

        string txtPage = m_curPage.ToString() + " / " + m_maxPage.ToString();

        cc.m_txtPage.text = txtPage;
        cc.m_txtPage.transform.Find("Text").GetComponent<Text>().text = cc.m_txtPage.text;
    }

    public void OnClickExitCheck()
    {
        ResetSelectItemInfo();
        DeselectEquipSlot();
        DeselectInvenSlot();

        m_goLeftPanel.SetActive(false);
        m_goRightPanel.SetActive(false);

        m_goReadyPanel.SetActive(true);
    }

    public void OnClickInvenNextPage()
    {
        m_curPage++;

        if (m_curPage > m_maxPage) m_curPage = 1;

        RefreshInventory();
    }

    public void OnClickInvenPrePage()
    {
        m_curPage--;

        if (m_curPage <= 0) m_curPage = m_maxPage;

        RefreshInventory();
    }

    static public void DeselectEquipSlot()
    {
        for(int i = 0; i < cc.m_dicRuneSlot.Count; i++)
        {
            var slot = cc.m_dicRuneSlot[i];
            slot.DeselectSlot();
        }
    }

    static public void DeselectInvenSlot()
    {
        foreach(var slot in cc.m_listInvenSlot)
        {
            slot.DeselectSlot();
        }
    }

    static public bool GetIsInvenItem(SlotInfo slotInfo)
    {
        foreach(var slot in cc.m_listInvenSlot)
        {
            if(slot == slotInfo) return true;
        }

        return false;
    }

    static public int GetListIdx(SlotInfo slotInfo)
    {
        if(cc.m_listInvenSlot.Contains(slotInfo))
        {
            return cc.m_listInvenSlot.IndexOf(slotInfo);
        }

        for(int i = 0; i < cc.m_dicRuneSlot.Count; i++)
        {
            var slot = cc.m_dicRuneSlot[i];

            if(slot == slotInfo) return i;
        }

        return -1;
    }

    static public void RefreshStatInfo()
    {
        CLASSSTATS stats = GameManager.gm.m_inGamePlayer.m_sCurStats;

        //���ǵ� ��
        cc.m_txtSpeed.text = stats.SPD.ToString();
        cc.m_txtSpeed.transform.Find("Text").GetComponent<Text>().text = cc.m_txtSpeed.text;

        //������ ��
        cc.m_txtInsight.text = stats.INSIGHT.ToString();
        cc.m_txtInsight.transform.Find("Text").GetComponent<Text>().text = cc.m_txtInsight.text;

        //�������ݷ� ��
        cc.m_txtPhysicalAtk.text = stats.Physical_POW.ToString();
        cc.m_txtPhysicalAtk.transform.Find("Text").GetComponent<Text>().text = cc.m_txtPhysicalAtk.text;

        //�������� ��
        cc.m_txtPhysicalDef.text = stats.Physical_DEF.ToString();
        cc.m_txtPhysicalDef.transform.Find("Text").GetComponent<Text>().text = cc.m_txtPhysicalDef.text;

        //�������ݷ� ��
        cc.m_txtMagicAtk.text = stats.Magic_POW.ToString();
        cc.m_txtMagicAtk.transform.Find("Text").GetComponent<Text>().text = cc.m_txtMagicAtk.text;

        //�������� ��
        cc.m_txtMagicDef.text = stats.Magic_DEF.ToString();
        cc.m_txtMagicDef.transform.Find("Text").GetComponent<Text>().text = cc.m_txtMagicDef.text;

        //ü��
        float value = (float)stats.curHp / (float)stats.maxHp;
        cc.m_sliderHp.value = value;
        cc.m_txtHp.text = stats.curHp.ToString() + " / " + stats.maxHp.ToString();

        //����
        value = (float)stats.curMp / (float)stats.maxMp;
        cc.m_sliderMp.value = value;
        cc.m_txtMp.text = stats.curMp.ToString() + " / " + stats.maxMp.ToString();
    }

    static public void RefreshInventory()
    {
        cc.m_curListInven = InvenManager.GetCurPageItem(cc.m_curPage);

        for(int i = 0; i < cc.m_listInvenSlot.Count; i++)
        {
            var slot = cc.m_listInvenSlot[i];

            if (i >= cc.m_curListInven.Count)
            {
                slot.ResetItem();
                continue;
            }

            else
            {
                slot.SetItem(cc.m_curListInven[i]);
            }
        }

        string txtPage = cc.m_curPage.ToString() + " / " + cc.m_maxPage.ToString();

        cc.m_txtPage.text = txtPage;
        cc.m_txtPage.transform.Find("Text").GetComponent<Text>().text = cc.m_txtPage.text;

        ResetSelectItemInfo();
        DeselectInvenSlot();
        RefreshActiveButton();
    }

    static public void RefreshEquipment()
    {
        cc.m_curDicEquipment = InvenManager.GetEquipment();

        for (int i = 0; i < cc.m_dicRuneSlot.Count; i++)
        {
            var slot = cc.m_dicRuneSlot[i];

            if (cc.m_curDicEquipment[i].idx == 0)
            {
                slot.ResetItem();
                continue;
            }

            else
            {
                slot.SetItem(cc.m_curDicEquipment[i]);
            }
        }

        //ĳ���� ���� ���� �ؾߵ�
    }

    static public void ShowInvenItemInfo(SlotInfo slotInfo)
    {
        if(slotInfo.m_item.idx == 0)
        {
            cc.m_imgSelectedItem.gameObject.SetActive(false);
            cc.m_txtSelectedItemInfo.gameObject.SetActive(false);
            cc.m_txtSelectedItemName.gameObject.SetActive(false);
            return;
        }

        //�̹��� ����
        cc.m_imgSelectedItem.sprite = slotInfo.m_item.sprite;
        cc.m_imgSelectedItem.gameObject.SetActive(true);

        //����� ����
        cc.m_txtSelectedItemInfo.text = slotInfo.m_item.strExplain;
        cc.m_txtSelectedItemInfo.transform.Find("Text").GetComponent<Text>().text = slotInfo.m_item.strExplain;

        //�̸� ����
        cc.m_txtSelectedItemName.text = slotInfo.m_item.strName;
        cc.m_txtSelectedItemName.transform.Find("Text").GetComponent<Text>().text = slotInfo.m_item.strName;

        cc.m_txtSelectedItemInfo.gameObject.SetActive(true);
        cc.m_txtSelectedItemName.gameObject.SetActive(true);
    }

    static public void ShowEquipRuneInfo(SlotInfo slotInfo)
    {
        if (slotInfo.m_item.idx == 0)
        {
            cc.m_txtSelectedRuneName.gameObject.SetActive(false);
            cc.m_txtSelectedRuneInfo.gameObject.SetActive(false);
            foreach (var txt in cc.m_arrSelectedRuneStats) txt.gameObject.SetActive(false);
            return;
        }

        //����� ����
        cc.m_txtSelectedRuneInfo.text = slotInfo.m_item.strExplain;
        cc.m_txtSelectedRuneInfo.transform.Find("Text").GetComponent<Text>().text = slotInfo.m_item.strExplain;

        //�̸� ����
        cc.m_txtSelectedRuneName.text = slotInfo.m_item.strName;
        cc.m_txtSelectedRuneName.transform.Find("Text").GetComponent<Text>().text = slotInfo.m_item.strName;

        cc.m_txtSelectedRuneInfo.gameObject.SetActive(true);
        cc.m_txtSelectedRuneName.gameObject.SetActive(true);

        //���� ���� ����
        for(int i = 0; i < cc.m_arrSelectedRuneStats.Length; i++)
        {
            var txtStat = cc.m_arrSelectedRuneStats[i];

            if (i < slotInfo.m_item.arrStats.Length)
            { 
                var stat = slotInfo.m_item.arrStats[i];
                string strStat = "";

                switch (stat.type)
                {
                    case STATTYPE.PHYSICALPOWER:
                        strStat = "�������ݷ� +";
                        break;

                    case STATTYPE.MAGICPOWER:
                        strStat = "�������ݷ� +";
                        break;

                    case STATTYPE.PHYSICALDEFENSE:
                        strStat = "�������� +";
                        break;

                    case STATTYPE.MAGICDEFENSE:
                        strStat = "�������� +";
                        break;

                    case STATTYPE.MENT:
                        strStat = "���ŷ� +";
                        break;

                    case STATTYPE.SPD:
                        strStat = "�ӵ� +";
                        break;

                    case STATTYPE.HP:
                        strStat = "ü�� +";
                        break;

                    case STATTYPE.INSIGHT:
                        strStat = "������ +";
                        break;
                }


                strStat += stat.iStat.ToString();
                txtStat.text = strStat;
                txtStat.transform.Find("Text").GetComponent<Text>().text = strStat;

                txtStat.gameObject.SetActive(true);
            }

            else
            {
                txtStat.gameObject.SetActive(false);
            }
        }
    }

    static public void RefreshActiveButton()
    {
        SlotInfo equipSlot = GetSelectedRune();
        SlotInfo invenSlot = GetSelectedInven();

        //�켱 ��� ��Ȱ��ȭ
        cc.m_goEquipButton.SetActive(false);
        cc.m_goDetachButton.SetActive(false);
        cc.m_goShiftButton.SetActive(false);
        cc.m_goGiveButton.SetActive(false);
        cc.m_goUseButton.SetActive(false);

        if (equipSlot == null && invenSlot == null) return;

        //equip�� ������ ���õǰ� �κ� ������ ���þȵǾ� �ְų� �󽽷��ΰ��
        //���� ��ư�� Ȱ��ȭ

        if(equipSlot != null)
        {
            if (equipSlot.m_item.idx != 0 && invenSlot == null)
                cc.m_goDetachButton.SetActive(true);

            if (equipSlot.m_item.idx != 0 && invenSlot != null)
                if (invenSlot.m_item.idx == 0)
                    cc.m_goDetachButton.SetActive(true);
        }

        //�κ����� ������ ������ �Ҹ�ǰ�ΰ��
        //��� �� ���� ��ư Ȱ��ȭ

        if(invenSlot != null)
        {
            if (invenSlot.m_item.itemTypeIdx == (int)ITEMTYPE.CONSUME)
            {
                cc.m_goGiveButton.SetActive(true);
                cc.m_goUseButton.SetActive(true);
            }

            //�κ����� ������ ������ ���̰� equip������ �ȵǾ��ִ°��
            //���� ��ư�� Ȱ��ȭ
            if (invenSlot.m_item.itemTypeIdx == (int)ITEMTYPE.RUNE && equipSlot == null)
            {
                cc.m_goGiveButton.SetActive(true);
            }

            if (invenSlot.m_item.itemTypeIdx == (int)ITEMTYPE.RUNE && equipSlot != null)
            {
                //�κ����� ������ ������ ���̰� equip�� �󽽷��� ���õȰ��
                //���� �� ���� Ȱ��ȭ
                if (equipSlot.m_item.idx == 0)
                {
                    cc.m_goEquipButton.SetActive(true);
                    cc.m_goGiveButton.SetActive(true);
                }

                //�κ����� ������ ������ ���̰� equip�� ���� ���õ� ���
                //��ü �� ���� Ȱ��ȭ
                if (equipSlot.m_item.itemTypeIdx == (int)ITEMTYPE.RUNE)
                {
                    cc.m_goShiftButton.SetActive(true);
                    cc.m_goGiveButton.SetActive(true);
                }
            }
        }
    }

    #region ������ ���� ���� ���� ���� ��ư �Լ�
    public void OnClickEquipRune()
    {
        SlotInfo equipSlot = GetSelectedRune();
        SlotInfo invenSlot = GetSelectedInven();

        int equipIdx = GetSelectedRuneIndex();
        int invenIdx = GetSelectedInvenIndex();

        ITEMINFO item = new ITEMINFO();
        if(invenSlot != null)
            item = invenSlot.m_item;

        InvenManager.EquipRune(invenIdx, equipIdx, item);

        if(equipSlot != null)
            equipSlot.SetRune(item);

        //ĳ���� ���� ���� ����ߵ�
    }

    public void OnClickDetachRune()
    {
        SlotInfo equipSlot = GetSelectedRune();

        int equipIdx = GetSelectedRuneIndex();

        ITEMINFO item = new ITEMINFO();
        if (equipSlot != null)
        {
            item = equipSlot.m_item;
            equipSlot.m_item = new ITEMINFO();
        }

        InvenManager.DetachRune(item, equipIdx);
    }

    public void OnClickShiftRune()
    {
        SlotInfo equipSlot = GetSelectedRune();
        SlotInfo invenSlot = GetSelectedInven();

        int equipIdx = GetSelectedRuneIndex();
        int invenIdx = GetSelectedInvenIndex();

        ITEMINFO item = new ITEMINFO();
        if (invenSlot != null)
            item = invenSlot.m_item;

        InvenManager.ShiftRune(item, equipIdx, invenIdx);

        if (equipSlot != null)
        {
            equipSlot.SetRune(item);
        }
    }

    //�Ҹ�ǰ ��� �Լ�
    //���� �Լ� �־�ߵ�

    #endregion

    #region �� ���� �� �� ���� ��ȭ �Լ�
    static public void StatCompareInfo()
    {
        SlotInfo equipSlot = GetSelectedRune();
        SlotInfo invenSlot = GetSelectedInven();

        int equipIdx = GetSelectedRuneIndex();
        int invenIdx = GetSelectedInvenIndex();

        if (invenSlot == null)
        {
            InitRuneStat();
            return;
        }

        if (invenSlot.m_item.itemTypeIdx != (int)ITEMTYPE.RUNE)
        {
            InitRuneStat();
            return;
        }

        //equipSlot�� ������ ������ �ְ�
        //
        if (equipSlot != null)
        {
            //�켱 �ʱ�ȭ ���Ѿߵ� ����?
            InitRuneStat();

            //�󽽷��̱⶧���� �κ��� ���õ� �� �ɷ�ġ�� �÷��� �����ش�
            foreach (var info in invenSlot.m_item.arrStats)
            {
                GameObject go = cc.m_dicStatUI[info.type].transform.Find("Text").gameObject;

                var txtStat = go.GetComponent<Text>().text;

                int iStat = int.Parse(txtStat);

                if (info.type == STATTYPE.MENT || info.type == STATTYPE.HP)
                    iStat += (info.iStat * 5);
                else
                    iStat += info.iStat;

                go.GetComponent<Text>().text = iStat.ToString();
                go.transform.Find("Text").GetComponent<Text>().text = iStat.ToString();

                Image img = cc.m_dicStatUI[info.type].transform.Find("RiseFallImage").GetComponent<Image>();

                img.transform.localRotation = Quaternion.Euler(0, 0, 90);
                img.color = Color.red;

                img.gameObject.SetActive(true);
                cc.m_dicStatUI[info.type].SetActive(true);
            }

            //������ ������ ���� ����.
            if (equipSlot.m_item.idx == 0) return;

            //���⼭ ����
            foreach (var info in equipSlot.m_item.arrStats)
            {
                GameObject go = cc.m_dicStatUI[info.type].transform.Find("Text").gameObject;

                var txtStat = go.GetComponent<Text>().text;

                int iStat = int.Parse(txtStat);

                if (info.type == STATTYPE.MENT || info.type == STATTYPE.HP)
                    iStat = iStat - (info.iStat * 5);
                else
                    iStat = iStat - info.iStat;

                int displayStat = Mathf.Abs(iStat);

                Image img = cc.m_dicStatUI[info.type].transform.Find("RiseFallImage").GetComponent<Image>();

                //��ġ ���� ǥ��
                if (iStat == 0)
                {
                    go.GetComponent<Text>().text = "-";
                    go.transform.Find("Text").GetComponent<Text>().text = "-";
                    img.gameObject.SetActive(false);
                    return;
                }

                go.GetComponent<Text>().text = displayStat.ToString();
                go.transform.Find("Text").GetComponent<Text>().text = displayStat.ToString();

                //��ġ�� �����ϰ��
                if (iStat < 0)
                {
                    img.transform.localRotation = Quaternion.Euler(0, 0, -90);
                    img.color = Color.blue;

                    img.gameObject.SetActive(true);
                    cc.m_dicStatUI[info.type].SetActive(true);
                }

                //��ġ�� ����ϰ��
                else
                {
                    img.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    img.color = Color.red;

                    img.gameObject.SetActive(true);
                    cc.m_dicStatUI[info.type].SetActive(true);
                }
            }
        }
    }

    //�� ������ �ʱ�ȭ
    static void InitRuneStat()
    {
        for (int i = 0; i < cc.m_dicStatUI.Count; i++)
        {
            var text = cc.m_dicStatUI[(STATTYPE)i].transform.Find("Text");

            text.GetComponent<Text>().text = "0";
            text.transform.Find("Text").GetComponent<Text>().text = "0";

            cc.m_dicStatUI[(STATTYPE)i].SetActive(false);
        }
    }

    static public void AddUserStatInfo(ITEMINFO item)
    {
        //ĳ���� ���� �����ϱ�
        foreach(var info in item.arrStats)
        {
            switch(info.type)
            {
                case STATTYPE.PHYSICALPOWER:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.Physical_POW += info.iStat;
                    break;

                case STATTYPE.MAGICPOWER:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.Magic_POW += info.iStat;
                    break;

                case STATTYPE.PHYSICALDEFENSE:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.Physical_DEF += info.iStat;
                    break;

                case STATTYPE.MAGICDEFENSE:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.Magic_DEF += info.iStat;
                    break;

                case STATTYPE.MENT:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.MENT += info.iStat;

                    float rateMp = (float)GameManager.gm.m_inGamePlayer.m_sCurStats.curMp /
                                (float)GameManager.gm.m_inGamePlayer.m_sCurStats.maxMp;

                    int curMp = Mathf.RoundToInt((float)(info.iStat * 5) * rateMp);

                    GameManager.gm.m_inGamePlayer.m_sCurStats.maxMp += (info.iStat * 5);
                    GameManager.gm.m_inGamePlayer.m_sCurStats.curMp += curMp;

                    break;

                case STATTYPE.SPD:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.SPD += info.iStat;
                    break;

                case STATTYPE.HP:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.HP += info.iStat;

                    float rateHp = (float)GameManager.gm.m_inGamePlayer.m_sCurStats.curHp /
                        (float)GameManager.gm.m_inGamePlayer.m_sCurStats.maxHp;

                    int curHp = Mathf.RoundToInt((float)(info.iStat * 5) * rateHp);

                    GameManager.gm.m_inGamePlayer.m_sCurStats.maxHp += (info.iStat * 5);
                    GameManager.gm.m_inGamePlayer.m_sCurStats.curHp += curHp;

                    break;

                case STATTYPE.INSIGHT:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.INSIGHT += info.iStat;
                    break;
            }
        }

        RefreshStatInfo();
    }

    static public void SubtractUserStatInfo(ITEMINFO item)
    {
        //ĳ���� ���� �����ϱ�
        foreach (var info in item.arrStats)
        {
            switch (info.type)
            {
                case STATTYPE.PHYSICALPOWER:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.Physical_POW -= info.iStat;
                    break;

                case STATTYPE.MAGICPOWER:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.Magic_POW -= info.iStat;
                    break;

                case STATTYPE.PHYSICALDEFENSE:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.Physical_DEF -= info.iStat;
                    break;

                case STATTYPE.MAGICDEFENSE:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.Magic_DEF -= info.iStat;
                    break;

                case STATTYPE.MENT:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.MENT -= info.iStat;

                    float rateMp = (float)GameManager.gm.m_inGamePlayer.m_sCurStats.curMp /
                            (float)GameManager.gm.m_inGamePlayer.m_sCurStats.maxMp;

                    int curMp = Mathf.RoundToInt((float)(info.iStat * 5) * rateMp);

                    GameManager.gm.m_inGamePlayer.m_sCurStats.maxMp -= (info.iStat * 5);
                    GameManager.gm.m_inGamePlayer.m_sCurStats.curMp -= curMp;

                    break;

                case STATTYPE.SPD:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.SPD -= info.iStat;
                    break;

                case STATTYPE.HP:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.HP -= info.iStat;

                    float rateHp = (float)GameManager.gm.m_inGamePlayer.m_sCurStats.curHp /
                                    (float)GameManager.gm.m_inGamePlayer.m_sCurStats.maxHp;

                    int curHp = Mathf.RoundToInt((float)(info.iStat * 5) * rateHp);

                    GameManager.gm.m_inGamePlayer.m_sCurStats.maxHp -= (info.iStat * 5);
                    GameManager.gm.m_inGamePlayer.m_sCurStats.curHp -= curHp;

                    break;

                case STATTYPE.INSIGHT:
                    GameManager.gm.m_inGamePlayer.m_sCurStats.INSIGHT -= info.iStat;
                    break;
            }
        }

        RefreshStatInfo();
    }
    #endregion

    static SlotInfo GetSelectedRune()
    {
        SlotInfo slot = null;

        for (int i = 0; i < cc.m_dicRuneSlot.Count; i++)
        {
            if (cc.m_dicRuneSlot[i].IsSelected())
            {
                slot = cc.m_dicRuneSlot[i];
                break;
            }
        }

        return slot;
    }

    static SlotInfo GetSelectedInven()
    {
        SlotInfo slot = null;

        for (int i = 0; i < cc.m_listInvenSlot.Count; i++)
        {
            if (cc.m_listInvenSlot[i].IsSelected())
            {
                slot = cc.m_listInvenSlot[i];
                break;
            }
        }

        return slot;
    }

    static int GetSelectedRuneIndex()
    {
        int index = 0;

        for (; index < cc.m_dicRuneSlot.Count; index++)
        {
            if (cc.m_dicRuneSlot[index].IsSelected()) break;
        }

        return index;
    }

    static int GetSelectedInvenIndex()
    {
        int index = 0;

        for (; index < cc.m_listInvenSlot.Count; index++)
        {
            if (cc.m_listInvenSlot[index].IsSelected()) break;
        }

        return index;
    }

    static public void ResetSelectItemInfo()
    {
        cc.m_txtSelectedItemInfo.gameObject.SetActive(false);
        cc.m_txtSelectedItemName.gameObject.SetActive(false);
        cc.m_txtSelectedRuneName.gameObject.SetActive(false);
        cc.m_txtSelectedRuneInfo.gameObject.SetActive(false);
        
        foreach(var txt in cc.m_arrSelectedRuneStats) txt.gameObject.SetActive(false);

        cc.m_imgSelectedItem.gameObject.SetActive(false);
        cc.m_goEquipButton.SetActive(false);
        cc.m_goDetachButton.SetActive(false);
        cc.m_goGiveButton.SetActive(false);
        cc.m_goUseButton.SetActive(false);
        cc.m_goShiftButton.SetActive(false);
    }
}

