using UnityEngine;
using System;

public class MainBtnController : MonoBehaviour {
    public GameObject
        ridingPanel,
        communityPanel,
        avatar,
        topPanel;

    private Riding ridingStore;
    private User userStore;
    private TopView topView;

    void Start() {
        topPanel = gameObject.transform.Find("TopPanel").gameObject;
        topView = topPanel.GetComponent<TopView>();
        
        ridingStore = GameManager.Instance.ridingStore;
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
        userStore.addListener(userListener);
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