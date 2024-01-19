using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Animator animator;
    int Idle = Animator.StringToHash("Idle");
    int Walk = Animator.StringToHash("Walk");
    int Gun_Pistol = Animator.StringToHash("Pistol");
    int Gun_Rifle = Animator.StringToHash("Rifle");
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        playerData = player.GetPlayerData();
        if(playerData.poseType == PoseType.None )
        {
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
}
