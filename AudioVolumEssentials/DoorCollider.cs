using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

public class DoorCollider : MonoBehaviour
{
    public UnityEvent<float> movingEvent;
    public Transform door;
    public BoxCollider col;
    Vector3 openDoorAngle;
    public AnimationCurve curve;
    float doorStateFloat = 0;
    [Range(0,1)]
    public float decreaseCurveValue = 1;
    [Range(0,1)]
    public float slowDownChangingCurveValue = 1;
    bool opened = false;

    public EventReference EventNameOpen;
    FMOD.Studio.EventInstance soundInstancesOpen;
    public EventReference EventNameClose;
    FMOD.Studio.EventInstance soundInstancesClose;

    private void Awake()
    {
        openDoorAngle = door.rotation.eulerAngles;
        soundInstancesOpen = FMODUnity.RuntimeManager.CreateInstance(EventNameOpen);
        soundInstancesOpen.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        soundInstancesClose = FMODUnity.RuntimeManager.CreateInstance(EventNameClose);
        soundInstancesClose.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        if (!soundInstancesOpen.isValid())
        {
            print("No valid event");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        opened = true;
        soundInstancesOpen.start();
    }
    private void OnTriggerExit(Collider other)
    {

        opened = false;
        soundInstancesClose.start();
    }

    private void Update()
    {
        if (opened)
        {
            if (doorStateFloat < 1)
            {
                print(doorStateFloat);
                doorStateFloat += Time.deltaTime*slowDownChangingCurveValue;
                door.SetPositionAndRotation(door.position, Quaternion.Euler(door.rotation.eulerAngles.x, (curve.Evaluate(doorStateFloat) * 90) + 180, door.rotation.eulerAngles.z));
                movingEvent.Invoke(doorStateFloat* decreaseCurveValue);
            }
        }
        else
        {
            if (doorStateFloat > 0)
            {
                doorStateFloat -= Time.deltaTime;
                door.SetPositionAndRotation(door.position, Quaternion.Euler(door.rotation.eulerAngles.x, (curve.Evaluate(doorStateFloat) * 90) + 180, door.rotation.eulerAngles.z));
                movingEvent.Invoke(doorStateFloat * decreaseCurveValue);
            }
        }
    }

}
