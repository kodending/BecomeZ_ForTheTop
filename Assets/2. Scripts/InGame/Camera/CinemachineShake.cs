using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviourPunCallbacks
{
    private CinemachineVirtualCamera virCam;
    private float m_fShakeTimer;

    private void Awake()
    {
        virCam = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera(float fIntensity, float fTime)
    {
        CinemachineBasicMultiChannelPerlin perlin = virCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        perlin.m_AmplitudeGain = fIntensity;
        m_fShakeTimer = fTime;
    }

    private void Update()
    {
        if (m_fShakeTimer > 0)
        {
            m_fShakeTimer -= Time.deltaTime;
            if(m_fShakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin perlin = virCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                perlin.m_AmplitudeGain = 0f;
            }
        }
    }
}
