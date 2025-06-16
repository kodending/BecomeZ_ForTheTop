using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCamPos : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float m_fTimer;

    private void Start()
    {
        StartCoroutine(JoinLobby());
    }

    IEnumerator JoinLobby()
    {
        yield return new WaitForSeconds(m_fTimer);

        rb.useGravity = true;
    }
}
