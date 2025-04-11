using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioData
{
    public float music_volume = 0.2f;
    public float effect_volume = 0.2f;
    public bool music_isMute = false;
    public bool effect_isMute = false;

    public void Deconstruct(out bool music_isMute, out float music_volume, out bool effect_isMute, out float effect_volume) =>
        (music_isMute, music_volume, effect_isMute, effect_volume) = (this.music_isMute, this.music_volume, this.effect_isMute, this.effect_volume);
}
public enum E_AudioType {
    E_NONE = 0,
    E_BACK_MUSIC,
    E_MUSIC,
    E_EFFECT,
}
public class AudioManager : Singleton<AudioManager>
{
    public AudioData current_audiodata;
    private AudioSource back_music;

    private void Save()
    {
        JsonMgr.Instance.SaveData(current_audiodata, "AudioData");
    }

    public void SetMusicVolume(float volume)
    {
        current_audiodata.music_volume = volume;
        if (back_music != null)
            back_music.volume = volume;
        Save();
    }

    public void SetEffectVolume(float volume)
    {
        current_audiodata.effect_volume = volume;
        Save();
    }

    public void SetMusicMute(bool isMute)
    {
        current_audiodata.music_isMute = isMute;
        if (back_music != null)
            back_music.mute = isMute;
        Save();
    }

    public void SetEffectMute(bool isMute)
    {
        current_audiodata.effect_isMute = isMute;
        Save();
    }

    public AudioData GetCurrentData()
    {
        return current_audiodata;
    }

    public void PlaySafely(AudioSource audio_source, E_AudioType audio_type)
    {
        switch (audio_type)
        {
            case E_AudioType.E_BACK_MUSIC:
                if (back_music != null)
                {
                    back_music.Stop();
                }
                back_music = audio_source;
                audio_source.volume = current_audiodata.music_volume;
                audio_source.mute = current_audiodata.music_isMute;
                break;
            case E_AudioType.E_MUSIC:
                audio_source.volume = current_audiodata.music_volume;
                audio_source.mute = current_audiodata.music_isMute;
                break;
            case E_AudioType.E_EFFECT:
                audio_source.volume = current_audiodata.effect_volume;
                audio_source.mute = current_audiodata.effect_isMute;
                break;
        }
        audio_source.Play();
    }

    public override void Init()
    {
        current_audiodata = JsonMgr.Instance.LoadData<AudioData>("AudioData");
    }

    public AudioData GetAudioData()
    {
        return current_audiodata;
    }

    public void CloseBackMusic()
    {
        if (back_music != null)
        {
            back_music.Stop();
            back_music = null;
        }
    }
}
