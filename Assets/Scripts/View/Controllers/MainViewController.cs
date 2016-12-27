using UnityEngine;
using System;

public class MainViewController : MonoBehaviour {
    public GameObject
        ridingPanel,
        communityPanel,
        avatar,
        topPanel;

    private RidingView ridingView;
    private Riding ridingStore;
    private User userStore;
    private TopView topView;

    void Start() {
        topPanel = gameObject.transform.Find("TopPanel").gameObject;
        topView = topPanel.GetComponent<TopView>();
        
        MainSceneManager msm = Camera.main.GetComponent<MainSceneManager>();
        ridingStore = msm.ridingStore;
        userStore = GameManager.Instance.userStore;

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

            case MAIN_BUTTON.COMMUNITY:
            communityPanel.SetActive(true);
            break;
        }
    }

    public void offRidingPanel() {
        ridingPanel.SetActive(false);
    }

    public void offCommunityPanel() {
        communityPanel.SetActive(false);
    }

    void addListener() {
        ridingView = ridingPanel.GetComponent<RidingView>();
        ridingStore.addListener(ridingListener);
        userStore.addListener(userListener);
    }

    void ridingListener() {
        //Debug.Log("MAIN VIEW RIDING LISTENER");
        float currSpeed = ridingStore.curSpeed;
        float avgSpeed = ridingStore.avgSpeed;
        double dist = Math.Round(ridingStore.totalDist,2);

        char delimeter = '.';
        string time = ridingStore.totalTime.ToString().Split(delimeter)[0];
        ridingView.refreshTxt(currSpeed, avgSpeed, dist, time);

        if(!ridingStore.isRiding) {
            ridingView.stopGPSReceive();
        }
    }

    void userListener() {
        topView.setNickName(userStore.nickName);
    }

    public void onAvatar() {
        avatar.SetActive(true);
    }

    public void offAvatar() {
        avatar.SetActive(false);
    }
}