using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class CollisionDetection : MonoBehaviour
{
    public UnityEvent<Collider> Enter;
    public UnityEvent<Collider> Exit;

    [Header("If trigger is simple or convex")]
    public UnityEvent Stay;
    bool simpleCollider = false;
    bool playerInside = false;

    Mesh mesh;
    MeshCollider col;

    private void Awake()
    {
        col = GetComponent<MeshCollider>();
        if (col == null)
        {
            simpleCollider = true;
        }
    }

    /* 
     * Register any entrances and exits of any sound collision objects and call events
     * 
     */

    private void OnTriggerEnter(Collider other)
    {
        Enter.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInside = false;
        }
        Exit.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInside = true;
        }
        Stay.Invoke(); 
    }


 /* 
 * Checking  if any Vector3 point is in collision mesh
 * 
 */

    public bool CheckForMesh(Vector3 probablePointInMesh)
    {
        float TotalAngle = 0;
        if (simpleCollider) 
        {
            //print("simple");
            return playerInside;
        }
        else
        {
            col = GetComponent<MeshCollider>();
            mesh = col.sharedMesh;

            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                Vector3 vA = transform.TransformPoint(mesh.vertices[mesh.triangles[i]]);
                Vector3 vB = transform.TransformPoint(mesh.vertices[mesh.triangles[i + 1]]);
                Vector3 vC = transform.TransformPoint(mesh.vertices[mesh.triangles[i + 2]]);

                Vector3 a = (vA - probablePointInMesh);
                Vector3 b = (vB - probablePointInMesh);
                Vector3 c = (vC - probablePointInMesh);

                float angle = GetSolidAngle(a, b, c);

                Vector3 normal = GetNormal(vA, vB, vC);
                Vector3 center = GetCenter(vA, vB, vC);

                Vector3 faceVector = probablePointInMesh - center;
                float normalDot = Vector3.Dot(normal, faceVector);
                float factor = (normalDot > 0) ? 1 : -1;
                TotalAngle += angle * factor;
            }
            float difference = Mathf.Abs(TotalAngle) - 12;
            if (Mathf.Abs(difference) < 0.6f)
            {
                //print("OK" + difference);
                return true;
            }
            else
            {
                print("not  OK" + difference+ "audio volume - "+transform.parent.name);
                return false;
            }
        }
    }

    float GetSolidAngle(Vector3 a, Vector3 b, Vector3 c)
    {
        a = a.normalized;
        b = b.normalized;
        c = c.normalized;

        float numer = Vector3.Dot(Vector3.Cross(a, b), c);
        float denum = 1 + Vector3.Dot(a, b) + Vector3.Dot(b, c)+ Vector3.Dot(a, c);
        float angle = 2* Mathf.Atan2(numer, denum);
        return Mathf.Abs(angle);
    }

    Vector3 GetNormal(Vector3 a,Vector3 b,Vector3 c)
    {
        return Vector3.Cross(b - a, c - b);
    }

    Vector3 GetCenter(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 sum = new Vector3((a.x + b.x + c.x)/3, (a.y + b.y + c.y) / 3, (a.z + b.z + c.z) / 3);
        return sum;
    }



}
