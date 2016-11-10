using UnityEngine;
using System.Collections;

public class RidingGauge : MonoBehaviour {
    [System.NonSerialized]
    public UISlider slider;

    void Start() {
        slider = gameObject.GetComponent<UISlider>();
    }

    public void onValueChange() {
        //slider.value = 
    }
}
