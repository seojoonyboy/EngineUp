using UnityEngine;
using System;

public class MainBtnController : MonoBehaviour {
    public GameObject
        ridingPanel,
        communityPanel,
        avatar,
        topPanel;

    void Start() {
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

    public void onAvatar() {
        avatar.SetActive(true);
    }

    public void offAvatar() {
        avatar.SetActive(false);
    }
}