﻿using UnityEngine;
using System.Collections;
using System;

public class RidingView : MonoBehaviour {
    public GameObject gpsPref;
    private GameObject gpsManager;
    private GameManager gameManager;

    public UILabel
        currSpeedLabel,
        avgSpeedLabel,
        distLabel,
        timeLabel;

    private Riding ridingStore;
    LocationInfo currentGPSPosition;

    void Start() {
        gameManager = GameManager.Instance;
    }

    void OnEnable() {
        gpsManager = Instantiate(gpsPref);
    }

    void OnDisable() {
        Destroy(gpsManager);
    }

    public void ridingListiner(float currSpeed, float avgSpeed,double dist, string time){
        currSpeedLabel.text = currSpeed.ToString() + " KM/H";
        avgSpeedLabel.text = avgSpeed.ToString() + " KM/H";
        distLabel.text = dist.ToString() + " KM";
        timeLabel.text = time;
    }

    public void rigingEnd() {
        RidingEndAction action = (RidingEndAction)ActionCreator.createAction(ActionTypes.RIDING_END);
        gameManager.gameDispatcher.dispatch(action);
    }
}