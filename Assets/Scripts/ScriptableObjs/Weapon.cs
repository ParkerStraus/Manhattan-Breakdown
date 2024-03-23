using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using static UnityEngine.RuleTile.TilingRuleOutput;

public enum PoseType
{
    None,
    Pistol,
    Rifle,
    Melee,
    TwoMelee
}

public interface IWeapon
{
    void Initialize();
    int UseWeapon(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player);
    // Other common methods...
}

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Weapon Object")]
public abstract class Weapon : ScriptableObject, IWeapon
{

    public PoseType _poseType;


    public string Name;
    public float damage;
    public float AttackTimer;
    public float AttackRate;


    [Header("Weapon Info")]
    public float AttackSize;



    [Header("Aesthetic stuff here")]
    public PlayerAudio ac;
    public Sprite floorImage;
    public Sprite HeldImage;
    public AudioClip audio_gunshot;
    public AudioClip audio_click;
    public AudioClip audio_impact;
    public GameObject Impact;
    public EffectsManager em;

    public abstract void Initialize();

    public abstract int UseWeapon(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player);

    public void KillConfirm(GameObject player)
    {
        player.SendMessage("KillConfirmed");
    }

    public void CreateParticle(GameObject particles, RaycastHit2D point)
    {
        Debug.Log("Now Shooting blud");
        Instantiate(particles, point.point, Quaternion.LookRotation(point.normal, Vector3.left));
    }

    public string GetName()
    {
        return Name;
    }

    public abstract string GetAmmo();

    public Sprite GetWeaponSprite()
    {
        return floorImage;
    }

    public Sprite GetWeaponSprite_Held()
    {
        return HeldImage;
    }

    public void PlaySound(AudioClip clip)
    {

        ac.PlaySound(clip, 0.2f);
    }
}

