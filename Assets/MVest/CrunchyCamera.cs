using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[ExecuteAlways]
public class CrunchyCamera : MonoBehaviour {

    [SerializeField] private RenderTexture _target;
    private Vector2Int oldResolution = Vector2Int.zero;

    void Awake() {
        UpdateResolution();
    }

    void Update() {
        UpdateResolution();
    }

    private void UpdateResolution() {
        if (oldResolution.x == Screen.width && oldResolution.y == Screen.height)
            return;
        if (Screen.width > _target.width || Screen.height > _target.height) {
            this.transform.localScale = new Vector3(_target.width / (float)_target.height, 1, 1);
            return;
        }
        //Debug.Assert(Screen.width > _target.width && Screen.height > _target.height, "Screen is not large enough to render even in 1:1 scale.");
        Vector2Int maxUpscale = new Vector2Int(Screen.width / _target.width, Screen.height / _target.height);
        int upscale = Math.Min(maxUpscale.x, maxUpscale.y);
        Vector3 scale = new Vector3(
            (_target.width * upscale) / (float)Screen.height,
            (_target.height * upscale) / (float)Screen.height, 1);
        this.transform.localScale = scale;
        oldResolution.x = Screen.width;
        oldResolution.y = Screen.height;
        //Debug.LogFormat("Upscaling: {0} pixels per render pixel", upscale);
    }
}
