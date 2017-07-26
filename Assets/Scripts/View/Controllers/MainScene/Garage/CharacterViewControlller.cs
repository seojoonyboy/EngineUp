using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterViewControlller : MonoBehaviour {
    GameManager gm;
    Char_Inventory charInvenStore;
    public User userStore;
    public TweenManager tM;
    public MainViewController mV_controller;
    public ScrollSnapRect scrollSnapRect;

    SoundManager sm;
    //character_inventory[] characters;

    public GameObject
        mainStage,
        lv1Slot,
        lv2Slot,
        lv3Slot,
        itemPref,
        selectedChar,
        scroll_pagePref,
        scroll_pageIconPref;

    public ScrollSnapRect sR;

    public GameObject 
        equipButton,
        nonepossessionButton,
        itemGrid,
        sideBarGrid,
        pageIconGrid,
        descModal;

    public Text[] 
        stats,
        incStats;

    public Text 
        lvLabel,
        charName;

    public Slider friendlySlider;

    private TweenPosition tP;
    private bool isReverse_tp;

    void Awake() {
        gm = GameManager.Instance;
        sm = SoundManager.Instance;

        charInvenStore = gm.charInvenStore;

        tP = GetComponent<TweenPosition>();
    }

    void OnEnable() {
        tweenPos();
        
        isReverse_tp = false;
    }

    void offPanel() {
        gameObject.SetActive(false);
        selectedChar = null;

        tP.ResetToBeginning();
        nonepossessionButton.SetActive(false);

        scrollSnapRect.enabled = false;
    }

    public void tweenPos() {
        bool isTweening = tM.isTweening;
        if(isTweening) {
            return;
        }
        tM.isTweening = true;
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

            sm.playEffectSound(0);
        }
    }

    public void tpFinished() {
        tM.isTweening = false;

        if (isReverse_tp) {
            offPanel();
            gameObject.transform.Find("TopPanel").gameObject.SetActive(false);
        }

        else {
            gameObject.transform.Find("TopPanel").gameObject.SetActive(true);

            makeList();
            character_inventory charInfo = charInvenStore.repCharacter;
            setMainChar(charInfo.character, charInfo.lv);
            setEquipButton(charInfo.character, charInfo.has_character);
            setSideBar();
            setStat();
        }

        isReverse_tp = true;
    }

    public void onCharInvenStore() {
        if(gameObject.activeSelf) {
            ActionTypes charStoreEventType = charInvenStore.eventType;

            if (charStoreEventType == ActionTypes.ITEM_INIT) {
                if (charInvenStore.storeStatus == storeStatus.NORMAL) {
                    makeList();

                    sR.init();

                    character_inventory charInfo = charInvenStore.repCharacter;
                    setMainChar(charInfo.character, charInfo.lv);
                    setEquipButton(charInfo.character, charInfo.has_character);
                    setSideBar();
                    setStat();
                }
            }
        }
    }

    public void onUserListener() {
        ActionTypes userStoreEventType = userStore.eventType;
        
    }

    //내 캐릭터중 하나 선택시
    public void charSelected(GameObject obj) {
        sm.playEffectSound(0);

        selectedChar = obj;

        Info info = obj.GetComponent<Info>();
        sbInfo sbInfo = obj.GetComponent<sbInfo>();
        setMainChar(info.characterId, info.lv);
        //setSideBar(info.characterId, info.lv);
        setSideBar();
        setSideBarName(sbInfo.name);
        setEquipButton(info.characterId, info.has_character);

        lvLabel.text = "친밀도 Lv " + info.lv.ToString();
        charName.text = sbInfo.name;
        setFriendlySlider(info.lv, sbInfo.lvup_exps, info.exp);

        stats[0].text = info.strength.ToString();
        stats[1].text = info.enurance.ToString();
        stats[2].text = info.speed.ToString();
        stats[3].text = info.recovery.ToString();
    }

    public void setEquipButton(int index, string hasChar) {
        if (hasChar == "true") {
            equipButton.SetActive(true);
            nonepossessionButton.SetActive(false);
            character_inventory charInfo = userStore.myData.represent_character.character_inventory;
            if (index == charInfo.character) {
                equipButton.transform.Find("Check").gameObject.SetActive(true);
            }
            else {
                equipButton.transform.Find("Check").gameObject.SetActive(false);
            }
        }
        else {
            equipButton.SetActive(false);
            nonepossessionButton.SetActive(true);
        }
    }

    public void setFriendlySlider(int lv, int[] lvUp_exp, int exp) {
        float offset = 0;
        switch(lv) {
            case 1:
                offset = 1 / (float)lvUp_exp[0];
                break;
            case 2:
                offset = 1 / (float)lvUp_exp[1];
                break;
        }
        friendlySlider.value = exp * offset;
    }

    public void setMainChar(int index, int lv) {
        int arrIndex = index - 1;
        int arrSubIndex = lv - 1;
        if(arrSubIndex == -1) {
            arrSubIndex = 0;
        }

        var character = mainStage.transform.Find("Character").gameObject;
        var charImg = character.GetComponent<Image>();
        charImg.sprite = mV_controller.characters_entire_body[arrIndex].images[arrSubIndex];
        var rect = character.GetComponent<RectTransform>();
        charImg.SetNativeSize();
        Vector3 originSize = rect.sizeDelta;
        rect.sizeDelta = new Vector3(originSize.x, originSize.y / 2f);
    }

    public void setSideBar() {
        Info selInfo = selectedChar.GetComponent<Info>();
        
        int index = selInfo.characterId;
        int lv = selInfo.lv;
        //Debug.Log("Index : " + index + ", Lv : " + lv);
        if(lv == 0) {
            lv = 1;
        }
        //var imageArr = mV_controller.characters_busts_sm;
        Image portrait = sideBarGrid.transform.Find("Lv1/Image").GetComponent<Image>();
        portrait.sprite = mV_controller.characters_slots[index - 1].images[lv - 1];

        GameObject tmp = sideBarGrid.transform.Find("Lv1").gameObject;
        Info _info;
        sbInfo _sbInfo;

        if(tmp.GetComponent<Info>() == null) {
            _info = tmp.AddComponent<Info>();
        }
        else {
            _info = tmp.GetComponent<Info>();
        }
        _info.id = selInfo.id;
        _info.characterId = selInfo.characterId;
        _info.lv = 1;

        if(tmp.GetComponent<sbInfo>() == null) {
            _sbInfo = tmp.AddComponent<sbInfo>();
        }
        else {
            _sbInfo = tmp.GetComponent<sbInfo>();
        }

        _info = null;

        portrait = sideBarGrid.transform.Find("Lv2/Image").GetComponent<Image>();
        portrait.sprite = mV_controller.characters_slots[index - 1].images[lv];

        tmp = sideBarGrid.transform.Find("Lv2").gameObject;
        if (tmp.GetComponent<Info>() == null) {
            _info = tmp.AddComponent<Info>();
        }
        else {
            _info = tmp.GetComponent<Info>();
        }
        _info.id = selInfo.id;
        _info.characterId = selInfo.characterId;
        _info.lv = 2;

        _info = null;

        if (lv < 2) {
            sideBarGrid.transform.Find("Lv2/Image/Deactive").gameObject.SetActive(true);
        }

        portrait = sideBarGrid.transform.Find("Lv3/Image").GetComponent<Image>();
        portrait.sprite = mV_controller.characters_slots[index - 1].images[lv + 1];
        if (lv < 3) {
            sideBarGrid.transform.Find("Lv3/Image/Deactive").gameObject.SetActive(true);
        }

        tmp = sideBarGrid.transform.Find("Lv3").gameObject;
        if (tmp.GetComponent<Info>() == null) {
            _info = tmp.AddComponent<Info>();
        }
        else {
            _info = tmp.GetComponent<Info>();
        }
        _info.id = selInfo.id;
        _info.characterId = selInfo.characterId;
        _info.lv = 3;
    }

    private void setSideBarName(string name) {
        lv1Slot.transform.Find("Name").GetComponent<Text>().text = name;
        lv2Slot.transform.Find("Name").GetComponent<Text>().text = name;
        lv3Slot.transform.Find("Name").GetComponent<Text>().text = name;
    }

    //캐릭터 근력, 지구력, 스피드, 회복력 정보
    public void setStat() {
        initStat();
        var spects = userStore.itemSpects;
        
        int strength = spects.Char_strength;
        int speed = spects.Char_speed;
        int endurance = spects.Char_endurance;
        int recovery = spects.Char_regeneration;

        stats[0].text = strength.ToString();
        stats[1].text = endurance.ToString();
        stats[2].text = speed.ToString();
        stats[3].text = recovery.ToString();
        
        //아이템 장착 효과
        int item_str = spects.Item_strength;
        int item_end = spects.Item_endurance;
        int item_speed = spects.Item_speed;
        int item_reg = spects.Item_regeneration;

        //아이템 장착 효과 UI 반영
        if (item_str == 0) {
            incStats[0].gameObject.SetActive(false);
        }
        else {
            incStats[0].text = "+ " + item_str.ToString();
        }

        if (item_end == 0) {
            incStats[1].gameObject.SetActive(false);
        }
        else {
            incStats[1].text = "+ " + item_end.ToString();
        }

        if (item_speed == 0) {
            incStats[2].gameObject.SetActive(false);
        }
        else {
            incStats[2].text = "+ " + item_speed.ToString();
        }

        if (item_reg == 0) {
            incStats[3].gameObject.SetActive(false);
        }
        else {
            incStats[3].text = "+ " + item_reg.ToString();
        }
    }

    private void initStat() {
        for (int i = 0; i < incStats.Length; i++) {
            incStats[i].gameObject.SetActive(true);
        }
    }

    public void sideBarClicked(GameObject obj) {
        sm.playEffectSound(0);

        GameObject _sprite = obj.transform.Find("Image").gameObject;

        string name = _sprite.GetComponent<Image>().sprite.name;
        string[] str = name.Split('-');
        setMainChar(Int32.Parse(str[0]), Int32.Parse(str[1]));
    }

    //캐릭터 장착하기
    public void equipCharButton() {
        equip_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_EQUIP) as equip_act;
        act._type = equip_act.type.CHAR;
        act.id = selectedChar.GetComponent<Info>().id;
        gm.gameDispatcher.dispatch(act);

        equipButton.transform.Find("Check").gameObject.SetActive(true);
    }

    public void makeList() {
        removeList();
        int repCharIndex = charInvenStore.repCharacter.character;

        //내 캐릭터 리스트 생성
        character_inventory[] myChars = charInvenStore.my_characters;
        var allChars = charInvenStore.all_characters;

        float tmp = (float)allChars.Count / 3f;
        int pageNum = Mathf.CeilToInt(tmp);

        GameObject[] pages = new GameObject[pageNum];
        for(int i=0; i<pageNum; i++) {
            pages[i] = Instantiate(scroll_pagePref);
            pages[i].transform.SetParent(itemGrid.transform, false);

            GameObject pageIcon = Instantiate(scroll_pageIconPref);
            pageIcon.transform.SetParent(pageIconGrid.transform);
        }

        int pageIndex = 0;
        int itemIndex = 0;

        foreach (KeyValuePair<int, all_characters> dC in allChars) {
            GameObject item = Instantiate(itemPref);

            Debug.Log("Page Index : " + pageIndex + ", Item Index : " + itemIndex);

            item.transform.SetParent(pages[pageIndex].transform.GetChild(itemIndex).transform, false);
            item.transform.localPosition = Vector3.zero;
            if(itemIndex < 2) {
                itemIndex++;
            }
            else {
                pageIndex++;
                itemIndex = 0;
            }

            Info info = item.AddComponent<Info>();
            info.characterId = dC.Key;
            sbInfo sbInfo = item.AddComponent<sbInfo>();
            sbInfo.name = dC.Value.name;
            sbInfo.desc = dC.Value.desc;
            sbInfo.cost = dC.Value.cost;
            sbInfo.lvup_exps = dC.Value.lvup_exps;

            if (info.characterId == repCharIndex) {
                selectedChar = item;
            }

            var element = Array.Find(myChars, arr => arr.character.Equals(dC.Key));
            Text puzzles = item.transform.Find("Bottom/Text").GetComponent<Text>();
            //보유한 캐릭터
            if (element != null) {
                info.id = element.id;
                info.paid = element.paid;
                info.lv = element.lv;
                info.exp = element.exp;
                info.has_character = element.has_character;
                info.strength = element.status.strength;
                info.enurance = element.status.endurance;
                info.recovery = element.status.regeneration;
                info.speed = element.status.speed;

                if (element.has_character == "true") {
                    item.transform.Find("DeactiveContainer").gameObject.SetActive(false);
                    puzzles.text = "보유중";
                }
                else {
                    puzzles.text = info.paid + " / " + sbInfo.cost.ToString();
                }
            }
            //보유하고 있지 않은 캐릭터의 경우
            else {
                puzzles.text = "미보유";
                info.lv = 1;
            }
            Image sprite = item.transform.Find("Image").GetComponent<Image>();
            int imgIndex = info.characterId - 1;
            sprite.sprite = mV_controller.characters_slots[imgIndex].images[0];
            
            item.GetComponent<Button>().onClick.AddListener(() => charSelected(item));
        }

        scrollSnapRect.enabled = true;
    }

    private void removeList() {
        itemGrid.transform.DestroyChildren();
        pageIconGrid.transform.DestroyChildren();
    }

    public class Info : MonoBehaviour {
        public int id;
        public int paid;
        public int lv;
        public int exp;
        public int characterId;
        public string has_character;

        public int strength;
        public int enurance;
        public int recovery;
        public int speed;
    }

    public class sbInfo : MonoBehaviour {
        public string name;
        public string desc;
        public int cost;
        public int[] lvup_exps;
    }

    //public void onIllustPanel() {
    //    IllustPanel.SetActive(true);
    //}

    //public void offIllustPanel() {
    //    IllustPanel.SetActive(false);
    //}

    public void onDescPanel() {
        sm.playEffectSound(1);

        descModal.SetActive(true);
        Text header = descModal.transform.Find("InnerModal/Name").GetComponent<Text>();
        Text desc = descModal.transform.Find("InnerModal/Desc").GetComponent<Text>();

        sbInfo sbInfo = selectedChar.GetComponent<sbInfo>();
        header.text = sbInfo.name;
        desc.text = sbInfo.desc;
    }

    public void offDescPanel() {
        descModal.SetActive(false);
        sm.playEffectSound(0);
    }

    [System.Serializable]
    public class CharPrefArr {
        public GameObject[] Pref;
    }
}
