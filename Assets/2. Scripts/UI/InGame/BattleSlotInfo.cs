using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSlotInfo : MonoBehaviour
{
    [SerializeField] GameObject m_goSelected, m_goStackBack;
    public ITEMINFO m_item = new ITEMINFO();
    public Image m_imgitem;
    public Text m_txtStack;
    bool isSelected = false;

    public void OnClickSelectSlot()
    {
        GameManager.m_curUI.GetComponent<UIManagerInGame>().DeselectInvenSlots();

        isSelected = true;

        UIManager.SelectSlot(m_goSelected);

        GameManager.m_curUI.GetComponent<UIManagerInGame>().ShowItemInfo(m_item);

        bool isActive = m_item.idx == 0 ? false : true;

        if (m_item.itemTypeIdx != (int)ITEMTYPE.CONSUME) isActive = false;
        
        GameManager.m_curUI.GetComponent<UIManagerInGame>().ActvieBattleItemButton(isActive);
    }

    public void DeselectSlot()
    {
        isSelected = false;
        m_goSelected.SetActive(false);
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void ResetItem()
    {
        m_item = new ITEMINFO();
        m_imgitem.gameObject.SetActive(false);
        m_goStackBack.gameObject.SetActive(false);
    }

    public void SetItem(ITEMINFO item)
    {
        m_item = item;

        m_imgitem.sprite = m_item.sprite;
        m_imgitem.gameObject.SetActive(true);

        if (item.curStack > 1)
        {
            m_goStackBack.SetActive(true);
            m_txtStack.text = item.curStack.ToString();
        }

        else
        {
            m_goStackBack.SetActive(false);
        }
    }
}
