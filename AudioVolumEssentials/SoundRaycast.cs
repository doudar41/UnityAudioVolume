using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRaycast : MonoBehaviour
{

    Collider coll;
    public float distance = 100; 
    private void Start()
    {
        /*Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        */CastRays();

    }

    public void CastRays()
    {

        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("SoundCollisions");

        Debug.DrawRay(transform.position, CalculateVectorDirection(new Vector3(0, 0, 10), transform.position), Color.red, 15);
        Debug.DrawRay(transform.position, CalculateVectorDirection(Vector3.back, transform.position), Color.yellow, 15);
        Debug.DrawRay(transform.position, CalculateVectorDirection(Vector3.left, transform.position), Color.magenta, 15);
        Debug.DrawRay(transform.position, CalculateVectorDirection(Vector3.right, transform.position), Color.green, 15);

        Dictionary<string, int> audioVolumesNames = new Dictionary<string, int>();
        Ray rayF = new Ray(transform.position, CalculateVectorDirection(Vector3.forward, transform.position));
        if (Physics.RaycastAll(rayF, distance, mask).Length !=0 )
        {
            var forward = Physics.RaycastAll(rayF, distance, mask);
            for(int i=0;i< forward.Length;i++)
            {
                print("forward " + i +" - "+forward[i].collider.transform.parent.name+ " - "+ forward[i].collider.name);

                /*if (hitF.collider.name == "ColliderTwo")
                {
                    if (!audioVolumesNames.TryAdd(hitF.collider.transform.parent.name, 1))
                    {
                        audioVolumesNames[hitF.collider.transform.parent.name] =
                    }
                }*/

            }
        }
        if (Physics.Raycast(transform.position, CalculateVectorDirection(Vector3.back, transform.position), out hit, distance, mask))
        {
            print("collide backward with " + hit.collider.name);
        }

        if (Physics.Raycast(transform.position, CalculateVectorDirection(Vector3.left, transform.position), out hit, distance, mask))
        {
            print("collide left with " + hit.collider.name);
        }
        if (Physics.Raycast(transform.position, CalculateVectorDirection(Vector3.right, transform.position), out hit, distance, mask))
        {
            print("collide right with " + hit.collider.name);
        }
    }


    Vector3 CalculateVectorDirection(Vector3 directionNormPoint, Vector3 originPoint)
    {
        directionNormPoint = new Vector3(originPoint.x + directionNormPoint.x,
            originPoint.y + directionNormPoint.y, originPoint.z + directionNormPoint.z);

        Vector3 output = new Vector3(directionNormPoint.x - originPoint.x,
            directionNormPoint.y - originPoint.y, directionNormPoint.z - originPoint.z);

        return output;
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            CastRays();
        }
    }
}   
