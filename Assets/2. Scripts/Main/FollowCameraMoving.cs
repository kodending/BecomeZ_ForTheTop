using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraMoving : MonoBehaviour
{
    [SerializeField] Transform m_trMoveTarget;
    [SerializeField] float m_fWaitTime;
    [SerializeField] float m_fMoveTime;

    private void Start()
    {
        StartCoroutine(MoveCam());
    }

    IEnumerator MoveCam()
    {
        yield return new WaitForSeconds(m_fWaitTime);

        transform.DOMove(m_trMoveTarget.position, m_fMoveTime, false).SetEase(Ease.InQuad);
    }
}
