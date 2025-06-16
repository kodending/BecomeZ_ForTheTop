using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager um;

    static Sequence m_seq;

    private void Awake()
    {
        um = this;
        DontDestroyOnLoad(um);
    }

    static public void StopSequence()
    {
        m_seq.Kill();
    }

    #region 페이드 인아웃 함수 정리
    static public void FadeOutImage(Image img, float fTime)
    {
        m_seq = DOTween.Sequence()
        .SetAutoKill(true)
        .Append(img.DOFade(0, fTime))
        .OnComplete(() =>
        {
            if (img != null)
                img.gameObject.SetActive(false);
        });
    }

    static public void FadeOutText(Text txt, float fTime)
    {
        m_seq = DOTween.Sequence()
        .SetAutoKill(true)
        .Append(txt.DOFade(0, fTime))
        .OnComplete(() =>
        {
            if (txt != null)
            txt.gameObject.SetActive(false);
        });
    }

    static public void FadeInOutText(Text txt, float fTime)
    {
        m_seq = DOTween.Sequence()
        .SetAutoKill(true)
        .OnStart(() =>
        {
            if (txt != null)
            txt.gameObject.SetActive(true);
        })
        .Append(txt.DOFade(1, 0.5f))
        .Append(txt.DOFade(1, fTime))
        .Append(txt.DOFade(0, 0.5f))
        .OnComplete(() =>
        {
            if (txt != null)
            txt.gameObject.SetActive(false);
        });
    }

    static public void FadeInImage(Image img, float fTime)
    {
        m_seq = DOTween.Sequence()
        .SetAutoKill(true)
        .OnStart(() =>
        {
            if (img != null)
            img.gameObject.SetActive(true);
        })
        .Append(img.DOFade(1, fTime));
    }

    static public void FadeInText(Text txt, float fTime)
    {
        m_seq = DOTween.Sequence()
        .SetAutoKill(true)
        .OnStart(() =>
        {
            if (txt != null)
             txt.gameObject.SetActive(true);
        })
        .Append(txt.DOFade(1, fTime));
    }
    #endregion

    #region 스케일 함수 정리

    static public void ShowScale(GameObject go, float fTime = 0.2f)
    {
        m_seq = DOTween.Sequence()
        .SetAutoKill(true)
        .OnStart(() =>
        {
            if (go != null)
            {
                go.SetActive(true);
                go.transform.localScale = Vector3.one * 0.1f;
            }
        })
        .Append(go.transform.DOScale(1.1f, fTime))
        .Append(go.transform.DOScale(1.0f, 0.1f));
    }

    static public void HideScale(GameObject go)
    {
        m_seq = DOTween.Sequence()
        .SetAutoKill(true)
        .Append(go.transform.DOScale(1.1f, 0.1f))
        .Append(go.transform.DOScale(0.2f, 0.2f))
        .OnComplete(() =>
        {
            if (go != null)
                go.SetActive(false);
        });
    }

    static public void SelectSlot(GameObject go, float fTime = 0.2f)
    {
        m_seq = DOTween.Sequence()
        .SetAutoKill(true)
        .OnStart(() =>
        {
            if (go != null)
            {
                go.SetActive(true);
                go.transform.localScale = Vector3.one * 1.2f;
            }
        })
        .Append(go.transform.DOScale(1.0f, fTime));
    }

    static public void EquipRune(GameObject go, float fTime = 0.1f)
    {
        m_seq = DOTween.Sequence()
        .SetAutoKill(true)
        .OnStart(() =>
        {
            if (go != null)
            {
                go.SetActive(true);
                go.transform.localScale = Vector3.one * 1.2f;
            }
        })
        .Append(go.transform.DOScale(1.0f, fTime));
    }

    static public void ShowVerticalScale(GameObject go, float fTime = 0.4f)
    {
        m_seq = DOTween.Sequence()
        .SetAutoKill(true)
        .OnStart(() =>
        {
            if (go != null)
            {
                go.SetActive(true);
                go.transform.localScale = new Vector3(1.0f, 0.1f, 1.0f);
            }
        })
        .Append(go.transform.DOScale(1.0f, fTime).SetEase(Ease.OutQuart));
    }

    static public void CardSelectEffect(GameObject go, float fTime = 0.1f)
    {
        Sequence seq = DOTween.Sequence()
            .SetAutoKill(true)
            .Append(go.transform.DOScale(1.2f, fTime).SetEase(Ease.OutBack))
            .Append(go.transform.DOScale(1.0f, 0.1f));
    }

    static public void SystemMsg(string msg)
    {
        GameObject go = GameObject.Find("Canvas").transform.Find("SystemMessage").gameObject;
        go.transform.Find("TextMessage").GetComponent<Text>().text = msg;

        m_seq = DOTween.Sequence()
        .SetAutoKill(true)
        .OnStart(() =>
        {
            if (go != null)
            {
                go.SetActive(true);
                go.transform.localScale = Vector3.one * 0.1f;
            }
        })
        .Append(go.transform.DOScale(1.1f, 0.2f))
        .Append(go.transform.DOScale(1.0f, 0.1f))
        .Append(go.transform.DOScale(1.0f, 1.5f))
        .Append(go.transform.DOScale(1.1f, 0.1f))
        .Append(go.transform.DOScale(0.2f, 0.2f))
        .OnComplete(() =>
        {
            if (go != null)
                go.SetActive(false);
        });
    }

    static public void ShowChat(GameObject go)
    {
        Vector3 chatScale = new Vector3(1.0f, 0.5f, 1.0f);

        m_seq = DOTween.Sequence()
        .SetAutoKill(true)
        .OnStart(() =>
        {
            if (go != null)
            {
                go.SetActive(true);
                go.transform.localScale = Vector3.one * 0.1f;
            }
        })
        .Append(go.transform.DOScale(chatScale, 0.1f))
        .Append(go.transform.DOScale(chatScale, 3.0f))
        .Append(go.transform.DOScale(0.2f, 0.2f))
        .OnComplete(() =>
        {
            if (go != null)
                go.SetActive(false);
        });
    }

    static public void ShowDamageText(GameObject go)
    {
        m_seq = DOTween.Sequence()
        .SetAutoKill(true)
        .OnStart(() =>
        {
            if (go != null)
            {
                go.SetActive(true);
                go.transform.localScale = Vector3.one * 0.0001f;
            }
        })
        .Append(go.transform.DOScale(0.012f, 0.1f))
        .Append(go.transform.DOScale(0.01f, 0.05f))
        .Append(go.transform.DOScale(0.01f, 1.0f))
        .OnComplete(() =>
        {
            if (go != null)
                go.SetActive(false);
        });
    }

    #endregion

    #region 생명력 수치
    static public void CountText(Text txtInfo, int Cnt)
    {
        int beforeCnt;
        int.TryParse(txtInfo.text, out beforeCnt);

        Sequence moneySeq = DOTween.Sequence()
        .SetAutoKill(true)
        .Append(txtInfo.DOCounter(beforeCnt, Cnt, 0.5f, true))
        .OnComplete(() =>
        {
            txtInfo.text = Cnt.ToString();
        });
    }

    #endregion

    #region 슬라이드 UI
    static public void ShowSlideUI(GameObject panel, UIDIRECTION eDir)
    {
        switch (eDir)
        {
            case UIDIRECTION.UP: //아래에서 위로 슬라이드하는 UI
                panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -500);

                m_seq = DOTween.Sequence()
                .SetAutoKill(true)
                .OnStart(() =>
                {
                    panel.SetActive(true);
                })
                .Append(panel.GetComponent<RectTransform>().DOAnchorPosY(20f, 0.3f))
                .Append(panel.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.1f));

                break;

            case UIDIRECTION.DOWN: //위에서 아래로 슬라이드하는 UI
                panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 500);

                m_seq = DOTween.Sequence()
                .SetAutoKill(true)
                .OnStart(() =>
                {
                    panel.SetActive(true);
                })
                .Append(panel.GetComponent<RectTransform>().DOAnchorPosY(-20f, 0.3f))
                .Append(panel.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.1f));

                break;

            case UIDIRECTION.LEFT: //왼쪽으로 슬라이드하는 UI
                panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(500, 0);

                m_seq = DOTween.Sequence()
                .SetAutoKill(true)
                .OnStart(() =>
                {
                    panel.SetActive(true);
                })
                .Append(panel.GetComponent<RectTransform>().DOAnchorPosX(-20f, 0.3f).SetEase(Ease.InCubic))
                .Append(panel.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.1f));

                break;

            case UIDIRECTION.RIGHT: //오른쪽으로 슬라이드하는 UI
                panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-500, 0);

                m_seq = DOTween.Sequence()
                .SetAutoKill(true)
                .OnStart(() =>
                {
                    panel.SetActive(true);
                })
                .Append(panel.GetComponent<RectTransform>().DOAnchorPosX(20f, 0.3f).SetEase(Ease.InCubic))
                .Append(panel.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.1f));


                break;
        }
    }
    #endregion
}
