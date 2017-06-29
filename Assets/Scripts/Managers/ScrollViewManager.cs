using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewManager : MonoBehaviour {
    public ScrollRect scrollView;
    
    void OnEnable() {
        scrollView.enabled = false;
    }

    void OnDisable() {
        scrollView.enabled = true;
    }
}
