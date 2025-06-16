using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] Text m_txtChat;
    public Image m_imgChat;
    public Image m_imgMine;

    private void Update()
    {
        transform.LookAt(Camera.main.transform);
    }

    public void ChatText(string strTxt)
    {
        m_txtChat.text = strTxt;
    }
}
