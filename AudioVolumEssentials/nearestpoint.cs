using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nearestpoint : MonoBehaviour
{

    public Collider col;
    public GameObject player;
    public Vector3 point;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        point = col.ClosestPoint(player.transform.position);
    }
}
