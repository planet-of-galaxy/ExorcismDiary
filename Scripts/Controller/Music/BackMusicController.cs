using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackMusicController : MonoBehaviour
{
    public AudioSource back_music;
    // Start is called before the first frame update
    void Start()
    {
        if (back_music == null)
            back_music = GetComponent<AudioSource>();

        AudioManager.Instance.PlaySafely(back_music, E_AudioType.E_BACK_MUSIC);
    }
}
