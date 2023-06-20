using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

[CustomStyle("AnnotationStyle")]
[DisplayName("Annotation")]
public class AnnotationMarker : Marker {
    public string title;
    public Color color = new Color(1f,1f,1f,0.5f);
    public bool showLineOverlay = true;
    [TextArea(10, 15)] public string description;

}
