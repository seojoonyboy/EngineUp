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

public class MainButtonListener : MonoBehaviour {
    public UIButton[] mainBtns;
    void Start() {
        addMainBtnEvent(0,MAIN_BUTTON.RIDING);
    }

    private void addMainBtnEvent(int index,MAIN_BUTTON type) {
        EventDelegate eventBtn = new EventDelegate(gameObject.GetComponent<MainViewController>(),"onMainBtnEvent");

        eventBtn.parameters[0].value = type;

        EventDelegate.Set(mainBtns[index].onClick,eventBtn);
        EventDelegate.Add(mainBtns[index].GetComponent<UIButton>().onClick,eventBtn);
    }
}
