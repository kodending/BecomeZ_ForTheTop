using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        //스킬이 들어가 있고 내 착용스킬 정보에 넣기
        SKILLINFO skill = new SKILLINFO();

        foreach(var skillInfo in GoogleSheetManager.m_skillInfo)
        {
            int skillIdx = int.Parse(skillInfo["INDEX"].ToString());
            var classes = GoogleSheetManager.ParseAvailableClassType(skillInfo["CLASSTYPE"].ToString());
            var engName = skillInfo["ENGNAME"].ToString();

            if (skillIdx == im.m_dicEquipRune[equipIdx].skillIdx &&
                engName == im.m_dicEquipRune[equipIdx].skillEngName)
            {
                skill.idx = skillIdx;
                skill.name = skillInfo["NAME"].ToString();
                skill.engName = skillInfo["ENGNAME"].ToString();
                skill.listAvailClasses = classes;
                skill.iAttackType = int.Parse(skillInfo["ATTACKTYPE"].ToString());
                skill.iSkillType = int.Parse(skillInfo["SKILLTYPE"].ToString());
                skill.fAttackPow = float.Parse(skillInfo["ATTACK"].ToString());
                skill.fAttackProb = float.Parse(skillInfo["ATTACKPROB"].ToString());
                skill.iConsumeMp = int.Parse(skillInfo["CONSUMEMP"].ToString());
                skill.fHitTime = float.Parse(skillInfo["HITTIME"].ToString());
                skill.iProbCnt = int.Parse(skillInfo["PROBCOUNT"].ToString());
                skill.iMaxCoolTime = int.Parse(skillInfo["COOLTIME"].ToString());
                skill.iCurCoolTime = int.Parse(skillInfo["COOLTIME"].ToString());
                skill.iAtkEffectIdx = int.Parse(skillInfo["ATTACKEFFECT"].ToString());
                skill.iHitEffectIdx = int.Parse(skillInfo["HITEFFECT"].ToString());
                skill.iAttackPosIdx = int.Parse(skillInfo["ATTACKPOS"].ToString());
                skill.bHoming = int.Parse(skillInfo["HOMING"].ToString()) != 0;

                GameManager.gm.m_inGamePlayer.m_sCurStats.listSkills.Add(skill);
                break;
            }
        }

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
        //착용한 룬에 들어간 스킬 빼기
        var listSkill = GameManager.gm.m_inGamePlayer.m_sCurStats.listSkills;

        for (int i = listSkill.Count - 1; i >= 0; --i)
        {
            if(listSkill[i].idx == im.m_dicEquipRune[listIdx].skillIdx)
            {
                listSkill.RemoveAt(i);
                break;
            }
        }

        im.m_dicEquipRune[listIdx] = new ITEMINFO();

        AddItem(item);

        CheckControl.SubtractUserStatInfo(item);
        CheckControl.RefreshInventory();
        CheckControl.RefreshEquipment();
    }

    static public void ShiftRune(ITEMINFO invenItem, int equipIdx, int invenIdx)
    {
        //먼저 착용중인 아이템 정보를 가져오고
        ITEMINFO shiftItem = im.m_dicEquipRune[equipIdx];

        //새로운 아이템 장착하고
        im.m_dicEquipRune[equipIdx] = invenItem;

        //인벤에서 템 정리하고
        RemoveItem(CheckControl.cc.m_curPage, invenIdx, invenItem);

        //착용중이었던 아이템을 인벤에 넣어준다.
        AddItem(shiftItem);

        //그다음 인벤은 최신화
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

    static public void ClearInven()
    {
        im.m_dicEquipRune.Clear();
        im.m_dicInvenInfoInPage.Clear();

        if (im.m_dicInvenInfoInPage.Count == 0)
        {
            List<ITEMINFO> listItem = new List<ITEMINFO>();
            im.m_dicInvenInfoInPage.Add(1, listItem);
        }

        for (int i = 0; i < 4; i++)
        {
            ITEMINFO item = new ITEMINFO();
            im.m_dicEquipRune.Add(i, item);
        }
    }
}
