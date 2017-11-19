using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformStairCollider : MonoBehaviour {

	//Reference to the parent script
	private Platform parent;

	//Used to tell the parent which collider this is
	public int colliderID;

	// Use this for initialization
	void Start () {
		parent = transform.parent.GetComponent<Platform> ();
	}

	//When there is a collision, pass it to the parent for game logic handleing
	void OnTriggerEnter(Collider otherCol) {
		parent.OnStairTriggerEnter (colliderID, otherCol);
	}
}
