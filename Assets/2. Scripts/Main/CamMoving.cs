using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMoving : MonoBehaviour
{
    [SerializeField] Transform m_trMoveTarget;
    [SerializeField] float m_fWaitTime;
    [SerializeField] float m_fMoveTime;

    public IEnumerator MoveCam()
    {
        yield return new WaitForSeconds(m_fWaitTime);

        Sequence m_seq = DOTween.Sequence()
        .SetAutoKill(true)
        .Append(transform.DOMove(m_trMoveTarget.position, m_fMoveTime, false).SetEase(Ease.InQuad))
        .OnComplete(() =>
        {
            transform.position = m_trMoveTarget.position;
        });
    }
}
