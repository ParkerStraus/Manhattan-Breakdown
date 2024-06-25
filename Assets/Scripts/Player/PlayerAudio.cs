using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAudio : MonoBehaviourPunCallbacks
{
    public static PlayerAudio localInstance;
    public PhotonView PV;
    public AudioSource ac;
    public AudioClip[] footsteps;
    public Dictionary<string, AudioClip> GunShots;
    public AudioClip KillConfirm_snd;
    public AssetDictionary Sounds;
    private AudioLowPassFilter lowPassFilter;
    public LayerMask wallLayer;
    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine) localInstance = this;
        ac = GetComponent<AudioSource>();
        Sounds = Resources.Load<AssetDictionary>("PlayerSounds");
        PV = GetComponent<PhotonView>();

        lowPassFilter = GetComponent<AudioLowPassFilter>();
        if (lowPassFilter == null)
        {
            lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
        }
        lowPassFilter.cutoffFrequency = 22000;
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            print("Now trying to be blocked");
            // Perform the raycast
            Vector2 direction = (localInstance.gameObject.transform.position - this.transform.position).normalized;
            float distance = Vector2.Distance(this.transform.position, localInstance.gameObject.transform.position);

            RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, direction, distance, wallLayer);

            // Check if any of the hits are a wall
            bool isBlockedByWall = false;
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("NonPermeable"))
                {
                    isBlockedByWall = true;
                    print("Blocked by a wall");
                    break;
                }
            }

            // Set the low pass filter based on whether the path is blocked by a wall
            if (isBlockedByWall)
            {
                lowPassFilter.cutoffFrequency = 2200; // Apply low-pass filter
            }
            else
            {
                lowPassFilter.cutoffFrequency = 22000; // Remove low-pass filter
            }
        }
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
