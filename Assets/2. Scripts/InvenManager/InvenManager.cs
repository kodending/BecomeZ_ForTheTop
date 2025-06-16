using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class InvenManager : MonoBehaviour
{
    public static InvenManager im;

    Dictionary<int, List<ITEMINFO>> m_dicInvenInfoInPage = new Dictionary<int, List<ITEMINFO>>();
    Dictionary<int, ITEMINFO> m_dicEquipRune = new Dictionary<int, ITEMINFO>();

    private void Awake()
    {
        im = this;
        DontDestroyOnLoad(im);
    }

    private void Start()
    {
        if (im.m_dicInvenInfoInPage.Count == 0)
        {
            List<ITEMINFO> listItem = new List<ITEMINFO>();
            im.m_dicInvenInfoInPage.Add(1, listItem);
        }

        for(int i = 0; i < 4; i++)
        {
            ITEMINFO item = new ITEMINFO();
            m_dicEquipRune.Add(i, item);
        }
    }

    static public void AddItem(ITEMINFO item)
    {
        for (int i = 0; i < im.m_dicInvenInfoInPage.Count; i++)
        {
            var curPage = im.m_dicInvenInfoInPage[i + 1];

            bool isStack = false;

            for(int j = 0; j < curPage.Count; j++)
            {
                var info = curPage[j];

                if(info.idx == item.idx && info.itemTypeIdx == item.itemTypeIdx &&
                   info.curStack < info.maxStack)
                {
                    info.curStack++;
                    curPage[j] = info;
                    isStack = true;
                    break;
                }
            }

            if (isStack) return;

            if (curPage.Count >= 9 && i + 1 == im.m_dicInvenInfoInPage.Count)
            {
                List<ITEMINFO> listItem = new List<ITEMINFO>();
                im.m_dicInvenInfoInPage.Add(im.m_dicInvenInfoInPage.Count + 1, listItem);
                continue;
            }

            else if (curPage.Count >= 9)
            {
                continue;
            }

            else
            {
                curPage.Add(item);
            }
        }
    }

    static public void EquipRune(int invenIdx, int equipIdx, ITEMINFO item)
    {
        im.m_dicEquipRune[equipIdx] = item;

        RemoveItem(CheckControl.cc.m_curPage, invenIdx, item);

        CheckControl.RefreshInventory();
    }

    static public void RemoveItem(int page, int listIdx, ITEMINFO item)
    {
        var curInven = im.m_dicInvenInfoInPage[page];

        if (item.curStack > 1)
        {
            item.curStack--;
            curInven[listIdx] = item;
            return;
        }

        curInven.RemoveAt(listIdx);
    }

    static public void DetachRune(ITEMINFO item, int listIdx)
    {
        im.m_dicEquipRune[listIdx] = new ITEMINFO();

        AddItem(item);

        CheckControl.SubtractUserStatInfo(item);
        CheckControl.RefreshInventory();
        CheckControl.RefreshEquipment();
    }

    static public void ShiftRune(ITEMINFO invenItem, int equipIdx, int invenIdx)
    {
        //���� �������� ������ ������ ��������
        ITEMINFO shiftItem = im.m_dicEquipRune[equipIdx];

        //���ο� ������ �����ϰ�
        im.m_dicEquipRune[equipIdx] = invenItem;

        //�κ����� �� �����ϰ�
        RemoveItem(CheckControl.cc.m_curPage, invenIdx, invenItem);

        //�������̾��� �������� �κ��� �־��ش�.
        AddItem(shiftItem);

        //�״��� �κ��� �ֽ�ȭ
        CheckControl.SubtractUserStatInfo(shiftItem);
        CheckControl.RefreshInventory();
    }

    static public List<ITEMINFO> GetCurPageItem(int page)
    {
        return im.m_dicInvenInfoInPage[page];
    }

    static public int GetMaxPage()
    {
        return im.m_dicInvenInfoInPage.Count;
    }

    static public Dictionary<int, ITEMINFO> GetEquipment()
    {
        return im.m_dicEquipRune;
    }
}
