using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(Collider))]
public class Grab : MonoBehaviour
{
    private SteamVR_Action_Boolean grabPinch;

    private List<GameObject> inGrabRange = new List<GameObject>();
    private List<GameObject> heldObjects = new List<GameObject>();

    private Dictionary<GameObject, ObjectInfo> heldObjectInfo = new Dictionary<GameObject, ObjectInfo>(); 

    private struct ObjectInfo
    {
        public readonly Transform Parent;
        public readonly bool IsKinematic; 

        public ObjectInfo(Transform parent, bool isKinematic)
        {
            Parent = parent;
            IsKinematic = isKinematic;
        }
    }

    private void Start()
    {
        grabPinch.onStateDown += OnGrabPressed; 
        grabPinch.onStateUp += OnGrabReleased;
    }

    private void OnGrabPressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        foreach (GameObject go in inGrabRange) 
        { 
            Rigidbody rb = go.GetComponent<Rigidbody>();
            heldObjectInfo.Add(go, new ObjectInfo(rb.transform.parent, rb.isKinematic));
            rb.isKinematic = true;
            rb.transform.SetParent(transform);
            heldObjects.Add(go); inGrabRange.Remove(go); 
        }
    }

    private void OnGrabReleased(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        foreach (GameObject go in heldObjects)
        {
            Rigidbody rb = go.GetComponent<Rigidbody>();
            rb.isKinematic = heldObjectInfo[go].IsKinematic;
            rb.transform.SetParent(heldObjectInfo[go].Parent);
            heldObjectInfo.Remove(go);
            heldObjects.Remove(go); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print($"Collided with {other.gameObject}");

        inGrabRange.Add(other.gameObject); 
    }

    private void OnTriggerExit(Collider other)
    {
        if (inGrabRange.Contains(other.gameObject))
            inGrabRange.Remove(other.gameObject);
    }
}
