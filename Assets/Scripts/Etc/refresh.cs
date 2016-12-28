using UnityEngine;
using System.Collections;

public class refresh : MonoBehaviour {
    UIPanel panel;
    UIScrollView scrollView;
    float startTime, time = 0;
	// Use this for initialization
	void Start () {
        panel = GetComponent<UIPanel>();
        scrollView = GetComponent<UIScrollView>();
        scrollView.onDragStarted += OnDragStart;
    }
	
	// Update is called once per frame
	void Update () {
        if (scrollView.isDragging) {
            if(panel.clipOffset.y >= 300f) {
                time = Time.time - startTime;
                //Debug.Log(time);
                if(time >= 2) {
                    Refresh();
                }
            }
        }
	}

    void OnDragStart() {
        startTime = Time.time;
    }

    //act like mobile pull display top and refresh
    void Refresh() {
        Debug.Log("Refresh");
    }
}
