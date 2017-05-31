using UnityEngine;
using System;

public class MainBtnController : MonoBehaviour {
    public GameObject
        ridingPanel,
        communityPanel,
        optionPanel,
        avatar,
        myhomePanel,
        myInfoPanel,
        boxPanel,
        notifyModal;

    public void onMainBtnEvent(MAIN_BUTTON type) {
        switch(type) {
            case MAIN_BUTTON.RIDING:
                ridingPanel.SetActive(true);
                break;
            case MAIN_BUTTON.COMMUNITY:
                notifyModal.SetActive(true);
                //communityPanel.SetActive(true);
                //GetMyFriendListAction initAct = ActionCreator.createAction(ActionTypes.GET_MY_FRIEND_LIST) as GetMyFriendListAction;
                //GameManager.Instance.gameDispatcher.dispatch(initAct);
                break;
            case MAIN_BUTTON.OPTION:
                optionPanel.SetActive(true);
                break;
            case MAIN_BUTTON.MYHOME:
                myhomePanel.SetActive(true);
                break;
            case MAIN_BUTTON.MYINFO:
                myInfoPanel.SetActive(true);
                break;
            case MAIN_BUTTON.BOX:
                boxPanel.SetActive(true);
                break;
            case MAIN_BUTTON.SHOP:
                notifyModal.SetActive(true);
                break;
        }
    }

    public void offCommunityPanel() {
        communityPanel.SetActive(false);
    }

    public void onAvatar() {
        avatar.SetActive(true);
    }

    public void offNotifyModal() {
        notifyModal.SetActive(false);
    }
}