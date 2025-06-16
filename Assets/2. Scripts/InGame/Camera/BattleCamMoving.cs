using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamMoving : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform[] m_trTarget;

    public IEnumerator MoveCam(BATTLECAMTYPE eType, float fWait, float fMove)
    {
        yield return new WaitForSeconds(fWait);

        transform.DOMove(m_trTarget[(int)eType].position, fMove, false).SetEase(Ease.InQuad);
    }
}
