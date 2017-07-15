using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RidingStartAnimController : MonoBehaviour {
    public Riding_VC controller;

    private IEnumerator coroutine;
    public Text label;
    public int count;

    public void startAnim() {
        coroutine = anim(1);
        StartCoroutine(coroutine);
    }

    private IEnumerator anim(float waitTime) {
        for(int i=count; i>=0; i--) {
            label.text = i.ToString();
            yield return new WaitForSeconds(waitTime);
        }
        StartCoroutine(controller.ridingStart());
    }
}
