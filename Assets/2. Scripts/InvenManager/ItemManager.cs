using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    static public ItemManager im;

    [SerializeField]
    List<Sprite> m_listConsumeSprite, m_listRuneSprite;

    private void Awake()
    {
        im = this;
        DontDestroyOnLoad(im);
    }

    //아이템 로드한거 정리하고 관련된 이미지 정리하기
    //아이템 생성할때 정보 주입하는 곳도 여기서 주입하기

    static public ITEMINFO GenerateItem(int itemTypeIdx, int itemIdx, int typeIdx, int rankIdx, int skillIdx)
    {
        ITEMINFO item = new ITEMINFO();

        return item;
    }

    static public ITEMINFO RandomGenerateItem(WEIGHTTYPE eWeightType, int curFloor = 1)
    {
        ITEMINFO item = new ITEMINFO();

        item.itemTypeIdx = Random.Range((int)ITEMTYPE.RUNE, (int)ITEMTYPE._MAX_);

        switch ((ITEMTYPE)item.itemTypeIdx)
        {
            case ITEMTYPE.RUNE:

                Dictionary<string, object> rune = RandomWeight.RandomItem(GoogleSheetManager.m_runeInfo, eWeightType);
                item.idx = int.Parse(rune["INDEX"].ToString());
                item.iType = int.Parse(rune["TYPE"].ToString());
                item.iRank = int.Parse(rune["RANK"].ToString());
                item.sprite = im.m_listRuneSprite[item.idx];
                item.maxStack = 1;
                item.curStack = 1;

                Dictionary<string, object> skill = RandomWeight.RandomItem(GoogleSheetManager.m_skillInfo, eWeightType);
                item.skillIdx = int.Parse(skill["INDEX"].ToString());
                item.strExplain = skill["EXPLAIN"].ToString();
                item.strName = skill["NAME"].ToString();
                item.skillEngName = skill["ENGNAME"].ToString();
                item.fPoint = float.Parse(skill["ATTACK"].ToString());

                //랜덤스탯 부여
                item.arrStats = GetStats(item.iRank);

                break;
            case ITEMTYPE.CONSUME:

                Dictionary<string, object> consume = RandomWeight.RandomItem(GoogleSheetManager.m_consumeInfo, eWeightType);
                item.idx = int.Parse(consume["INDEX"].ToString());
                item.iType = int.Parse(consume["TYPE"].ToString());
                item.iRank = int.Parse(consume["RANK"].ToString());
                item.sprite = im.m_listConsumeSprite[item.idx];
                item.maxStack = int.Parse(consume["MAXSTACK"].ToString());
                item.curStack = 1;
                item.skillIdx = 0;
                item.strExplain = consume["EXPLAIN"].ToString();
                item.strName = consume["NAME"].ToString();
                item.fPoint = float.Parse(consume["POINT"].ToString());
                item.arrStats = null;

                break;
        }


        return item;
    }


    static public RUNESTAT[] GetStats(int iRank)
    {
        RUNESTAT[] arr = new RUNESTAT[iRank];

        Dictionary<string, object> info = GoogleSheetManager.m_RankStatInfo[iRank - 1];

        for(int i = 0; i < iRank; i++)
        {
            RUNESTAT runeStat = new RUNESTAT();

            int min = int.Parse(info["MINSTAT"].ToString());
            int max = int.Parse(info["MAXSTAT"].ToString()) + 1;

            if (iRank >= 3)
                runeStat.type = (STATTYPE)Random.Range((int)STATTYPE.PHYSICALPOWER, (int)STATTYPE._MAX_);
            else
                runeStat.type = (STATTYPE)Random.Range((int)STATTYPE.PHYSICALPOWER, (int)STATTYPE.INSIGHT);

            if (runeStat.type == STATTYPE.INSIGHT)
                runeStat.iStat = 1;
            else
                runeStat.iStat = Random.Range(min, max);

            arr[i] = runeStat;
        }

        return arr;
    }

    static public Sprite GetSprite(int itemType, int idx)
    {
        Sprite sprite = null;

        switch ((ITEMTYPE)itemType)
        {
            case ITEMTYPE.RUNE:
                sprite = im.m_listRuneSprite[idx];
                break;

            case ITEMTYPE.CONSUME:
                sprite = im.m_listConsumeSprite[idx];
                break;
        }

        return sprite;
    }
}
