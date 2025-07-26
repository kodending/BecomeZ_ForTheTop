using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserUIInfo : MonoBehaviourPunCallbacks
{
    public GameObject[] maxInsights;
    public GameObject[] curInsights;
    public Text name;
    public Image attributeImg;
    public Text curHpText;
    public Slider sliderHp;
    public Slider sliderHpBack;
    public Slider sliderMp;
    public Slider sliderMpBack;
    public Text curMaxHpText;
    public Text curMaxMpText;
    public Text curPhysicalPowText;
    public Text curMagicPowText;
    public Text curPhysicalDefText;
    public Text curMagicDefText;
    public Text curSpeedText;
}
