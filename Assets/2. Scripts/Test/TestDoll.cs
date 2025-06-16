using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class TestDoll : MonoBehaviour
{
    [SerializeField] GameObject charObj;
    [SerializeField] GameObject ragdollObj;
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ChangeRagdoll());
        }
    }

    IEnumerator ChangeRagdoll()
    {
        CopyCharTransform(charObj.transform, ragdollObj.transform);

        anim.StartPlayback();

        charObj.SetActive(false);
        ragdollObj.SetActive(true);

        rb.AddForce(new Vector3(0, 150f, 0f), ForceMode.Impulse);

        yield return new WaitForSeconds(2.5f);

        Rigidbody[] arrRb = ragdollObj.GetComponentsInChildren<Rigidbody>();

        foreach(var rigid in arrRb)
        {
            rigid.useGravity = false;
            rigid.isKinematic = true;
        }
    }


    void CopyCharTransform(Transform beforeTr, Transform afterTr)
    {
        for (int i = 0; i < beforeTr.childCount; i++)
        {
            if(beforeTr.childCount != 0)
            {
                CopyCharTransform(beforeTr.GetChild(i), afterTr.GetChild(i));
            }

            afterTr.GetChild(i).localPosition = beforeTr.GetChild(i).localPosition;
            afterTr.GetChild(i).localRotation = beforeTr.GetChild(i).localRotation;
        }
    }
}
