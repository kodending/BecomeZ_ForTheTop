using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerInMain : MonoBehaviourPunCallbacks
{
    #region �ΰ�
    [SerializeField] Image m_imgLogo;
    [SerializeField] Text m_txtStart;
    #endregion

    #region ���θ޴�
    [SerializeField] Text m_txtAccess;
    [SerializeField] Text m_txtOption;
    #endregion

    #region �α��θ޴�
    [SerializeField] InputField m_inputLoginEmail;
    [SerializeField] InputField m_inputLoginPassword;
    [SerializeField] Text m_txtLogin;
    [SerializeField] Text m_txtJoinMenu;
    #endregion

    #region ȸ�����Ը޴�
    [SerializeField] InputField m_inputJoinEmail;
    [SerializeField] InputField m_inputJoinNickName;
    [SerializeField] InputField m_inputJoinPassword;
    [SerializeField] InputField m_inputJoinPasswordConfirm;
    [SerializeField] Text m_txtJoin;
    #endregion

    #region ����
    [SerializeField] Text m_txtReturn;
    [SerializeField] Image m_imgDarkChange;
    #endregion

    public void OnClickStart()
    {
        UIManager.FadeOutImage(m_imgLogo, 1f);
        UIManager.FadeOutText(m_txtStart, 1f);

        StartCoroutine(OnMainPanel());
    }

    IEnumerator OnMainPanel()
    {
        yield return new WaitForSeconds(1f);

        UIManager.FadeInText(m_txtAccess, 0.5f);
        UIManager.FadeInText(m_txtOption, 0.5f);

        yield return new WaitForSeconds(0.5f);

        if(NetworkManager.CheckLogin())
        {
            UIManager.SystemMsg("�ȳ��ϼ���. " + NetworkManager.nm.m_myPlayFabInfo.DisplayName + "��");
        }
    }

    public void OnClickAccess()
    {
        UIManager.FadeOutText(m_txtAccess, 0.5f);
        UIManager.FadeOutText(m_txtOption, 0.5f);

        //�α����� �Ǿ��ִٸ� ���� ���� �Լ�
        if (NetworkManager.CheckLogin())
        {
            NetworkManager.Connet();
            UIManager.FadeInImage(m_imgDarkChange, 1f);
        }
        else
        {
            //�α����� �ȵǾ��ִٸ� �α��θ޴� �����ֱ�
            StartCoroutine(OnLoginMenu());
        }
    }

    IEnumerator OnLoginMenu()
    {
        yield return new WaitForSeconds(0.5f);

        UIManager.ShowScale(m_inputLoginEmail.gameObject);
        UIManager.ShowScale(m_inputLoginPassword.gameObject);
        UIManager.ShowScale(m_txtReturn.gameObject);
        UIManager.FadeInText(m_txtLogin, 0.5f);
        UIManager.FadeInText(m_txtJoinMenu, 0.5f);
    }

    public void OnClickJoinMenu()
    {
        UIManager.HideScale(m_inputLoginEmail.gameObject);
        UIManager.HideScale(m_inputLoginPassword.gameObject);
        UIManager.FadeOutText(m_txtLogin, 0.5f);
        UIManager.FadeOutText(m_txtJoinMenu, 0.5f);

        StartCoroutine(OnJoinMenu());
    }
    
    IEnumerator OnJoinMenu()
    {
        yield return new WaitForSeconds(0.5f);

        UIManager.ShowScale(m_inputJoinEmail.gameObject);
        UIManager.ShowScale(m_inputJoinNickName.gameObject);
        UIManager.ShowScale(m_inputJoinPassword.gameObject);
        UIManager.ShowScale(m_inputJoinPasswordConfirm.gameObject);
        UIManager.FadeInText(m_txtJoin, 0.5f);
    }

    public void OnClickReturnMenu()
    {
        UIManager.HideScale(m_inputLoginEmail.gameObject);
        UIManager.HideScale(m_inputLoginPassword.gameObject);
        UIManager.HideScale(m_inputJoinEmail.gameObject);
        UIManager.HideScale(m_inputJoinNickName.gameObject);
        UIManager.HideScale(m_inputJoinPassword.gameObject);
        UIManager.HideScale(m_inputJoinPasswordConfirm.gameObject);
        UIManager.HideScale(m_txtReturn.gameObject);
        UIManager.FadeOutText(m_txtLogin, 0.5f);
        UIManager.FadeOutText(m_txtJoinMenu, 0.5f);
        UIManager.FadeOutText(m_txtJoin, 0.5f);

        StartCoroutine(OnMainPanel());
    }

    public void OnClickLogin()
    {
        NetworkManager.Login(m_inputLoginEmail.text, m_inputLoginPassword.text);
        m_inputLoginPassword.text = "";
    }

    public IEnumerator OnSuccessLogin()
    {
        yield return new WaitForSeconds(0.5f);

        OnClickReturnMenu();
    }

    public void OnClickSignUp()
    {
        if(m_inputJoinPassword.text != m_inputJoinPasswordConfirm.text)
        {
            UIManager.SystemMsg("��й�ȣ�� ��ġ���� �ʽ��ϴ�");
            m_inputJoinPassword.text = "";
            m_inputJoinPasswordConfirm.text = "";
            return;
        }

        NetworkManager.Register(m_inputJoinEmail.text, m_inputJoinPassword.text, m_inputJoinNickName.text);

        m_inputJoinEmail.text = "";
        m_inputJoinNickName.text = "";
        m_inputJoinPassword.text = "";
        m_inputJoinPasswordConfirm.text = "";
    }

    public void OnSuccessSignUp()
    {
        OnClickReturnMenu();
    }
}
