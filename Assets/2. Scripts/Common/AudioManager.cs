using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviourPunCallbacks
{
    public static AudioManager am;

    [Header("BGM")]
    public AudioClip[] bgmClips;
    public float bgmVolume;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;

    [Header("SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    private void Awake()
    {
        am = this;
        DontDestroyOnLoad(am);

        //배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.SetParent(transform);
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;

        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        //효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.SetParent(transform);
        sfxPlayers = new AudioSource[channels];

        for (int idx = 0; idx < sfxPlayers.Length; idx++)
        {
            sfxPlayers[idx] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[idx].playOnAwake = false;
            sfxPlayers[idx].bypassListenerEffects = true;
            sfxPlayers[idx].volume = sfxVolume;
        }
    }
    static public void PlayBGM(BGM eBgm, bool isPlay)
    {
        am.bgmPlayer.clip = am.bgmClips[(int)eBgm];

        if (isPlay) am.bgmPlayer.Play();
        else am.bgmPlayer.Stop();
    }

    static public bool IsPlayBGM(BGM eBgm)
    {
        am.bgmPlayer.clip = am.bgmClips[(int)eBgm];

        return am.bgmPlayer.isPlaying;
    }

    static public void PlaySfx(SFX eSfx)
    {
        for (int idx = 0; idx < am.sfxPlayers.Length; idx++)
        {
            int loopIdx = (idx + am.channelIndex) % am.sfxPlayers.Length;

            if (am.sfxPlayers[loopIdx].isPlaying) continue;

            am.channelIndex = loopIdx;
            am.sfxPlayers[loopIdx].clip = am.sfxClips[(int)eSfx];
            am.sfxPlayers[loopIdx].Play();
            break;
        }
    }
}
