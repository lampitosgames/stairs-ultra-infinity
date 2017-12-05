using UnityEditor;
using System.Collections;
using UnityEngine;

[CustomEditor(typeof(EditorStair))]
public class StairInspectorEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		//Vertical
		GUILayout.BeginVertical();
		{
			//Horizontal line 1
			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button("Set North")) {
					EditorStair stairTarget = (EditorStair)target;
					recordStair(stairTarget);

					//Do things
					stairTarget.bounds.SetDirection(Direction.NORTH);
				}
				if (GUILayout.Button("Set South")) {
					EditorStair stairTarget = (EditorStair)target;
					recordStair(stairTarget);

					//Do things
					stairTarget.bounds.SetDirection(Direction.SOUTH);
				}
				if (GUILayout.Button("Set East")) {
					EditorStair stairTarget = (EditorStair)target;
					recordStair(stairTarget);

					//Do things
					stairTarget.bounds.SetDirection(Direction.EAST);
				}
				if (GUILayout.Button("Set West")) {
					EditorStair stairTarget = (EditorStair)target;
					recordStair(stairTarget);

					//Do things
					stairTarget.bounds.SetDirection(Direction.WEST);
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button("Up")) {
					EditorStair stairTarget = (EditorStair)target;
					recordStair(stairTarget);

					//Do things
					stairTarget.bounds.SetRotation(SRotation.UP);
				}
				if (GUILayout.Button("Right")) {
					EditorStair stairTarget = (EditorStair)target;
					recordStair(stairTarget);

					//Do things
					stairTarget.bounds.SetRotation(SRotation.RIGHT);
				}
				if (GUILayout.Button("Down")) {
					EditorStair stairTarget = (EditorStair)target;
					recordStair(stairTarget);

					//Do things
					stairTarget.bounds.SetRotation(SRotation.DOWN);
				}
				if (GUILayout.Button("Left")) {
					EditorStair stairTarget = (EditorStair)target;
					recordStair(stairTarget);

					//Do things
					stairTarget.bounds.SetRotation(SRotation.LEFT);
				}
			}
			GUILayout.EndHorizontal();
		}
		//End vertical
		GUILayout.EndVertical();
	}

	private void recordStair(EditorStair stair) {
		//Record any changes that are made to these objects. Changes will then be undone with ctrl-z
		Object[] toRecord = {
			stair,
			stair.transform
		};
		Undo.RecordObjects(toRecord, "Set stair to face North/South");
	}
}
    