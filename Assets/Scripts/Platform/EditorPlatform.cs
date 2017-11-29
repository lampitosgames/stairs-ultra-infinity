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

	//On object update, just set the size
	void Update() {
		if (bounds == null) {
			bounds = new PlatformBounds(gameObject, sidesCount);
		}
		
		size = bounds.Size;
	}

	[MenuItem("Snapping/Snap to position %g")]
	static void SnapPlatform() {
		if (Selection.activeGameObject.tag == "platform") {
            
		}
	}
}

