using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CinemachineMoving : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;
    public CinemachineBrain brain;

    public void ChangeCameraTargetSmooth(Transform originFollow, Transform newFollow, Transform originLookAt, Transform newLookAt, float duration, Ease eType)
    {
        originFollow.transform.DOMove(newFollow.transform.position, duration).SetEase(eType);
        originLookAt.transform.DOMove(newLookAt.transform.position, duration).SetEase(eType);
    }
}
