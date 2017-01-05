using UnityEngine;
using System;

public class Riding_VC : MonoBehaviour {
    public GameObject gpsPref;
    private GameObject gpsManager;
    private GameManager gameManager;

    public UILabel
        currSpeedLabel,
        avgSpeedLabel,
        distLabel,
        timeLabel;

    public Riding ridingStore;
    public User userStore;

    LocationInfo currentGPSPosition;

    void Start() {
        gameManager = GameManager.Instance;
    }

    void OnEnable() {
        gpsManager = Instantiate(gpsPref);
    }
    
    public void onRidingListener() {
        float currSpeed = ridingStore.curSpeed;
        float avgSpeed = ridingStore.avgSpeed;
        double dist = Math.Round(ridingStore.totalDist, 2);

        char delimeter = '.';
        string time = ridingStore.totalTime.ToString().Split(delimeter)[0];
        refreshTxt(currSpeed, avgSpeed, dist, time);

        if (ridingStore.eventType == ActionTypes.RIDING_END) {
            stopGPSReceive();
            Debug.Log("Stop GPS RECEIVE");
        }
    }

    public void refreshTxt(float currSpeed, float avgSpeed,double dist, string time){
        //Debug.Log("RIDING LISTENER");
        currSpeedLabel.text = (Math.Round(currSpeed, 2, MidpointRounding.AwayFromZero)).ToString() + " KM/H";
        avgSpeedLabel.text = (Math.Round(avgSpeed,2,MidpointRounding.AwayFromZero)).ToString() + " KM/H";
        distLabel.text = (Math.Round(dist,2,MidpointRounding.AwayFromZero)).ToString() + " KM";
        timeLabel.text = time;
    }

    public void ridingEnd() {
        RidingEndAction action = (RidingEndAction)ActionCreator.createAction(ActionTypes.RIDING_END);
        gameManager.gameDispatcher.dispatch(action);

        RidingResultAction resultAction = (RidingResultAction)ActionCreator.createAction(ActionTypes.RIDING_RESULT);
        resultAction.nickname = userStore.nickName;
        gameManager.gameDispatcher.dispatch(resultAction);
    }

    public void stopGPSReceive() {
        Destroy(gpsManager);
    }
}