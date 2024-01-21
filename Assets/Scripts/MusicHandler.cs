using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicHandler : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip musicStart;
    public AudioMixer mixer;

    // Start is called before the first frame update
    void Start()
    {
        if(musicStart != null) {
        musicSource.PlayOneShot(musicStart);
        musicSource.PlayScheduled(AudioSettings.dspTime + musicStart.length);
        }
        else
        {
            musicSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TriggerFilter(float FilterPoint, float MoveTime)
    {
        Debug.Log("Now Moving Filter to " + FilterPoint);
        StartCoroutine(MoveFilter(FilterPoint, MoveTime));
    }

    IEnumerator MoveFilter(float FilterPoint, float MoveTime)
    {
        float StartPoint = 0;
        mixer.GetFloat("Filter", out StartPoint);
        float Offset = FilterPoint - StartPoint;
        float TimeCurrent = 0;
        while (true)
        {
            TimeCurrent += Time.deltaTime;
            mixer.SetFloat("Filter", (TimeCurrent / MoveTime) * Offset + StartPoint);
            if (TimeCurrent < MoveTime)
            {
                yield return null;

            }
            else break;
        }
        mixer.SetFloat("Filter", FilterPoint);
    }
}
