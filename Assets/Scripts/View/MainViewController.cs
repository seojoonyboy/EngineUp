using UnityEngine;
using System.Collections;

public enum MAIN_BUTTON {
    HOME,
    ITEM,
    AVATAR,
    HISTORY,
    SHOP,
    RIDING,
    BATTLE
};

public class MainViewController : MonoBehaviour {
    public GameObject
        ridingPanel,
        uploadPanel,
        avatar;

    public UIButton[] mainBtns;
    
    void Start() {
        addMainBtnEvent(0,MAIN_BUTTON.RIDING);
    }

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

    private void addMainBtnEvent(int index, MAIN_BUTTON type) {
        EventDelegate eventBtn = new EventDelegate(gameObject.GetComponent<MainViewController>(),"onMainBtnEvent");

        eventBtn.parameters[0].value = type;

        EventDelegate.Set(mainBtns[index].onClick,eventBtn);
        EventDelegate.Add(mainBtns[index].GetComponent<UIButton>().onClick,eventBtn);
    }
}