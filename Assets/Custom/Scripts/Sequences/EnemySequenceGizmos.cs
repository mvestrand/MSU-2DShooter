using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EnemySequenceGizmos : MonoBehaviour
{
    // private static bool _haveBeenDrawn = false;

    [SerializeField] private Rect cam = new Rect();


    void Reset() {
        // _haveBeenDrawn = false;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos() {
        // if (_haveBeenDrawn)
        //     return;
        // else {
        //     StartCoroutine(UnsetOnFrameEnd());
        //     _haveBeenDrawn = true;
        // }
        Gizmos.color = Color.white;
        DrawRect(cam);
        Gizmos.color = Color.blue * new Color(0.75f, 0.75f, 0.75f, 1);
        DrawLine(cam, .25f, .25f, 0, .75f);
        DrawLine(cam, .75f, .75f, 0, .75f);
        // DrawVerticalLine(cam, 0.25f);
        // DrawVerticalLine(cam, 0.75f);
        DrawHorizontalLine(cam, 0.25f);

        Gizmos.color = Color.blue;
        DrawLine(cam, .5f, .5f, 0, .75f);

        // DrawVerticalLine(cam, 0.5f);
        DrawHorizontalLine(cam, 0.5f);

        Gizmos.color = Color.red;
        DrawHorizontalLine(cam, 0.75f);
        DrawLine(cam, .5f, .5f, .75f, 1);
        DrawLine(cam, .25f, .25f, .75f, 1);
        DrawLine(cam, .75f, .75f, .75f, 1);


        //Gizmos.DrawLine(new Vector3(cam.x, cam.size.y / 4, 0), new Vector3(cam.xMax, cam.y / 2, 0));
        //Gizmos.DrawLine(new Vector3(cam.x, cam.y / 2, 0), new Vector3(cam.xMax, cam.y / 2, 0));
    }


    private void DrawLine(in Rect rect, float txMax, float txMin, float tyMin, float tyMax) {
        float xMin = Mathf.Lerp(rect.xMin, rect.xMax, txMax);
        float xMax = Mathf.Lerp(rect.xMin, rect.xMax, txMin);
        float yMin = Mathf.Lerp(rect.yMin, rect.yMax, tyMin);
        float yMax = Mathf.Lerp(rect.yMin, rect.yMax, tyMax);
        Gizmos.DrawLine(new Vector3(xMin, yMin, 0), new Vector3(xMax, yMax, 0));
    }

    private void DrawVerticalLine(in Rect rect, float t) {
        DrawLine(rect, t, t, 0, 1);
    }

    private void DrawHorizontalLine(in Rect rect, float t) {
        DrawLine(rect, 0, 1, t, t);
    }

    private void DrawRect(in Rect rect) {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }

    // IEnumerator UnsetOnFrameEnd() {
    //     yield return new WaitForEndOfFrame();
    //     _haveBeenDrawn = false;
    // }
}
