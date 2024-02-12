using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class FieldOfView : MonoBehaviour {


    public LayerMask obstructionLayer;
    public float fov;
    public int rayCount;
    public float startAngle;
    public float viewDist;
    private Vector3 origin;
    Mesh mesh;


    public static Vector3 Angle2Vector(float angle)
    {
        float radians = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(radians), Mathf.Sin(radians));
    }

    public static float Vector2Angle(Vector3 v)
    {
        v = v.normalized;
        float n = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        if (n < 0) { n += 360; }
        return n;
    }

    public void SetOrigin(Vector3 origin) { this.origin = origin; }

    //sets FOV to face given direction (in radians)
    public void SetDirection(Vector3 direction)
    {
        startAngle = Vector2Angle(direction) + fov / 2 - 180;
        if (startAngle < 0)
        {
            startAngle += 360;
        }
    }

    public void SetFOV(float fov)
    {
        this.fov = fov;
    }

    public void SetViewDist(float viewDist)
    {
        this.viewDist = viewDist;
    }

    void Start()
    {
        //creates mesh of FOV (what you see onscreen)
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void LateUpdate()
    {
        //for fov generation, it raycasts at regular intervals within the FOV, either setting points on collide to out to view distance
        //it then uses those vertices from raycasting and creates triangles, constructing the mesh
        float angle = startAngle;
        float angleInc = fov / rayCount;

        //this one updates origin on movement, remove to add to enemy script
        // also add direction set for enemy
        //setOrigin(transform.position);


        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 v;
            RaycastHit2D raycast = Physics2D.Raycast(origin, Angle2Vector(angle), viewDist, obstructionLayer);
            if (raycast.collider == null)
            {
                v = origin + Angle2Vector(angle) * viewDist;
            }
            else
            {
                v = raycast.point;
            }
            vertices[vertexIndex] = v;

            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleInc;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }



}
