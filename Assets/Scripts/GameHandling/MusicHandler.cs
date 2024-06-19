using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicHandler : MonoBehaviour
{
    public bool StartMusicNow;
    public bool ResetFilters;
    public AudioSource musicSource;
    public AudioClip musicStart;
    public AudioMixer mixer;
    public MusicSelector musicSelector;

    public AudioMixerSnapshot Base;
    public AudioMixerSnapshot MuffledMusic;

    [Header("UI Elements")]
    public Animator musicInfoUiAnimator;
    public Image AlbumArt;
    public TMP_Text Title;
    public TMP_Text Artist;


    // Start is called before the first frame update
    void Start()
    {
        mixer.SetFloat("MusicMaster", Mathf.Log10(Mathf.Lerp(0.001f, 1, PlayerPrefs.GetFloat("Set_Music", 1))));
        mixer.SetFloat("SoundMaster", Mathf.Log10(Mathf.Lerp(0.001f, 1, PlayerPrefs.GetFloat("Set_Sound", 1))));
        musicSelector = Resources.Load<MusicSelector>("MusicList");
        if (StartMusicNow)
        {
            StartMusic();
        }
        if (ResetFilters) TriggerSnapshot(0, 0.0f);
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

    public void PlayRandomSong()
    {
        MusicFile music = musicSelector.GetRandomMusic();
        QueueNewSong(music.getIntro(), music.getLoop());
        AlbumArt.sprite = music.GetImage();
        Title.text = music.GetTitle();
        Artist.text = music.GetArtist();
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

    public void ShowSongInfo()
    {
        musicInfoUiAnimator.SetTrigger("Show");
    }

    public void TriggerFilter(float FilterPoint, float MoveTime)
    {
        Debug.Log("Now Moving Filter to " + FilterPoint);
        StartCoroutine(MoveFilter(FilterPoint, MoveTime));
    }

    public void TriggerSnapshot(int filterType, float TimeInterp)
    {
        switch(filterType)
        {
            case 0:
                //base
                Debug.Log("Now set to normal");
                Base.TransitionTo(TimeInterp);
                break;
            case 1:
                //base
                Debug.Log("Now set to muffled");
                MuffledMusic.TransitionTo(TimeInterp);
                break;
        }
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
