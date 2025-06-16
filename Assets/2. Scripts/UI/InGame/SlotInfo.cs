using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotInfo : MonoBehaviour
{
    [SerializeField] GameObject m_goSelected, m_goStackBack;
    public ITEMINFO m_item = new ITEMINFO();
    public Image m_imgitem;
    public Text m_txtStack;
    bool isSelected = false;

    public void OnClickSelectSlot()
    {
        //인벤 슬롯인지 확인
        bool isInven = CheckControl.GetIsInvenItem(this);

        if(isInven)
        {
            //인벤 슬롯만 셀렉을 초기화 한다.
            CheckControl.DeselectInvenSlot();
            CheckControl.ShowInvenItemInfo(this);
        }
        else
        {
            //룬 슬롯만 셀렉을 초기화 한다.
            CheckControl.DeselectEquipSlot();
            CheckControl.ShowEquipRuneInfo(this);
        }

        isSelected = true;
        UIManager.SelectSlot(m_goSelected);
        CheckControl.StatCompareInfo();
        CheckControl.RefreshActiveButton();
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

        if(item.curStack > 1)
        {
            m_goStackBack.SetActive(true);
            m_txtStack.text = item.curStack.ToString();
        }

        else
        {
            m_goStackBack.SetActive(false);
        }
    }

    public void SetRune(ITEMINFO item)
    {
        SetItem(item);

        m_imgitem.sprite = m_item.sprite;

        UIManager.EquipRune(m_imgitem.gameObject);
        OnClickSelectSlot();

        CheckControl.AddUserStatInfo(item);
    }
}
