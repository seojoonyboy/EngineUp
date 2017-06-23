using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutotrialManager : MonoBehaviour {
    public GameObject[] 
        effects,
        buttons,
        clonedPanels,
        realPanels;
    public UILabel context;
    public GameObject
        PARTNER,
        myCharacter,
        talkContainer,
        container,
        background,
        fading;
    public Riding_VC ridingController;
    public MyHomeViewController myhomeController;
    public MainViewController mainViewController;
    Tutorial[] text;
    int count = 0;
    bool 
        isFirstClick = false,
        canNextPage = true,
        isFirstDeactTalkBalloon = true;

    TypewriterEffect typingEffect;
    void Start() {
        TextAsset file = (TextAsset)Resources.Load("TutorialContext");
        text = JsonHelper.getJsonArray<Tutorial>(file.text);
        typingEffect = context.GetComponent<TypewriterEffect>();
        EventDelegate.Add(typingEffect.onFinished, typingFinished);
        setFirstState();
    }

    void setFirstState() {
        context.text = text[count].context;
    }

    public void panelClicked() {
        Debug.Log("Count : " + count);
        if (count > text.Length - 1) { return; }
        if (!isFirstClick) {
            isFirstClick = true;
            typingEffect.Finish();
            return;
        }
        //effectInit();
        switch (count) {
            case 0:
                nextPage();
                onEffect(0, false);
                break;
            case 1:
                nextPage();
                break;
            case 2:
                offClonedButtons(buttons[0]);
                onEffect(2);
                nextPage();
                break;
            case 3:
                nextPage();
                break;
            case 4:
                offClonedButtons(buttons[5]);
                onEffect(4);
                nextPage();
                break;
            case 5:
                offClonedButtons(buttons[8]);
                nextPage();
                break;
            case 6:
                onEffect(6);
                break;
        }
    }

    void onEffect(int index, bool needOffContainer = true) {
        if(needOffContainer) {
            talkContainer.SetActive(false);
        }
        switch (index) {
            case 0:
                buttons[0].SetActive(true);
                break;
            case 2:
                buttons[5].SetActive(true);
                break;
            case 4:
                buttons[8].SetActive(true);
                break;
            case 6:
                fading.SetActive(true);
                PARTNER.SetActive(false);
                offClonedButtons(buttons[8]);
                StartCoroutine(FadeOut());
                break;
        }
    }

    public void nextPage() {
        Debug.Log("다음 페이지로");
        talkContainer.SetActive(true);
        count++;
        context.text = text[count].context;

        typingEffect.ResetToBeginning();
        isFirstClick = false;
    }

    void typingFinished() {
        isFirstClick = true;
    }

    void offFading() {
        fading.SetActive(false);
    }

    void effectInit() {
        PARTNER.SetActive(false);
        myCharacter.SetActive(false);
    }

    void shakeEffect() {
        container.GetComponent<Animator>().Play("Shaking");
    }

    IEnumerator activateButton(int index) {
        Debug.Log("Active Button");
        yield return new WaitForSeconds(1.0f);
        buttons[index].SetActive(true);
    }

    public void onClonedPanel(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        clonedPanels[index].SetActive(true);
        switch(index) {
            case 0:
                buttons[1].SetActive(false);
                break;
        }
    }

    //실제 Panel 활성화
    public void onPanel(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        realPanels[index].SetActive(true);
    }

    public void offClonedPanel(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        clonedPanels[index].SetActive(false);
    }

    public void offClonedButtons(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        buttons[index].SetActive(false);
    }

    public void onMyhome() {
        background.SetActive(false);
        myhomeController.gameObject.SetActive(true);
        Invoke("onBackground", 2.0f);
        Invoke("nextPageWithInterval", 2.5f);
    }

    public void ridingPaused() {
        background.SetActive(true);
        nextPage();
    }

    public void ridingResume() {
        ridingController.pauseButtonPressed();
        Debug.Log("Time Scale : " + Time.timeScale);
    }

    //하단 라이딩 종료버튼 클릭시
    public void ridingStop() {
        buttons[3].SetActive(true);
        buttons[2].SetActive(false);
    }

    //라이딩 결과화면 종료
    public void resultExit() {
        background.SetActive(false);
        StartCoroutine(FadeInOut(true));
        Invoke("onBackground", 2.0f);
    }

    //서재로 가기 버튼 클릭
    public void onLibrary() {
        buttons[6].SetActive(false);
        nextPage();
    }

    //서재에서 메인화면으로 가기 버튼 클릭
    public void offLibrary() {
        buttons[7].SetActive(false);
        StartCoroutine(FadeInOut(true));
        Invoke("onBackground", 2.0f);
    }

    public void resetCount() {
        count = 0;
    }

    void tutorialEnd() {
        gameObject.SetActive(false);
        mainViewController.charSprite.gameObject.SetActive(true);
        PlayerPrefs.SetInt("isFirstPlay", 1);
    }

    IEnumerator FadeOut(bool needNextPange = false) {
        fading.GetComponent<UISprite>().alpha = 0;
        TweenAlpha.Begin(fading, 2.0f, 1.0f);
        yield return new WaitForSeconds(2.0f);
        fading.SetActive(false);
        tutorialEnd();
        if (needNextPange) {
            nextPage();
        }
    }

    IEnumerator FadeIn(bool needNextPange = false) {
        fading.GetComponent<UISprite>().alpha = 1;
        TweenAlpha.Begin(fading, 2.0f, 0.0f);
        yield return new WaitForSeconds(2.0f);
        fading.SetActive(false);

        if (needNextPange) {
            nextPage();
        }
    }

    IEnumerator FadeInOut(bool needNextPange = false) {
        fading.GetComponent<UISprite>().alpha = 0;
        TweenAlpha.Begin(fading, 2.0f, 1.0f);
        yield return new WaitForSeconds(2.0f);
        TweenAlpha.Begin(fading, 2.0f, 0.0f);
        yield return new WaitForSeconds(2.0f);
        fading.SetActive(false);

        if (needNextPange) {
            nextPage();
        }
    }

    void onBackground() {
        background.SetActive(true);
    }

    void nextPageWithInterval() {
        nextPage();
    }
}

[System.Serializable]
class Tutorial {
    public int id;
    public string context;
    public string[] choice;

    public static Tutorial fromJSON(string json) {
        return JsonUtility.FromJson<Tutorial>(json);
    }
}