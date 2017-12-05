using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
	NONE = -1,
	NORTH = 0,
	EAST = 1,
	SOUTH = 2,
	WEST = 3
}

public class Platform : MonoBehaviour {
	//Number of sides this platform has
	public int sidesCount = 4;

	//Is the platform active?
	public bool active = false;

	//Stairs rotation speed variables
	public float stairRotSpeed, platformRotSpeed;
	public float stairRotDegrees, platformRotDegrees;

	public float localStairRot = 0f, localPlatformRot = 0f, nextStairRot = 0f, nextPlatformRot = 0f;
	bool rotateStairClockwise = false, rotateStairCounter = false;
	bool rotatePlatformClockwise = false, rotatePlatformCounter = false;
	bool rotateStairs = true;

	//Dictionary of colliders on all sides of the platform
	Dictionary<Direction, PlatformStairCollider> stairColliders = new Dictionary<Direction, PlatformStairCollider>();

	//Dictionary of stairs colliding with the platform
	Dictionary<Direction, Stair> attachedStairs = new Dictionary<Direction, Stair>();

	//Bounds of this platform
	public PlatformBounds bounds;

	// Use this for initialization
	void Start() {
		//Create a bounds object so we can properly place this platform
		bounds = new PlatformBounds(gameObject, sidesCount);

		//Get child colliders and give them directions
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
		rotateStairs = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? true : false;

		if (Input.GetKeyDown(KeyCode.Q) && active) {
			if (rotateStairs && !rotatePlatformCounter && !rotatePlatformClockwise) {
				if (!rotateStairClockwise) {
					nextStairRot = (nextStairRot + stairRotDegrees) % 360f;
					rotateStairCounter = false;
				}
				rotateStairClockwise = true;
			} else if (!rotateStairCounter && !rotateStairClockwise) {
				if (!rotatePlatformCounter) {
					nextPlatformRot -= platformRotDegrees;
					if (nextPlatformRot < 0f) {
						nextPlatformRot = 360f + nextPlatformRot;
					}
					rotatePlatformClockwise = false;
				}
				rotatePlatformCounter = true;
			}
		}
		if (Input.GetKeyDown(KeyCode.E) && active) {
			if (rotateStairs && !rotatePlatformCounter && !rotatePlatformClockwise) {
				if (!rotateStairCounter) {
					nextStairRot -= stairRotDegrees;
					if (nextStairRot < 0f) {
						nextStairRot = 360f + nextStairRot;
					}
					rotateStairClockwise = false;
				}
				rotateStairCounter = true;
			} else if (!rotateStairCounter && !rotateStairClockwise) {
				if (!rotatePlatformClockwise) {
					nextPlatformRot = (nextPlatformRot + platformRotDegrees) % 360f;
					rotatePlatformCounter = false;
				}
				rotatePlatformClockwise = true;
			}
		}

		//If rotating stairs counter clockwise
		if (rotateStairCounter) {
			localStairRot -= Mathf.PI * stairRotSpeed;
			if (localStairRot < 0f) {
				localStairRot = 360f + localStairRot;
			}
		} else if (rotateStairClockwise) {
			localStairRot = (localStairRot + Mathf.PI * stairRotSpeed) % 360f;
		}

		//If rotating stairs counter clockwise
		if (rotatePlatformCounter) {
			localPlatformRot -= Mathf.PI * platformRotSpeed;
			if (localPlatformRot < 0f) {
				localPlatformRot = 360f + localPlatformRot;
			}
		} else if (rotatePlatformClockwise) {
			localPlatformRot = (localPlatformRot + Mathf.PI * platformRotSpeed) % 360f;
		}

		//Update rotations
		SetRotations(localPlatformRot, localStairRot);

		//If close to snap point
		if (localStairRot % stairRotDegrees < 4f && Mathf.Abs(nextStairRot - localStairRot) < 4f && (rotateStairCounter || rotateStairClockwise)) {
			//Set to snap point and end rotation
			localStairRot = nextStairRot;
			SetRotations(localPlatformRot, localStairRot);
			rotateStairCounter = false;
			rotateStairClockwise = false;
			ResetStairs();
		}

		//If close to snap point
		if (localPlatformRot % platformRotDegrees < 4f && Mathf.Abs(nextPlatformRot - localPlatformRot) < 4f && (rotatePlatformCounter || rotatePlatformClockwise)) {
			//Set to snap point and end rotation
			localPlatformRot = nextPlatformRot;
			SetRotations(localPlatformRot, localStairRot);
			rotatePlatformCounter = false;
			rotatePlatformClockwise = false;
		}
	}

	public void SetRotations(float platRot, float stairRot) {
		//Rotate platform
		transform.localEulerAngles = new Vector3(0f, platRot, 0f);
		//Rotate stairs
		stairColliders[Direction.NORTH].transform.localEulerAngles = new Vector3(0f, 0f, stairRot);
		stairColliders[Direction.SOUTH].transform.localEulerAngles = new Vector3(0f, 0f, -stairRot);
		stairColliders[Direction.EAST].transform.localEulerAngles = new Vector3(stairRot, 0f, 0f);
		stairColliders[Direction.WEST].transform.localEulerAngles = new Vector3(-stairRot, 0f, 0f);
	}

	public void ResetStairs() {
		foreach (KeyValuePair<Direction, Stair> pair in attachedStairs) {
			StairBounds stair = pair.Value.bounds;
			stair.trans.SetParent(null);
			stair.RoundRotation();
			stair.trans.SetParent(stairColliders[pair.Key].transform, true);
		}
	}

	public static void SnapObject(bool isPlatform, GameObject target) {
		Transform thisTrans = target.GetComponent<Transform>();
		PlatformBounds otherBounds;
		//Sphere cast to get nearest platforms
		Collider[] otherPlats = Physics.OverlapSphere(thisTrans.position, Stair.stairLength * 2);
		//Loop and get the nearest platform
		int nearestPlatIndex = -1;
		float smallestPlatDist = 90000000f;
		for (int i = 0; i < otherPlats.Length; i++) {
			if (otherPlats[i].tag == "platformMesh") {
				//Get the other platform's component
				otherBounds = otherPlats[i].transform.parent.gameObject.GetComponent<Platform>().bounds;
				//If this is a platform and it is the same as the other platform, continue
				if (isPlatform) {
					if (target.GetComponent<Platform>().bounds == otherBounds) {
						continue;
					}
				}
				float dist = Vector3.Distance(thisTrans.position, otherBounds.Center);
				if (dist < smallestPlatDist) {
					nearestPlatIndex = i;
					smallestPlatDist = dist;
				}
			}
		}
		//If there are no platforms, return
		if (nearestPlatIndex == -1) {
			return;
		}

		//Get the nearest platform gameObject
		otherBounds = otherPlats[nearestPlatIndex].transform.parent.gameObject.GetComponent<Platform>().bounds;

		//Calculate variables
		float theta = Mathf.Deg2Rad * Stair.stairSlopeDeg;
		float wStair = Stair.stairLength / 2f;
		float hStair = Stair.stairHeight / 2f;

		float angles = hStair * Mathf.Sin(theta) + wStair * Mathf.Cos(theta);
		//Get the hor shift
		float horShift = otherBounds.HalfWidth.x + hStair * Mathf.Sin(theta) + wStair * Mathf.Cos(theta);
		//Get the vert shift
		float vertShift = wStair * Mathf.Sin(theta);
		//If it is a platform, calculate differently
		if (isPlatform) {
			horShift = (target.GetComponent<EditorPlatform>().bounds.HalfWidth.x + angles) + (otherBounds.HalfWidth.x + angles);
			vertShift *= 2f;
		}

		//Cache the other platform's position
		Vector3 otherPos = otherBounds.Center;

		//4*4 possible positions.  Total of 16
		//Yes there is probably a better way to do this.  But it is an editor script so I don't give a damn :)
		Vector3[] positions = {
			//North
			//Up
			new Vector3(otherPos.x, otherPos.y + vertShift, otherPos.z + horShift),
			//Down
			new Vector3(otherPos.x, otherPos.y - vertShift, otherPos.z + horShift),
			//Right
			new Vector3(otherPos.x + vertShift, otherPos.y, otherPos.z + horShift),
			//Left
			new Vector3(otherPos.x - vertShift, otherPos.y, otherPos.z + horShift),


			//South
			//Up
			new Vector3(otherPos.x, otherPos.y + vertShift, otherPos.z - horShift),
			//Down
			new Vector3(otherPos.x, otherPos.y - vertShift, otherPos.z - horShift),
			//Right
			new Vector3(otherPos.x - vertShift, otherPos.y, otherPos.z - horShift),
			//Left
			new Vector3(otherPos.x + vertShift, otherPos.y, otherPos.z - horShift),


			//East
			//Up
			new Vector3(otherPos.x + horShift, otherPos.y + vertShift, otherPos.z),
			//Down
			new Vector3(otherPos.x + horShift, otherPos.y - vertShift, otherPos.z),
			//Right
			new Vector3(otherPos.x + horShift, otherPos.y, otherPos.z - vertShift),
			//Left
			new Vector3(otherPos.x + horShift, otherPos.y, otherPos.z + vertShift),


			//West
			//Up
			new Vector3(otherPos.x - horShift, otherPos.y + vertShift, otherPos.z),
			//Down
			new Vector3(otherPos.x - horShift, otherPos.y - vertShift, otherPos.z),
			//Right
			new Vector3(otherPos.x - horShift, otherPos.y, otherPos.z + vertShift),
			//Left
			new Vector3(otherPos.x - horShift, otherPos.y, otherPos.z - vertShift)
		};

		//Loop through the list of possible points and snap to the closest
		float smallestDist = 9000000000f;
		int closestInd = -1;
		for (int v = 0; v < positions.Length; v++) {
			float dist = Vector3.Distance(thisTrans.position, positions[v]);
			if (dist < smallestDist) {
				closestInd = v;
				smallestDist = dist;
			}
		}
		//FINALLY set the selected position
		thisTrans.position = positions[closestInd];
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
				//stair.transform.parent = null;
			}
		}
	}
}
