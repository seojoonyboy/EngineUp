using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyHomeViewController : MonoBehaviour {
    public GameObject[] subPanels;
    private TweenPosition tP;
    private SoundManager sm;

    public GameObject blockingCollPanel;
    private TweenManager tm;

    private UISprite panel;
    private float color;

    private bool
        isReverse_tp;

    void Awake() {
        tP = gameObject.transform.Find("Background").GetComponent<TweenPosition>();
        tm = GetComponent<TweenManager>();
        sm = SoundManager.Instance;

        panel = gameObject.transform.Find("Background").GetComponent<UISprite>();
        color = panel.alpha;

        panel.alpha = 0;
    }

    public void onPanel() {
        panel.alpha = color;
        tweenPos();

        blockingCollPanel.SetActive(true);
        isReverse_tp = false;
    }

    private void offPanel() {
        panel.alpha = 0f;
    }

    public void tweenPos() {
        sm.playEffectSound(0);
        bool isTweening = tm.isTweening;
        if (isTweening) {
            return;
        }
        tm.isTweening = true;
        blockingCollPanel.SetActive(true);
        if (!isReverse_tp) {
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
        tm.isTweening = false;
        blockingCollPanel.SetActive(false);

        if(isReverse_tp) {
            offPanel();
            gameObject.transform.Find("TopPanel").gameObject.SetActive(false);
        }
        else {
            gameObject.transform.Find("TopPanel").gameObject.SetActive(true);
        }

        isReverse_tp = true;
    }

    public void onClick(GameObject obj) {
        sm.playEffectSound(0);
        int index = obj.GetComponent<ButtonIndex>().index;
        switch(index) {
            //차고지(자전거)
            case 0:
                subPanels[1].GetComponent<BicycleViewController>().onPanel();
                break;
            //서재(기록실)
            case 1:
                subPanels[2].SetActive(true);
                subPanels[2].GetComponent<HistoryViewController>().onPanel();
                break;
            //파트너룸(캐릭터)
            case 2:
                subPanels[0].GetComponent<CharacterViewControlller>().onPanel();
                break;
        }
    }
}
