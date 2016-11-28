using UnityEngine;
using System.Collections;

public class MainViewController : MonoBehaviour {
    public GameObject
        ridingPanel,
        uploadPanel,
        avatar;

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
}