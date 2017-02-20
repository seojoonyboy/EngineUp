using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GroupAddViewController : MonoBehaviour {
    public UIPopupList
        provinceMenu,
        cityMenu;

    private enum PROVINCE { };

    void Start() {
        PROVINCE AAA = (PROVINCE)1;

        provinceMenu.items = new List<string>() {
            "aaa"
        };
    }

    public void provinceSelected() {
        Debug.Log("지역 선택");
    }

    private void switchCity() {

    }
}
