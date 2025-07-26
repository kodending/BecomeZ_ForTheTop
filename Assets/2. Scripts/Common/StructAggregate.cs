using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

//struct 정의하는 곳

[System.Serializable]
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
    public float fSize;
    public List<SKILLINFO> listSkills;
}

public struct BATTLEINFO
{
    public GameObject go;
    public float SPD;
    public bool bUser;
    public int iRegistrationOrder;
}

public struct ITEMINFO
{
    public int idx;
    public int itemTypeIdx;     //아이템 종류 : 소모성, 룬 등등 분류
    public int iType;           //아이템안에 어떤 타입인지 분류,
    public int iRank;           // 등급
    public int skillIdx;        // 룬아이템에 붙은 스킬인덱스
    public string skillEngName;        // 룬아이템에 붙은 스킬이름
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

[System.Serializable]
public struct SKILLINFO
{
    public int idx;
    public string name;
    public string engName;  //영어 이름
    public List<string> listAvailClasses; // 사용가능한 클래스 정의
    public int iAttackType;
    public int iSkillType;
    public float fAttackPow; //공격 계수
    public float fAttackProb; //공격 확률
    public float fHitTime;    //공격 히트 애니메이션 시간
    public int iConsumeMp;      //소모 MP
    public int iProbCnt;
    public int iAtkEffectIdx; //공격자 effect
    public int iHitEffectIdx; //맞은놈 effect
    public bool bHoming;        //추격하는 이펙트인지
    public int iAttackPosIdx;        //이펙트 위치
    public float fSelectWeight;  //스킬 선택 확률[에너미용] //룬에서 스킬 나올 확률[플레이어용]
    public int iCurCoolTime; //현재 스킬의 쿨타임
    public int iMaxCoolTime; //스킬의 원래쿨타임
}




