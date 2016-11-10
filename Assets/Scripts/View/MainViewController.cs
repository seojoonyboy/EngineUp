using UnityEngine;
using System.Collections;

public class MainViewController : MonoBehaviour {
    public GameObject
        ridingPanel;

    public void onRidingPanel() {
        ridingPanel.SetActive(true);
    }

    public void offRidingPanel() {
        ridingPanel.SetActive(false);
        Actions act = ActionCreator.createAction(ActionTypes.RIDING_START);
        GameManager.Instance.gameDispatcher.dispatch(act);
    }
}