using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

	public int sidesCount = 4;

	Dictionary<int, PlatformStairCollider> stairColliders = new Dictionary<int, PlatformStairCollider>();


	// Use this for initialization
	void Start () {
		//Get children
		for (int i = 0; i < transform.childCount; i++) {
			GameObject child = transform.GetChild (i).gameObject;
			if (child.tag == "platformStairCollider") {
				PlatformStairCollider childScript = child.GetComponent<PlatformStairCollider> ();
				childScript.colliderID = i;
				stairColliders.Add(i, childScript);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnStairTriggerEnter(int stairColliderID, Collider other) {
		//Only do things if the other game object is a stair
		if (other.gameObject.tag == "stair") {

		}
	}
}
