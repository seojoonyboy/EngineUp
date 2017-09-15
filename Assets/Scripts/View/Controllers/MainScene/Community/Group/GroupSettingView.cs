using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupSettingView : MonoBehaviour {
    public GroupViewController controller;
    private Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    void OnEnable() {
        Invoke("playSlideIn", 0.2f);
    }

    void playSlideIn() {
        animator.Play("SlideIn");
    }

    public void onBackButton() {
        animator.Play("SlideOut");
    }

    public void slideFinished(AnimationEvent animationEvent) {
        int boolParm = animationEvent.intParameter;

        //slider in
        if (boolParm == 1) {

        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
    }
}
