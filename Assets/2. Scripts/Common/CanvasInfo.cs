using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasInfo : MonoBehaviour
{
    [SerializeField] Text m_txtDamage;

    private void FixedUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }

    public void OnDamage(int iAtk, int atkResult)
    {
        switch((ATTACKRESULT)atkResult)
        {
            case ATTACKRESULT.PHYSICAL_ATTACK:
            case ATTACKRESULT.MAGIC_ATTACK:
            case ATTACKRESULT.PHYSICAL_PIERCING_ATTACK:
            case ATTACKRESULT.MAGIC_PIERCING_ATTACK:
            case ATTACKRESULT.PHYSICAL_AREA_ATTACK:
            case ATTACKRESULT.MAGIC_AREA_ATTACK:
                m_txtDamage.text = iAtk.ToString();

                break;

            case ATTACKRESULT.MISS:
                m_txtDamage.text = "회피";
                break;

            case ATTACKRESULT.BLOCK:
                m_txtDamage.text = "방어";
                break;
        }
        UIManager.ShowDamageText(m_txtDamage.gameObject);
    }
}
