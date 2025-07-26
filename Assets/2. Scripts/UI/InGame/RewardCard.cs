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

    //맨위에 이미지를 투영하기 위함
    [HideInInspector]
    public Transform parentAfterDrag;

    Vector3 clickDiffPos = new Vector3();

    public ITEMINFO m_item = new ITEMINFO();
    public Image m_imgItem;
    public Text m_txtItemName;
    public Text[] m_arrTxtStats;

    [SerializeField]
    [Range(0f, 1f)]
    float dragThresholdRatio = 0.1f;  // 화면 가로의 10%를 기준으로 설정

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

            //선택이 됐다는 것을 알려줘야됨
            StartCoroutine(PaymentItem());
        }

        transform.SetParent(parentAfterDrag);
        rankImg.raycastTarget = true;
    }

    IEnumerator PaymentItem()
    {
        //아이템을 지급하고
        InvenManager.AddItem(m_item);

        yield return new WaitForSeconds(0.3f);

        //이 창을 먼저 끄고
        GameManager.m_curUI.GetComponent<UIManagerInGame>().m_goRewardPanel.SetActive(false);

        //Check패널창으로 넘어간다.
        GameManager.m_curUI.GetComponent<UIManagerInGame>().OnCheckPanel();
    }

    public void GetRandomItem()
    {
        m_item = ItemManager.RandomGenerateItem(WEIGHTTYPE.SELECT);

        m_imgItem.sprite = m_item.sprite;

        m_txtItemName.text = m_item.strName;
        m_txtItemName.transform.Find("Text").GetComponent<Text>().text = m_item.strName;

        //일단 초기화
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
                        strStat = "물리공격력 +";
                        break;

                    case STATTYPE.MAGICPOWER:
                        strStat = "마법공격력 +";
                        break;

                    case STATTYPE.PHYSICALDEFENSE:
                        strStat = "물리방어력 +";
                        break;

                    case STATTYPE.MAGICDEFENSE:
                        strStat = "마법방어력 +";
                        break;

                    case STATTYPE.MENT:
                        strStat = "정신력 +";
                        break;

                    case STATTYPE.SPD:
                        strStat = "속도 +";
                        break;

                    case STATTYPE.HP:
                        strStat = "체력 +";
                        break;

                    case STATTYPE.INSIGHT:
                        strStat = "통찰력 +";
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
