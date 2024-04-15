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
    public Dictionary<string, AudioClip> GunShots;
    public AudioClip KillConfirm_snd;
    public AssetDictionary Sounds;
    // Start is called before the first frame update
    void Start()
    {
        ac = GetComponent<AudioSource>();
        Sounds = Resources.Load<AssetDictionary>("PlayerSounds");
        PV = GetComponent<PhotonView>();
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
        int i = Sounds.GetIndex(sound);
        //PV.RPC("PlaySoundRPC", RpcTarget.Others, new object[]{ i, 1 });
    }


    public void PlaySound(AudioClip sound, float pitchRange)
    {
        //Debug.Log("Now Playing sound");
        ac.pitch = 1 + Random.Range(-pitchRange, pitchRange);
        ac.PlayOneShot(sound);
        int i = Sounds.GetIndex(sound);
        //PV.RPC("PlaySoundRPC", RpcTarget.Others, new object[] { i, ac.pitch });
    }

    [PunRPC]
    public void PlaySoundRPC(int soundId, float pitch)
    {
        Debug.Log("Now Playing sound remotely");
        ac.pitch = pitch;
        ac.PlayOneShot((AudioClip)Sounds.GetValue(soundId));
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
