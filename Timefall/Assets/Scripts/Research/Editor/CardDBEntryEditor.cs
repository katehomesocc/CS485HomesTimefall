using UnityEditor;
using UnityEngine;

using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(CardDBEntry))]
public class CardDBEntry_PropertyDrawer : PropertyDrawer
{
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        Debug.Log(property.type);
        // Create a new VisualElement to be the root the property UI.
        var container = new VisualElement();

        // Create property fields.
        var cardField = new PropertyField(property.FindPropertyRelative("card"), "Card");
        var amountField = new PropertyField(property.FindPropertyRelative("amount"), "#");

        // Add fields to the container.
        container.Add(cardField);
        container.Add(amountField);

        return container;
    }
}
