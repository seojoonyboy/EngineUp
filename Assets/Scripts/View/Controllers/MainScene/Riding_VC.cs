﻿#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414
using UnityEngine;
using System;
using System.Collections;

public class Riding_VC : MonoBehaviour {
    public GameObject 
        gpsPref,
        pauseModal,
        StartPanel,
        beforeStartModal_ButtonContainer,
        beforeStartModal_AnimContainer,
        ridingPanel;

    private GameObject gpsReceiver;

    private bool isPausePressed = false;

    //private GameObject gpsManager;
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
        //gosReceiver = Instantiate(gpsPref);
    }

    void OnDisable() {
        isPausePressed = false;
        pauseModal.SetActive(isPausePressed);
    }

    public void onRidingStartButton() {
        offBeforeStartModal();
        beforeStartModal_AnimContainer.SetActive(true);
    }

    public void offBeforeStartModal() {
        beforeStartModal_ButtonContainer.SetActive(false);
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

        stopGPSReceive();
    }

    public void stopGPSReceive() {
        Destroy(gpsReceiver);
    }

    public IEnumerator ridingStart() {
        ridingPanel.SetActive(true);
        yield return new WaitForSeconds(1);
        StartPanel.SetActive(false);
        beforeStartModal_ButtonContainer.SetActive(true);

        Actions act = ActionCreator.createAction(ActionTypes.RIDING_START);
        GameManager.Instance.gameDispatcher.dispatch(act);

        gpsReceiver = Instantiate(gpsPref);
    }

    public void pauseButtonPressed() {
        //이미 일시정지 버튼을 누른 상태인 경우
        if (isPausePressed) {
            isPausePressed = false;
            Time.timeScale = 1;
        }
        else {
            isPausePressed = true;
            Time.timeScale = 0;
        }
        pauseModal.SetActive(isPausePressed);
    }

    public void onRidingListener() {
        float currSpeed = ridingStore.curSpeed;
        float avgSpeed = ridingStore.avgSpeed;
        double dist = Math.Round(ridingStore.totalDist, 2);

        char delimeter = '.';
        //string time = ridingStore.totalTime.ToString().Split(delimeter)[0];
        string time = ridingStore.totalTime;
        refreshTxt(currSpeed, avgSpeed, dist, time);

        //if (ridingStore.eventType == ActionTypes.RIDING_END) {
        //    stopGPSReceive();
        //    Debug.Log("Stop GPS RECEIVE");
        //}
    }
}