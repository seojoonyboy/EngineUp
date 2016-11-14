using UnityEngine;
using System.Collections;

public class MainViewController : MonoBehaviour {
    public GameObject
        ridingPanel,
        uploadPanel;

    public void onRidingPanel() {
        Actions act = ActionCreator.createAction(ActionTypes.RIDING_START);
        GameManager.Instance.gameDispatcher.dispatch(act);
        ridingPanel.SetActive(true);
    }

    public void offRidingPanel() {
        ridingPanel.SetActive(false);
    }

    public void onUploadPanel() {
        uploadPanel.SetActive(true);
    }

    public void offUploadPanel() {
        uploadPanel.SetActive(false);
    }
}