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


    public void PlaySoundClient_AudioClip(AudioClip sound)
    {
        ac.pitch = 1;
        ac.PlayOneShot(sound);
    }

    public void PlaySoundClient(int soundIndex)
    {
        ac.pitch = 1;
        AudioClip sound = (AudioClip)Sounds.GetAsset(soundIndex);
        ac.PlayOneShot(sound);
    }

    public void PlaySoundClient(int soundIndex, float pitchRange)
    {
        ac.pitch = 1 + Random.Range(-pitchRange, pitchRange);
        AudioClip sound = (AudioClip)Sounds.GetAsset(soundIndex);
        ac.PlayOneShot(sound);
    }

    public void PlaySound(int soundIndex)
    {
        ac.pitch = 1;
        AudioClip sound = (AudioClip)Sounds.GetAsset(soundIndex);
        ac.PlayOneShot(sound);
        PV.RPC("PlaySoundRPC", RpcTarget.Others, new object[] { soundIndex, ac.pitch });
    }

    public void PlaySound(int soundIndex, float pitchRange)
    {
        ac.pitch = 1 + Random.Range(-pitchRange, pitchRange);
        AudioClip sound = (AudioClip)Sounds.GetAsset(soundIndex);
        ac.PlayOneShot(sound);
        PV.RPC("PlaySoundRPC", RpcTarget.Others, new object[] { soundIndex, ac.pitch });
    }

    public void PlaySound_Disconnected(int soundIndex)
    {
        AudioSource.PlayClipAtPoint((AudioClip)Sounds.GetAsset(soundIndex), transform.position);
        PV.RPC("PlaySound_DisconnectedRPC", RpcTarget.Others, soundIndex);
    }

    [PunRPC]
    public void PlaySound_DisconnectedRPC(int soundIndex)
    {
        AudioSource.PlayClipAtPoint((AudioClip)Sounds.GetAsset(soundIndex), transform.position);
    }

    [PunRPC]
    public void PlaySoundRPC(int soundId, float pitch)
    {
        Debug.Log("Now Playing sound remotely");
        ac.pitch = pitch;
        ac.PlayOneShot((AudioClip)Sounds.GetAsset(soundId));
    }

    public void Footstep()
    {

        PlaySoundClient_AudioClip(footsteps[Random.Range(0,(int)footsteps.Length-1)]);
    }
}
