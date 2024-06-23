using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using Photon.Pun;

public class ExplosiveEntity : GameElement, IDamageable
{
    [SerializeField] private float Health;
    [SerializeField] private float damage;
    [SerializeField] private float Force;
    public AudioClip[] explosion;
    public GameObject particles;
    public GameObject impactEffect;
    public float BlastRadius;
    private int rayAmount = 60;
    private PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Damage(float damage)
    {
        Health -= damage;
        if(Health <= 0 && PhotonNetwork.IsMasterClient) OnExplosion();
        PV.RPC("SetHealth", RpcTarget.All, Health);
        return false;
    }


    [PunRPC]
    public void SetHealth(float Health)
    {
        this.Health = Health;
    }

    void OnExplosion()
    {
        //Create Blast
        PV.RPC("ParticleEffects", RpcTarget.All);
        float raySegment = 360f / rayAmount;
        GetComponent<Collider2D>().enabled = false;
        for (int i = 0; i < rayAmount; i++)
        {
            Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, raySegment * i) * Vector2.left);
            RaycastHit2D ray = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, raySegment * i) * Vector2.left, BlastRadius);
            //Debug.Log(ray.collider.gameObject.name);
            if (ray.collider != null && ray.collider.gameObject.GetComponent<IDamageable>() != null)
            {
                ray.collider.gameObject.GetComponent<IDamageable>().Damage(damage);
            }
            //Apply force to push object away from explosion
            if (ray.collider != null && ray.collider.gameObject.GetComponent<IForceObject>() != null)
            {
                ray.collider.gameObject.GetComponent<IForceObject>().ApplyForce((ray.collider.transform.position - transform.position));
            }
        }
        PV.RPC("Destroy", RpcTarget.All);
    }

    [PunRPC]
    public void ParticleEffects()
    {

        AudioSource.PlayClipAtPoint(explosion[(int)(Random.value * explosion.Length - 1)], transform.position);
        GameObject exp = Instantiate(particles);
        exp.transform.position = transform.position;
        GameObject.Find("VirCam").GetComponent<VirCamStuff>().Shake(2f, 3f, 0.5f, 0.3f);
    }
    [PunRPC]
    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public GameObject GetImpactEffect()
    {
        return impactEffect;
    }

    public bool GetTangible()
    {
        return true;
    }
}
