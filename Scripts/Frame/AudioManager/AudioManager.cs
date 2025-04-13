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
    E_LOADING,
}
public class AudioManager : Singleton<AudioManager>
{
    public AudioData current_audiodata;
    public const float GRADUALLY_MIN_VOLUME = 0.01f; // �������С���� �������������ͻ��Զ�ֹͣ
    private Music back_music;
    private GameObject back_music_gameObject; // �������ָ��ŵ���Ϸ����
    private List<Music> musics = new(); // ���ָ��ŵ���Ϸ����
    private GameObject music_gameObject;
    /// <summary>
    ///  �ڲ��� Ϊ�˾�ȷ�������ֵĲ���״̬
    ///  audio_source�� ��ƵԴ
    ///  coroutine����������Ҫ����ʱ��ʹ��Э�������������ı仯
    ///  callBack: �������ʱ�Ļص�����
    /// </summary>
    private class Music {
        public AudioSource audio_source; // ��Դ
        public Coroutine coroutine;
        public Action<Music> callBack; // Э�̶�Ӧ�Ļص�
        public E_PlayState play_state; // ����״̬
        public E_AudioType audio_type; // ��Ƶ����
        public GameObject gameObject; // ���ŵ���Ϸ����
        public string name; // ��Ƶ����
        public int ID { get; }

        // ��ʼ������ Э�̺ͻص�Ĭ��Ϊ��
        public Music(AudioSource audio_source, E_AudioType audio_type)
        {
            this.audio_source = audio_source;
            this.audio_type = audio_type;
        }
        public Music(GameObject gameObject, string name, AudioSource audio_source, E_AudioType audio_type) { 
            this.gameObject = gameObject;
            this.name = name;
            this.audio_source = audio_source;
            this.audio_type = audio_type;
        }

        // �ṩ���� ���������� ��ɺ���ûص�����
        public void GraduallyUpper(Action<Music> callBack) {
            // ������ڽ����������� ��ôȡ����
            if (play_state == E_PlayState.E_UPPER_GRADUALLY || play_state == E_PlayState.E_LOWER_GRADUALLY)
                CancelCoroutine();

            // ���õ�ǰ״̬Ϊ����
            play_state = E_PlayState.E_UPPER_GRADUALLY;
            this.callBack = callBack;
            audio_source.volume = 0;
            audio_source.Play();
            // ����Э�� �����ֽ��䵽���
            coroutine = MonoMgr.Instance.ChangeFloatGradually(audio_source.volume, AudioManager.Instance.GetCurrentData().music_volume, SetUpperVolume);
        }

        // �ṩ���� �𽥼�С���� ��ɺ���ûص�����
        public void GraduallyLower(Action<Music> callBack)
        {
            // ���AudioSource�Ѿ���ϵͳ���� ��ô��������Ҳ����
            if (audio_source == null)
                Destroy();
            // ������ڽ����������� ��ôȡ����
            if (play_state == E_PlayState.E_UPPER_GRADUALLY || play_state == E_PlayState.E_LOWER_GRADUALLY)
                CancelCoroutine();

            play_state = E_PlayState.E_LOWER_GRADUALLY;
            this.callBack = callBack;
            // ����Э�� �����ֽ��䵽0
            coroutine = MonoMgr.Instance.ChangeFloatGradually(audio_source.volume, 0, SetLowerVolume);
        }
        public void LoadClipAsync(Action<Music> callBack) {
            play_state = E_PlayState.E_LOADING;
            this.callBack = callBack;
            ABMgr.Instance.LoadResAsync<AudioClip>("music", name, LoadClipCallback);
        }
        public void LoadClip() {
            audio_source.clip = ABMgr.Instance.LoadRes<AudioClip>("music", name);
        }
        public void LoadClipCallback(AudioClip clip) {
            play_state = E_PlayState.E_STOP;
            audio_source.clip = clip;
            audio_source.clip.LoadAudioData();
            callBack?.Invoke(this);
        }
        public void Play() {
            if (audio_source != null && audio_source.clip != null)
                audio_source.Play();

            play_state = E_PlayState.E_PLAYING;
        }
        public void Pause() {
            play_state = E_PlayState.E_PAUSE;
            CancelCoroutine();
            audio_source.Pause();
        }
        public void Close() {
            play_state = E_PlayState.E_STOP;
            audio_source?.Stop();
            CancelCoroutine();
        }

        // �ṩ���� ȡ������
        private void CancelCoroutine() {
            if (coroutine != null)
            {
                MonoMgr.Instance.StopCoroutine(coroutine);
                coroutine = null;
            }
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
                audio_source.Stop();
                play_state = E_PlayState.E_STOP;
                callBack?.Invoke(this);
            }
        }

        public void Destroy() {
            Close();
            GameObject.Destroy(audio_source);
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
                // ��ʼ������ ����Ϊ�˽���ȥ�������0 ���������ڲ����Լ���
                audio_source.volume = current_audiodata.music_volume;
                // ��ʹisMuteΪtrue��Ҳ����ȡ��������ȷ��������ͻȻ��ʱ��������
                audio_source.mute = current_audiodata.music_isMute;
                // ����������б����������±�������ͬ ��ô�ȹر�֮ǰ�ı����� �������±�����
                if (back_music != null && back_music.audio_source != audio_source)
                {
                    back_music.Close();
                    back_music = new Music(audio_source, audio_type);
                }
                // ���ԭ��û�б����� ����һ���±�����
                else if (back_music == null) {
                    back_music = new Music(audio_source, audio_type);
                }
                // �����ﹲ��3�����
                // 1. ԭ��û�б����� �����Ѿ��������±�����
                // 2. ԭ���б����� �����Ѿ��滻�����±�����
                // 3. ����ԭ�б���������
                // �������û������E_MUSIC���͵������ڲ��� ��ô��ʼ������������
                // �������ôֱ�ӷ��� ���Ȳ�����
                if (IsAnyMusicPlaying())
                    return;

                // ���ý������� �����뽥����ɵĻص�����
                back_music.GraduallyUpper(MusicGraduallyUpperCallBack);
                break;
            case E_AudioType.E_MUSIC:
                Music music;
                // ��ʼ������ ����Ϊ�˽���ȥ�������0 ���������ڲ����Լ���
                audio_source.volume = current_audiodata.music_volume;
                // ��ʹisMuteΪtrue��Ҳ����ȡ��������ȷ��������ͻȻ��ʱ��������
                audio_source.mute = current_audiodata.music_isMute;
                // ���û�м�¼��ͬ����audio_source ��ô��ȥ����һ���µ�
                if (!HasMusic(audio_source))
                {
                    // ����һ���������������� ����ӵ������б���
                    music = new Music(audio_source, audio_type);
                    // ��ӵ������б���
                    musics.Add(music);
                }
                else {
                    // ��ȡ���е�music
                    music = GetMusic(audio_source);
                    // ����Ѿ����ڲ��� �����Ѿ��ڽ����� ��ô��ֱ�ӷ���
                    if (music.play_state == E_PlayState.E_PLAYING ||
                        music.play_state == E_PlayState.E_UPPER_GRADUALLY)
                        return;
                }

                // ������ڲ��ű������� ��ֹͣ����
                if (back_music != null &&
                    (back_music.play_state == E_PlayState.E_UPPER_GRADUALLY ||
                     back_music.play_state == E_PlayState.E_LOWER_GRADUALLY ||
                     back_music.play_state == E_PlayState.E_PLAYING)) {
                    back_music.Close();
                }
                // ���ý������� �����뽥����ɵĻص�����
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
        // �᲻�����˰ѿմ�������������ô�����ȷ�һ��
        if (audio_source == null)
            return;

        Music music = null;
        // ����Ǳ������ֵĻ� ��ֵΪ��������
        if (back_music != null && audio_source == back_music.audio_source) {
            music = back_music;
        }
        // ������������ֵĻ� ��ֵΪ��������
        if (GetMusic(audio_source) is Music exis_music) {
            music = exis_music;
        }

        // ������ֹͣ
        music?.GraduallyLower(MusicGraduallyLowerCallBack);
    }
    public void StopSafely(string name)
    {
        Music music = null;
        // ����Ǳ������ֵĻ� ��ֵΪ��������
        if (back_music != null && back_music.name == name)
        {
            music = back_music;
        }
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
                back_music?.GraduallyUpper(MusicGraduallyUpperCallBack);
                break;
        }
    }
    private void MusicGraduallyUpperCallBack(Music music)
    {
        // ������ɻص����� Ŀǰû��ʲô�ô����
    }
    private void MusicGraduallyCloseCallBack(Music music) {
        switch (music.audio_type)
        {
            case E_AudioType.E_BACK_MUSIC:
                // �ÿձ�����
                back_music = null;
                break;
            case E_AudioType.E_MUSIC:
                // �Ƴ��Ѿ��رյ�����
                RemoveMusic(music);
                // ����б����� ��ʼ���²��ű�����
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
    private Music GetMusic(AudioSource audio_source) {
        Music music = null;
        for (int i = 0; i < musics.Count; i++)
        {
            if (musics[i].audio_source == audio_source) { music = musics[i]; break; }
        }
        return music;
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

    public void CloseBackMusic()
    {
        if (back_music != null)
        {
            back_music.GraduallyLower(MusicGraduallyCloseCallBack);
        }
    }
    // �������� �ÿձ������������б�
    public void Clear() {
        back_music?.Close();

        for (int i = 0; i < musics.Count; i++) {
            musics[i].Close();
        }
        back_music = null;
        musics.Clear();
    }

    public void PlaySafely(string name, E_AudioType audio_type) {
        AudioSource audio_source;
        Music music;
        switch (audio_type) {
            case E_AudioType.E_BACK_MUSIC:
                if (back_music_gameObject == null) {
                    back_music_gameObject = new GameObject("BackMusic");
                }
                // ����������Ѿ����� ������������������ ��ôֱ�Ӳ���
                if (back_music != null && back_music.name == name)
                {
                    audio_source = back_music.audio_source;
                    if (IsAnyMusicPlaying())
                    {
                        Debug.Log("���������ڲ��� �޷����ű�����");
                        return;
                    }
                    back_music.GraduallyUpper(MusicGraduallyUpperCallBack);
                    return;
                }
                // ����������Ѿ����� �������ֲ�һ�� ��ô���֮ǰ�ı����� �ټ����µı�����
                else if (back_music != null)
                {
                    if (back_music.name == null || back_music.name != name)
                    {
                        back_music.Destroy();
                        back_music = null;
                    }
                    audio_source = back_music_gameObject.AddComponent<AudioSource>();
                    music = new Music(back_music_gameObject, name, audio_source, audio_type);
                    music.LoadClipAsync(LoadClipCallBack);
                    back_music = music;
                    return;
                }
                else {
                    audio_source = back_music_gameObject.AddComponent<AudioSource>();
                    music = new Music(back_music_gameObject, name, audio_source, audio_type);
                    music.LoadClipAsync(LoadClipCallBack);
                    back_music = music;
                    return;
                }
            case E_AudioType.E_MUSIC:
                if (music_gameObject == null)
                {
                    music_gameObject = new GameObject("Music");
                }
                if (GetMusic(name) is Music current_music) {
                    if (back_music != null) {
                        back_music.Close();
                    }
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
                if (back_music != null && (back_music.play_state == E_PlayState.E_PLAYING ||
                    back_music.play_state == E_PlayState.E_LOWER_GRADUALLY)) {
                    back_music.Close();
                }
                music.GraduallyUpper(MusicGraduallyUpperCallBack);
                break;
        }
    }
}
