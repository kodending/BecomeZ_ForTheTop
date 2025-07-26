using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public List<BuffInfo> m_listBuffInfo = new();

    public void ApplyBuff(BuffInfo buff)
    {
        var exitingBuff = m_listBuffInfo.Find(b => b.eBuffType == buff.eBuffType);

        if (exitingBuff != null)
        {

            //지속시간이 더 높은 쪽으로 갱신
            exitingBuff.iRemainTurns = Mathf.Max(exitingBuff.iRemainTurns, buff.iRemainTurns);

            if(buff.value > exitingBuff.value)
            {
                exitingBuff.value = buff.value;
            }
        }

        else
        {
            m_listBuffInfo.Add(buff);
        }
    }

    public void TickTurn()
    {
        for (int i = 0; i < m_listBuffInfo.Count; i++)
        {
            m_listBuffInfo[i].iRemainTurns--;
            if (m_listBuffInfo[i].iRemainTurns <= 0)
            {
                m_listBuffInfo.RemoveAt(i);
            }
        }
    }

    public float GetBufferdStatus(STATTYPE eType, float fStat)
    {
        float buffMultiplier = 1f;
        float debuffMultiplier = 1f;

        switch (eType)
        {
            case STATTYPE.PHYSICALPOWER:

                if(m_listBuffInfo.Exists(buff => buff.eBuffType == BUFFTYPE.PHYSICAL_ATTACK_BUFF))
                {
                    var info = m_listBuffInfo.Find(buff => buff.eBuffType == BUFFTYPE.PHYSICAL_ATTACK_BUFF);
                    buffMultiplier += info.value;
                }

                if (m_listBuffInfo.Exists(buff => buff.eBuffType == BUFFTYPE.PHYSICAL_ATTACK_DEBUFF))
                {
                    var info = m_listBuffInfo.Find(buff => buff.eBuffType == BUFFTYPE.PHYSICAL_ATTACK_DEBUFF);
                    debuffMultiplier -= info.value;
                }

                break;

            case STATTYPE.MAGICPOWER:

                if (m_listBuffInfo.Exists(buff => buff.eBuffType == BUFFTYPE.MAGIC_ATTACK_BUFF))
                {
                    var info = m_listBuffInfo.Find(buff => buff.eBuffType == BUFFTYPE.MAGIC_ATTACK_BUFF);
                    buffMultiplier += info.value;
                }

                if (m_listBuffInfo.Exists(buff => buff.eBuffType == BUFFTYPE.MAGIC_ATTACK_DEBUFF))
                {
                    var info = m_listBuffInfo.Find(buff => buff.eBuffType == BUFFTYPE.MAGIC_ATTACK_DEBUFF);
                    debuffMultiplier -= info.value;
                }

                break;

            case STATTYPE.PHYSICALDEFENSE:

                if (m_listBuffInfo.Exists(buff => buff.eBuffType == BUFFTYPE.PHYSICAL_DEFENSE_BUFF))
                {
                    var info = m_listBuffInfo.Find(buff => buff.eBuffType == BUFFTYPE.PHYSICAL_DEFENSE_BUFF);
                    buffMultiplier += info.value;
                }

                if (m_listBuffInfo.Exists(buff => buff.eBuffType == BUFFTYPE.PHYSICAL_DEFENSE_DEBUFF))
                {
                    var info = m_listBuffInfo.Find(buff => buff.eBuffType == BUFFTYPE.PHYSICAL_DEFENSE_DEBUFF);
                    debuffMultiplier -= info.value;
                }

                break;

            case STATTYPE.MAGICDEFENSE:

                if (m_listBuffInfo.Exists(buff => buff.eBuffType == BUFFTYPE.MAGIC_DEFENSE_BUFF))
                {
                    var info = m_listBuffInfo.Find(buff => buff.eBuffType == BUFFTYPE.MAGIC_DEFENSE_BUFF);
                    buffMultiplier += info.value;
                }

                if (m_listBuffInfo.Exists(buff => buff.eBuffType == BUFFTYPE.MAGIC_DEFENSE_DEBUFF))
                {
                    var info = m_listBuffInfo.Find(buff => buff.eBuffType == BUFFTYPE.MAGIC_DEFENSE_DEBUFF);
                    debuffMultiplier -= info.value;
                }

                break;

            case STATTYPE.SPD:

                if (m_listBuffInfo.Exists(buff => buff.eBuffType == BUFFTYPE.SPEED_BUFF))
                {
                    var info = m_listBuffInfo.Find(buff => buff.eBuffType == BUFFTYPE.SPEED_BUFF);
                    buffMultiplier += info.value;
                }

                if (m_listBuffInfo.Exists(buff => buff.eBuffType == BUFFTYPE.SPEED_DEBUFF))
                {
                    var info = m_listBuffInfo.Find(buff => buff.eBuffType == BUFFTYPE.SPEED_DEBUFF);
                    debuffMultiplier -= info.value;
                }

                break;
        }

        float finalStat = fStat * buffMultiplier * debuffMultiplier;

        return Mathf.Max(0f, finalStat);
    }
}
