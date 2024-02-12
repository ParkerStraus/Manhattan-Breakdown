using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAudio audio;
    int Idle = Animator.StringToHash("Idle");
    int Walk = Animator.StringToHash("Walk");
    int Gun_Pistol = Animator.StringToHash("Pistol");
    int Gun_Rifle = Animator.StringToHash("Rifle");

    int PunchL = Animator.StringToHash("Punch_Left");
    int PunchR = Animator.StringToHash("Punch_Right");
    [SerializeField] private bool flipped = false;

    private float _lockedTill;

    private float FootStepTimer = 0;
    [SerializeField] private float FootStepThreshold;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        audio = GetComponent<PlayerAudio>();
    }

    // Update is called once per frame
    void Update()
    {

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
        playerData = player.GetPlayerData();
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
                animator.CrossFade(Walk, 0, 0);
            }
            else animator.CrossFade(Idle, 0, 0);
        }
        else
        {
            switch(playerData.poseType)
            {
                case PoseType.Pistol:
                    animator.CrossFade(Gun_Pistol, 0, 0);
                    break;
                case PoseType.Rifle:
                    animator.CrossFade(Gun_Rifle, 0, 0);
                    break;
                default:
                    animator.CrossFade(Idle, 0, 0);
                    break;
            }
        }
    }

    void LockState(int s, float t)
    {
        _lockedTill = Time.time + t;
        animator.CrossFade(s, 0, 0);
        return;
    }
}
