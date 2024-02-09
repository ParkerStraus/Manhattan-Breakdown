using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicList", menuName = "Music List")]
public class MusicSelector : ScriptableObject
{
    [SerializeField] private MusicFile[] musicInstances;
    // Start is called before the first frame update
    public MusicFile GetRandomMusic()
    {
        return musicInstances[(int)(Random.Range(0f, musicInstances.Length))];
    }
}

[System.Serializable]
public class MusicFile
{
    [SerializeField] private AudioClip music_intro;
    [SerializeField] private AudioClip music_loop;
    [SerializeField] private string Title;
    [SerializeField] private string Artist;
    [SerializeField] private Sprite AlbumArt;
    public MusicFile(AudioClip music_intro, AudioClip music_loop)
    {
        this.music_intro = music_intro;
        this.music_loop = music_loop;
    }

    public AudioClip getIntro()
    {
        return this.music_intro;
    }

    public AudioClip getLoop()
    {
        return this.music_loop;
    }

    public string GetTitle()
    {
        return Title;
    }

    public string GetArtist()
    {
        return Artist;
    }

    public Sprite GetImage()
    {
        return AlbumArt;
    }
}
