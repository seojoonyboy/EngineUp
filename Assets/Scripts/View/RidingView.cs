using UnityEngine;
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
    private User userStore;

    LocationInfo currentGPSPosition;

    void Start() {
        gameManager = GameManager.Instance;
        ridingStore = Camera.main.GetComponent<MainSceneManager>().ridingStore;
        userStore = gameManager.userStore;
    }

    void OnEnable() {
        gpsManager = Instantiate(gpsPref);
    }

    void OnDisable() {
        Destroy(gpsManager);
    }

    public void refreshTxt(float currSpeed, float avgSpeed,double dist, string time){
        //Debug.Log("RIDING LISTENER");
        currSpeedLabel.text = currSpeed.ToString() + " KM/H";
        avgSpeedLabel.text = avgSpeed.ToString() + " KM/H";
        distLabel.text = dist.ToString() + " KM";
        timeLabel.text = time;
    }

    public void ridingEnd() {
        RidingEndAction action = (RidingEndAction)ActionCreator.createAction(ActionTypes.RIDING_END);
        gameManager.gameDispatcher.dispatch(action);

        RidingResultAction resultAction = (RidingResultAction)ActionCreator.createAction(ActionTypes.RIDING_RESULT);
        resultAction.nickname = userStore.nickName;
        Debug.Log(ridingStore.resultData);
        resultAction.data = ridingStore.resultData;
        gameManager.gameDispatcher.dispatch(resultAction);
    }
}