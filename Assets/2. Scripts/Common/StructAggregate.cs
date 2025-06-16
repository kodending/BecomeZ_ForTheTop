using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

//struct 정의하는 곳

public struct CLASSSTATS
{
    public int HP;
    public int Physical_POW;
    public int Magic_POW;
    public int Physical_DEF;
    public int Magic_DEF;
    public int MENT;
    public int SPD;
    public int INSIGHT;
    public int curInsight;
    public string name;
    public Sprite attribute;
    public int maxHp;
    public int curHp;
    public int maxMp;
    public int curMp;
    public CLASSTYPE eClass;
    public List<SKILLINFO> listSkills;
}

public struct ENEMYSTATS
{
    public int INDEX;
    public int HP;
    public int Physical_POW;
    public int Magic_POW;
    public int Physical_DEF;
    public int Magic_DEF;
    public int MENT;
    public float SPD;
    public string name;
    public int maxHp;
    public int curHp;
}

public struct BATTLEINFO
{
    public GameObject go;
    public float SPD;
    public bool bUser;
}

public struct ITEMINFO
{
    public int idx;
    public int itemTypeIdx;     //아이템 종류 : 소모성, 룬 등등 분류
    public int iType;           //아이템안에 어떤 타입인지 분류
    public int iRank;           // 등급
    public int skillIdx;        // 룬아이템에 붙은 스킬인덱스
    public int maxStack;        // 아이템 스택최대갯수
    public int curStack;        // 아이템 현재스택갯수
    public Sprite sprite;       // 아이템 이미지
    public string strExplain;   // 아이템 설명
    public string strName;      // 아이템 이름
    public float fPoint;        // 소모성아이템이나 스킬의 수치 데이터 표시
    public RUNESTAT[] arrStats;      // 룬의 부여된 스텟
}

public struct RUNESTAT
{
    public STATTYPE type;
    public int iStat;
}

public struct SKILLINFO
{
    public int idx;
    public string name;
    public int iClassType;
    public int iAttackType;
    public int iSkillType;
    public float fAttackPow; //공격 계수
    public float fAttackProb; //공격 확률
    public float fHitTime;    //공격 히트 애니메이션 시간
    public int iConsumeMp;      //소모 MP
    public int iProbCnt;
    public int iEffectIdx;
}




