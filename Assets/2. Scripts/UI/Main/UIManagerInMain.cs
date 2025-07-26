using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIManagerInMain : MonoBehaviourPunCallbacks
{
    #region 로고
    [SerializeField] Image m_imgLogo;
    [SerializeField] Text m_txtStart;
    #endregion

    #region 메인메뉴
    [SerializeField] Text m_txtAccess;
    [SerializeField] Text m_txtOption;
    #endregion

    #region 로그인메뉴
    [SerializeField] InputField m_inputLoginEmail;
    [SerializeField] InputField m_inputLoginPassword;
    [SerializeField] Text m_txtLogin;
    [SerializeField] Text m_txtJoinMenu;
    #endregion

    #region 회원가입메뉴
    [SerializeField] InputField m_inputJoinEmail;
    [SerializeField] InputField m_inputJoinNickName;
    [SerializeField] InputField m_inputJoinPassword;
    [SerializeField] InputField m_inputJoinPasswordConfirm;
    [SerializeField] Text m_txtJoin;
    #endregion

    #region 공통
    [SerializeField] Text m_txtReturn;
    [SerializeField] Image m_imgDarkChange;
    #endregion

    #region 옵션
    [SerializeField] GameObject m_goOptionPanel;
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;
    float bgmRatio;
    float sfxRatio;
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
            UIManager.SystemMsg("안녕하세요. " + NetworkManager.nm.m_myPlayFabInfo.DisplayName + "님");
        }
    }

    public void OnClickAccess()
    {
        UIManager.FadeOutText(m_txtAccess, 0.5f);
        UIManager.FadeOutText(m_txtOption, 0.5f);

        //로그인이 되어있다면 게임 접속 함수
        if (NetworkManager.CheckLogin())
        {
            NetworkManager.Connet();
            UIManager.FadeInImage(m_imgDarkChange, 1f);
        }
        else
        {
            //로그인이 안되어있다면 로그인메뉴 보여주기
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
            UIManager.SystemMsg("비밀번호가 일치하지 않습니다");
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

    public void OnClickLoadOption()
    {
        UIManager.FadeOutText(m_txtAccess, 0.5f);
        UIManager.FadeOutText(m_txtOption, 0.5f);

        UIManager.ShowScale(m_goOptionPanel, 0.5f);

        masterSlider.minValue = 0.0001f;
        masterSlider.maxValue = 1f;
        bgmSlider.minValue = 0.0001f;
        bgmSlider.maxValue = 1f;
        sfxSlider.minValue = 0.0001f;
        sfxSlider.maxValue = 1f;


        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float bgm = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

        masterSlider.value = master;
        bgmSlider.value = bgm;
        sfxSlider.value = sfx;

        bgmRatio = bgm / master;
        sfxRatio = sfx / master;

        SetMasterVolume(master);
        SetBGMVolume(bgm);
        SetSFXVolume(sfx);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        //audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        AudioManager.SetMasterVolume(value);
        PlayerPrefs.SetFloat("MasterVolume", value);

        // BGM, SFX도 비율에 맞춰 조정
        bgmSlider.SetValueWithoutNotify(value * bgmRatio);
        sfxSlider.SetValueWithoutNotify(value * sfxRatio);

        SetBGMVolume(value * bgmRatio);
        SetSFXVolume(value * sfxRatio);

        PlayerPrefs.Save();
    }

    public void SetBGMVolume(float value)
    {
        //audioMixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
        AudioManager.SetBGMVolume(value);
        PlayerPrefs.SetFloat("BGMVolume", value);

        bgmRatio = value / masterSlider.value;

        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        AudioManager.SetSFXVolume(value);
        //audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", value);

        sfxRatio = value / masterSlider.value;

        PlayerPrefs.Save();
    }

    public void OnClickBackMain()
    {
        UIManager.HideScale(m_goOptionPanel);

        UIManager.FadeInText(m_txtAccess, 0.5f);
        UIManager.FadeInText(m_txtOption, 0.5f);
    }
}
