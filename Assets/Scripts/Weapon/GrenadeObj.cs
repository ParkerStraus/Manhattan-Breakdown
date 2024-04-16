using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GrenadeObj : IForceObject
{
    [SerializeField] private float FuseTime;
    [SerializeField] private float Damage;
    public AudioClip[] SFX;
    public GameObject particles;
    public float BlastRadius;
    private int rayAmount = 60;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PrepareGrenade(float Damage, float fuse, Vector2 throwSpeed, float SlowDownSpeed, float InertTime)
    {
        ForceInterp = SlowDownSpeed;
        SetInertia(InertTime);
        this.Damage = Damage;
        this.FuseTime = fuse;
        ApplyForce(throwSpeed);

    }


    // Update is called once per frame
    void Update()
    {
        FuseTime -= Time.deltaTime;
        if(FuseTime <= 0)
        {


            AudioSource.PlayClipAtPoint(SFX[(int)(Random.value * SFX.Length - 1)], transform.position);
            //Create Blast
            float raySegment = 360f / rayAmount;
            GetComponent<Collider2D>().enabled = false;
            for (int i = 0; i < rayAmount; i++)
            {
                Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, raySegment * i) * Vector2.left);
                RaycastHit2D ray = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, raySegment * i) * Vector2.left, BlastRadius);
                //Debug.Log(ray.collider.gameObject.name);
                if (ray.collider != null && ray.collider.gameObject.GetComponent<IDamageable>() != null)
                {
                    ray.collider.gameObject.GetComponent<IDamageable>().Damage(Damage);
                }
                //Apply force to push object away from explosion
                if (ray.collider != null && ray.collider.gameObject.GetComponent<IForceObject>() != null)
                {
                    ray.collider.gameObject.GetComponent<IForceObject>().ApplyForce((ray.collider.transform.position - transform.position));
                }
            }
            GameObject.Find("VirCam").GetComponent<VirCamStuff>().Shake(2f, 3f, 0.5f, 0.3f);
            GameObject exp = Instantiate(particles);
            exp.transform.position = transform.position;
            Destroy(gameObject);
        }
        base.Update();
    }
}
