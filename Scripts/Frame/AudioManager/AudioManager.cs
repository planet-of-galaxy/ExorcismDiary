using System;
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

public class AudioManager : Singleton<AudioManager>
{
    public AudioData current_audiodata;
    public const float GRADUALLY_MIN_VOLUME = 0.01f; // �������С���� �������������ͻ��Զ�ֹͣ
    private List<Music> musics = new(); // ���ָ��ŵ���Ϸ����
    private GameObject music_gameObject;
    /// <summary>
    ///  �ڲ��� Ϊ�˾�ȷ�������ֵĲ���״̬
    ///  audio_source�� ��ƵԴ
    ///  coroutine����������Ҫ����ʱ��ʹ��Э�������������ı仯
    ///  callBack: �������ʱ�Ļص�����
    /// </summary>

    public override void Init()
    {
        current_audiodata = JsonMgr.Instance.LoadData<AudioData>("AudioData");
    }
    private void Save()
    {
        JsonMgr.Instance.SaveData(current_audiodata, "AudioData");
    }
    // �����������ֵ�������С ���Զ�����
    public void SetMusicVolume(float volume)
    {
        current_audiodata.music_volume = volume;

        BackMusicManager.Instance.SetVolume(volume);
        Save();
    }
    // ����������Ч��������С ���Զ�����
    public void SetEffectVolume(float volume)
    {
        current_audiodata.effect_volume = volume;
        Save();
    }
    // �����������ֵľ���״̬ ���Զ�����
    public void SetMusicMute(bool isMute)
    {
        current_audiodata.music_isMute = isMute;
        // Ϊ����������
        BackMusicManager.Instance.SetMute(isMute);
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

    public void PlayBackMusic(string name) {
        BackMusicManager.Instance.PlayBackMusic(name);
    }
    [Obsolete]
    public void StopSafely(string name)
    {
        Music music = null;
        // ������������ֵĻ� ��ֵΪ��������
        if (GetMusic(name) is Music exits_music)
        {
            music = exits_music;
        }
        if (music == null)
            Debug.Log("û�ҵ�");
        // ������ֹͣ
        music?.GraduallyLower(MusicGraduallyLowerCallBack);
    }

    // ���������ص� �����ֽ��䵽0ʱ �ûص���ɾ����Ӧ����
    private void MusicGraduallyLowerCallBack(Music music) {
        switch (music.audio_type) {
            case E_AudioType.E_BACK_MUSIC:
                break;
            case E_AudioType.E_MUSIC:
                // ����б����� ��ʼ���²��ű�����
                BackMusicManager.Instance.Play();
                break;
        }
    }
    private void MusicGraduallyUpperCallBack(Music music)
    {
        // ������ɻص����� Ŀǰû��ʲô�ô����
    }
    public bool IsAnyMusicPlaying() {
        bool ret = false;
        // ���������ֳ��� ��Ϊ�����Ͼ�Ҫ�ر���
        for (int i = 0; i < musics.Count; i++) {
            if (musics[i].play_state == E_PlayState.E_PLAYING ||
                musics[i].play_state == E_PlayState.E_UPPER_GRADUALLY)
            { ret = true; break; }

        }
        return ret;
    }

    public bool HasMusic(AudioSource audio_source) {
        bool ret = false;
        for (int i = 0; i < musics.Count; i++) {
            if (musics[i].audio_source == audio_source) { ret = true; break; }
        }
        return ret;
    }
    private Music GetMusic(string name) {
        Music music = null;
        for (int i = 0; i < musics.Count; i++)
        {
            if (musics[i].name == name) { music = musics[i]; break; }
        }
        return music;
    }

    public AudioData GetAudioData()
    {
        return current_audiodata;
    }

    public void StopBackMusic()
    {
        BackMusicManager.Instance.Stop();
    }
    // �������� �ÿձ������������б�
    public void Clear() {
        BackMusicManager.Instance.Clear();
        for (int i = 0; i < musics.Count; i++) {
            musics[i].Close();
        }
        musics.Clear();
    }
    [Obsolete]
    public void PlaySafely(string name, E_AudioType audio_type) {
        AudioSource audio_source;
        Music music;
        switch (audio_type) {
            case E_AudioType.E_MUSIC:
                if (music_gameObject == null)
                {
                    music_gameObject = new GameObject("Music");
                }
                if (GetMusic(name) is Music current_music) {
                    BackMusicManager.Instance.Stop();
                    current_music.GraduallyUpper(MusicGraduallyUpperCallBack);
                    return;
                }
                audio_source = music_gameObject.AddComponent<AudioSource>();
                music = new Music(music_gameObject, name, audio_source, audio_type);
                musics.Add(music);
                music.LoadClipAsync(LoadClipCallBack);
                break;
        }
    }

    private void LoadClipCallBack(Music music) {
        Debug.Log("���ֵļ���״̬Ϊ:" + music.audio_source.clip.loadType);
        switch (music.audio_type) {
            case E_AudioType.E_BACK_MUSIC:
                if (IsAnyMusicPlaying()) {
                    // ������������ڲ��� ��ô���Ȳ�����
                    return;
                }
                music.GraduallyUpper(MusicGraduallyUpperCallBack);
                break;
            case E_AudioType.E_MUSIC:
                BackMusicManager.Instance.Stop();
                music.GraduallyUpper(MusicGraduallyUpperCallBack);
                break;
        }
    }
}
