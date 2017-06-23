using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBtnController : MonoBehaviour {
    public GameObject[] 
        activeImages,
        activeLabels,
        deaciveImages,
        deactiveLabels;

    public GameObject[] panels;
    public GameObject notifyModal;

    public void buttonPressListener(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        activeImages[index].SetActive(true);
        activeLabels[index].SetActive(true);
        deaciveImages[index].SetActive(false);
        deactiveLabels[index].SetActive(false);
    }

    public void buttonReleaseListener(GameObject obj) {
        var buttonIndex = obj.GetComponent<ButtonIndex>();
        if (buttonIndex != null) {
            int index = obj.GetComponent<ButtonIndex>().index;
            activeImages[index].SetActive(false);
            activeLabels[index].SetActive(false);
            deaciveImages[index].SetActive(true);
            deactiveLabels[index].SetActive(true);
        }

        var btnEnum = obj.GetComponent<MainButtonEnum>();
        var type = btnEnum.buttonType;

        switch (type) {
            case MainButtonEnum.Type.MYHOME:
                panels[0].GetComponent<MyHomeViewController>().onPanel();
                break;
            case MainButtonEnum.Type.COMMUNITY:
                notifyModal.SetActive(true);
                break;
            case MainButtonEnum.Type.RIDING:
                panels[2].GetComponent<Riding_VC>().onPanel();
                break;
            case MainButtonEnum.Type.BOX:
                panels[3].GetComponent<BoxViewController>().onPanel();
                break;
            case MainButtonEnum.Type.STORE:
                notifyModal.SetActive(true);
                break;
            case MainButtonEnum.Type.OPTION:
                panels[5].GetComponent<OptionController>().onPanel();
                break;
            case MainButtonEnum.Type.MYINFO:
                panels[4].GetComponent<StatViewController>().onPanel();
                break;
        }
    }

    public void offNotifyModal() {
        notifyModal.SetActive(false);
    }
}
