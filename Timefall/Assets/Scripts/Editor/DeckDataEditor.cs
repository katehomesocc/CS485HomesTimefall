using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

//make your Unity lists functional with ReorderableList | Valentin Simonov
//https://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/

//https://blog.terresquall.com/2020/03/creating-reorderable-lists-in-the-unity-inspector/

[CustomEditor(typeof(CardDatabase))]
[System.Serializable]
public class CardDatabaseEditor : Editor {

    SerializedProperty cardList;  
	ReorderableList list;
	
	public void OnEnable()
    {

        //"link" the SerializedProperties to the properties of cardList
        cardList = serializedObject.FindProperty("cardList");
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("cardList"), true, true, true, true);

        //draw header & calculate card totals
        string cardListLabel = string.Format("{0} Cards | X Agent | X Essence | X Event", list.count);

        list.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, cardListLabel, EditorStyles.boldLabel);
        };

        //draw list items
        list.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);

                //testing 

                // var targetProperty = element.FindPropertyRelative("name");
                // if (targetProperty == null) {
                //     Debug.Log("No property 'faction' found in element");
                //     return;
                // }
                // else
                // {
                //     Debug.Log("Found:" + targetProperty);
                // }
                // var elementobj = element.FindPropertyRelative( "faction" ).objectReferenceValue;
                // Debug.Log("Found 'faction'");
                
                // EditorGUI.ObjectField ( new Rect ( rect.x, rect.y, 150, EditorGUIUtility.singleLineHeight ), elementobj, elementobj.GetType(), true );
            };

        
    }

	
	public override void OnInspectorGUI() {
		serializedObject.Update();

        list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
	}


    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); // The element in the list

        //Create a property field and label field for each property. 
        if (element == null) {
            Debug.LogError("element is null");
            return;
        }
        var targetProperty = element.FindPropertyRelative("target");
        if (targetProperty == null) {
            Debug.Log("No property 'target' found in element");
            return;
        }
        var elementobj = element.FindPropertyRelative ( "target" ).objectReferenceValue;
        
        EditorGUI.ObjectField ( new Rect ( rect.x, rect.y, 150, EditorGUIUtility.singleLineHeight ), elementobj, elementobj.GetType(), true );


        // EditorGUI.LabelField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), "Faction:");

        // SerializedProperty factionStr = element.FindPropertyRelative("faction");

        // if(factionStr != null)
        // {
        //     EditorGUI.PropertyField(
        //     new Rect(rect.x + 50, rect.y, 100, EditorGUIUtility.singleLineHeight), 
        //     factionStr,
        //     GUIContent.none
        // );
        // } else
        // {

        // }
        
    }
}