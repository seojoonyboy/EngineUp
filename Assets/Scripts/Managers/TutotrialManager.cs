using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutotrialManager : MonoBehaviour {
    public GameObject[] effects;
    public UILabel
        header, 
        context;
    public GameObject
        NPC,
        myCharacter,
        talkContainer;

    Tutorial[] text;
    int count = 0;
    void Start() {
        TextAsset file = (TextAsset)Resources.Load("TutorialContext");
        text = JsonHelper.getJsonArray<Tutorial>(file.text);
        nextPage();
    }

    public void nextPage() {
        if(count > text.Length - 1) { return; }
        effectInit();
        addEffect(count);
        header.text = text[count].id.ToString();
        context.text = text[count].context;
        count++;
    }

    void addEffect(int count) {
        switch(count) {
            case 1:
                NPC.SetActive(true);
                break;
        }
    }

    void effectInit() {
        NPC.SetActive(false);
        myCharacter.SetActive(false);
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