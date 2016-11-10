using UnityEngine;
using System.Collections;
using System;

public class RidingViewController : MonoBehaviour {
    public GameObject gpsPref;
    private GameObject gpsManager;
    public UILabel
        latitudeLabel,
        longitudeLabel,
        accuracyLabel,
        distLabel;
    private float
        latitude,
        longitude,
        distance,
        time;
    private LocationState state;

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
        Debug.Log("야호");
    }

    // IEnumerator DistBetweenLoc() {
    //     while(state == LocationState.Enabled) {
    //         currentGPSPosition = Input.location.lastData;
    //         latitude = currentGPSPosition.latitude;
    //         longitude = currentGPSPosition.longitude;
    //         //3초 간격으로 위치 갱신, 거리 계산
    //         yield return new WaitForSeconds(3.0f);

    //         float deltaDist = calcDist(ref latitude, ref longitude) * 1000f;

    //         if (deltaDist > 0) {
    //             distance += deltaDist;
    //         }

    //         distLabel.text = distance.ToString() + "m";
    //         latitudeLabel.text = latitude.ToString();
    //         longitudeLabel.text = longitude.ToString();
    //         accuracyLabel.text = currentGPSPosition.horizontalAccuracy.ToString();
    //     }
    // }


}