using UnityEngine;
using System.Collections;

public class RidingGauge : MonoBehaviour {
    [System.NonSerialized]
    public UISlider slider;

    private Riding ridingStore;
    private int maxSpeed = 40;
    void Start() {
        slider = gameObject.GetComponent<UISlider>();

        ridingStore = GameManager.Instance.ridingStore;
        ridingStore.addListener(ridingListiner);
    }

    public void onValueChange() {
        //slider.value = 
    }

    void ridingListiner() {
        //Debug.Log(ridingStore.curSpeed);
        if(ridingStore.avgSpeed >= maxSpeed) {
            slider.value = maxSpeed;
        }
        else {
            slider.value = ridingStore.avgSpeed;
        }
    }
}
