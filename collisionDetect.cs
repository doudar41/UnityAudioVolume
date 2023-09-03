using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class collisionDetect : MonoBehaviour
{
    public UnityEvent e1Enter;
    public UnityEvent e1Exit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") { print("player"); }
        e1Enter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        e1Exit.Invoke();
    }


}
