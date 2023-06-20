using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteAlways]
public class ViewportRatioFitter : MonoBehaviour
{
    [SerializeField] private Vector2Int targetRatio = Vector2Int.one;
    private Vector2Int _resolution;

    private void UpdateViewport() {
        
        _resolution = new Vector2Int(Screen.width, Screen.height);
        if (_resolution.x <= 0 || _resolution.y <= 0) // Ignore invalid sizes
            return;

        if (_resolution.y * targetRatio.x < _resolution.x * targetRatio.y) { // Too wide

            float scale = (_resolution.y * targetRatio.x) / (float)(_resolution.x * targetRatio.y);
            GetComponent<Camera>().rect = new Rect((1 - scale) / 2f, 0, scale, 1);
        } else if (_resolution.y * targetRatio.x > _resolution.x * targetRatio.y) { // Too tall
            float scale = (_resolution.x * targetRatio.y) / (float)(_resolution.y * targetRatio.x);
            GetComponent<Camera>().rect = new Rect(0, (1 - scale) / 2f, 1, scale);

        } else { // Just right
            GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
        }
    }

    private void UpdateDirtyViewport() {
        if (_resolution.y != Screen.height || _resolution.x != Screen.width)
            UpdateViewport();
    }

    // void OnAwake() {
    //     UpdateViewport();        
    // }

    void LateUpdate() {
        UpdateViewport();
    }
}
