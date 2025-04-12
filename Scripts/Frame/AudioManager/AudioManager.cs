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
public enum E_AudioType {
    E_NONE = -1,
    E_BACK_MUSIC,
    E_MUSIC,
    E_EFFECT,
}
public enum E_PlayState
{
    E_NONE = -1,
    E_PLAYING,
    E_UPPER_GRADUALLY,
    E_LOWER_GRADUALLY,
    E_PAUSE,
    E_STOP,
}
public class AudioManager : Singleton<AudioManager>
{
    public AudioData current_audiodata;
    public const float GRADUALLY_MIN_VOLUME = 0.01f; // 渐变的最小音量 到达这个音量后就会自动停止
    private AudioSource back_music;
    private List<PlayingMusic> musics = new();
    /// <summary>
    ///  内部类 为了精确控制音乐的播放状态
    ///  audio_source： 音频源
    ///  coroutine：当音乐需要渐变时，使用协程来控制音量的变化
    ///  callBack: 渐变完成时的回调函数
    /// </summary>
    private class PlayingMusic {
        public AudioSource audio_source;
        public Coroutine coroutine;
        public Action<PlayingMusic> callBack;
        public E_PlayState play_state; // 播放状态

        // 初始化方法 协程和回调默认为空
        public PlayingMusic(AudioSource audio_source) {
            this.audio_source = audio_source;
        }

        // 提供方法 逐渐增大音量 完成后调用回调函数
        public void GraduallyUpper(Action<PlayingMusic> callBack) {
            // 如果正在进行音量渐变 那么取消它
            if (play_state == E_PlayState.E_UPPER_GRADUALLY || play_state == E_PlayState.E_LOWER_GRADUALLY)
                CancelCoroutine();

            // 设置当前状态为渐增
            play_state = E_PlayState.E_UPPER_GRADUALLY;
            this.callBack = callBack;
            // 开启协程 让音乐渐变到最大
            coroutine = MonoMgr.Instance.ChangeFloatGradually(audio_source.volume, AudioManager.Instance.GetCurrentData().music_volume, SetUpperVolume);
        }

        // 提供方法 逐渐减小音量 完成后调用回调函数
        public void GraduallyLower(Action<PlayingMusic> callBack)
        {
            // 如果正在进行音量渐变 那么取消它
            if (play_state == E_PlayState.E_UPPER_GRADUALLY || play_state == E_PlayState.E_LOWER_GRADUALLY)
                CancelCoroutine();

            play_state = E_PlayState.E_LOWER_GRADUALLY;
            this.callBack = callBack;
            // 开启协程 让音乐渐变到0
            coroutine = MonoMgr.Instance.ChangeFloatGradually(audio_source.volume, 0, SetLowerVolume);
        }

        // 提供方法 取消渐变
        private void CancelCoroutine() {
            if (coroutine != null)
            {
                MonoMgr.Instance.StopCoroutine(coroutine);
                coroutine = null;
            }
            play_state = E_PlayState.E_PLAYING;
            callBack = null;
        }

        private void SetUpperVolume(float volume) {
            audio_source.volume = volume;

            // 当音量即将达到目标音量时，停止渐变
            if (Mathf.Abs(AudioManager.Instance.GetCurrentData().music_volume - volume) <= AudioManager.GRADUALLY_MIN_VOLUME) {
                audio_source.volume = AudioManager.Instance.GetCurrentData().music_volume;
                if (coroutine != null)
                {
                    MonoMgr.Instance.StopCoroutine(coroutine);
                    coroutine = null;
                }
                play_state = E_PlayState.E_PLAYING;
                callBack?.Invoke(this);
            }
        }
        private void SetLowerVolume(float volume)
        {
            audio_source.volume = volume;

            // 当音量即将达到0时，停止渐变
            if (volume <= AudioManager.GRADUALLY_MIN_VOLUME)
            {
                if (coroutine != null)
                {
                    MonoMgr.Instance.StopCoroutine(coroutine);
                    coroutine = null;
                }
                play_state = E_PlayState.E_STOP;
                callBack?.Invoke(this);
            }
        }

        public void Destroy() {
            audio_source.Stop();
            CancelCoroutine();
        }
    }
    public override void Init()
    {
        current_audiodata = JsonMgr.Instance.LoadData<AudioData>("AudioData");
    }
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
                // 先把音量设为0 为渐增播放做准备
                audio_source.volume = 0;
                // 即使isMute为true，也不用取消渐增，确保当音量突然打开时音量正常
                audio_source.mute = current_audiodata.music_isMute;
                // 创建一个播放音数据类型
                PlayingMusic playing_music = new PlayingMusic(audio_source);
                // 添加到播放列表中
                musics.Add(playing_music);
                // 调用渐增方法 并传入渐增完成的回调函数
                playing_music.GraduallyUpper(MusicGraduallyCallBack);

                break;
            case E_AudioType.E_EFFECT:
                audio_source.volume = current_audiodata.effect_volume;
                audio_source.mute = current_audiodata.effect_isMute;
                break;
        }
        audio_source.Play();
    }
    public void StopSafely(AudioSource audio_source)
    {
        // 会不会有人把空传进来？不管怎么样，先防一手
        if (audio_source == null)
            return;

        // 如果是背景音乐的话，停止背景音乐
        if (audio_source == back_music) {
            CloseBackMusic();
            return;
        }

        Debug.Log("其他音乐类型");
        // 如果是其他音乐的话，渐变停止音乐
        for (int i = 0; i < musics.Count; i++)
        {
            if (musics[i].audio_source == audio_source)
            {
                // 找到对应audio_source
                Debug.Log("找到对应audio_source");
                musics[i].GraduallyLower(MusicGraduallyCallBack);
                return;
            }
        }
    }

    // 渐变音量回调 当音乐渐变到0时 该回调会删除对应音乐
    private void MusicGraduallyCallBack(PlayingMusic music) {
        if (music.play_state == E_PlayState.E_STOP)
            RemovePlayingMusic(music);

        // 如果是渐增完成，那么不必理会 PlayingMusic内部已经完成处理
    }

    private void RemovePlayingMusic(PlayingMusic music)
    {
        if (music != null)
        {
            music.Destroy();
            musics.Remove(music);
        }
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
