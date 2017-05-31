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
        background;
    public Riding_VC ridingController;
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
        addEffect(count);
    }

    public void nextPage() {
        if (count > text.Length - 1) { return; }
        if (!isFirstClick) {
            isFirstClick = true;
            typingEffect.Finish();
            return;
        }

        header.text = text[count].id.ToString();
        context.text = text[count].context;

        typingEffect.ResetToBeginning();

        addEffect(count);
    }

    void typingFinished() {
        isFirstClick = true;
    }

    void addEffect(int index) {
        Debug.Log("Count : " + count);
        effectInit();
        canNextPage = true;
        switch (index) {
            case 1:
                PARTNER.SetActive(true);
                break;
            case 2:
                myCharacter.SetActive(true);
                break;
            case 3:
                PARTNER.SetActive(true);
                break;
            case 4:
                myCharacter.SetActive(true);
                break;
            case 7:
                shakeEffect();
                effects[0].SetActive(true);
                break;
            case 8:
                NPC.SetActive(true);
                effects[0].SetActive(false);
                break;
            case 11:
                canNextPage = false;

                talkContainer.SetActive(false);
                if(isFirstDeactTalkBalloon) {
                    StartCoroutine(activateButton(0));
                    isFirstDeactTalkBalloon = false;
                }
                else {
                    count++;
                    StartCoroutine(activateButton(1));
                    buttons[1].SetActive(false);
                }
                break;
            case 12:
                isFirstDeactTalkBalloon = true;
                buttons[1].SetActive(false);

                talkContainer.SetActive(true);
                typingEffect.ResetToBeginning();

                header.text = text[count].id.ToString();
                context.text = text[count].context;
                break;
            case 14:
                gameObject.SetActive(false);

                //라이딩 시작
                clonedPanels[0].SetActive(false);
                ridingController.onRidingStartButton(true);
                break;
            case 16:
                ridingResume();
                StartCoroutine(activateButton(2));
                canNextPage = false;
                talkContainer.SetActive(false);
                break;
            case 18:
                StartCoroutine(activateButton(4));
                canNextPage = false;
                talkContainer.SetActive(false);
                break;
        }

        if(canNextPage) {
            count++;
        }
    }

    public void onTalkBalloon() {
        talkContainer.SetActive(true);
        typingEffect.ResetToBeginning();
    }

    public void countIncrease() {
        count++;
    }

    void boolInit() {
        canNextPage = true;
    }

    void effectInit() {
        NPC.SetActive(false);
        PARTNER.SetActive(false);
        myCharacter.SetActive(false);
        isFirstClick = false;
    }

    void shakeEffect() {
        Debug.Log("Shaking");
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

    public void ridingPaused() {
        gameObject.SetActive(true);

        typingEffect.ResetToBeginning();

        header.text = text[count].id.ToString();
        context.text = text[count].context;
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

    //모달 내, 라이딩 최종 종료버튼 클릭시
    public void ridingEnd() {
        talkContainer.SetActive(true);

        typingEffect.ResetToBeginning();

        header.text = text[count].id.ToString();
        context.text = text[count].context;

        buttons[3].SetActive(false);

        count++;
        addEffect(count);
    }

    //라이딩 결과화면 종료
    public void resultExit() {
        buttons[4].SetActive(false);

        background.SetActive(false);
        object[] parms = new object[2] { background, true };
        StartCoroutine(fadeIn(parms));
    }

    IEnumerator fadeOut(object[] parms) {
        bool autoFadeIn = (bool)parms[0];
        GameObject target = (GameObject)parms[1];

        float interval = 2.0f;
        while(interval >= 0.0f) {
            yield return new WaitForSeconds(1.0f);
            interval -= 1;
        }
        target.SetActive(false);
    }

    IEnumerator fadeIn(object[] parms) {
        GameObject target = (GameObject)parms[0];
        bool needTalkBalloon = (bool)parms[1];
        
        float interval = 2.0f;
        while (interval >= 0.0f) {
            yield return new WaitForSeconds(1.0f);
            interval -= 1;
        }

        if(needTalkBalloon) {
            talkContainer.SetActive(true);
            header.text = text[17].id.ToString();
            context.text = text[17].context;

            typingEffect.ResetToBeginning();
        }
        target.SetActive(true);
    }

    public void resetCount() {
        count = 0;
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