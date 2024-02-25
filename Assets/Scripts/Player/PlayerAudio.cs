using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource ac;
    public AudioClip[] footsteps;
    public AudioClip KillConfirm_snd;
    // Start is called before the first frame update
    void Start()
    {
        ac = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(AudioClip sound)
    {
        //Debug.Log("Now Playing sound");
        ac.PlayOneShot(sound);
    }

    public void Footstep()
    {

        PlaySound(footsteps[Random.Range(0,(int)footsteps.Length-1)]);
    }

    public void KillConfirmed()
    {
        PlaySound(KillConfirm_snd);
    }
}
