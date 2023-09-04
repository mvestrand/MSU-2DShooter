using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class ScrollingBox : MonoBehaviour
{
    [SerializeField] private BackgroundController controller;
    [SerializeField] private float height;
    [SerializeField] private float scrollSpeed;

    [ContextMenuItem("Randomize sprites", "RandomizeSprites")]
    [SerializeField] public List<SpriteSet> spriteSets = new List<SpriteSet>();
    [SerializeField] private bool runInEditor = false;
    [Range(0f, 1f)]
    public float transparency = 1.0f;
    public int spritesIndex = 0;

    public bool randomizeX = false;
    public float minX = -10;
    public float maxX = 10;



    // Update is called once per frame
    void Update() {
        if (Application.isEditor && !Application.isPlaying && !runInEditor)
            return;
        if (height <= 0)
            return;
            
        foreach (Transform tf in transform) {
            float deltaY = scrollSpeed * Time.deltaTime * (controller != null ? controller.SpeedMultiplier : 1);
            float y = tf.localPosition.y - deltaY;
            float x = tf.localPosition.x;
            if (y <= -height / 2) {
                if (tf.tag == "BackgroundPanel" && tf.TryGetComponent<SpriteRenderer>(out var r)) {
                    y += height;
                    if (SpriteSetIsValid())
                        spriteSets[spritesIndex].ChangeSprite(r);
                    else {
                        r.sprite = null;
                    }
                    if (randomizeX) {
                        x = Random.Range(minX, maxX);
                    }
                } else {
                    Destroy(tf.gameObject);
                }
            } else if (y > height / 2) {
                y -= height;
            }
            tf.localPosition = new Vector3(x, y, tf.localPosition.z);

            if (tf.tag == "BackgroundPanel" && tf.TryGetComponent<SpriteRenderer>(out var renderer))
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, transparency);                
        }
    }

    // public void SetSpriteSet(SpriteSet newSet) {
    //     spriteSet = newSet;
    // }

    public void RandomizeSprites() {
        foreach (Transform tf in transform) {
            if (tf.tag == "BackgroundPanel" && tf.TryGetComponent<SpriteRenderer>(out var r)) {
                if (SpriteSetIsValid())
                    spriteSets[spritesIndex].ChangeSprite(r);
                else
                    r.sprite = null;
            }
        }
    }


    private bool SpriteSetIsValid() {
        return spriteSets != null && spritesIndex >= 0 && spritesIndex < spriteSets.Count && spriteSets[spritesIndex] != null;
    }

    protected void OnDrawGizmosSelected() {
        Vector3 p0 = transform.TransformPoint(minX, height / 2, 0);
        Vector3 p1 = transform.TransformPoint(maxX, height / 2, 0);
        Vector3 p2 = transform.TransformPoint(maxX, -height / 2, 0);
        Vector3 p3 = transform.TransformPoint(minX, -height / 2, 0);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p0);

    }
}
