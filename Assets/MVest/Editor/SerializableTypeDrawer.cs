using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;


namespace MVest {





    [CustomPropertyDrawer(typeof(SerializableType))]
    public class SerializableTypeDrawer : PropertyDrawer {


        static private Dictionary<string, int> typenameIndices; // Alias -> dropdown index

        static private string[] optionsUnknown; // Used when the typename is not a listed type
        static private string[] options;
        
        static SerializableTypeDrawer() {

            // Get common basic types
            var basicTypes = new Type[]{
                typeof(float),
                typeof(double),
                typeof(int),
                typeof(long),
                typeof(bool), 
                typeof(string),
                typeof(Vector2),
                typeof(Vector2Int),
                typeof(Vector3),
                typeof(Vector3Int),
                typeof(Vector4),
                typeof(GameObject),
                typeof(UnityEngine.Object),
                typeof(UnityEngine.Component),
                typeof(UnityEngine.MonoBehaviour)}
                .Select(x => x.FullName);

            // Get all custom serializable types
            var customTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    .Where(x => x.GetCustomAttributes(typeof(SerializableTypeAttribute),true).Length > 0)
                    .Select(x => (SerializableType.NeedsAssemblyQualifiedName(x) ?  x.AssemblyQualifiedName : x.FullName))
                    .OrderBy(x => x);
            
            var allTypes = basicTypes.Concat(customTypes).ToArray();

            optionsUnknown = new string[]{"Unknown"}
                    .Concat(allTypes)
                    .ToArray();
            options = allTypes.ToArray();

            // Initialize typename index lookup
            typenameIndices = new Dictionary<string, int>();
            for (int i = 0; i < options.Length; i++) {
                typenameIndices[options[i]] = i;
            }

        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Specify input field area coordinates
            Rect popupRect = new Rect(position.x, position.y, 20, position.height);
            Rect manualInputRect = new Rect(position.x + 20, position.y, position.width - 20, position.height);

            // Get the current typename value
            var typenameProp = property.FindPropertyRelative("_typename");
            string typename = typenameProp.stringValue;
            string oldTypename = typename;

            int index = 0;
            // Check for a known typename
            if (typenameIndices.TryGetValue(typename, out index)) {
                int oldIndex = index;

                // Draw the input fields
                index = EditorGUI.Popup(popupRect, index, options);
                typename = EditorGUI.TextField(manualInputRect, typename);

                // Popup input
                if (oldIndex != index) {
                    typenameProp.stringValue = options[index];
                } // Textfield input
                else if (!oldTypename.Equals(typename)) {
                    typenameProp.stringValue = typename;
                }            

            } // Unknown typename
            else {
                index = 0;

                optionsUnknown[0] = typename;

                // Draw the input fields
                index = EditorGUI.Popup(popupRect, index, optionsUnknown);
                typename = EditorGUI.TextField(manualInputRect, typename);

                // Popup input
                if (index != 0) {
                    typenameProp.stringValue = options[index-1];
                } // Textfield input
                else if (!oldTypename.Equals(typename)) {
                    typenameProp.stringValue = typename;
                }            

            }

            EditorGUI.EndProperty();
        }
    }





}
