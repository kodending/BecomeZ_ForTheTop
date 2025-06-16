using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enum�� �����ϴ� ��

public enum LOCALDATALOADTYPE
{
    CLASSINFO,
    ENEMYINFO,
    RUNEINFO,
    CONSUMEINFO,
    SKILLINFO,
    RANKSTATINFO,
    _MAX_
}


public enum GAMESTATE
{
    LOAD,
    MAIN,
    LOBBY,
    WAITROOM,
    INGAME,
    _MAX_
}

public enum BATTLESTATE
{
    APEAR, //����
    SELECT, //���ݹ�� ��� ����
    BATTLE, //����
    REWARDCHECK, //���� �� ���� �ܰ�
    NEXTFLOOR, //�������̵�
    _MAX_
}

public enum BATTLECAMTYPE
{
    APEAR,
    PLAYERATTACK,
    ENEMYATTACK,
    NEXTFLOOR,
    _MAX_
}


public enum POOLTYPE
{
    LOBBYPLAYER,
    INGAMEPLAYER,
    INGAMEORDER,
    ENEMIES,
    _MAX_
}

public enum CLASSTYPE
{
    KNIGHT, //����Ʈ
    DARKKNIGHT, //��ũ����Ʈ
    _MAX_
}

public enum PLAYERSTATE
{
    IDLE,
    MOVE,
    ATTACK,
    ATTACK_MOVE,
    BACK_MOVE,
    HIT,
    DEATH,
    RETURN_MOVE,
    DODGE,
    BLOCK,
    _MAX_
}

public enum ENEMYSTATE
{
    IDLE,
    MOVE,
    ATTACK,
    ATTACK_MOVE,
    BACK_MOVE,
    HIT,
    DEATH,
    DODGE,
    BLOCK,
    _MAX_
}

public enum ATTACKRESULT
{
    PHYSICAL_ATTACK,
    MAGIC_ATTACK,
    MISS,
    BLOCK,
    _MAX_
}

public enum UIDIRECTION
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    _MAX_
}

public enum ITEMTYPE
{
    RUNE = 1,
    CONSUME,
    _MAX_
}

public enum WEIGHTTYPE
{
    BOX,
    SELECT,
    _MAX_
}

public enum STATTYPE
{
    PHYSICALPOWER,
    MAGICPOWER,
    PHYSICALDEFENSE,
    MAGICDEFENSE,
    MENT,
    SPD,
    HP,
    INSIGHT,
    _MAX_
}

public enum ATTACKTYPE
{
    PHYSICAL_SINGLE = 1, //��������
    MAGIC_SINGLE,       //��������
    COMPLEX_SINGLE,     //�������� ���� ����
}

public enum SFX //ȿ����
{
    ATTACK = 1,
    _MAX_
}

public enum BGM //�����
{
    MAIN = 1,
    _MAX_
}

public enum EFFECTTYPE
{
    BLOOD_DROP,
    BLOOD_EXPLISION,
    COLD_AIR,
    DARK_EXPLOSION,
    ELECTRON_EXPLOSION,
    ELECTRON_VOLUME,
    FIRE_SPARK,
    HIT_BLOOD,
    POISON,
    SHINING,
    SOS_FLAG,
    STORM,
    TAKE_DOWN,
    _MAX_
}









