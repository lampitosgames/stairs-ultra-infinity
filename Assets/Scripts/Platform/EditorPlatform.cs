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
			Platform.SnapObject(true, Selection.activeGameObject);
		}
		//If the selected game object is a stair
		if (Selection.activeGameObject.GetComponent<EditorStair>() != null) {
			Platform.SnapObject(false, Selection.activeGameObject);
		}
	}
}

