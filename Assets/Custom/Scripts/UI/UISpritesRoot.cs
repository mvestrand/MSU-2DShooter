using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[ExecuteAlways]
public class UISpritesRoot : MonoBehaviour
{

    public enum ScaleMode {
        Viewport,
        Pixels
    }

    [SerializeField] private Camera uiCamera;
    [SerializeField] private ScaleMode scaleMode;
    private bool warningIssued = false;
    Vector2Int storedResolution = Vector2Int.zero;

    public event Action<Vector2Int> onResolutionChange;


    // Start is called before the first frame update
    void Start()
    {
        UpdateScale(true);
    }

    // Update is called once per frame
    void Update() {
        UpdateScale(false);
    }

    private void UpdateScale(bool forceUpdate) {
        if (uiCamera == null)
            return;
        if (!uiCamera.orthographic) {
            if (!warningIssued) {
                warningIssued = true;
                Debug.LogWarning("UISpritesRoot camera must be set to orthographic projection mode to use", this);                
            }
            return;
        }
        if (uiCamera.scaledPixelHeight == storedResolution.x && uiCamera.scaledPixelWidth == storedResolution.y) {
            return; // Screen size has not changed
        } else {
            storedResolution.x = uiCamera.scaledPixelHeight;
            storedResolution.y = uiCamera.scaledPixelWidth;
            onResolutionChange?.Invoke(storedResolution);
        }

        switch (scaleMode) {
            case ScaleMode.Viewport:
                float halfScaleY = uiCamera.orthographicSize;
                float halfScaleX = halfScaleY * uiCamera.aspect;
                transform.localPosition = new Vector3(-halfScaleX, -halfScaleY, 0);
                transform.localScale = new Vector3(2 * halfScaleX, 2 * halfScaleY, 1);
                break;
            case ScaleMode.Pixels:
                transform.localScale = new Vector3(uiCamera.pixelWidth, uiCamera.pixelHeight, 1);
                break;
            default:
                break;
        }
    }

    void OnValidate() {
        UpdateScale(true);
    }

}
