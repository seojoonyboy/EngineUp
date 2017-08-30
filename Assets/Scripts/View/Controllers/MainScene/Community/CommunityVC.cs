using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommunityVC : MonoBehaviour {
    public GameObject[] subPanels;
    private Animator animator;

    public GameObject notifyModal;
    void Awake() {
        animator = GetComponent<Animator>();
    }

    void OnEnable() {
        Invoke("playSlideIn", 0.2f);
    }

    void Start() {
        
    }

    void playSlideIn() {
        animator.Play("SlideIn");
    }

    public void onBackButton() {
        animator.Play("SlideOut");
    }

    public void slideFinished(AnimationEvent animationEvent) {
        int boolParm = animationEvent.intParameter;

        //slider in
        if (boolParm == 1) {
            subPanels[1].SetActive(true);
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);

            initToggle();
        }
    }

    public void OnToggle(GameObject obj) {
        bool isOn = obj.GetComponent<Toggle>().isOn;

        obj.transform.Find("DeactiveImg").gameObject.SetActive(!isOn);
        obj.transform.Find("DeactiveLabel").gameObject.SetActive(!isOn);
        obj.transform.Find("ActiveImg").gameObject.SetActive(isOn);
        obj.transform.Find("ActiveLabel").gameObject.SetActive(isOn);

        Text header = gameObject.transform.Find("TopPanel/Header").GetComponent<Text>();
        if (obj.name == "Feeds") {
            header.text = "피드";
        }
        else if (obj.name == "Friends") {
            header.text = "친구";
        }
        else if (obj.name == "Group") {
            header.text = "그룹";
        }
    }

    private void initToggle() {
        var tmp = transform.Find("TogglePanel/Friends").gameObject;
        tmp.GetComponent<Toggle>().isOn = true;
        OnToggle(tmp);

        foreach (GameObject panel in subPanels) {
            panel.SetActive(false);
        }

        tmp = transform.Find("TogglePanel/Feeds").gameObject;
        tmp.GetComponent<Toggle>().isOn = false;
        OnToggle(tmp);

        tmp = transform.Find("TogglePanel/Group").gameObject;
        tmp.GetComponent<Toggle>().isOn = false;
        OnToggle(tmp);
    }
}
