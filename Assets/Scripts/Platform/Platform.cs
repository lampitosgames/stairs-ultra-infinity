using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    NORTH,
    EAST,
    SOUTH,
    WEST
}

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Transform))]
public class Platform : MonoBehaviour {
    //Number of sides this platform has
    public int sidesCount = 4;

    //Dictionary of colliders on all sides of the platform
    Dictionary<Direction, PlatformStairCollider> stairColliders = new Dictionary<Direction, PlatformStairCollider>();

    //Dictionary of stairs colliding with the platform
    Dictionary<Direction, Stair> attachedStairs = new Dictionary<Direction, Stair>();

    PlatformBounds bounds;


    // Use this for initialization
    void Start() {
        this.bounds = new PlatformBounds(gameObject, 4);
        bounds.XSize = 4;
        //Get children
        for (int i = 0; i < transform.childCount; i++) {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.tag == "platformStairCollider") {
                PlatformStairCollider childScript = child.GetComponent<PlatformStairCollider>();
                Transform childTransform = child.GetComponent<Transform>();
                //Detect direction the collider represents
                if (childTransform.localPosition.x > 0) {
                    childScript.dir = Direction.EAST;
                } else if (childTransform.localPosition.x < 0) {
                    childScript.dir = Direction.WEST;
                }

                if (childTransform.localPosition.z > 0) {
                    childScript.dir = Direction.NORTH;
                } else if (childTransform.localPosition.z < 0) {
                    childScript.dir = Direction.SOUTH;
                }
                //Add the stair collider to the dictionary
                stairColliders.Add(childScript.dir, childScript);
            }
        }
    }
	
    // Update is called once per frame
    void Update() {
        transform.RotateAround(bounds.Center, transform.up, Mathf.PI / 10f);
        foreach (KeyValuePair<Direction, Stair> item in attachedStairs) {
            Transform stairTransform = item.Value.gameObject.GetComponent<Transform>();
            Vector3[] pointToRotate = bounds.SidePivot(item.Key);
            stairTransform.RotateAround(pointToRotate[0], pointToRotate[1], Mathf.PI / 10f);
            stairTransform.RotateAround(bounds.Center, transform.up, Mathf.PI / 10f);
        }
    }

    public void OnStairTriggerEnter(Direction colliderDirection, Collider other) {
        //Only do things if the other game object is a stair
        if (other.gameObject.GetComponent<Stair>() != null) {
            //Attach the stair object to this platform
            attachedStairs.Add(colliderDirection, other.gameObject.GetComponent<Stair>());
        }
    }

    public void OnStairTriggerLeave(Direction colliderDirection, Collider other) {
        //Only do things if the other game object is a stair
        if (other.gameObject.GetComponent<Stair>() != null) {
            //Remove the stair object from this platform
            attachedStairs.Remove(colliderDirection);
        }
    }

    public void OnPlayerEnter(Collider playerCol) {
        //Do things
    }
}
