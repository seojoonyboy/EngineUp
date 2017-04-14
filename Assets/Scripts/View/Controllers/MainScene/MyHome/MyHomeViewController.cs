using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyHomeViewController : MonoBehaviour {
    public GameObject[] subPanels;

    public void onClick(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        switch(index) {
            //차고지(자전거)
            case 0:
                subPanels[1].SetActive(true);
                break;
            //서재(기록실)
            case 1:
                subPanels[2].SetActive(true);
                break;
            //파트너룸(캐릭터)
            case 2:
                subPanels[0].SetActive(true);
                break;
        }
    }

    public void offPanel() {
        UIToggle toggle = UIToggle.GetActiveToggle(2);
        toggle.value = false;
        gameObject.SetActive(false);
    }
}
