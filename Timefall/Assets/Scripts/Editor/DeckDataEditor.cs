using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

//make your Unity lists functional with ReorderableList | Valentin Simonov
//https://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/

// [CustomEditor(typeof(CardDatabase))]
// public class CardDatabaseEditor : Editor {
// 	private ReorderableList list;
	
// 	private void OnEnable() {
// 		list = new ReorderableList(serializedObject, 
//         		serializedObject.FindProperty("cardList"), 
//         		true, true, true, true); 

// 	}
	
// 	public override void OnInspectorGUI() {
// 		serializedObject.Update();
// 		list.DoLayoutList();
// 		serializedObject.ApplyModifiedProperties();

//         //EditorUtility.SetDirty(target);
// 	}
// }