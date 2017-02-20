using UnityEngine;
using System;

public class Riding_VC : MonoBehaviour {
    public GameObject 
        gpsPref,
        pauseModal,
        onPauseImg,
        onPauseLabel;

    private bool isPausePressed = false;

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

    void OnDisable() {
        isPausePressed = false;
        pauseModal.SetActive(isPausePressed);
        onPauseLabel.SetActive(isPausePressed);
        onPauseImg.SetActive(isPausePressed);
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
        //currSpeedLabel.text = (Math.Round(currSpeed, 2, MidpointRounding.AwayFromZero)).ToString() + " KM/H";
        avgSpeedLabel.text = (Math.Round(avgSpeed,2,MidpointRounding.AwayFromZero)).ToString() + " KM/H";
        distLabel.text = (Math.Round(dist,2,MidpointRounding.AwayFromZero)).ToString() + " KM";
        timeLabel.text = time;
    }

    public void ridingEnd() {
        gameObject.SetActive(false);

        RidingEndAction action = (RidingEndAction)ActionCreator.createAction(ActionTypes.RIDING_END);
        gameManager.gameDispatcher.dispatch(action);

        RidingResultAction resultAction = (RidingResultAction)ActionCreator.createAction(ActionTypes.RIDING_RESULT);
        resultAction.nickname = userStore.nickName;
        gameManager.gameDispatcher.dispatch(resultAction);
    }

    public void stopGPSReceive() {
        Destroy(gpsManager);
    }

    public void pauseButtonPressed() {
        //이미 일시정지 버튼을 누른 상태인 경우
        if (isPausePressed) {
            isPausePressed = false;
        }
        else {
            isPausePressed = true;
        }
        pauseModal.SetActive(isPausePressed);
        onPauseImg.SetActive(isPausePressed);
        onPauseLabel.SetActive(isPausePressed);
    }
}