using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideMove : MonoBehaviour {
    public float speed = 1f;

    private bool 
        isOn = false,
        isFinished = false;
    Vector3 leftPos = new Vector3(-122, 0, 0);
    Vector3 rightPos = new Vector3(122, 0, 0);
    float time;
    UILabel label;

    void Start() {
        label = gameObject.transform.Find("InnerImage/Label").GetComponent<UILabel>();
    }

    public void onPressed() {
        if(!isFinished) {
            isOn = GetComponent<boolIndex>().isOn;
            time = 0;
            StartCoroutine("Slide");
        }
    }

    IEnumerator Slide() {
        while(!isFinished) {
            Vector3 pos = transform.localPosition;
            time += Time.deltaTime;
            if (!isOn) {
                transform.localPosition = Vector3.Lerp(rightPos, leftPos, time * speed);
                if (pos.x <= leftPos.x) {
                    StopCoroutine("Slide");
                    GetComponent<boolIndex>().isOn = true;
                    label.text = "ON";
                }
            }
            else {
                transform.localPosition = Vector3.Lerp(leftPos, rightPos, time * speed);
                if (pos.x >= rightPos.x) {
                    StopCoroutine("Slide");
                    GetComponent<boolIndex>().isOn = false;
                    label.text = "OFF";
                }
            }
            yield return 0;
        }
    }
}