using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : UIBaseController
{
    public Button close_btn;
    public Toggle music_toggle;
    public Toggle effect_toggle;
    public Slider music_slider;
    public Slider effect_slider;

    protected override void Init()
    {
        // 初始化音频数据
        AudioDataInit();

        close_btn.onClick.AddListener(() =>
        {
            UIManager.Instance.RemovePanel<SettingPanel>();
        });
        music_toggle.onValueChanged.AddListener((value) =>
        {
            AudioManager.Instance.SetMusicMute(value);
        });
        effect_toggle.onValueChanged.AddListener((value) =>
        {
            AudioManager.Instance.SetEffectMute(value);
        });
        music_slider.onValueChanged.AddListener((value) =>
        {
            AudioManager.Instance.SetMusicVolume(value);
        });
        effect_slider.onValueChanged.AddListener((value) =>
        {
            AudioManager.Instance.SetEffectVolume(value);
        });
    }

    private void AudioDataInit() {
        AudioData audioData = AudioManager.Instance.GetCurrentData();

        music_slider.value = audioData.music_volume;
        effect_slider.value = audioData.effect_volume;
        music_toggle.isOn = audioData.music_isMute;
        effect_toggle.isOn = audioData.effect_isMute;
    }
}
