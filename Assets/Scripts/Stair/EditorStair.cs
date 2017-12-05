using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class EditorStair : MonoBehaviour {
	public StairBounds bounds;

	public Direction dir;
	public SRotation rot;

	void Update() {
		if (Application.isPlaying) {
			return;
		}
		if (bounds == null) {
			bounds = new StairBounds(gameObject);
		}
		dir = bounds.Dir;
		rot = bounds.Rot;
	}
}