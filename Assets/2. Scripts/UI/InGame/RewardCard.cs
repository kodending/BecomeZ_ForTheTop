using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class RewardCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image rankImg;
    [SerializeField]
    Sprite[] rankSprites;

    //������ �̹����� �����ϱ� ����
    [HideInInspector]
    public Transform parentAfterDrag;

    Vector3 clickDiffPos = new Vector3();

    public ITEMINFO m_item = new ITEMINFO();
    public Image m_imgItem;
    public Text m_txtItemName;
    public Text[] m_arrTxtStats;

    [SerializeField]
    [Range(0f, 1f)]
    float dragThresholdRatio = 0.1f;  // ȭ�� ������ 10%�� �������� ����

    public void OnBeginDrag(PointerEventData eventData)
    {
        clickDiffPos = Input.mousePosition - transform.position;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        rankImg.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition - clickDiffPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float distance = Vector2.Distance(parentAfterDrag.position, this.transform.position);

        float threshold = Screen.width * dragThresholdRatio;

        if(distance >= threshold)
        {
            UIManager.CardSelectEffect(this.gameObject);

            //������ �ƴٴ� ���� �˷���ߵ�
            StartCoroutine(PaymentItem());
        }

        transform.SetParent(parentAfterDrag);
        rankImg.raycastTarget = true;
    }

    IEnumerator PaymentItem()
    {
        //�������� �����ϰ�
        InvenManager.AddItem(m_item);

        yield return new WaitForSeconds(0.3f);

        //�� â�� ���� ����
        GameManager.m_curUI.GetComponent<UIManagerInGame>().m_goRewardPanel.SetActive(false);

        //Check�г�â���� �Ѿ��.
        GameManager.m_curUI.GetComponent<UIManagerInGame>().OnCheckPanel();
    }

    public void GetRandomItem()
    {
        m_item = ItemManager.RandomGenerateItem(WEIGHTTYPE.SELECT);

        m_imgItem.sprite = m_item.sprite;

        m_txtItemName.text = m_item.strName;
        m_txtItemName.transform.Find("Text").GetComponent<Text>().text = m_item.strName;

        //�ϴ� �ʱ�ȭ
        foreach (var txt in m_arrTxtStats)
        {
            txt.gameObject.SetActive(false);
        }

        if (m_item.itemTypeIdx == (int)ITEMTYPE.CONSUME)
        {
            rankImg.sprite = rankSprites[0];

            m_arrTxtStats[1].text = m_item.strExplain;
            m_arrTxtStats[1].transform.Find("Text").GetComponent<Text>().text = m_item.strExplain;

            m_arrTxtStats[1].gameObject.SetActive(true);
        }

        else
        {
            rankImg.sprite = rankSprites[m_item.iRank];
        }

        if (m_item.itemTypeIdx != (int)ITEMTYPE.RUNE) return;

        for (int i = 0; i < m_arrTxtStats.Length; i++) 
        {
            var txt = m_arrTxtStats[i];

            if(i < m_item.arrStats.Length)
            {
                var stat = m_item.arrStats[i];
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
                txt.text = strStat;
                txt.transform.Find("Text").GetComponent<Text>().text = strStat;

                txt.gameObject.SetActive(true);
            }

            else
            {
                txt.gameObject.SetActive(false);
            }
        }
    }
}
