using UnityEngine;
using System.Collections;
using System;

public class MainViewController : MonoBehaviour {
    public GameObject
        ridingPanel,
        uploadPanel,
        avatar;

    public RidingView ridingView;
    private Riding ridingStore;

    void Start() {    
        MainSceneManager msm = Camera.main.GetComponent<MainSceneManager>();
        ridingStore = msm.ridingStore;

        addListener();        
    }

    public void onMainBtnEvent(MAIN_BUTTON type) {
        switch(type) {
            case MAIN_BUTTON.RIDING:
            Actions act = ActionCreator.createAction(ActionTypes.RIDING_START);
            GameManager.Instance.gameDispatcher.dispatch(act);
            ridingPanel.SetActive(true);
            avatar.SetActive(false);
            break;
        }
    }

    public void onUploadPanel() {
        ridingPanel.SetActive(false);
        uploadPanel.SetActive(true);
    }

    public void offUploadPanel() {
        avatar.SetActive(true);
        uploadPanel.SetActive(false);
    }

    void addListener() {
        ridingStore.addListener(ridingListener);
        ridingView = ridingPanel.GetComponent<RidingView>();
    }

    void ridingListener() {
        float currSpeed = ridingStore.curSpeed;
        float avgSpeed = ridingStore.avgSpeed;
        double dist = Math.Round(ridingStore.totalDist,2);

        char delimeter = '.';
        string time = ridingStore.totalTime.ToString().Split(delimeter)[0];
        ridingView.ridingListiner(currSpeed, avgSpeed, dist, time);
    }
}