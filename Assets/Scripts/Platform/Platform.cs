using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
	NORTH,
	EAST,
	SOUTH,
	WEST
}

[RequireComponent(typeof(Transform))]
public class Platform : MonoBehaviour {
	//Number of sides this platform has
	public int sidesCount = 4;

	//Is the platform active?
	public bool active = false;

	//Stairs rotation speed variables
	public float stairRotSpeed;
	public float platformRotSpeed;

	float localStairRot;
	float localPlatformRot;

	bool rotateStairs = true;

	//Dictionary of colliders on all sides of the platform
	Dictionary<Direction, PlatformStairCollider> stairColliders = new Dictionary<Direction, PlatformStairCollider>();

	//Dictionary of stairs colliding with the platform
	Dictionary<Direction, Stair> attachedStairs = new Dictionary<Direction, Stair>();

	//Bounds of this platform
	PlatformBounds bounds;

	// Use this for initialization
	void Start() {
		//Create a bounds object so we can properly place this platform
		bounds = new PlatformBounds(gameObject, sidesCount);

		localStairRot = 0.0f;
		localPlatformRot = 0.0f;
         
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

		bool rotateClockwise = false;
		bool rotateCounter = false;

		if (!active)
			return;

		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			rotateStairs = false;
		} else {
			rotateStairs = true;
		}

		if (Input.GetKey(KeyCode.Q)) {
			rotateCounter = true;
		}
		if (Input.GetKey(KeyCode.E)) {
			rotateClockwise = true;
		}
        
		float stairAngle = 0.0f;
		float platformAngle = 0.0f;
		if (rotateClockwise) {
			if (rotateStairs) {
				stairAngle += Mathf.PI * stairRotSpeed;
			} else {
				platformAngle += Mathf.PI * platformRotSpeed;
			} 
		}
		if (rotateCounter) {
			if (rotateStairs) {
				stairAngle -= Mathf.PI * stairRotSpeed;
			} else {
				platformAngle -= Mathf.PI * platformRotSpeed;
			}
		}

		localStairRot += stairAngle;
		localPlatformRot += platformAngle;

		if (!rotateStairs) {
			transform.localEulerAngles = new Vector3(0f, localPlatformRot, 0f);
		} else {
			Transform NTransform = stairColliders[Direction.NORTH].transform;
			NTransform.localEulerAngles = new Vector3(0f, 0f, localStairRot);

			Transform STransform = stairColliders[Direction.SOUTH].transform;
			STransform.localEulerAngles = new Vector3(0f, 0f, -localStairRot);

			Transform ETransform = stairColliders[Direction.EAST].transform;
			ETransform.localEulerAngles = new Vector3(localStairRot, 0f, 0f);

			Transform WTransform = stairColliders[Direction.WEST].transform;
			WTransform.localEulerAngles = new Vector3(-localStairRot, 0f, 0f);
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
		if (playerCol.gameObject.tag == "Player") {
			this.active = true;
			foreach (KeyValuePair<Direction, Stair> stairItem in attachedStairs) {
				Stair stair = stairItem.Value;
				//Parent the stair's transform
				stair.transform.SetParent(stairColliders[stairItem.Key].transform, true);
			}
		}
	}

	public void OnPlayerLeave(Collider playerCol) {
		if (playerCol.gameObject.tag == "Player") {
			this.active = false;
			//Unparent all stairs
			foreach (KeyValuePair<Direction, Stair> stairItem in attachedStairs) {
				Stair stair = stairItem.Value;
				//Remove the parented relationship
				stair.transform.parent = null;
			}
		}
	}
}
