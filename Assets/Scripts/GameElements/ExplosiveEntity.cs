using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ExplosiveEntity : MonoBehaviour, IDamageable
{
    [SerializeField] private float Health;
    [SerializeField] private float damage;
    [SerializeField] private float Force;
    public AudioClip[] explosion;
    public float BlastRadius;
    private int rayAmount = 60;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Damage(float damage)
    {
        Health -= damage;
        if(Health <= 0 ) OnExplosion(); 
        return false;
    }

    void OnExplosion()
    {
        AudioSource.PlayClipAtPoint(explosion[(int)(Random.value * explosion.Length-1)],transform.position);
        //Create Blast

        float raySegment = 360f / rayAmount;
        GetComponent<Collider2D>().enabled = false;
        for (int i = 0; i < rayAmount; i++)
        {
            Debug.DrawRay(transform.position, Quaternion.Euler(0,0,raySegment*i) * Vector2.left);
            RaycastHit2D ray = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, raySegment * i) * Vector2.left, BlastRadius);
            //Debug.Log(ray.collider.gameObject.name);
            if (ray.collider != null && ray.collider.gameObject.GetComponent<IDamageable>() != null)
            {
                ray.collider.gameObject.GetComponent<IDamageable>().Damage(damage);
            }
        }
        Destroy(gameObject);
    }
}
