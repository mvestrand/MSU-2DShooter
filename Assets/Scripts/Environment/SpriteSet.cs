using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="SpriteSet")]
public class SpriteSet : ScriptableObject {

    public List<Sprite> sprites = new List<Sprite>();
    public bool canFlipX = true;
    public bool canFlipY = true;

    public void ChangeSprite(SpriteRenderer renderer) {
        if (sprites.Count == 0) {
            renderer.sprite = null;
            return;
        }

        int i = Random.Range(0, sprites.Count);
        renderer.sprite = sprites[i];
        if (canFlipX)
            renderer.flipX = Random.Range(0, 2) == 1;
        else
            renderer.flipX = false;

        if (canFlipY)
            renderer.flipY = Random.Range(0, 2) == 1;
        else
            renderer.flipY = false;

    }

}
