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

    public void Tracer(RaycastHit2D hit, Transform attackPoint)
    {
        StartCoroutine(BulletTrailRoutine(attackPoint.position, hit.point));
        PV.RPC("TracerRPC", RpcTarget.Others, hit.point, (Vector2)attackPoint.position);
    }

    public void Tracer(Vector2 hit, Transform attackPoint)
    {
        StartCoroutine(BulletTrailRoutine(attackPoint.position, hit));
        PV.RPC("TracerRPC", RpcTarget.Others, hit, (Vector2)attackPoint.position);
    }

    [PunRPC]
    public void TracerRPC(Vector2 hit, Vector2 attackPoint)
    {
        StartCoroutine(BulletTrailRoutine(attackPoint, hit));
    }

    private IEnumerator BulletTrailRoutine(Vector3 attackPoint, Vector2 hit)
    {
        float time = 0f;
        var trail = Instantiate(TrailRenderer);
        trail.transform.position = attackPoint;
        Vector3 startPos = attackPoint;
        Vector3 hitPos = hit; // Ensure hit is converted to Vector3

        while (time < 1f)
        {
            trail.transform.position = Vector3.Lerp(startPos, hitPos, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hitPos;
        Destroy(trail.gameObject, trail.time);
    }

}
