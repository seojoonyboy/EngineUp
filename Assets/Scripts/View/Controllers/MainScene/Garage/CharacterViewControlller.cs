using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterViewControlller : MonoBehaviour {
    GameManager gm;
    Char_Inventory charInvenStore;
    public User userStore;
    public TweenManager tM;
    //character_inventory[] characters;

    public GameObject
        mainStage,
        lv1Slot,
        lv10Slot,
        lv20Slot,
        itemPref,
        selectedChar,
        IllustPanel,
        DescPanel,
        blockingCollPanel;

    public GameObject 
        equipButton,
        nonpossessionButton;
    public UILabel[] 
        stats,
        incStats;

    public UIAtlas[] atlasArr;

    public UIGrid
        itemGrid,
        sideBarGrid;

    public UILabel 
        lvLabel,
        charName;
    public CharPrefArr[] Characters;

    public UISlider friendlySlider;

    private GameObject prevMainChar;

    private TweenPosition tP;
    private bool isReverse_tp;

    void Awake() {
        gm = GameManager.Instance;
        charInvenStore = gm.charInvenStore;

        tP = gameObject.transform.Find("Background").GetComponent<TweenPosition>();
    }

    void OnEnable() {
        tweenPos();

        blockingCollPanel.SetActive(true);
        isReverse_tp = false;
    }

    void OnDisable() {
        selectedChar = null;

        tP.ResetToBeginning();
        nonpossessionButton.SetActive(false);
    }

    public void tweenPos() {
        bool isTweening = tM.isTweening;
        if(isTweening) {
            return;
        }
        tM.isTweening = true;
        blockingCollPanel.SetActive(true);
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
        }
    }

    public void tpFinished() {
        tM.isTweening = false;
        blockingCollPanel.SetActive(false);

        if (isReverse_tp) {
            gameObject.SetActive(false);
            gameObject.transform.Find("TopPanel").gameObject.SetActive(false);
        }

        else {
            gameObject.transform.Find("TopPanel").gameObject.SetActive(true);

            getCharacters_act act = ActionCreator.createAction(ActionTypes.GARAGE_CHAR_INIT) as getCharacters_act;
            gm.gameDispatcher.dispatch(act);
        }

        isReverse_tp = true;
    }

    public void onCharInvenStore() {
        if(gameObject.activeSelf) {
            ActionTypes charStoreEventType = charInvenStore.eventType;

            if (charStoreEventType == ActionTypes.GARAGE_CHAR_INIT) {
                if (charInvenStore.storeStatus == storeStatus.NORMAL) {
                    character_inventory charInfo = userStore.myData.represent_character.character_inventory;
                    int equpedCharIndex = charInfo.character;
                    makeList(equpedCharIndex);
                }
            }
        }
    }

    public void onUserListener() {
        ActionTypes userStoreEventType = userStore.eventType;

        if (userStoreEventType == ActionTypes.MYINFO) {
            if (userStore.storeStatus == storeStatus.NORMAL) {
                if(gameObject.activeSelf) {
                    character_inventory charInfo = userStore.myData.represent_character.character_inventory;
                    setMainChar(charInfo.character, charInfo.lv);
                    setSideBar(charInfo.character, charInfo.lv);
                    foreach (character_inventory character in charInvenStore.my_characters) {
                        if (character.character == charInfo.character) {
                            setStat(character);
                        }
                    }

                    if (charInvenStore.all_characters.ContainsKey(charInfo.character).Equals(true)) {
                        all_characters tmp = charInvenStore.all_characters[charInfo.character];
                        setFriendlySlider(charInfo.lv, tmp.lvup_exps, charInfo.exp);
                        setSideBarName(tmp.name);
                        charName.text = tmp.name;
                    }

                    equipButton.SetActive(true);
                    equipButton.transform.Find("Check").gameObject.SetActive(true);
                    lvLabel.text = "Lv " + charInfo.lv.ToString();
                }
            }
        }
    }

    //내 캐릭터중 하나 선택시
    public void charSelected(GameObject obj) {
        selectedChar = obj;

        Info info = obj.GetComponent<Info>();
        sbInfo sbInfo = obj.GetComponent<sbInfo>();
        setMainChar(info.characterId, info.lv);
        setSideBar(info.characterId, info.lv);
        setSideBarName(sbInfo.name);
        setEquipButton(info.characterId, info.has_character);

        lvLabel.text = "Lv. " + info.lv.ToString();
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
            nonpossessionButton.SetActive(false);
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
            nonpossessionButton.SetActive(true);
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
        if(prevMainChar != null) {
            Destroy(prevMainChar);
        }

        int arrIndex = index - 1;
        int arrSubIndex = lv - 1;
        if(arrSubIndex == -1) {
            arrSubIndex = 0;
        }
        GameObject charPref = Instantiate(Characters[arrIndex].Pref[arrSubIndex]);
        prevMainChar = charPref;
        charPref.transform.SetParent(mainStage.transform);
        charPref.transform.localPosition = new Vector3(0f, 40f, 0f);
        charPref.transform.localScale = Vector3.one;
        charPref.name = "Character";
    }

    public void setSideBar(int index, int lv) {
        sideBarGrid.transform.Find("Lv10Container/DeactiveContainer").gameObject.SetActive(false);
        sideBarGrid.transform.Find("Lv20Container/DeactiveContainer").gameObject.SetActive(false);

        UISprite sprite = sideBarGrid.transform.Find("Lv1Container/Sprite").GetComponent<UISprite>();
        sprite.atlas = atlasArr[index - 1];
        sprite.spriteName = index + "-1";

        sprite = sideBarGrid.transform.Find("Lv10Container/Sprite").GetComponent<UISprite>();
        sprite.atlas = atlasArr[index - 1];
        sprite.spriteName = index + "-2";
        if(lv < 2) {
            sideBarGrid.transform.Find("Lv10Container/DeactiveContainer").gameObject.SetActive(true);
        }

        sprite = sideBarGrid.transform.Find("Lv20Container/Sprite").GetComponent<UISprite>();
        sprite.atlas = atlasArr[index - 1];
        sprite.spriteName = index + "-3";
        if(lv < 3) {
            sideBarGrid.transform.Find("Lv20Container/DeactiveContainer").gameObject.SetActive(true);
        }
    }

    private void setSideBarName(string name) {
        lv1Slot.transform.Find("Label").GetComponent<UILabel>().text = "Lv1\n" + name;
        lv10Slot.transform.Find("Label").GetComponent<UILabel>().text = "Lv2\n" + name;
        lv20Slot.transform.Find("Label").GetComponent<UILabel>().text = "Lv3\n" + name;
    }

    //캐릭터 근력, 지구력, 스피드, 회복력 정보
    public void setStat(character_inventory character) {
        initStat();
        charStat stat = character.status;
        
        int strength = stat.strength;
        int speed = stat.speed;
        int endurance = stat.endurance;
        int recovery = stat.regeneration;

        stats[0].text = strength.ToString();
        stats[1].text = endurance.ToString();
        stats[2].text = speed.ToString();
        stats[3].text = recovery.ToString();

        BicycleItem_Inventory bS = gm.bicycleInventStore;
        var euipedItems = bS.equipedItemIndex;

        int itemEnd = 0;
        int itemSpeed = 0;
        int itemRecovery = 0;
        int itemStrength = 0;

        for (int i = 0; i < euipedItems.Length; i++) {
            if (euipedItems[i] != null) {
                itemEnd += euipedItems[i].item.endurance;
                itemSpeed += euipedItems[i].item.speed;
                itemRecovery += euipedItems[i].item.regeneration;
                itemStrength += euipedItems[i].item.strength;
            }
        }

        if (itemStrength == 0) {
            incStats[0].gameObject.SetActive(false);
        }
        else {
            incStats[0].text = "+ " + itemStrength;
        }

        if (itemEnd == 0) {
            incStats[1].gameObject.SetActive(false);
        }
        else {
            incStats[1].text = "+ " + itemEnd;
        }

        if (itemSpeed == 0) {
            incStats[2].gameObject.SetActive(false);
        }
        else {
            incStats[2].text = "+ " + itemSpeed;
        }

        if (itemRecovery == 0) {
            incStats[3].gameObject.SetActive(false);
        }
        else {
            incStats[3].text = "+ " + itemRecovery;
        }
    }

    private void initStat() {
        for (int i = 0; i < incStats.Length; i++) {
            incStats[i].gameObject.SetActive(true);
        }
    }

    public void sideBarClicked(GameObject obj) {
        GameObject _sprite = obj.transform.Find("Sprite").gameObject;

        string name = _sprite.GetComponent<UISprite>().spriteName;
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

    public void makeList(int equipedCharIndex = -1) {
        removeList();
        int repCharIndex = charInvenStore.representChar.character_inventory.id;

        //내 캐릭터 리스트 생성
        character_inventory[] myChars = charInvenStore.my_characters;
        var allChars = charInvenStore.all_characters;

        foreach(KeyValuePair<int, all_characters> dC in allChars) {
            GameObject item = Instantiate(itemPref);
            item.transform.SetParent(itemGrid.transform);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;

            Info info = item.AddComponent<Info>();
            info.characterId = dC.Key;
            sbInfo sbInfo = item.AddComponent<sbInfo>();
            sbInfo.name = dC.Value.name;
            sbInfo.desc = dC.Value.desc;
            sbInfo.cost = dC.Value.cost;
            sbInfo.lvup_exps = dC.Value.lvup_exps;

            var element = Array.Find(myChars, arr => arr.character.Equals(dC.Key));
            //보유한 캐릭터
            if(element != null) {
                info.paid = element.paid;
                info.lv = element.lv;
                info.exp = element.exp;
                info.has_character = element.has_character;
                info.strength = element.status.strength;
                info.enurance = element.status.endurance;
                info.recovery = element.status.regeneration;
                info.speed = element.status.speed;

                if (repCharIndex == element.character) {
                    selectedChar = item;
                }

                if(element.has_character == "true") {
                    item.transform.Find("DeactiveContainer").gameObject.SetActive(false);
                }

                item.transform.Find("Puzzles/Value").GetComponent<UILabel>().text = info.paid + " / " + sbInfo.cost.ToString();
            }
            //보유하고 있지 않은 캐릭터의 경우
            else {
                item.transform.Find("Puzzles/Value").GetComponent<UILabel>().text = "미보유";
            }
            UISprite sprite = item.transform.Find("Portrait").GetComponent<UISprite>();
            int atlasIndex = info.characterId;
            sprite.atlas = atlasArr[atlasIndex - 1];
            sprite.spriteName = atlasIndex + "-1-slider";

            UIDragScrollView dScrollView = item.AddComponent<UIDragScrollView>();
            dScrollView.scrollView = itemGrid.transform.parent.GetComponent<UIScrollView>();

            EventDelegate.Parameter param = new EventDelegate.Parameter();
            EventDelegate onClick = new EventDelegate(this, "charSelected");
            param.obj = item;
            onClick.parameters[0] = param;
            EventDelegate.Add(item.GetComponent<UIButton>().onClick, onClick);
        }
        //sidebar 갱신
        init();
    }

    private void removeList() {
        itemGrid.transform.DestroyChildren();
    }

    private void init() {
        itemGrid.repositionNow = true;
        itemGrid.Reposition();
    }

    private class Info : MonoBehaviour {
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

    private class sbInfo : MonoBehaviour {
        public string name;
        public string desc;
        public int cost;
        public int[] lvup_exps;
    }

    public void onIllustPanel() {
        IllustPanel.SetActive(true);
    }

    public void offIllustPanel() {
        IllustPanel.SetActive(false);
    }

    public void onDescPanel() {
        DescPanel.SetActive(true);

        GameObject _desc = DescPanel.transform.Find("InnerBackground").gameObject;
        UILabel header = _desc.transform.Find("Header").GetComponent<UILabel>();
        UILabel desc = _desc.transform.Find("Desc").GetComponent<UILabel>();

        sbInfo sbInfo = selectedChar.GetComponent<sbInfo>();
        header.text = sbInfo.name;
        desc.text = sbInfo.desc;

        Info info = selectedChar.GetComponent<Info>();
        int imageIndex = info.characterId - 1;
        UISprite sprite = _desc.transform.Find("Portrait").GetComponent<UISprite>();
        sprite.atlas = atlasArr[imageIndex];
        sprite.spriteName = info.characterId + "-" + info.lv;
    }

    public void offDescPanel() {
        DescPanel.SetActive(false);
    }

    [System.Serializable]
    public class CharPrefArr {
        public GameObject[] Pref;
    }
}
