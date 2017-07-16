using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonManager : MonoBehaviour {
    public GameObject[] 
        ActiveImg,
        DeactiveImg;

    void OnEnable() {
        var toggle = GetComponent<Toggle>();
        int index = GetComponent<ButtonIndex>().index;

        if(toggle != null) {
            int isOn = 0;
            switch (index) {
                case 0:
                    isOn = PlayerPrefs.GetInt("BGM");
                    break;
                case 1:
                    isOn = PlayerPrefs.GetInt("ES");
                    break;
                case 2:
                    isOn = PlayerPrefs.GetInt("SILENT");
                    break;
            }
            if(isOn == 1) {
                GetComponent<Toggle>().isOn = true;
            }
            else {
                GetComponent<Toggle>().isOn = false;
            }

            OnToggle(gameObject);
        }
    }

    public void OnToggle(GameObject obj) {
        var toggle = obj.GetComponent<Toggle>();
        if(toggle != null) {
            if(toggle.isOn) {
                foreach(GameObject activeImg in ActiveImg) {
                    activeImg.SetActive(true);
                }
                foreach (GameObject deactiveImg in DeactiveImg) {
                    deactiveImg.SetActive(false);
                }
            }
            else {
                foreach (GameObject deactiveImg in DeactiveImg) {
                    deactiveImg.SetActive(true);
                }
                foreach (GameObject deactiveImg in ActiveImg) {
                    deactiveImg.SetActive(false);
                }
            }
            var btnIndex = GetComponent<ButtonIndex>();
            var boolIndex = GetComponent<boolIndex>();
            if(btnIndex != null) {
                if(boolIndex != null) {
                    bool isFirstSet = boolIndex.isOn;

                    if (!isFirstSet) {
                        int index = btnIndex.index;
                        int isOn = 0;
                        if (toggle.isOn) {
                            isOn = 1;
                        }
                        else {
                            isOn = 0;
                        }
                        switch (index) {
                            case 0:
                                PlayerPrefs.SetInt("BGM", isOn);
                                break;
                            case 1:
                                PlayerPrefs.SetInt("ES", isOn);
                                break;
                            case 2:
                                PlayerPrefs.SetInt("SILENT", isOn);
                                break;
                        }
                    }
                }
            }
        }
    }
}
