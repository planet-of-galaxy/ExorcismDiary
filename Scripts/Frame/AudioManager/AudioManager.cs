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
    public const float GRADUALLY_MIN_VOLUME = 0.01f; // �������С���� �������������ͻ��Զ�ֹͣ
    private AudioSource back_music;
    private List<PlayingMusic> musics = new();
    /// <summary>
    ///  �ڲ��� Ϊ�˾�ȷ�������ֵĲ���״̬
    ///  audio_source�� ��ƵԴ
    ///  coroutine����������Ҫ����ʱ��ʹ��Э�������������ı仯
    ///  callBack: �������ʱ�Ļص�����
    /// </summary>
    private class PlayingMusic {
        public AudioSource audio_source;
        public Coroutine coroutine;
        public Action<PlayingMusic> callBack;
        public E_PlayState play_state; // ����״̬

        // ��ʼ������ Э�̺ͻص�Ĭ��Ϊ��
        public PlayingMusic(AudioSource audio_source) {
            this.audio_source = audio_source;
        }

        // �ṩ���� ���������� ��ɺ���ûص�����
        public void GraduallyUpper(Action<PlayingMusic> callBack) {
            // ������ڽ����������� ��ôȡ����
            if (play_state == E_PlayState.E_UPPER_GRADUALLY || play_state == E_PlayState.E_LOWER_GRADUALLY)
                CancelCoroutine();

            // ���õ�ǰ״̬Ϊ����
            play_state = E_PlayState.E_UPPER_GRADUALLY;
            this.callBack = callBack;
            // ����Э�� �����ֽ��䵽���
            coroutine = MonoMgr.Instance.ChangeFloatGradually(audio_source.volume, AudioManager.Instance.GetCurrentData().music_volume, SetUpperVolume);
        }

        // �ṩ���� �𽥼�С���� ��ɺ���ûص�����
        public void GraduallyLower(Action<PlayingMusic> callBack)
        {
            // ������ڽ����������� ��ôȡ����
            if (play_state == E_PlayState.E_UPPER_GRADUALLY || play_state == E_PlayState.E_LOWER_GRADUALLY)
                CancelCoroutine();

            play_state = E_PlayState.E_LOWER_GRADUALLY;
            this.callBack = callBack;
            // ����Э�� �����ֽ��䵽0
            coroutine = MonoMgr.Instance.ChangeFloatGradually(audio_source.volume, 0, SetLowerVolume);
        }

        // �ṩ���� ȡ������
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

            // �����������ﵽĿ������ʱ��ֹͣ����
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

            // �����������ﵽ0ʱ��ֹͣ����
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
                // �Ȱ�������Ϊ0 Ϊ����������׼��
                audio_source.volume = 0;
                // ��ʹisMuteΪtrue��Ҳ����ȡ��������ȷ��������ͻȻ��ʱ��������
                audio_source.mute = current_audiodata.music_isMute;
                // ����һ����������������
                PlayingMusic playing_music = new PlayingMusic(audio_source);
                // ��ӵ������б���
                musics.Add(playing_music);
                // ���ý������� �����뽥����ɵĻص�����
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
        // �᲻�����˰ѿմ�������������ô�����ȷ�һ��
        if (audio_source == null)
            return;

        // ����Ǳ������ֵĻ���ֹͣ��������
        if (audio_source == back_music) {
            CloseBackMusic();
            return;
        }

        Debug.Log("������������");
        // ������������ֵĻ�������ֹͣ����
        for (int i = 0; i < musics.Count; i++)
        {
            if (musics[i].audio_source == audio_source)
            {
                // �ҵ���Ӧaudio_source
                Debug.Log("�ҵ���Ӧaudio_source");
                musics[i].GraduallyLower(MusicGraduallyCallBack);
                return;
            }
        }
    }

    // ���������ص� �����ֽ��䵽0ʱ �ûص���ɾ����Ӧ����
    private void MusicGraduallyCallBack(PlayingMusic music) {
        if (music.play_state == E_PlayState.E_STOP)
            RemovePlayingMusic(music);

        // ����ǽ�����ɣ���ô������� PlayingMusic�ڲ��Ѿ���ɴ���
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
