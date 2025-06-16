using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWood : MonoBehaviour
{
    public Light m_light;

    float m_fTimer;

    float m_fInitRange;
    float m_fRandMaxTimer;

    private void Start()
    {
        m_fInitRange = m_light.range;
        m_fRandMaxTimer = 0.5f;
    }

    private void Update()
    {
        m_fTimer += Time.deltaTime;

        if(m_fTimer >= m_fRandMaxTimer)
        {
            m_fTimer = 0f;
            m_fRandMaxTimer = Random.Range(0.3f, 1f);

            float fRand = Random.Range(-0.2f, 0.2f);

            m_light.range = m_fInitRange + fRand;
        }
    }
}
