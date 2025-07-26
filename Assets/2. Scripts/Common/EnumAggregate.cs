using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enum문 정의하는 곳

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

public enum GOOGLEDATALOADTYPE
{
    CLASSINFO = 0,
    CONSUMEINFO = 862397853,
    ENEMYINFO = 1838755227,
    RANKSTATINFO = 1200005675,
    RUNEINFO = 1444824895,
    SKILLINFO = 733882408,
    LEVELDESIGN = 1026627123,
    ENEMYATTACKINFO = 1078051955,
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
    APEAR, //등장
    SELECT, //공격방어 등등 선택
    BATTLE, //전투
    REWARDCHECK, //보상 및 점검 단계
    NEXTFLOOR, //다음층이동
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
    KNIGHT, //나이트
    DARKKNIGHT, //다크나이트
    RANGER, //궁수
    _MAX_
}

public enum PLAYERSTATE
{
    IDLE,
    MOVE,
    MELEE_ATTACk,
    ATTACK_MOVE,
    BACK_MOVE,
    HIT_MOVE,
    HIT,
    DEATH,
    RETURN_MOVE,
    DODGE_MOVE,
    DODGE,
    BLOCK,
    RANGED_ATTACk,
    _MAX_
}

public enum ENEMYSTATE
{
    IDLE,
    MOVE,
    MELEE_ATTACK,
    ATTACK_MOVE,
    BACK_MOVE,
    HIT_MOVE,
    HIT,
    DEATH,
    DODGE_MOVE,
    DODGE,
    BLOCK,
    RANGED_ATTACK,
    _MAX_
}

public enum ATTACKRESULT
{
    PHYSICAL_ATTACK = 1,
    MAGIC_ATTACK,
    PHYSICAL_PIERCING_ATTACK,
    MAGIC_PIERCING_ATTACK,
    PHYSICAL_AREA_ATTACK,
    MAGIC_AREA_ATTACK,
    SHIELD_SOLO_STRIKE,
    SHIELD_AREA_STRIKE,
    PHYSICAL_DEF_DEBUFF_SOLO,
    MAGIC_DEF_DEBUFF_SOLO,
    PHYSICAL_ATK_DEBUFF_SOLO,
    MAGIC_ATK_DEBUFF_SOLO,
    MISS = 99,
    BLOCK = 999,
    _MAX_
}


public enum BUFFTYPE
{
    PHYSICAL_ATTACK_BUFF,
    MAGIC_ATTACK_BUFF,
    PHYSICAL_DEFENSE_BUFF,
    MAGIC_DEFENSE_BUFF,
    SPEED_BUFF,
    PHYSICAL_ATTACK_DEBUFF,
    MAGIC_ATTACK_DEBUFF,
    PHYSICAL_DEFENSE_DEBUFF,
    MAGIC_DEFENSE_DEBUFF,
    SPEED_DEBUFF
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
    PHYSICAL_SINGLE = 1, //물리단일
    MAGIC_SINGLE,       //마법단일
    COMPLEX_SINGLE,     //물리마법 복합 단일
}

public enum SKILLTYPE
{
    MELEE_ATTACk = 1,  //근거리 공격
    RANGED_ATTACK,      //원거리 공격
}


public enum SFX //효과음
{
    ATTACK,
    _MAX_
}

public enum BGM //배경음
{
    MAIN,
    LOBBY,
    WAITING,
    BATTLE_NORMAL,
    _MAX_
}

public enum EFFECTTYPE
{
    BLOOD_DROP = 1,
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
    HIT_02,
    HIT_03,
    HIT_04,
    HIT_05,
    HIT_06,
    HIT_07,
    HIT_08,
    HIT_09,
    HIT_10,
    HIT_11,
    HIT_12,
    HIT_13,
    HIT_14,
    HIT_15,
    HIT_16,
    HIT_17,
    HIT_18,
    HIT_19,
    HIT_20,
    HIT_21,
    MAGIC_01,
    MAGIC_02,
    MAGIC_BATS,
    SMOKE_01,
    MISSILES_EX,
    REDCUBE,
    SKILL_GREEN,
    SKILL_01,
    SPLASH_02,
    STARS_EX,
    GLOW_01,
    BLACKCUBE,
    DARK_EX_01,
    DARK_EX_02,
    DARK_EX_03,
    EXPLOSION_03,
    SPLASH_CUBE,
    GHOST_ATK,
    GREEN_CUBE,
    BIG_RED_CUB,
    BLOOD_EXPLOSION,
    _MAX_
}

public enum CONSUMETYPE
{
    HP_POTION = 1,
    MP_POTION,
    RANDOM_STAT_POT,
    _MAX_
}

public enum PLAYERSKILLINDEX //플레이어 공격 인덱스 설명 (공격모션 정리를위함)
{ 
    DEFAULT_ATTACK,  //캐릭터 별 기본 공격(근접직업)
    SHIELD_STRIKE,   //방패치기
}

public enum CONVERT_STATS //체력, 마나, 공격 환산스텟
{
    DEF = 3,
    ATK = 5,
    HP_MP = 10,
}










