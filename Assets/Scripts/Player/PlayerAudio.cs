using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAudio : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
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
        ac.pitch = 1;
        ac.PlayOneShot(sound);
    }


    public void PlaySound(AudioClip sound, float pitchRange)
    {
        //Debug.Log("Now Playing sound");
        ac.pitch = 1 + Random.Range(-pitchRange, pitchRange);
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
