using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using Photon.Pun;

public class PlayerAnimation : MonoBehaviourPunCallbacks
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAudio audio;
    private PhotonView PV;
    int Idle = Animator.StringToHash("Idle");
    int Walk = Animator.StringToHash("Walk");
    int Gun_Pistol = Animator.StringToHash("Pistol");
    int Gun_Rifle = Animator.StringToHash("Rifle");

    int PunchL = Animator.StringToHash("Punch_Left");
    int PunchR = Animator.StringToHash("Punch_Right");


    int MeleeL = Animator.StringToHash("MeleeL");
    int MeleeR = Animator.StringToHash("MeleeR");
    int MeleeLIdle = Animator.StringToHash("MeleeL_Idle");
    int MeleeRIdle = Animator.StringToHash("MeleeR_Idle");


    [SerializeField] private bool flipped = false;

    [SerializeField] private float _lockedTill;

    [SerializeField] private float FootStepTimer = 0;
    [SerializeField] private float FootStepThreshold;

    [SerializeField] private bool Dead = false;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Sprite[] deathSprites;
    [SerializeField] private int SpriteOffset;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        audio = GetComponent<PlayerAudio>();
        _sprite = GetComponent<SpriteRenderer>();
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        playerData = player.GetPlayerData();
        //Debug.Log(playerData.poseType);
        if ((Dead||!PV.IsMine) && !playerData.offline) return;

        if (playerData.Moving == true)
        {
            FootStepTimer += Time.deltaTime;
            if (FootStepTimer >= FootStepThreshold)
            {
                audio.Footstep();
                FootStepTimer = 0;
            }
        }
        else
        {
            FootStepTimer = 0;
        }
        if (Time.time < _lockedTill) return;
        if(playerData.poseType == PoseType.None )
        {
            if (playerData.attacking == true)
            {
                if (flipped == true)
                    LockState(PunchL, 0.26666666666f);
                else LockState(PunchR, 0.26666666666f);
                flipped = !flipped;
            }
            if (playerData.Moving)
            {
                AnimateCrossfade(Walk);
            }
            else AnimateCrossfade(Idle);
        }
        else
        {
            switch(playerData.poseType)
            {
                case PoseType.Pistol:
                    AnimateCrossfade(Gun_Pistol);
                    break;
                case PoseType.Rifle:
                    AnimateCrossfade(Gun_Rifle);
                    break;
                case PoseType.TwoMelee:

                    if (playerData.attacking == true)
                    {
                        if (flipped == true)
                            LockState(MeleeL, 0.25f);
                        else LockState(MeleeR, 0.25f);
                        flipped = !flipped;
                    }
                    else
                    {

                        if (flipped == true)
                            AnimateCrossfade(MeleeLIdle);
                        else AnimateCrossfade(MeleeRIdle);
                    }
                    break;

                default:
                    AnimateCrossfade(Idle);
                    break;
            }
        }
    }

    void AnimateCrossfade(int anim)
    {
        animator.CrossFade(anim, 0, 0);
        PV.RPC("AnimateCrossfadeRPC", RpcTarget.Others, anim);
    }


    [PunRPC]
    void AnimateCrossfadeRPC(int animate)
    {
        try
        {
            animator.CrossFade(animate, 0, 0);
        }
        catch(UnassignedReferenceException e)
        {
            print("Animator not initialized yet");
        }
    }

    void LockState(int s, float t)
    {
        _lockedTill = Time.time + t;
        AnimateCrossfadeRPC(s);
        PV.RPC("LockStateRPC", RpcTarget.Others, new object[] { s, t });
    }

    [PunRPC]
    void LockStateRPC(int s, float t)
    {
        _lockedTill = Time.time + t;
        animator.CrossFade(s, 0, 0);
        AnimateCrossfadeRPC(s);
    }


    public void OnDeath()
    {
        animator.enabled = false;
        Dead = true;
        //set random sprite

        int spriteIndex = (int)Random.Range(0, deathSprites.Length);

        _sprite.sprite = deathSprites[spriteIndex];

        PV.RPC("OnDeathRPC", RpcTarget.All, spriteIndex);
    }

    [PunRPC]
    public void OnDeathRPC(int spr)
    {
        animator.enabled = false;
        Dead = true;
        _sprite.sprite = deathSprites[spr];
    }
}
