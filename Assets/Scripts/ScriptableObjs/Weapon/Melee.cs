using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Melee")]
public class Melee : Weapon
{
    public override int UseWeapon(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player)
    {
        //Debug.Log(_firemode);
        //Swing Melee at enemies

        //Swing the thing like a bat
        //Debug.Log("Melee");
        if (Input.GetButtonDown("Fire1"))
        {
            Attacking = false;
            if (AttackTimer >= AttackRate)
            {
                AttackTimer = 0;
                Debug.Log("Swoop");
                ac.PlaySound(audio_gunshot);
                MeleeAtk(attackPoint, ac, player);
                Attacking = true;
            }
        }
        AttackTimer += Time.deltaTime;
        return 0;
    }

    private void MeleeAtk(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, 0.5f, ~(LayerMask.GetMask("Items")));

        foreach (Collider2D hit in hitEnemies)
        {
            if (hit.gameObject != player)
            {
                if (hit.gameObject.GetComponent<IDamageable>() != null)
                {
                    Debug.Log("Hit");
                    ac.PlaySound(audio_impact);
                    GameObject.Find("VirCam").GetComponent<VirCamStuff>().Shake(0.9f, 1.5f, 0.2f, 0f);
                    if (hit.gameObject.GetComponent<IDamageable>().Damage(damage))
                    {
                        KillConfirm(player);
                    }
                }

            }
        }
    }

    public override string GetAmmoString()
    {
        return "";
    }

    public override void Initialize()
    {
        
    }
}
