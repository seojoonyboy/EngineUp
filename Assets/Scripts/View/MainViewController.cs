using UnityEngine;
using System.Collections;
using System;

public class MainViewController : MonoBehaviour {
    public GameObject
        ridingPanel,
        avatar;

    private RidingView ridingView;
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
            offAvatar();
            break;
        }
    }

    public void offRidingPanel() {
        ridingPanel.SetActive(false);
    }

    void addListener() {
        ridingView = ridingPanel.GetComponent<RidingView>();
        ridingStore.addListener(ridingListener);        
    }

    void ridingListener() {
        //Debug.Log("MAIN VIEW RIDING LISTENER");
        float currSpeed = ridingStore.curSpeed;
        float avgSpeed = ridingStore.avgSpeed;
        double dist = Math.Round(ridingStore.totalDist,2);

        char delimeter = '.';
        string time = ridingStore.totalTime.ToString().Split(delimeter)[0];
        ridingView.refreshTxt(currSpeed, avgSpeed, dist, time);
    }

    public void onAvatar() {
        avatar.SetActive(true);
    }

    public void offAvatar() {
        avatar.SetActive(false);
    }
}