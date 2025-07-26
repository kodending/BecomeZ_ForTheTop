using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunPoolManager : MonoBehaviourPunCallbacks, IPunPrefabPool
{
    static public PunPoolManager ppm;

    Dictionary<POOLTYPE, List<GameObject>> m_dicPrefabs = new Dictionary<POOLTYPE, List<GameObject>>();

    #region 로비플레이어 함수
    [SerializeField] List<GameObject> m_listLobbyPlayers;
    Queue<GameObject> m_poolingLobbyPlayer =new Queue<GameObject>();
    public GameObject m_goLobbyParent;
    #endregion

    #region 인게임플레이어 함수
    [SerializeField] List<GameObject> m_listInGamePlayers;
    Queue<GameObject> m_poolingInGamePlayer = new Queue<GameObject>();
    public GameObject m_goInGameParent;
    #endregion

    #region 인게임 순서 이미지 함수
    [SerializeField] List<GameObject> m_listInGameOrder;
    Queue<GameObject> m_poolingInGameOrder = new Queue<GameObject>();
    #endregion

    #region 에너미 함수
    [SerializeField] List<GameObject> m_listEnemies;
    Queue<GameObject> m_poolingEnemies = new Queue<GameObject>();
    public GameObject m_goEnemyParent;
    #endregion

    private void Awake()
    {
        ppm = this;
        PhotonNetwork.PrefabPool = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        m_dicPrefabs.Add(POOLTYPE.LOBBYPLAYER, m_listLobbyPlayers);
        m_dicPrefabs.Add(POOLTYPE.INGAMEPLAYER, m_listInGamePlayers);
        m_dicPrefabs.Add(POOLTYPE.INGAMEORDER, m_listInGameOrder);
        m_dicPrefabs.Add(POOLTYPE.ENEMIES, m_listEnemies);
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        string[] strInfo = prefabId.Split('_');

        POOLTYPE eCurType = POOLTYPE._MAX_;

        for (int i = 0; i < (int)POOLTYPE._MAX_; i++)
        {
            if ((POOLTYPE.LOBBYPLAYER + i).ToString() == strInfo[0])
            {
                eCurType = POOLTYPE.LOBBYPLAYER + i;
                break;
            }
        }

        if (eCurType == POOLTYPE._MAX_) return null;

        else if (eCurType == POOLTYPE.LOBBYPLAYER)
        {
            if (m_poolingLobbyPlayer.Count > 0)
            {
                var go = m_poolingLobbyPlayer.Dequeue();
                go.transform.position = position;
                go.transform.rotation = rotation;
                return go;
            }
            else
            {
                foreach (var fab in m_dicPrefabs[eCurType])
                {
                    if (fab.name == strInfo[1])
                    {
                        GameObject go = Instantiate(fab, position, rotation);
                        go.transform.SetParent(m_goLobbyParent.transform);
                        go.GetComponent<LobbyPlayerController>().InitClassFormInfo();
                        go.SetActive(false);
                        return go;
                    }
                }
            }
        }

        else if (eCurType == POOLTYPE.INGAMEPLAYER)
        {
            if (m_poolingInGamePlayer.Count > 0)
            {
                var go = m_poolingInGamePlayer.Dequeue();
                go.transform.position = position;
                go.transform.rotation = rotation;
                GameManager.gm.m_listPlayerInfo.Add(go.GetComponent<InGamePlayerController>());
                return go;
            }
            else
            {
                foreach (var fab in m_dicPrefabs[eCurType])
                {
                    if (fab.name == strInfo[1])
                    {
                        GameObject go = Instantiate(fab, position, rotation);
                        go.transform.SetParent(m_goInGameParent.transform);
                        //go.GetComponent<InGamePlayerController>().InitClassFormInfo();
                        GameManager.gm.m_listPlayerInfo.Add(go.GetComponent<InGamePlayerController>());
                        go.SetActive(false);
                        return go;
                    }
                }
            }
        }

        else if (eCurType == POOLTYPE.INGAMEORDER)
        {
            if (m_poolingInGameOrder.Count > 0)
            {
                var go = m_poolingInGameOrder.Dequeue();
                go.transform.position = position;
                go.transform.rotation = rotation;
                go.transform.SetParent(GameManager.m_curUI.GetComponent<UIManagerInGame>().m_goTimelineParent.transform);
                GameManager.gm.m_listOrderInfo.Add(go.GetComponent<OrderInfo>());
                return go;
            }
            else
            {
                foreach (var fab in m_dicPrefabs[eCurType])
                {
                    if (fab.name == strInfo[1])
                    {
                        GameObject go = Instantiate(fab, position, rotation);
                        go.transform.SetParent(GameManager.m_curUI.GetComponent<UIManagerInGame>().m_goTimelineParent.transform);
                        GameManager.gm.m_listOrderInfo.Add(go.GetComponent<OrderInfo>());
                        go.SetActive(false);
                        return go;
                    }
                }
            }
        }

        else if (eCurType == POOLTYPE.ENEMIES)
        {
            if (m_poolingEnemies.Count > 0)
            {
                var go = m_poolingEnemies.Dequeue();
                go.transform.position = position;
                go.transform.rotation = rotation;
                GameManager.gm.m_listEnemyInfo.Add(go.GetComponent<EnemyFSM>());
                return go;
            }
            else
            {
                foreach (var fab in m_dicPrefabs[eCurType])
                {
                    if (fab.name == strInfo[1])
                    {
                        GameObject go = Instantiate(fab, position, rotation);
                        go.transform.SetParent(m_goEnemyParent.transform);
                        //go.GetComponent<EnemyFSM>().InitFormInfo();
                        GameManager.gm.m_listEnemyInfo.Add(go.GetComponent<EnemyFSM>());
                        go.SetActive(false);
                        return go;
                    }
                }
            }
        }

        return null;
    }

    public void Destroy(GameObject gameObject)
    {
        if (gameObject.GetComponent<LobbyPlayerController>() != null)
        {
            gameObject.SetActive(false);
            gameObject.GetComponent<LobbyPlayerController>().m_playerCanvas.m_imgMine.gameObject.SetActive(false);
            m_poolingLobbyPlayer.Enqueue(gameObject);
        }

        if (gameObject.GetComponent<InGamePlayerController>() != null)
        {
            gameObject.SetActive(false);
            gameObject.GetComponent<InGamePlayerController>().m_playerCanvas.m_imgMine.gameObject.SetActive(false);
            m_poolingInGamePlayer.Enqueue(gameObject);
            GameManager.gm.m_listPlayerInfo.Remove(gameObject.GetComponent<InGamePlayerController>());
        }

        if (gameObject.GetComponent<OrderInfo>() != null)
        {
            GameManager.gm.m_listOrderInfo.Remove(gameObject.GetComponent<OrderInfo>());
            gameObject.SetActive(false);
            gameObject.transform.SetParent(this.transform);
            m_poolingInGameOrder.Enqueue(gameObject);
        }

        if (gameObject.GetComponent<EnemyFSM>() != null)
        {
            gameObject.SetActive(false);
            m_poolingEnemies.Enqueue(gameObject);
            GameManager.gm.m_listEnemyInfo.Remove(gameObject.GetComponent<EnemyFSM>());
        }
    }

    public void ResetAll()
    {
        while(m_poolingInGameOrder.Count>0)
        {
            Object.Destroy(m_poolingInGameOrder.Dequeue());
        }

        while(m_poolingEnemies.Count >0)
        {
            Object.Destroy(m_poolingEnemies.Dequeue());
            GameManager.gm.m_listEnemyInfo.Clear();
        }

        while(m_poolingInGamePlayer.Count > 0)
        {
            Object.Destroy(m_poolingInGamePlayer.Dequeue());
        }

        while (m_poolingLobbyPlayer.Count > 0)
        {
            Object.Destroy(m_poolingLobbyPlayer.Dequeue());
        }
    }
}

