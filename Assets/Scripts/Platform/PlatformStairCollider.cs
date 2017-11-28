using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlatformStairCollider : MonoBehaviour {

    //Reference to the parent script
    private Platform parent;

    //Used to tell the parent which direction this collider represents
    public Direction dir;

    // Use this for initialization
    void Start() {
        parent = transform.parent.GetComponent<Platform>();
    }

    //When there is a collision, pass it to the parent for game logic handleing
    void OnTriggerEnter(Collider otherCol) {
        parent.OnStairTriggerEnter(dir, otherCol);
    }

    //When the collision stops, pass it to the parent for game logic handling
    void OnTriggerExit(Collider otherCol) {
        parent.OnStairTriggerLeave(dir, otherCol);
    }
}
