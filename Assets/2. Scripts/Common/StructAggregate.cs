using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

//struct �����ϴ� ��

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
    public int itemTypeIdx;     //������ ���� : �Ҹ�, �� ��� �з�
    public int iType;           //�����۾ȿ� � Ÿ������ �з�
    public int iRank;           // ���
    public int skillIdx;        // ������ۿ� ���� ��ų�ε���
    public int maxStack;        // ������ �����ִ밹��
    public int curStack;        // ������ ���罺�ð���
    public Sprite sprite;       // ������ �̹���
    public string strExplain;   // ������ ����
    public string strName;      // ������ �̸�
    public float fPoint;        // �Ҹ𼺾������̳� ��ų�� ��ġ ������ ǥ��
    public RUNESTAT[] arrStats;      // ���� �ο��� ����
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
    public float fAttackPow; //���� ���
    public float fAttackProb; //���� Ȯ��
    public float fHitTime;    //���� ��Ʈ �ִϸ��̼� �ð�
    public int iConsumeMp;      //�Ҹ� MP
    public int iProbCnt;
    public int iEffectIdx;
}




