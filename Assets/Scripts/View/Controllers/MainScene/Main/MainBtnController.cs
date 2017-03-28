using UnityEngine;
using System;

public class MainBtnController : MonoBehaviour {
    public GameObject
        ridingPanel,
        communityPanel,
        optionPanel,
        avatar,
        garagePanel;

    public void onMainBtnEvent(MAIN_BUTTON type) {
        switch(type) {
            case MAIN_BUTTON.RIDING:
                ridingPanel.SetActive(true);
                break;

            case MAIN_BUTTON.COMMUNITY:
                communityPanel.SetActive(true);
                GetMyFriendListAction initAct = ActionCreator.createAction(ActionTypes.GET_MY_FRIEND_LIST) as GetMyFriendListAction;
                GameManager.Instance.gameDispatcher.dispatch(initAct);
                break;

            case MAIN_BUTTON.OPTION:
                optionPanel.SetActive(true);
                break;
            case MAIN_BUTTON.GARAGE:
                garagePanel.SetActive(true);
                break;
        }
    }

    public void offCommunityPanel() {
        communityPanel.SetActive(false);
    }

    public void onAvatar() {
        avatar.SetActive(true);
    }

    public void offToggleGroup() {
        UIToggle toggle = UIToggle.GetActiveToggle(2);
        toggle.value = false;
    }
}