using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections;

// No handling of multi-object editing, undo, and prefab overrides!
[CustomEditor(typeof(Cutscene_Manager))]
public class CutsceneManagerEditor : Editor {

	private SerializedProperty timeToChange;
	private SerializedProperty scenes;
	private int timeArraySize;
	private int scenesArraySize;

	// For foldouts
	private static bool timeToChangeBool = true;
	private static bool scenesBool = true;

	//int numOfScenes = 1;

	void OnEnable()
	{
		// Initialise the SerializedProperties
		timeToChange = serializedObject.FindProperty("TimeToChange");
		scenes = serializedObject.FindProperty("Scenes");

		timeArraySize = timeToChange.FindPropertyRelative("Array.size").intValue;
		scenesArraySize = scenes.FindPropertyRelative("Array.size").intValue;
	}

	public override void OnInspectorGUI()
	{
		// Grab the script component as the target
		Cutscene_Manager csmgr = (Cutscene_Manager)target;

		// Update the Inspector
		serializedObject.Update();

		// For spacing
		EditorGUILayout.Space();

		// Scene Management
		EditorGUILayout.LabelField("Scene Management", EditorStyles.boldLabel);						// Section Header
		csmgr.NextLevel = EditorGUILayout.IntField("Next Level", csmgr.NextLevel);					// IntField for Next Level
			csmgr.NextScene = EditorGUILayout.TextField("Next Scene", csmgr.NextScene);				// TextField for Next Scene
		csmgr.GameStageNum = EditorGUILayout.IntField("Game Stage Number", csmgr.GameStageNum);		// IntField for Game Stage Number
		
		// For spacing
		EditorGUILayout.Space();

		// Cutscenes' Timing
		EditorGUILayout.LabelField("Cutscenes' Timing", EditorStyles.boldLabel);					// Section Header

		// Create a foldout with a triangle to display the elements of the TimeToChange array
		timeToChangeBool = EditorGUILayout.Foldout(timeToChangeBool, "Cutscene Timings");
		if (timeToChangeBool)
			EditorGUILayout.IntSlider(timeToChange.FindPropertyRelative("Array.size"), 1, 25, "Number of Scenes");		// Display Array Size of TimeToChange
		for (int i = 0; i < timeToChange.arraySize; i++)
		{
			// If foldout is open, set the array elements under it to be indented
			// Once done, set the indentLevel back to default (0)
			if (timeToChangeBool)
			{
				EditorGUI.indentLevel += 1;
				EditorGUILayout.PropertyField(timeToChange.GetArrayElementAtIndex(i), new GUIContent("Cutscene #" + (i + 1)));
				EditorGUI.indentLevel -= 1;
			}
		}

		if (timeToChangeBool)
			csmgr.TimeToFade = EditorGUILayout.Slider("Fade Duration", csmgr.TimeToFade, 0, 1);     // Slider for Fade Timing

		// For spacing
		EditorGUILayout.Space();

		// Cutscenes' Sprites
		EditorGUILayout.LabelField("Cutscenes' Sprites", EditorStyles.boldLabel);
		scenesBool = EditorGUILayout.Foldout(scenesBool, "Cutscene Sprites");           // Foldout for Scenes
		if (scenesBool)
			EditorGUILayout.IntSlider(scenes.FindPropertyRelative("Array.size"), 1, 25, "Number of Scenes");      // Display Array Size of Scenes
		for (int i = 0; i < scenes.arraySize; i++)
		{
			// If foldout is open, set the array elements under it to be indented
			// Once done, set the indentLevel back to default (0)
			if (scenesBool)
			{
				EditorGUI.indentLevel += 1;
				EditorGUILayout.PropertyField(scenes.GetArrayElementAtIndex(i), new GUIContent("Cutscene #" + (i + 1)));
				EditorGUI.indentLevel -= 1;
			}
		}

		// Apply the properties after being modified
		serializedObject.ApplyModifiedProperties();
		
		// ObjectField to drop Scene objects into for CutsceneObject & Fader
		csmgr.CutsceneObject = (UnityEngine.UI.Image)EditorGUILayout.ObjectField("GameObject To Change", csmgr.CutsceneObject, typeof(UnityEngine.UI.Image), true);
		csmgr.Fader = (UnityEngine.UI.Image)EditorGUILayout.ObjectField("Fader GameObject", csmgr.Fader, typeof(UnityEngine.UI.Image), true);

		// For spacing
		EditorGUILayout.Space();

		// For Debugging
		EditorGUILayout.LabelField("For Debugging", EditorStyles.boldLabel);
		EditorGUILayout.LabelField("Frame Count: ", (csmgr.FrameCount + 1).ToString());		// Current Frame (Read-only)
		EditorGUILayout.LabelField("Time Elapsed: ", csmgr.TimeElapsed.ToString());			// Time Elapsed (Read-only)

		// Update the scene if Inspector is updated
		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}

		// To load the default inspector
		//DrawDefaultInspector();
	}
}
