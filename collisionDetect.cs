using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionDetection : MonoBehaviour
{
    public UnityEvent Enter;
    public UnityEvent Exit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") { print("player"); }
        Enter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") { print("player"); }
        Exit.Invoke();
    }
}
