using UnityEngine;
using System.Collections;

public enum MAIN_BUTTON {
    HOME,
    GARAGE,
    COMMUNITY,
    SHOP,
    RIDING,
    BATTLE,
    OPTION,
    MYHOME,
    MYINFO,
    BOX
};

public class MainButtonListener : MonoBehaviour {
    public UIButton[] mainBtns;
    void Start() {
        addMainBtnEvent(0,MAIN_BUTTON.RIDING);
        addMainBtnEvent(1, MAIN_BUTTON.COMMUNITY);
        addMainBtnEvent(2, MAIN_BUTTON.OPTION);
        addMainBtnEvent(3, MAIN_BUTTON.MYHOME);
        addMainBtnEvent(4, MAIN_BUTTON.MYINFO);
        addMainBtnEvent(5, MAIN_BUTTON.BOX);
        addMainBtnEvent(6, MAIN_BUTTON.SHOP);
        addMainBtnEvent(7, MAIN_BUTTON.OPTION);
    }

    private void addMainBtnEvent(int index,MAIN_BUTTON type) {
        EventDelegate eventBtn = new EventDelegate(gameObject.GetComponent<MainBtnController>(),"onMainBtnEvent");

        eventBtn.parameters[0].value = type;

        EventDelegate.Set(mainBtns[index].onClick,eventBtn);
        EventDelegate.Add(mainBtns[index].GetComponent<UIButton>().onClick,eventBtn);
    }
}
