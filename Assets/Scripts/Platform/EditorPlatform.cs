using UnityEditor;
using UnityEngine;

//Script that runs in the editor. It generates platform data for the snapmap script
[ExecuteInEditMode]
public class EditorPlatform : MonoBehaviour {
	//How many sides this platform has
	public int sidesCount = 4;

	//Shows what the bounds code thinks the platform size is
	public Vector3 size;
	//The bounds object for this platform.  Another one will be generated at runtime in the Platform script
	public PlatformBounds bounds;

	private Transform platformMesh;

	//On object update, just set the size
	void Update() {
		if (Application.isPlaying) {
			return;
		}
		if (bounds == null) {
			bounds = new PlatformBounds(gameObject, sidesCount);
		}
		if (platformMesh == null) {
			for (int i = 0; i < transform.childCount; i++) {
				Transform thisChild = transform.GetChild(i);
				if (thisChild.tag == "platformMesh") {
					platformMesh = thisChild;
					break;
				}
			}
		} else if (size != bounds.Size) {
			//Reposition colliders and other child objects for the platform
			bounds.RepositionChildren(platformMesh.localScale);
			size = bounds.Size;
		}
	}

	[MenuItem("Snapping/Snap to position %g")]
	static void SnapObjectSelector() {
		//If the selected game object is a platform
		if (Selection.activeGameObject.GetComponent<EditorPlatform>() != null) {
			SnapObject(true, Selection.activeGameObject);
		}
		//If the selected game object is a stair
		if (Selection.activeGameObject.GetComponent<EditorStair>() != null) {
			SnapObject(false, Selection.activeGameObject);
		}
	}

	static void SnapObject(bool isPlatform, GameObject target) {
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
}

