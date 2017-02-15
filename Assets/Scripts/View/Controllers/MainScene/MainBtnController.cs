using UnityEngine;
using System;

public class MainBtnController : MonoBehaviour {
    public GameObject
        ridingPanel,
        communityPanel,
        optionPanel,
        avatar;

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
                GetMyFriendListAction initAct = ActionCreator.createAction(ActionTypes.GET_MY_FRIEND_LIST) as GetMyFriendListAction;
                GameManager.Instance.gameDispatcher.dispatch(initAct);
                break;

            case MAIN_BUTTON.OPTION:
                optionPanel.SetActive(true);
                break;
        }
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