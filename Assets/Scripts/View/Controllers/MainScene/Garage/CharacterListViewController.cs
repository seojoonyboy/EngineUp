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
        equipButton,
        nonepossessionButton,
        Content,
        RowContainer;
    public SpritesManager spriteManager;
    public MainViewController mV;

    private Animator animator;
    void Awake() {
        gm = GameManager.Instance;
        charInvenStore = gm.charInvenStore;

        animator = GetComponent<Animator>();
    }

    void OnDisable() {
        removeList();
    }

    void OnEnable() {
        Invoke("playSlideIn", 0.2f);
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
            makeList();
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
    }

    //내 캐릭터중 하나 선택시
    public void charSelected(GameObject obj) {
        //sm.playEffectSound(0);

        //selectedChar = obj;

        //Info info = obj.GetComponent<Info>();
        //sbInfo sbInfo = obj.GetComponent<sbInfo>();
        //setMainChar(info.characterId, info.lv);
        ////setSideBar(info.characterId, info.lv);
        //setSideBar();
        //setSideBarName(sbInfo.name);
        //setEquipButton(info.characterId, info.has_character);

        //lvLabel.text = "친밀도 Lv " + info.lv.ToString();
        //charName.text = sbInfo.name;
        //setFriendlySlider(info.lv, sbInfo.lvup_exps, info.exp);

        //stats[0].text = info.strength.ToString();
        //stats[1].text = info.enurance.ToString();
        //stats[2].text = info.speed.ToString();
        //stats[3].text = info.recovery.ToString();
    }

    public void setEquipButton(int index, string hasChar) {
        //if (hasChar == "true") {
        //    equipButton.SetActive(true);
        //    nonepossessionButton.SetActive(false);
        //    character_inventory charInfo = userStore.myData.represent_character.character_inventory;
        //    if (index == charInfo.character) {
        //        equipButton.transform.Find("Check").gameObject.SetActive(true);
        //    }
        //    else {
        //        equipButton.transform.Find("Check").gameObject.SetActive(false);
        //    }
        //}
        //else {
        //    equipButton.SetActive(false);
        //    nonepossessionButton.SetActive(true);
        //}
    }

    public void sideBarClicked(GameObject obj) {
        //sm.playEffectSound(0);

        //GameObject _sprite = obj.transform.Find("Image").gameObject;

        //string name = _sprite.GetComponent<Image>().sprite.name;
        //string[] str = name.Split('-');
        //setMainChar(Int32.Parse(str[0]), Int32.Parse(str[1]));
    }

    //캐릭터 장착하기
    public void equipCharButton() {
        //equip_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_EQUIP) as equip_act;
        //act._type = equip_act.type.CHAR;
        //act.id = selectedChar.GetComponent<Info>().id;
        //gm.gameDispatcher.dispatch(act);

        //equipButton.transform.Find("Check").gameObject.SetActive(true);
    }

    public void makeList() {
        Character_inventory[] myChars = (Character_inventory[])charInvenStore.myCharacters.ToArray(typeof(Character_inventory));

        int itemNum = myChars.Length;
        int count = 0;
        int containerNum = (int)System.Math.Ceiling(itemNum / 4.0f);

        //var allChars = charInvenStore.all_characters;
        //var myChars = charInvenStore.my_characters;

        //int itemNum = allChars.Count;
        //int count = 0;

        //int containerNum = (int)System.Math.Ceiling(itemNum / 4.0f);
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
                    if (info.status != null) {
                        info.status.endurance = myChars[count].endurance;
                        info.status.strength = myChars[count].strength;
                        info.status.speed = myChars[count].speed;
                        info.status.regeneration = myChars[count].regeneration;
                    }
                    info.name = myChars[count].name;
                    info.imageId = myChars[count].imageId;
                    item.transform.Find("Character").gameObject.SetActive(true);
                    item.transform.Find("Info").gameObject.SetActive(true);

                    Image img = item.transform.Find("Character/Image").GetComponent<Image>();
                    img.sprite = mV.characters_entire_body[info.imageId - 1].images[info.lv];

                    Image grade = item.transform.Find("Info/Grade").GetComponent<Image>();

                    Text name = item.transform.Find("Info/Name").GetComponent<Text>();
                    name.text = info.name;
                    count++;
                }
            }
        }
        //int repCharIndex = charInvenStore.repCharacter.character;

        //내 캐릭터 리스트 생성
        //character_inventory[] myChars = charInvenStore.my_characters;
        //var allChars = charInvenStore.all_characters;

        //float tmp = (float)allChars.Count / 3f;
        //int pageNum = Mathf.CeilToInt(tmp);

        //GameObject[] pages = new GameObject[pageNum];
        //for (int i = 0; i < pageNum; i++) {
        //    pages[i] = Instantiate(scroll_pagePref);
        //    pages[i].transform.SetParent(itemGrid.transform, false);

        //    GameObject pageIcon = Instantiate(scroll_pageIconPref);
        //    pageIcon.transform.SetParent(pageIconGrid.transform);
        //}

        //int pageIndex = 0;
        //int itemIndex = 0;

        //foreach (KeyValuePair<int, all_characters> dC in allChars) {
        //    GameObject item = Instantiate(itemPref);

        //    //Debug.Log("Page Index : " + pageIndex + ", Item Index : " + itemIndex);

        //    item.transform.SetParent(pages[pageIndex].transform.GetChild(itemIndex).transform, false);
        //    item.transform.localPosition = Vector3.zero;
        //    if (itemIndex < 2) {
        //        itemIndex++;
        //    }
        //    else {
        //        pageIndex++;
        //        itemIndex = 0;
        //    }

        //    Info info = item.AddComponent<Info>();
        //    info.characterId = dC.Key;
        //    sbInfo sbInfo = item.AddComponent<sbInfo>();
        //    sbInfo.name = dC.Value.name;
        //    sbInfo.desc = dC.Value.desc;
        //    sbInfo.cost = dC.Value.cost;
        //    sbInfo.lvup_exps = dC.Value.lvup_exps;

        //    if (info.characterId == repCharIndex) {
        //        selectedChar = item;
        //    }

        //    var element = Array.Find(myChars, arr => arr.character.Equals(dC.Key));
        //    Text puzzles = item.transform.Find("Bottom/Text").GetComponent<Text>();
        //    //보유한 캐릭터
        //    if (element != null) {
        //        info.id = element.id;
        //        info.paid = element.paid;
        //        info.lv = element.lv;
        //        info.exp = element.exp;
        //        info.has_character = element.has_character;
        //        info.strength = element.status.strength;
        //        info.enurance = element.status.endurance;
        //        info.recovery = element.status.regeneration;
        //        info.speed = element.status.speed;

        //        if (element.has_character == "true") {
        //            item.transform.Find("DeactiveContainer").gameObject.SetActive(false);
        //            puzzles.text = "보유중";
        //        }
        //        else {
        //            puzzles.text = info.paid + " / " + sbInfo.cost.ToString();
        //        }
        //    }
        //    //보유하고 있지 않은 캐릭터의 경우
        //    else {
        //        puzzles.text = "미보유";
        //        info.lv = 1;
        //    }
        //    Image sprite = item.transform.Find("Image").GetComponent<Image>();
        //    int imgIndex = info.characterId - 1;
        //    sprite.sprite = mV_controller.characters_slots[imgIndex].images[0];

        //    item.GetComponent<Button>().onClick.AddListener(() => charSelected(item));
        //}

        //scrollSnapRect.enabled = true;
    }

    private void removeList() {
        //itemGrid.transform.DestroyChildren();
        //pageIconGrid.transform.DestroyChildren();
    }

    public class CharInfo : MonoBehaviour {
        public int id;
        public int lv;
        public string name;

        public int needPaid;
        public int paid;
        public string has_character;
        public int imageId;
        public int exp;

        public charStat status;
    }
}
