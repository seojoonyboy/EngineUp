using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class history_refresh : MonoBehaviour {
    UIPanel panel;
    UIScrollView scrollView;
    float startTime, time = 0;
    public HistoryViewController controller;
    public bool flag;

    void Start() {
        panel = GetComponent<UIPanel>();
        scrollView = GetComponent<UIScrollView>();
        scrollView.onDragStarted += OnDragStart;
        scrollView.onDragFinished += OnDragFinished;
    }

    void OnEnable() {
        flag = true;
        time = 0;
    }

    // Update is called once per frame
    void Update() {
        if (scrollView.isDragging) {
            Vector3 constraint = panel.CalculateConstrainOffset(scrollView.bounds.min, scrollView.bounds.max);
            if (constraint.y < 0) {
                time = Time.time - startTime;
                if (time >= 0.5f && flag) {
                    flag = false;
                    time = 0;
                    Debug.Log("추가로딩");
                    controller.getRidingDataSets();
                }
            }
        }
    }

    void OnDragStart() {
        startTime = Time.time;
    }

    void OnDragFinished() {
        flag = true;
    }
}
