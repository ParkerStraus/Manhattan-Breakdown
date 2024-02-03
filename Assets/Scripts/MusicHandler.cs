using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicHandler : MonoBehaviour
{
    public bool StartMusicNow;
    public AudioSource musicSource;
    public AudioClip musicStart;
    public AudioMixer mixer;

    // Start is called before the first frame update
    void Start()
    {
        if (StartMusicNow)
        {
            StartMusic();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartMusic()
    {
        if (musicStart != null)
        {
            musicSource.PlayOneShot(musicStart);
            musicSource.PlayScheduled(AudioSettings.dspTime + musicStart.length);
        }
        else
        {
            musicSource.Play();
        }
    }

    public void QueueNewSong(AudioClip songStart, AudioClip songLoop)
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();

        }

        musicSource.clip = songLoop;
        if (songStart != null)
        {
            musicSource.PlayOneShot(songStart);
            musicSource.PlayScheduled(AudioSettings.dspTime + songStart.length);
        }
        else
        {
            musicSource.Play();
        }
    }

    public void TriggerFilter(float FilterPoint, float MoveTime)
    {
        Debug.Log("Now Moving Filter to " + FilterPoint);
        StartCoroutine(MoveFilter(FilterPoint, MoveTime));
    }


    public void TriggerFilter(float[] param)
    {
        Debug.Log("Now Moving Filter to " + param[0]);
        StartCoroutine(MoveFilter(param[0], param[1]));
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
