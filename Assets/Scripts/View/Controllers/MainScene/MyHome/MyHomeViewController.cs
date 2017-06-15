using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyHomeViewController : MonoBehaviour {
    public GameObject[] subPanels;
    private TweenPosition tP;

    public GameObject blockingCollPanel;

    private bool isReverse_tp;

    void Awake() {
        tP = gameObject.transform.Find("Background").GetComponent<TweenPosition>();
    }

    void OnEnable() {
        tweenPos();

        blockingCollPanel.SetActive(true);
        isReverse_tp = false;
    }

    void OnDisable() {
        tP.transform.Find("TopPanel").gameObject.SetActive(false);
        tP.ResetToBeginning();
    }

    public void tweenPos() {
        if(!isReverse_tp) {
            tP.PlayForward();
        }
        else {
            //swap
            Vector3 tmp;
            tmp = tP.to;
            tP.to = tP.from;
            tP.from = tmp;

            tP.ResetToBeginning();
            tP.PlayForward();
        }
    }

    public void tPFinished() {
        tP.transform.Find("TopPanel").gameObject.SetActive(true);
        blockingCollPanel.SetActive(false);

        if(isReverse_tp) {
            gameObject.SetActive(false);
        }

        isReverse_tp = true;
    }

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
}
