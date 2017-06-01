using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutotrialManager : MonoBehaviour {
    public GameObject[] 
        effects,
        buttons,
        clonedPanels,
        realPanels;
    public UILabel
        header, 
        context;
    public GameObject
        PARTNER,
        NPC,
        myCharacter,
        talkContainer,
        container,
        background,
        fading;
    public Riding_VC ridingController;
    public MyHomeViewController myhomeController;
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
        header.text = text[count].id.ToString();
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
        effectInit();
        switch (count) {
            case 0:
            case 1:
                nextPage();
                break;
            case 2:
                PARTNER.SetActive(true);
                nextPage();
                break;
            case 3:
            case 4:
            case 5:
                nextPage();
                break;
            case 6:
                nextPage();
                shakeEffect();
                break;
            case 7:
                nextPage();
                NPC.SetActive(true);
                break;
            case 8:
            case 9:
                nextPage();
                break;
            case 10:
                onEffect(count);
                break;
            case 11:
                onEffect(count);
                break;
            case 12:
                nextPage();
                break;
            case 13:
                onEffect(count);
                break;
            case 14:
                nextPage();
                break;
            case 15:
                ridingResume();
                onEffect(count);
                break;
            case 16:
            case 17:
            case 18:
                onEffect(count);
                break;
            case 19:
            case 20:
                nextPage();
                break;
            case 21:
                onEffect(count);
                break;
            case 22:
            case 23:
                nextPage();
                break;
            case 24:
                onEffect(count);
                break;
            case 25:
            case 26:
            case 27:
            case 28:
                nextPage();
                break;
            case 29:
                onEffect(count);
                break;
            case 30:
            case 31:
                nextPage();
                break;
            case 32:
                onEffect(count);
                break;
            case 33:
            case 34:
                nextPage();
                break;
            case 35:
                onEffect(count);
                break;
        }
    }

    public void nextPage() {
        Debug.Log("다음 페이지로");
        talkContainer.SetActive(true);
        count++;
        header.text = text[count].id.ToString();
        context.text = text[count].context;

        typingEffect.ResetToBeginning();
        isFirstClick = false;
    }

    void typingFinished() {
        isFirstClick = true;
    }

    void onEffect(int index) {
        talkContainer.SetActive(false);
        switch(index) {
            case 10:
                StartCoroutine(activateButton(0));
                break;
            case 11:
                StartCoroutine(activateButton(1));
                break;
            case 13:
                background.SetActive(false);
                clonedPanels[0].SetActive(false);
                ridingController.onRidingStartButton(true);
                break;
            case 15:
                StartCoroutine(activateButton(2));
                break;
            case 16:
                StartCoroutine(activateButton(4));
                break;
            case 17:
                StartCoroutine(activateButton(5));
                break;
            case 18:
                StartCoroutine(activateButton(6));
                break;
            case 21:
                StartCoroutine(activateButton(7));
                break;
            case 24:
                StartCoroutine(activateButton(8));
                break;
            case 29:
            case 32:
                fading.SetActive(true);
                StartCoroutine(FadeInOut(true));
                break;
            case 35:
                fading.SetActive(true);
                StartCoroutine(FadeOut(false));
                Invoke("tutorialEnd", 3.0f);
                break;
        }
    }

    void offFading() {
        fading.SetActive(false);
    }

    void effectInit() {
        NPC.SetActive(false);
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
        PlayerPrefs.SetInt("isFirstPlay", 1);
    }

    IEnumerator FadeOut(bool needNextPange = false) {
        fading.GetComponent<UISprite>().alpha = 0;
        TweenAlpha.Begin(fading, 2.0f, 1.0f);
        yield return new WaitForSeconds(2.0f);
        fading.SetActive(false);

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