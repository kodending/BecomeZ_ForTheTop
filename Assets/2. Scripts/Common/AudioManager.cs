using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        bgmPlayer.volume = PlayerPrefs.GetFloat("BGMVolume", 1f);

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
            sfxPlayers[idx].volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }
    }
    static public void PlayBGM(BGM eBgm, bool isPlay)
    {
        if (am.bgmPlayer.clip == am.bgmClips[(int)eBgm]) return;

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

    static public void SetMasterVolume(float volume)
    {
        SetBGMVolume(volume);
        SetSFXVolume(volume);
    }

    static public void SetBGMVolume(float volume)
    {
        am.bgmPlayer.volume = volume;
    }

    static public void SetSFXVolume(float volume)
    {
        for (int idx = 0; idx < am.sfxPlayers.Length; idx++)
        {
            am.sfxPlayers[idx].volume = volume;
        }
    }
}
