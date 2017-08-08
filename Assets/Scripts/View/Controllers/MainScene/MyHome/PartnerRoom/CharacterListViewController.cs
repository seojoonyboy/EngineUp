using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterListViewController : MonoBehaviour {
    GameManager gm;
    Char_Inventory charInvenStore;
    public User userStore;

    SoundManager sm;
    public GameObject
        Content,
        RowContainer,
        selectedItem;
    public SpritesManager spriteManager;
    public MainViewController mV;
    public CharacterDetailViewController childController;

    private Animator animator;
    public tmpCharStat[] tmpCharacterStats;

    void Awake() {
        gm = GameManager.Instance;
        charInvenStore = gm.charInvenStore;

        animator = GetComponent<Animator>();

        TextAsset file = (TextAsset)Resources.Load("CharStat");
        tmpCharacterStats = JsonHelper.getJsonArray<tmpCharStat>(file.text);
    }

    void OnEnable() {
        Invoke("playSlideIn", 0.2f);
    }

    void playSlideIn() {
        animator.Play("SlideIn");
    }

    public void onBackButton() {
        animator.Play("SlideOut");
        removeList();
    }

    public void slideFinished(AnimationEvent animationEvent) {
        int boolParm = animationEvent.intParameter;

        //slider in
        if (boolParm == 1) {
            makeList();
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
    }

    public void makeList() {
        removeList();
        Character_inventory[] myChars = (Character_inventory[])charInvenStore.myCharacters.ToArray(typeof(Character_inventory));

        int itemNum = myChars.Length;
        int count = 0;
        int containerNum = (int)System.Math.Ceiling(itemNum / 4.0f);

        for (int i = 0; i < containerNum; i++) {
            GameObject rC = Instantiate(RowContainer);
            rC.transform.SetParent(Content.transform, false);

            for (int j = 0; j < rC.transform.childCount; j++) {
                if (count < itemNum) {
                    GameObject item = rC.transform.GetChild(j).gameObject;

                    CharInfo info = item.AddComponent<CharInfo>();
                    info.paid = myChars[count].paid;
                    info.id = myChars[count].id;
                    info.lv = myChars[count].lv;
                    info.exp = myChars[count].exp;
                    info.cost = myChars[count].cost;
                    info.has_character = myChars[count].hasCharacter;
                    info.name = myChars[count].name;
                    info.imageId = myChars[count].imageId;
                    item.transform.Find("Character").gameObject.SetActive(true);
                    item.transform.Find("Info").gameObject.SetActive(true);

                    Image img = item.transform.Find("Character/Image").GetComponent<Image>();
                    if(info.has_character) {
                        img.sprite = mV.characters_entire_body[info.imageId - 1].images[info.lv - 1];
                        info.endurance = myChars[count].endurance;
                        info.strength = myChars[count].strength;
                        info.speed = myChars[count].speed;
                        info.regeneration = myChars[count].regeneration;
                    }
                    else {
                        img.sprite = mV.characters_entire_body[info.imageId - 1].images[0];
                        info.endurance = tmpCharacterStats[info.imageId - 1].character[0].status.endurance;
                        info.strength = tmpCharacterStats[info.imageId - 1].character[0].status.strength;
                        info.speed = tmpCharacterStats[info.imageId - 1].character[0].status.speed;
                        info.regeneration = tmpCharacterStats[info.imageId - 1].character[0].status.regeneration;
                    }
                    

                    Image grade = item.transform.Find("Info/Grade").GetComponent<Image>();

                    Text name = item.transform.Find("Info/Name").GetComponent<Text>();
                    name.text = info.name;

                    Text puzzle = item.transform.Find("Info/Puzzle").GetComponent<Text>();
                    if(info.has_character) {
                        puzzle.text = info.paid + "/" + info.cost;
                    }
                    else {
                        puzzle.text = "미보유";
                    }

                    Button btn = item.GetComponent<Button>();
                    btn.onClick.AddListener(() => OnDetail(item));
                    count++;
                }
            }
        }
    }

    private void removeList() {
        foreach (Transform child in Content.transform) {
            Destroy(child.gameObject);
        }
    }

    private void OnDetail(GameObject obj) {
        selectedItem = obj;

        childController.gameObject.SetActive(true);
    }

    public void onFilterButton(GameObject obj) {
        obj.SetActive(!obj.activeSelf);
    }

    public void filterSelected(int index) {
        PlayerPrefs.SetInt("Filter_BICYCLE", index);
        itemSort act = ActionCreator.createAction(ActionTypes.CHAR_SORT) as itemSort;
        switch (index) {
            case 1:
                Debug.Log("이름순 정렬");
                act._type = itemSort.type.NAME;
                break;
            case 2:
                Debug.Log("등급순 정렬");
                act._type = itemSort.type.GRADE;
                break;
        }
        gm.gameDispatcher.dispatch(act);
    }
}

public class CharInfo : MonoBehaviour {
    public int id;
    public int lv;
    public string name;

    public int needPaid;
    public int paid;
    public bool has_character;
    public int imageId;
    public int exp;
    public int cost;

    public int strength;        //능력치
    public int speed;
    public int endurance;
    public int regeneration;
}

[System.Serializable]
public class tmpCharStat {
    public int id;
    public string name;
    public tmpCharacter[] character;
}

[System.Serializable]
public class tmpCharacter {
    public int lv;
    public tmpStatus status;
}

[System.Serializable]
public class tmpStatus {
    public int strength;
    public int speed;
    public int endurance;
    public int regeneration;
}