using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class ScrollingBox : MonoBehaviour
{
    [SerializeField] private float height;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private SpriteSet spriteSet;
    [SerializeField] private bool runInEditor = false;
    [Range(0f, 1f)]
    public float transparency = 1.0f;


    // Update is called once per frame
    void Update() {
        if (Application.isEditor && !Application.isPlaying && !runInEditor)
            return;
        if (height <= 0)
            return;
            
        foreach (Transform tf in transform) {
            float y = tf.localPosition.y - scrollSpeed * Time.deltaTime;
            if (y <= -height / 2) {
                y += height;
                if (tf.tag == "BackgroundPanel" && tf.TryGetComponent<SpriteRenderer>(out var r)) {
                    if (spriteSet != null)
                        spriteSet.ChangeSprite(r);
                    else {
                        r.sprite = null;
                    }
                }
            } else if (y > height / 2) {
                y -= height;
            }
            tf.localPosition = new Vector3(tf.localPosition.x, y, tf.localPosition.z);

            if (tf.tag == "BackgroundPanel" && tf.TryGetComponent<SpriteRenderer>(out var renderer))
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, transparency);                
        }
    }
}
