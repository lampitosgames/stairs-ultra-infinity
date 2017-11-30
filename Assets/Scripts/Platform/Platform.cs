using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
	NORTH = 0,
	EAST = 1,
	SOUTH = 2,
	WEST = 3
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

    public float stairRotDegrees;
    public float platformRotDegrees;

	float localStairRot;
	float localPlatformRot;

    float nextStairRot;
    float nextPlatformRot;

    bool rotateStairClockwise = false;
    bool rotateStairCounter = false;

    bool rotatePlatformClockwise = false;
    bool rotatePlatformCounter = false;

    bool rotateStairs = true;
    bool rotatePlatform = false;

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

        nextStairRot = 0.0f;
        nextPlatformRot = 0.0f;

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

		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			rotateStairs = false;
		} else {
			rotateStairs = true;
		}

		if (Input.GetKeyDown(KeyCode.Q) && active) {
            if (rotateStairs)
            {
                if (!rotateStairCounter)
                {
                    nextStairRot += stairRotDegrees;
                    rotateStairClockwise = false;
                }
                rotateStairCounter = true;
            }
            else
            {
                if (!rotatePlatformCounter)
                {
                    nextPlatformRot -= platformRotDegrees;
                    rotatePlatformClockwise = false;
                }
                rotatePlatformCounter = true;
            }
		}
		if (Input.GetKeyDown(KeyCode.E) && active) {
            if (rotateStairs)
            {

                if (!rotateStairClockwise)
                {
                    nextStairRot -= stairRotDegrees;
                    rotateStairCounter = false;
                }
                rotateStairClockwise = true;
            }
            else
            {
                if (!rotatePlatformClockwise)
                {
                    nextPlatformRot += platformRotDegrees;
                    rotatePlatformCounter = false;
                }
                rotatePlatformClockwise = true;
            }
        }
        
		float stairAngle = 0.0f;
		float platformAngle = 0.0f;
		if (rotateStairClockwise) {

			stairAngle -= Mathf.PI * stairRotSpeed;

            if (localStairRot < nextStairRot)
            {
                if (nextStairRot <= 0.0f)
                {
                    nextStairRot += 360.0f;
                }
                localStairRot = nextStairRot;
                rotateStairClockwise = false;
            }
        }
		if (rotateStairCounter) {

			stairAngle += Mathf.PI * stairRotSpeed;

            if (localStairRot > nextStairRot)
            {
                if (nextStairRot >= 360.0f)
                {
                    nextStairRot -= 360.0f;
                }
                localStairRot = nextStairRot;
                rotateStairCounter = false;
            }
        }

        if (rotatePlatformClockwise)
        {

            platformAngle += Mathf.PI * platformRotSpeed;

            if (localPlatformRot > nextPlatformRot)
            {
                if (nextPlatformRot >= 360.0f)
                {
                    nextPlatformRot -= 360.0f;
                }
                localPlatformRot = nextPlatformRot;
                rotatePlatformClockwise = false;
            }
        }

        if (rotatePlatformCounter)
        {

            platformAngle -= Mathf.PI * platformRotSpeed;

            if (localPlatformRot < nextPlatformRot)
            {
                if (nextPlatformRot <= 0.0f)
                {
                    nextPlatformRot += 360.0f;
                }
                localPlatformRot = nextPlatformRot;
                rotatePlatformCounter = false;
            }
        }

        localStairRot += stairAngle;
		localPlatformRot += platformAngle;

        
		
		transform.localEulerAngles = new Vector3(0f, localPlatformRot, 0f);
		
		Transform NTransform = stairColliders[Direction.NORTH].transform;
		NTransform.localEulerAngles = new Vector3(0f, 0f, localStairRot);

		Transform STransform = stairColliders[Direction.SOUTH].transform;
		STransform.localEulerAngles = new Vector3(0f, 0f, -localStairRot);

		Transform ETransform = stairColliders[Direction.EAST].transform;
		ETransform.localEulerAngles = new Vector3(localStairRot, 0f, 0f);

		Transform WTransform = stairColliders[Direction.WEST].transform;
		WTransform.localEulerAngles = new Vector3(-localStairRot, 0f, 0f);

	}
	
	public static void SnapObject(bool isPlatform, GameObject target) {
		Transform thisTrans = target.GetComponent<Transform>();
		EditorPlatform otherPlat;
		//Sphere cast to get nearest platforms
		Collider[] otherPlats = Physics.OverlapSphere(thisTrans.position, Stair.stairLength * 2);
		//Loop and get the nearest platform
		int nearestPlatIndex = -1;
		float smallestPlatDist = 90000000f;
		for (int i = 0; i < otherPlats.Length; i++) {
			if (otherPlats[i].tag == "platformMesh") {
				//Get the other platform's component
				otherPlat = otherPlats[i].transform.parent.gameObject.GetComponent<EditorPlatform>();
				//If this is a platform and it is the same as the other platform, continue
				if (isPlatform) {
					if (target.GetComponent<EditorPlatform>() == otherPlat) {
						continue;
					}
				}
				float dist = Vector3.Distance(thisTrans.position, otherPlat.bounds.Center);
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
		otherPlat = otherPlats[nearestPlatIndex].transform.parent.gameObject.GetComponent<EditorPlatform>();

		//Calculate variables
		float theta = Mathf.Deg2Rad * Stair.stairSlopeDeg;
		float wStair = Stair.stairLength / 2f;
		float hStair = Stair.stairHeight / 2f;

		float angles = hStair * Mathf.Sin(theta) + wStair * Mathf.Cos(theta);
		//Get the hor shift
		float horShift = otherPlat.bounds.HalfWidth.x + hStair * Mathf.Sin(theta) + wStair * Mathf.Cos(theta);
		//Get the vert shift
		float vertShift = wStair * Mathf.Sin(theta);
		//If it is a platform, calculate differently
		if (isPlatform) {
			horShift = (target.GetComponent<EditorPlatform>().bounds.HalfWidth.x + angles) + (otherPlat.bounds.HalfWidth.x + angles);
			vertShift *= 2f;
		}

		//Cache the other platform's position
		Vector3 otherPos = otherPlat.bounds.Center;

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
