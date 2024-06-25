using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EffectableObject : MonoBehaviourPunCallbacks, IInGameEffects
{
    private IDamageable damageable;
    public float Time_Burning { get { return timeBurning; } set { timeBurning = value; } }
    [SerializeField] private float timeBurning;
    public bool Burnable;
    public GameObject BurnParticles;
    public GameObject BurnParticlesOBJ;
    private float timeToDamage_Burning;
    public float Time_Blinded { get { return timeBlinded; } set { timeBlinded = value; } }
    [SerializeField] private float timeBlinded;
    public bool Blindable;
    // Start is called before the first frame update
    void Start()
    {
        damageable = GetComponent<IDamageable>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            Time_Burning -= Time.deltaTime;
            Time_Blinded -= Time.deltaTime;
            if(BurnParticlesOBJ != null)
            {
                photonView.RPC("SetBurnEnabled", RpcTarget.All, false);
            }
            if (Time_Burning > 0)
            {
                timeToDamage_Burning += Time.deltaTime;
                if (timeToDamage_Burning >= 1)
                {
                    timeToDamage_Burning -= 1;
                    damageable.Damage(5);
                }
            }
        }
    }

    public void InjectBurning(float time)
    {
        if (PhotonNetwork.OfflineMode)
        {
            InjectBurningRPC(time);
        }
        if(Burnable) photonView.RPC("InjectBurning", RpcTarget.All, time);
    }

    [PunRPC]
    public void InjectBurningRPC(float time)
    {
        if (Time_Burning < time && photonView.IsMine)
        {
            photonView.RPC("SetBurnEnabled", RpcTarget.All, true);
            Time_Burning = time;
            if (Time_Burning <= 0)
            {
                timeToDamage_Burning = 0;
            }
        }
    }

    [PunRPC]
    public void SetBurnEnabled(bool enabled)
    {
        if (enabled)
        {
            if(BurnParticlesOBJ == null)BurnParticlesOBJ = Instantiate(BurnParticles, this.transform);
        }
        else
        {
            Destroy(BurnParticlesOBJ);
        }
    }
}
