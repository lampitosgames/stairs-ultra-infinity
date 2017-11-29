using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class EditorStair : MonoBehaviour {
	public StairBounds bounds;

	public Direction dir;
	public SRotation rot;

	void Update() {
		if (bounds == null) {
			bounds = new StairBounds(gameObject);
		}
		dir = bounds.Dir;
		rot = bounds.Rot;
	}

	[MenuItem("Snapping/Snap to position %g")]
	static void SnapPlatform() {
		if (Selection.activeGameObject.tag == "stair") {

		}
	}
}


