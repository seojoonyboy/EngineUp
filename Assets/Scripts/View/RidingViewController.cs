using UnityEngine;
using System.Collections;
using System;

public class RidingViewController : MonoBehaviour {
    public GameObject gpsPref;
    private GameObject gpsManager;

    public UILabel
        currSpeedLabel,
        avgSpeedLabel,
        distLabel,
        timeLabel;

    private Riding ridingStore;
    LocationInfo currentGPSPosition;

    void OnEnable() {
        gpsManager = Instantiate(gpsPref);
    }

    void OnDisable() {
        Destroy(gpsManager);
    }

    void Start(){
        MainSceneManager msm = Camera.main.GetComponent<MainSceneManager>();
        ridingStore = msm.ridingStore;
        ridingStore.addListener(ridingListiner);
    }

    void ridingListiner(){
        //Debug.Log("야호");
        currSpeedLabel.text = ridingStore.curSpeed.ToString() + " KM/H";
        avgSpeedLabel.text = ridingStore.avgSpeed.ToString() + " KM/H";
        distLabel.text = Math.Round(ridingStore.totalDist, 2).ToString() + " KM";

        char delimeter = '.';
        timeLabel.text = ridingStore.totalTime.ToString().Split(delimeter)[0];
    }
}