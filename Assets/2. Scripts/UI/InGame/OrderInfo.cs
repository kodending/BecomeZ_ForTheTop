using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

#region Ŭ������ ���̽� ����
[System.Serializable]
public class DicClassFace : SerializableDictionary<CLASSTYPE, GameObject> { }
#endregion

#region ���ʹ� �ε����� ���̽� ����
[System.Serializable]
public class DicEnemyFace : SerializableDictionary<int, GameObject> { }
#endregion

public class OrderInfo : MonoBehaviourPunCallbacks
{
    public bool isTarget;
    public Image img;
    public int orderIdx;
    public PhotonView PV;
    public GameObject faceParent;

    [SerializeField]
    DicEnemyFace m_dicEnemyFace;
    [SerializeField]
    DicClassFace m_dicClassFace;

    [SerializeField] GameObject classParent, enemyParent;

    float m_fDelta = 0;
    float m_fDuration = 1.0f;

    private void Start()
    {
        transform.localScale = Vector3.one;
    }

    private void Update()
    {
        if (GameManager.gm.m_listOrderInfo.Count > 0)
        {
            for (int i = 0; i < GameManager.gm.m_listOrderInfo.Count; i++)
            {
                var info = GameManager.gm.m_listOrderInfo[i];
                if(info == this)
                {
                    orderIdx = i;
                    break;
                }
            }
        }

        if (orderIdx == 0)
        {
            var pos = GameManager.m_curUI.GetComponent<UIManagerInGame>().m_trEndPoint.localPosition;

            if (m_fDelta <= m_fDuration)
            {
                float t = m_fDelta / m_fDuration;
                t = t * t * t * t;
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(pos.x, transform.localPosition.y, 0), t);
                m_fDelta += Time.deltaTime;
            }

            else
            {
                m_fDelta = 0;
            }
        }

        else
        {
            //���� �� ������Ʈ ��ġ�� width / 2 ���ϰ� + �̰ݰŸ� + ���� width / 2 ���ϸ� ���� �̵�����Ʈ�� �� �� ����

            if(GameManager.gm.m_listOrderInfo[orderIdx - 1] == null)
            {
                Debug.Log("Ÿ�Ӷ��� �� ���� : " + orderIdx);
                return;
            }

            var info = GameManager.gm.m_listOrderInfo[orderIdx - 1];
            var movePoint = info.transform.localPosition.x + (info.GetComponent<RectTransform>().rect.width / 2f) + 20 +
                            (this.GetComponent<RectTransform>().rect.width / 2f);

            if (m_fDelta <= m_fDuration)
            {
                float t = m_fDelta / m_fDuration;
                t = t * t * t * t;
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(movePoint, transform.localPosition.y, 0), t);
                m_fDelta += Time.deltaTime;
            }

            else
            {
                m_fDelta = 0;
            }
        }
    }

    public void SetTarget(bool bSizeUp)
    {
        if(bSizeUp)
            StartCoroutine(SetSizeUp());
        else
            StartCoroutine(SetSizeNormalize());
    }

    IEnumerator SetSizeUp()
    {
        float delta = 0;
        float duration = 0.2f;
        var newSize = new Vector2(100f, 100f);
        var newScale = new Vector3(1.5f, 1.5f, 1.5f);
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, -20f, 0);

        while (delta <= duration)
        {
            float t = delta / duration;
            t = t * t * t * t;

            this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(this.GetComponent<RectTransform>().rect.size, newSize, t);

            faceParent.transform.localScale = Vector3.Lerp(faceParent.transform.localScale,
                newScale, t);

            delta += Time.deltaTime;
            yield return null;
        }

        this.GetComponent<RectTransform>().sizeDelta = newSize;
        faceParent.transform.localScale = newScale;
    }

    IEnumerator SetSizeNormalize()
    {
        float delta = 0;
        float duration = 0.2f;
        var newSize = new Vector2(62.5f, 62.5f);
        var newScale = new Vector3(1f, 1f, 1f);
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, 0, 0);

        while (delta <= duration)
        {
            float t = delta / duration;
            t = t * t * t * t;

            this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(this.GetComponent<RectTransform>().rect.size, newSize, t);
            faceParent.transform.localScale = Vector3.Lerp(faceParent.transform.localScale,
    newScale, t);

            delta += Time.deltaTime;
            yield return null;
        }

        this.GetComponent<RectTransform>().sizeDelta = newSize;
        faceParent.transform.localScale = newScale;
    }

    [PunRPC]
    public void InitInfo(bool isUser, int idx)
    {
        transform.localScale = Vector3.one;
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(62.5f, 62.5f);
        faceParent.transform.localScale = new Vector3(1f, 1f, 1f);
        transform.position = GameManager.m_curUI.GetComponent<UIManagerInGame>().m_trStartPoint.position;

        for(int i = 0; i < m_dicClassFace.Count; i++)
        {
            m_dicClassFace[(CLASSTYPE)i].SetActive(false);
        }

        for (int i = 0; i < m_dicEnemyFace.Count; i++)
        {
            m_dicEnemyFace[i + 1].SetActive(false);
        }

        if (isUser)
        {
            var player = BattleManager.m_listBattleTimelineInfo[idx].go.GetComponent<InGamePlayerController>();
            m_dicClassFace[player.m_eCurClass].SetActive(true);
            img.color = player.m_myColor;
        }
        else
        {
            var enemy = BattleManager.m_listBattleTimelineInfo[idx].go.GetComponent<EnemyFSM>();

            m_dicEnemyFace[enemy.m_sCurStats.INDEX].SetActive(true);
            img.color = Color.white;

            if (enemy.m_bSelected) StartCoroutine(SetSizeUp());
            else StartCoroutine(SetSizeNormalize());
        }

        gameObject.SetActive(true);
    }

    //�ٵ� ��ġ�� �������ϴµ� // ��Ʈ �浹�� ����� �浹 �� �� ���� ������ �������� �ֵ��� �����.
    //Distance�� �׻� üũ
}
