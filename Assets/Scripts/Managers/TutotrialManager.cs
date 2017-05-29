using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutotrialManager : MonoBehaviour {
    public GameObject[] 
        effects,
        buttons;
    public UILabel
        header, 
        context;
    public GameObject
        PARTNER,
        NPC,
        myCharacter,
        talkContainer,
        container;
    public Riding_VC ridingController;
    Tutorial[] text;
    int count = 0;
    bool isFirstClick = false;
    TypewriterEffect typingEffect;
    void Start() {
        TextAsset file = (TextAsset)Resources.Load("TutorialContext");
        text = JsonHelper.getJsonArray<Tutorial>(file.text);
        typingEffect = context.GetComponent<TypewriterEffect>();
        EventDelegate.Add(typingEffect.onFinished, typingFinished);
        setFirstState();
    }

    void OnDisable() {
        count = 0;
    }

    void setFirstState() {
        header.text = text[count].id.ToString();
        context.text = text[count].context;
        count++;
    }

    public void nextPage() {
        if(count > text.Length - 1) { return; }
        if (!isFirstClick) {
            isFirstClick = true;
            typingEffect.Finish();
            return;
        }
        effectInit();
        addEffect(count);
        header.text = text[count].id.ToString();
        context.text = text[count].context;
        count++;
        typingEffect.ResetToBeginning();
    }

    void typingFinished() {
        isFirstClick = true;
    }

    void addEffect(int count) {
        switch(count) {
            case 3:
            case 5:
                PARTNER.SetActive(true);
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
                StartCoroutine(activateButton(0));
                talkContainer.transform.Find("Background").GetComponent<BoxCollider>().enabled = false;
                break;
            case 12:
                buttons[0].SetActive(false);
                ridingController.gameObject.SetActive(true);
                talkContainer.transform.Find("Background").GetComponent<BoxCollider>().enabled = true;
                break;
            case 13:
                talkContainer.SetActive(false);
                StartCoroutine(activateButton(1));
                break;
        }
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
        yield return new WaitForSeconds(1.0f);
        buttons[index].SetActive(true);
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