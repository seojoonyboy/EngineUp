using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fbl_Toggle : MonoBehaviour {
    public GameObject[] target;
    public GameObject[] buttons;

    public void onClick(GameObject obj) {
        Toggle(obj);
        offPanel(obj);
    }

    void OnEnable() {
        for (int i = 0; i < target.Length; i++) {
            fbl_ToggleObjects toggleObj = target[i].GetComponent<fbl_ToggleObjects>();
            if(toggleObj.isFirstActive) {
                set(target[i], true);
                for (int j = 0; j < toggleObj.active.Length; j++) {
                    set(toggleObj.active[j], true);
                }
            }
            else {
                set(target[i], false);
                for (int j = 0; j < toggleObj.active.Length; j++) {
                    set(toggleObj.active[j], false);
                }
                for (int j = 0; j < toggleObj.deactive.Length; j++) {
                    set(toggleObj.deactive[j], true);
                }
            }
        }
    }

    public void Toggle(GameObject obj) {
        for (int i = 0; i < target.Length; i++) {
            fbl_ToggleObjects toggleObj = target[i].GetComponent<fbl_ToggleObjects>();
            if (obj != target[i]) {
                for (int j = 0; j < toggleObj.deactive.Length; j++) {
                    set(toggleObj.deactive[j], true);
                }
                for (int j = 0; j < toggleObj.active.Length; j++) {
                    set(toggleObj.active[j], false);
                }
            }
            else {
                for (int j = 0; j < toggleObj.active.Length; j++) {
                    set(toggleObj.active[j], true);
                }
                for (int j = 0; j < toggleObj.deactive.Length; j++) {
                    set(toggleObj.deactive[j], false);
                }
                obj.SetActive(true);
            }
        }
    }

    private void set(GameObject obj, bool state) {
        if (obj != null) {
            NGUITools.SetActive(obj, state);
        }
    }

    private void offPanel(GameObject obj) {
        for(int i = 0; i < target.Length; i++) {
            if(obj != target[i]) {
                set(target[i], false);
            }
        }
    }
}
