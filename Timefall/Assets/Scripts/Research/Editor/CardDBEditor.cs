using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(ResearchCardDB))]
public class CardDBEditor : Editor
{
    public VisualTreeAsset m_InspectorXML;

    SerializedProperty eventsProp;
    SerializedProperty essenceProp;
    SerializedProperty egentsProp;
    SerializedProperty dbNameProp;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        eventsProp = serializedObject.FindProperty ("events");
        essenceProp = serializedObject.FindProperty ("essence");
        egentsProp = serializedObject.FindProperty ("agents");
        dbNameProp = serializedObject.FindProperty ("dbName");
    }

    public override VisualElement CreateInspectorGUI()
    {
    var researchCardDB = target as ResearchCardDB;

    researchCardDB.CalcTotalSize();
     // Create a new VisualElement to be the root of our Inspector UI.
     VisualElement myInspector = new VisualElement();

     // Add a simple label.
     myInspector.Add(new Label("This is a custom Inspector"));

     // Load the UXML file.
     m_InspectorXML= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Research/Editor/CardDBEditor_UXML.uxml");

     // Instantiate the UXML.
     myInspector = m_InspectorXML.Instantiate();

    //  Debug.Log("Updating: " + dbNameProp.stringValue);

     // Return the finished Inspector UI.
        return myInspector;
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }
}