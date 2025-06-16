using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIInfo : MonoBehaviourPunCallbacks
{
    public Text name;
    public Image attributeImg;
    public Text curHpText;
    public Text curPhysicalDefText;
    public Text curMagicDefText;
    public Slider sliderHp;
    public Slider sliderHpBack;
}
