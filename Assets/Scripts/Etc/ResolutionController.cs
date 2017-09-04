using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionController : MonoBehaviour {
    public Vector2 standard_resolution;
    public Camera camera;

    private float rate;

    void Start() {
        rate = standard_resolution.y / standard_resolution.x;

        float mobile_rate = (float)Screen.height / (float)Screen.width;

        //위아래가 길어지는 경우
        if(mobile_rate > rate) {
            float h = rate / mobile_rate;

            Rect fixR = camera.GetComponent<Camera>().GetComponent<Camera>().rect;
            fixR.y = ((1 - h) / 2) + (h * fixR.y);
            fixR.height *= h;
            camera.GetComponent<Camera>().GetComponent<Camera>().rect = fixR;

            camera.GetComponent<Camera>().rect = new Rect(0, (1 - h) / 2, 1, h);
        }

        //좌우로 늘어나는 경우
        else {
            float w = mobile_rate / rate;

            Rect fixR = camera.GetComponent<Camera>().GetComponent<Camera>().rect;
            fixR.x = ((1 - w) / 2) + (w * fixR.x);
            fixR.width *= w;
            camera.GetComponent<Camera>().GetComponent<Camera>().rect = fixR;

            camera.GetComponent<Camera>().rect = new Rect((1 - w) / 2, 0, w, 1);
        }
    }
}
