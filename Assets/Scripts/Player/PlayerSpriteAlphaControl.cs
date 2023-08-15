using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteAlphaControl : MonoBehaviour {

    [SerializeField] private Controller controller;
    [SerializeField] private SpriteRenderer sr;

    // Update is called once per frame
    void Update() {
        if (sr == null || controller == null)
            return;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, controller.focus);

    }
}
