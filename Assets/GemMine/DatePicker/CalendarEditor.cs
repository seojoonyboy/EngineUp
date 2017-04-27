using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(Calendar))]
public class CalendarEditor : Editor {

	Texture2D logoTexture;

	SerializedProperty calendarFont;
	SerializedProperty startCalOpen;
	SerializedProperty actualMonthImg;
	SerializedProperty otherMonthImg;
	SerializedProperty currentMonthImg;
	SerializedProperty navBarImg;
	SerializedProperty fontActiveColor;
	SerializedProperty fontDeactiveColor;
	SerializedProperty spacingColor;
	SerializedProperty horizSpacing;
	SerializedProperty vertSpacing;
	SerializedProperty zoomAnimation;
	SerializedProperty hideAnimation;

	void OnEnable()
	{
		logoTexture = Resources.Load ("gemmine-logo", typeof(Texture2D)) as Texture2D;
		calendarFont = serializedObject.FindProperty("calendarFont");
		startCalOpen = serializedObject.FindProperty ("startCalOpen");
		zoomAnimation = serializedObject.FindProperty ("zoomAnimation");
		hideAnimation = serializedObject.FindProperty ("hideAnimation");
		actualMonthImg = serializedObject.FindProperty ("actualMonthImg");
		otherMonthImg = serializedObject.FindProperty ("otherMonthImg");
		currentMonthImg = serializedObject.FindProperty ("currentMonthImg");
		navBarImg = serializedObject.FindProperty ("navBarImg");
		fontActiveColor = serializedObject.FindProperty ("fontActiveColor");
		fontDeactiveColor = serializedObject.FindProperty ("fontDeactiveColor");
		spacingColor = serializedObject.FindProperty ("spacingColor");
		horizSpacing = serializedObject.FindProperty ("horizSpacing");
		vertSpacing = serializedObject.FindProperty ("vertSpacing");
	}

	public override void OnInspectorGUI()
	{
		GUILayout.Space (10);

		if (logoTexture != null) {
			Rect rect = GUILayoutUtility.GetRect (logoTexture.width, logoTexture.height);
			GUI.DrawTexture (rect, logoTexture, ScaleMode.ScaleToFit);
		}

		GUILayout.Space (10);

		GUILayout.BeginVertical ("Box");
		GUILayout.Label ("Calendar Configuration", EditorStyles.boldLabel);
		serializedObject.Update();
		EditorGUILayout.PropertyField(calendarFont);
		EditorGUILayout.PropertyField(startCalOpen);
		EditorGUILayout.PropertyField (zoomAnimation);
		EditorGUILayout.PropertyField (hideAnimation);
		EditorGUILayout.PropertyField (actualMonthImg);
		EditorGUILayout.PropertyField (otherMonthImg);
		EditorGUILayout.PropertyField (currentMonthImg);
		EditorGUILayout.PropertyField (navBarImg);
		GUILayout.BeginHorizontal ();
		GUILayoutOption[] options = { GUILayout.MaxWidth (64f), GUILayout.MinHeight (64f) }; 
		EditorGUILayout.ObjectField ((Sprite)actualMonthImg.objectReferenceValue, typeof(Sprite), options);
		EditorGUILayout.ObjectField ((Sprite)otherMonthImg.objectReferenceValue, typeof(Sprite), options);
		EditorGUILayout.ObjectField ((Sprite)currentMonthImg.objectReferenceValue, typeof(Sprite), options);
		EditorGUILayout.ObjectField ((Sprite)navBarImg.objectReferenceValue, typeof(Sprite), options);
		GUILayout.EndHorizontal ();
		EditorGUILayout.PropertyField (fontActiveColor);
		EditorGUILayout.PropertyField (fontDeactiveColor);
		EditorGUILayout.PropertyField (spacingColor);
		EditorGUILayout.PropertyField (horizSpacing);
		EditorGUILayout.PropertyField (vertSpacing);

		serializedObject.ApplyModifiedProperties();
		GUILayout.Space (10);
		GUILayout.EndVertical ();
	}
}
#endif
