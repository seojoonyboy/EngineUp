using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyHomeViewController : MonoBehaviour {
    public GameObject[] subPanels;
    private Animator animator;
    private ToggleGroup tg;

    void Awake() {
        animator = GetComponent<Animator>();
        tg = transform.Find("TogglePanel").GetComponent<ToggleGroup>();
    }

    void OnEnable() {
        Invoke("playSlideIn", 0.2f);
    }

    void playSlideIn() {
        animator.Play("SlideIn");
    }

    public void slideFinished(AnimationEvent animationEvent) {
        int boolParm = animationEvent.intParameter;

        //slider in
        if (boolParm == 1) {
            subPanels[0].SetActive(true);
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);

            initToggle();
        }
    }

    public void onBackButton() {
        animator.Play("SlideOut");
    }

    public void OnToggle(GameObject obj) {
        bool isOn = obj.GetComponent<Toggle>().isOn;

        obj.transform.Find("DeactiveImg").gameObject.SetActive(!isOn);
        obj.transform.Find("DeactiveLabel").gameObject.SetActive(!isOn);
        obj.transform.Find("ActiveImg").gameObject.SetActive(isOn);
        obj.transform.Find("ActiveLabel").gameObject.SetActive(isOn);

        Text header = gameObject.transform.Find("TopPanel/Header").GetComponent<Text>();
        if(obj.name == "GarageButton") {
            header.text = "차고지";
        }
        else if(obj.name == "PartnerButton") {
            header.text = "파트너룸";
        }
        else if(obj.name == "RecordButton") {
            header.text = "기록실";
        }
    }

    private void initToggle() {
        var tmp = transform.Find("TogglePanel/GarageButton").gameObject;
        tmp.GetComponent<Toggle>().isOn = true;
        OnToggle(tmp);

        foreach(GameObject panel in subPanels) {
            panel.SetActive(false);
        }

        tmp = transform.Find("TogglePanel/PartnerButton").gameObject;
        tmp.GetComponent<Toggle>().isOn = false;
        OnToggle(tmp);

        tmp = transform.Find("TogglePanel/RecordButton").gameObject;
        tmp.GetComponent<Toggle>().isOn = false;
        OnToggle(tmp);
    }
}
