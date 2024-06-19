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
    // Start is called before the first frame update
    void Start()
    {
        DisplayName.text = PlayerPrefs.GetString("DisplayName");
    }

    public void SetMusic(float value)
    {
        mixer.SetFloat("MusicMaster", Mathf.Log10(Mathf.Lerp(0.001f,1,value)) * 20);
    }
    public void SetSound(float value)
    {
        mixer.SetFloat("SoundMaster", Mathf.Log10(Mathf.Lerp(0.001f, 1, value)) * 20);
    }

    

    public void Exit()
    {
        PlayerPrefs.SetString("DisplayName", DisplayName.text);
        Destroy(gameObject);
    }
}
