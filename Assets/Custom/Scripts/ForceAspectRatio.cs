using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ForceAspectRatio : MonoBehaviour
{
    [SerializeField] private Vector2 targetAspectRatio = new Vector2(16, 9);
    private Vector2Int currResolution;

    void Start() {
        UpdateCameraViewport();
    }

    void Update() {
        if (ResolutionHasChanged())
            UpdateCameraViewport();
    }

    bool ResolutionHasChanged() {
        return (currResolution.x != Screen.width || currResolution.y != Screen.height);
    }

    void UpdateCameraViewport() {
        if (Screen.width <= 0 || Screen.height <= 0) // Do nothing if there is no valid screen
            return;

        Camera camera = GetComponent<Camera>();

        float targetAspect = targetAspectRatio.x / targetAspectRatio.y;

        float windowAspect = Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1) { // Use letterbox to fit viewport
            Rect viewRect = new Rect();
            viewRect.width = 1;
            viewRect.height = scaleHeight;
            viewRect.x = 0;
            viewRect.y = (1 - scaleHeight) / 2;
            camera.rect = viewRect;
        } 
        else {  // Use pillarbox to fit viewport
            Rect viewRect = new Rect();

            float scaleWidth = 1 / scaleHeight;

            viewRect.width = scaleWidth;
            viewRect.height = 1;
            viewRect.x = (1 - scaleWidth) / 2;
            viewRect.y = 0;

            camera.rect = viewRect;

        }

        currResolution.x = Screen.width;
        currResolution.y = Screen.height;
    }
}
