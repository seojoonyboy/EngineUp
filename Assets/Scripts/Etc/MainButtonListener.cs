using UnityEngine;
using System.Collections;

public enum MAIN_BUTTON {
    HOME,
    ITEM,
    AVATAR,
    COMMUNITY,
    SHOP,
    RIDING,
    BATTLE
};

public class MainButtonListener : MonoBehaviour {
    public UIButton[] mainBtns;
    void Start() {
        addMainBtnEvent(0,MAIN_BUTTON.RIDING);
        addMainBtnEvent(1, MAIN_BUTTON.COMMUNITY);
    }

    private void addMainBtnEvent(int index,MAIN_BUTTON type) {
        EventDelegate eventBtn = new EventDelegate(gameObject.GetComponent<MainBtnController>(),"onMainBtnEvent");

        eventBtn.parameters[0].value = type;

        EventDelegate.Set(mainBtns[index].onClick,eventBtn);
        EventDelegate.Add(mainBtns[index].GetComponent<UIButton>().onClick,eventBtn);
    }
}
