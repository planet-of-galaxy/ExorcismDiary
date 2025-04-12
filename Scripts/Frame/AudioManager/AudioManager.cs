using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
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
    private Music back_music;
    private List<Music> musics = new();
    /// <summary>
    ///  内部类 为了精确控制音乐的播放状态
    ///  audio_source： 音频源
    ///  coroutine：当音乐需要渐变时，使用协程来控制音量的变化
    ///  callBack: 渐变完成时的回调函数
    /// </summary>
    private class Music {
        public AudioSource audio_source;
        public Coroutine coroutine;
        public Action<Music> callBack;
        public E_PlayState play_state; // 播放状态
        public E_AudioType audio_type;

        // 初始化方法 协程和回调默认为空
        public Music(AudioSource audio_source, E_AudioType audio_type)
        {
            this.audio_source = audio_source;
            this.audio_type = audio_type;
        }

        // 提供方法 逐渐增大音量 完成后调用回调函数
        public void GraduallyUpper(Action<Music> callBack) {
            // 如果正在进行音量渐变 那么取消它
            if (play_state == E_PlayState.E_UPPER_GRADUALLY || play_state == E_PlayState.E_LOWER_GRADUALLY)
                CancelCoroutine();

            // 设置当前状态为渐增
            play_state = E_PlayState.E_UPPER_GRADUALLY;
            this.callBack = callBack;
            audio_source.volume = 0;
            audio_source.Play();
            // 开启协程 让音乐渐变到最大
            coroutine = MonoMgr.Instance.ChangeFloatGradually(audio_source.volume, AudioManager.Instance.GetCurrentData().music_volume, SetUpperVolume);
        }

        // 提供方法 逐渐减小音量 完成后调用回调函数
        public void GraduallyLower(Action<Music> callBack)
        {
            // 如果正在进行音量渐变 那么取消它
            if (play_state == E_PlayState.E_UPPER_GRADUALLY || play_state == E_PlayState.E_LOWER_GRADUALLY)
                CancelCoroutine();

            play_state = E_PlayState.E_LOWER_GRADUALLY;
            this.callBack = callBack;
            // 开启协程 让音乐渐变到0
            coroutine = MonoMgr.Instance.ChangeFloatGradually(audio_source.volume, 0, SetLowerVolume);
        }
        public void Pause() {
            CancelCoroutine();
            audio_source.Pause();
        }
        public void Close() {
            Destroy();
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
                audio_source.Stop();
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
            back_music.audio_source.volume = volume;
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
            back_music.audio_source.mute = isMute;
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
                // 初始化音量 不用为了渐增去把它设成0 渐增函数内部会自己做
                audio_source.volume = current_audiodata.music_volume;
                // 即使isMute为true，也不用取消渐增，确保当音量突然打开时音量正常
                audio_source.mute = current_audiodata.music_isMute;
                // 如果本来就有背景音且与新背景音不同 那么先关闭之前的背景音 并创建新背景音
                if (back_music != null && back_music.audio_source != audio_source)
                {
                    back_music.Close();
                    back_music = new Music(audio_source, audio_type);
                }
                // 如果原本没有背景音 创建一个新背景音
                else if (back_music == null) {
                    back_music = new Music(audio_source, audio_type);
                }
                // 到这里共有3种情况
                // 1. 原本没有背景音 现在已经创建了新背景音
                // 2. 原本有背景音 现在已经替换成了新背景音
                // 3. 保持原有背景音不变
                // 如果现在没有其他E_MUSIC类型的音乐在播放 那么开始渐增背景音量
                // 如果有那么直接返回 就先不播放
                if (IsAnyMusicPlaying())
                    return;

                // 调用渐增方法 并传入渐增完成的回调函数
                back_music.GraduallyUpper(MusicGraduallyUpperCallBack);
                break;
            case E_AudioType.E_MUSIC:
                Music music;
                // 初始化音量 不用为了渐增去把它设成0 渐增函数内部会自己做
                audio_source.volume = current_audiodata.music_volume;
                // 即使isMute为true，也不用取消渐增，确保当音量突然打开时音量正常
                audio_source.mute = current_audiodata.music_isMute;
                // 如果没有记录过同样的audio_source 那么就去创建一个新的
                if (!HasMusic(audio_source))
                {
                    // 创建一个播放音数据类型 并添加到音乐列表里
                    music = new Music(audio_source, audio_type);
                    // 添加到播放列表中
                    musics.Add(music);
                }
                else {
                    // 获取已有的music
                    music = GetMusic(audio_source);
                    // 如果已经正在播放 或者已经在渐增了 那么就直接返回
                    if (music.play_state == E_PlayState.E_PLAYING ||
                        music.play_state == E_PlayState.E_UPPER_GRADUALLY)
                        return;
                }

                // 如果正在播放背景音乐 先停止播放
                if (back_music != null && 
                    (back_music.play_state == E_PlayState.E_UPPER_GRADUALLY ||
                     back_music.play_state == E_PlayState.E_LOWER_GRADUALLY ||
                     back_music.play_state == E_PlayState.E_PLAYING)) {
                    back_music.Close();
                }
                // 调用渐增方法 并传入渐增完成的回调函数
                music.GraduallyUpper(MusicGraduallyUpperCallBack);
                break;
            case E_AudioType.E_EFFECT:
                audio_source.volume = current_audiodata.effect_volume;
                audio_source.mute = current_audiodata.effect_isMute;
                audio_source.Play();
                break;
        }
    }
    public void StopSafely(AudioSource audio_source)
    {
        // 会不会有人把空传进来？不管怎么样，先防一手
        if (audio_source == null)
            return;

        Music music = null;
        // 如果是背景音乐的话 赋值为背景音乐
        if (back_music != null && audio_source == back_music.audio_source) {
            music = back_music;
        }
        // 如果是其他音乐的话 赋值为其他音乐
        if (GetMusic(audio_source) is Music exis_music) {
            music = exis_music;
        }

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
                back_music?.GraduallyUpper(MusicGraduallyUpperCallBack);
                break;
        }
    }
    private void MusicGraduallyUpperCallBack(Music music)
    { 
        // 渐增完成回调函数 目前没有什么好处理的
    }
    private void MusicGraduallyCloseCallBack(Music music) {
        switch (music.audio_type)
        {
            case E_AudioType.E_BACK_MUSIC:
                // 置空背景音
                back_music = null;
                break;
            case E_AudioType.E_MUSIC:
                // 移除已经关闭的音乐
                RemoveMusic(music);
                // 如果有背景音 开始重新播放背景音
                back_music?.GraduallyUpper(MusicGraduallyUpperCallBack);
                break;
        }
    }

    private void RemoveMusic(Music music)
    {
        if (music != null)
        {
            music.Close();
            musics.Remove(music);
        }
    }
    public bool IsAnyMusicPlaying() {
        bool ret = false;
        // 渐减的音乐除外 因为它马上就要关闭了
        for (int i = 0;i<musics.Count;i++) {
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
    private Music GetMusic(AudioSource audio_source) {
        Music music = null;
        for (int i = 0; i < musics.Count; i++)
        {
            if (musics[i].audio_source == audio_source) { music = musics[i]; break; }
        }
        return music;
    }

    public AudioData GetAudioData()
    {
        return current_audiodata;
    }

    public void CloseBackMusic()
    {
        if (back_music != null)
        {
            back_music.GraduallyLower(MusicGraduallyCloseCallBack);
        }
    }
    // 过场景用 置空背景音和音乐列表
    public void Clear() {
        back_music?.Close();

        for (int i = 0;i<musics.Count;i++) {
            musics[i].Close();
        }
        back_music = null;
        musics.Clear();
    }
}
