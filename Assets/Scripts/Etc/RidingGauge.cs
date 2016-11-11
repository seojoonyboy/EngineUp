using UnityEngine;
using System.Collections;

public class RidingGauge : MonoBehaviour {
    [System.NonSerialized]
    public UISlider slider;

    private Riding ridingStore;
    private int maxSpeed = 40;
    void Start() {
        slider = gameObject.GetComponent<UISlider>();

        MainSceneManager msm = Camera.main.GetComponent<MainSceneManager>();
        ridingStore = msm.ridingStore;
        ridingStore.addListener(ridingListiner);
    }

    public void onValueChange() {
        //slider.value = 
    }

    void ridingListiner() {
        //Debug.Log(ridingStore.curSpeed);
        slider.value = ridingStore.curSpeed / maxSpeed;
        //slider.value = 1;
    }
}
