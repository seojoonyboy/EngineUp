using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PagingButton : MonoBehaviour {
    public UIScrollView scrollView;
    public UIGrid grid;
    public float springStrength = 8.0f;

    private int elementsPerPage;
    private float scrollAmount;

    private SpringPanel springpanel = null;
    // Use this for initialization
    void Start () {
        elementsPerPage = grid.transform.childCount;
        scrollAmount = grid.cellWidth;
        springpanel = scrollView.GetComponent<SpringPanel>();

        if(springpanel == null) {
            springpanel = scrollView.gameObject.AddComponent<SpringPanel>();
        }
        scrollView.onStoppedMoving += OnStoppedMoving;
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    private void OnStoppedMoving() {
        //note:  as suggested above - it is a better idea to use the scrollview's panel data (rather than the springpanel data) to calculate what object is visible.

        int pagewidth = 320;
        int pageposition = (int)springpanel.target.x;
        int page = Math.Abs(pageposition / pagewidth) + 1;

        print("page " + (page));
    }

    public void OnPress(GameObject obj) {
        int num = obj.GetComponent<ButtonIndex>().index;
        Vector3 target;
        switch(num) {
            case 0:
                target = new Vector3(479f, 0);
                springpanel.target = target;
                break;
            case 1:
                target = new Vector3(-514.7f, 0);
                springpanel.target = target;
                break;
            case 2:
                target = new Vector3(-1508.54f, 0);
                springpanel.target = target;
                break;
        }
        springpanel.enabled = true;
        }
}