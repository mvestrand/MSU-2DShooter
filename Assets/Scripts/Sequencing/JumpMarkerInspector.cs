using System.Collections;
using System.Collections.Generic;

using System.Linq;

using UnityEngine;
using UnityEngine.Timeline;

using UnityEditor;
using UnityEditor.Timeline;

[CustomEditor(typeof(JumpMarker))]
public class JumpMarkerInspector : Editor {

    const string timeLabel = "Timeline will jump at time {0}";
    const string noJumpLabel = "{0} is deactivated";
    const string noDestinationsLabel = "No destination marker found on this track";
    const string jumpTo = "Jump to";
    const string noneLabel = "None";

    SerializedProperty destinationMarker;
    SerializedProperty emitOnce;
    SerializedProperty emitInEditor;
    SerializedProperty m_Time;

    void OnEnable() {
        // Hook-up the marker's serialized properties
        destinationMarker = serializedObject.FindProperty("destinationMarker");
        emitOnce = serializedObject.FindProperty("emitOnce");
        emitInEditor = serializedObject.FindProperty("emitInEditor");
        m_Time = serializedObject.FindProperty("m_Time");        
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        var marker = target as JumpMarker;

        
        using (var changeScope = new EditorGUI.ChangeCheckScope()) {
            EditorGUILayout.PropertyField(m_Time);
            
            EditorGUILayout.Space();

            var destinationMarkers = DestinationMarkersFor(marker);
            if (!destinationMarkers.Any()) {
                DrawNoJump();
            } else {
                DrawJumpOptions(destinationMarkers);
            }

            if (changeScope.changed) {
                serializedObject.ApplyModifiedProperties();
                TimelineEditor.Refresh(RefreshReason.ContentsModified);
            }
        }
    }

    void DrawNoJump() {
        // Issue no destination markers warning
        EditorGUILayout.HelpBox(noDestinationsLabel, MessageType.Info);

        // Draw the disabled property fields
        using (new EditorGUI.DisabledScope(true)) {
            EditorGUILayout.Popup(jumpTo, 0, new[] { noneLabel });
            EditorGUILayout.PropertyField(emitOnce);
            EditorGUILayout.PropertyField(emitInEditor);

        }
    }

    void DrawJumpOptions(IList<DestinationMarker> destinationMarkers) {
        var destination = DrawDestinationPopup(destinationMarkers);
        DrawTimeLabel(destination);
        EditorGUILayout.PropertyField(emitOnce);
        EditorGUILayout.PropertyField(emitInEditor);
    }

    DestinationMarker DrawDestinationPopup(IList<DestinationMarker> destinationMarkers) {
        var popupIndex = 0;
        var destinationMarkerIndex = destinationMarkers.IndexOf(destinationMarker.objectReferenceValue as DestinationMarker);
        if (destinationMarkerIndex != -1)
            popupIndex = destinationMarkerIndex + 1; // Add one for the 'None' option in the popup

        DestinationMarker destination = null;
        using (var changeScope = new EditorGUI.ChangeCheckScope()) {
            var newIndex = EditorGUILayout.Popup(jumpTo, popupIndex, GeneratePopupOptions(destinationMarkers).ToArray());

            if (newIndex > 0)
                destination = destinationMarkers.ElementAt(newIndex - 1);

            if (changeScope.changed)
                destinationMarker.objectReferenceValue = destination;
        }
        return destination;
    }

    static void DrawTimeLabel(DestinationMarker destinationMarker) {
        if (destinationMarker!=null) {
            if (destinationMarker.active)
                EditorGUILayout.HelpBox(string.Format(timeLabel, destinationMarker.time.ToString("0.##")), MessageType.Info);
            else
                EditorGUILayout.HelpBox(string.Format(noJumpLabel, destinationMarker.name), MessageType.Warning);
        }
    }

    static IList<DestinationMarker> DestinationMarkersFor(Marker marker) {
        // Get all destination markers in the parent track
        var destinationMarkers = new List<DestinationMarker>();
        var parent = marker.parent;
        if (parent != null)
            destinationMarkers.AddRange(parent.GetMarkers().OfType<DestinationMarker>().ToList());
        return destinationMarkers;
    }

    static IEnumerable<string> GeneratePopupOptions(IEnumerable<DestinationMarker> markers) {
        yield return noneLabel;
        foreach (var marker in markers) {
            yield return marker.name;
        }
    }

}
