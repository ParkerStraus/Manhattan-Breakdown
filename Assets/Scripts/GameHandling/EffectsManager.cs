using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;

public class EffectsManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public TrailRenderer TrailRenderer;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator BulletTrailRoutine(TrailRenderer trail, RaycastHit2D hit)
    {
        float time = 0;
        Vector3 startPos = trail.transform.position;

        while(time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;
        Destroy( trail.gameObject, trail.time );
    }
    public IEnumerator BulletTrailRoutine(TrailRenderer trail, Vector2 hit)
    {
        float time = 0;
        Vector3 startPos = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, hit, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit;
        Destroy(trail.gameObject, trail.time);
    }
    
}
