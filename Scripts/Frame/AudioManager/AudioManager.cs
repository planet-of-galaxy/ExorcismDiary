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
    public const float GRADUALLY_MIN_VOLUME = 0.01f; // 渐变的最小音量 到达这个音量后就会自动停止
    private List<Music> musics = new(); // 音乐附着的游戏物体
    private GameObject music_gameObject;
    /// <summary>
    ///  内部类 为了精确控制音乐的播放状态
    ///  audio_source： 音频源
    ///  coroutine：当音乐需要渐变时，使用协程来控制音量的变化
    ///  callBack: 渐变完成时的回调函数
    /// </summary>

    public override void Init()
    {
        current_audiodata = JsonMgr.Instance.LoadData<AudioData>("AudioData");
    }
    private void Save()
    {
        JsonMgr.Instance.SaveData(current_audiodata, "AudioData");
    }
    // 调节整体音乐的音量大小 并自动保存
    public void SetMusicVolume(float volume)
    {
        current_audiodata.music_volume = volume;

        BackMusicManager.Instance.SetVolume(volume);
        Save();
    }
    // 调节整体音效的音量大小 并自动保存
    public void SetEffectVolume(float volume)
    {
        current_audiodata.effect_volume = volume;
        Save();
    }
    // 调节整体音乐的静音状态 并自动保存
    public void SetMusicMute(bool isMute)
    {
        current_audiodata.music_isMute = isMute;
        // 为背景音设置
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
        // 如果是其他音乐的话 赋值为其他音乐
        if (GetMusic(name) is Music exits_music)
        {
            music = exits_music;
        }
        if (music == null)
            Debug.Log("没找到");
        // 渐减并停止
        music?.GraduallyLower(MusicGraduallyLowerCallBack);
    }

    // 渐变音量回调 当音乐渐变到0时 该回调会删除对应音乐
    private void MusicGraduallyLowerCallBack(Music music) {
        switch (music.audio_type) {
            case E_AudioType.E_BACK_MUSIC:
                break;
            case E_AudioType.E_MUSIC:
                // 如果有背景音 开始重新播放背景音
                BackMusicManager.Instance.Play();
                break;
        }
    }
    private void MusicGraduallyUpperCallBack(Music music)
    {
        // 渐增完成回调函数 目前没有什么好处理的
    }
    public bool IsAnyMusicPlaying() {
        bool ret = false;
        // 渐减的音乐除外 因为它马上就要关闭了
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
    // 过场景用 置空背景音和音乐列表
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
        Debug.Log("音乐的加载状态为:" + music.audio_source.clip.loadType);
        switch (music.audio_type) {
            case E_AudioType.E_BACK_MUSIC:
                if (IsAnyMusicPlaying()) {
                    // 如果有音乐正在播放 那么就先不播放
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
