using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public AudioMixer mixer;
    public TMP_InputField DisplayName;
    public Scrollbar MusicBar;
    public Scrollbar SoundBar;
    // Start is called before the first frame update
    void Start()
    {
        DisplayName.text = PlayerPrefs.GetString("DisplayName", "Player");
        MusicBar.value = PlayerPrefs.GetFloat("Set_Music", 1);
        SoundBar.value = PlayerPrefs.GetFloat("Set_Sound", 1);
    }

    public void SetMusic(float value)
    {
        PlayerPrefs.SetFloat("Set_Music", value);
        mixer.SetFloat("MusicMaster", Mathf.Log10(Mathf.Lerp(0.001f,1,value)) * 20);
        print(value);
    }
    public void SetSound(float value)
    {
        PlayerPrefs.SetFloat("Set_Sound", value);
        mixer.SetFloat("SoundMaster", Mathf.Log10(Mathf.Lerp(0.001f, 1, value)) * 20);
        print(value);
    }

    

    public void Exit()
    {
        PlayerPrefs.SetString("DisplayName", DisplayName.text);
        Destroy(gameObject);
    }
}
