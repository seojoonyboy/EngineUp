using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterViewControlller : MonoBehaviour {
    GameManager gm;
    Char_Inventory charInvenStore;
    public User userStore;
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

    public GameObject equipButton;
    public UILabel[] stats;

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

        getCharacters_act act = ActionCreator.createAction(ActionTypes.GARAGE_CHAR_INIT) as getCharacters_act;
        gm.gameDispatcher.dispatch(act);
    }

    void OnDisable() {
        selectedChar = null;

        tP.transform.Find("TopPanel").gameObject.SetActive(false);
        tP.ResetToBeginning();
    }

    public void tweenPos() {
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
        tP.transform.Find("TopPanel").gameObject.SetActive(true);
        blockingCollPanel.SetActive(false);

        if (isReverse_tp) {
            gameObject.SetActive(false);
        }

        isReverse_tp = true;
    }

    public void onCharInvenStore() {
        ActionTypes charStoreEventType = charInvenStore.eventType;

        if (charStoreEventType == ActionTypes.GARAGE_CHAR_INIT) {
            if (charInvenStore.storeStatus == storeStatus.NORMAL) {
                character_inventory charInfo = userStore.myData.represent_character.character_inventory;
                int equpedCharIndex = charInfo.character;
                makeList(equpedCharIndex);
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
                    setSideBar(charInfo.character);

                    if (charInvenStore.all_characters.ContainsKey(charInfo.character).Equals(true)) {
                        all_characters tmp = charInvenStore.all_characters[charInfo.character];
                        setFriendlySlider(charInfo.lv, tmp.lvup_exps, charInfo.exp);
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
        Debug.Log("ID : " + info.id);
        setMainChar(info.characterId, info.lv);
        setSideBar(info.characterId);
        setEquipButton(info.characterId, info.has_character);
        
        lvLabel.text = "Lv. " + info.lv.ToString();
        charName.text = sbInfo.name;
        setFriendlySlider(info.lv, sbInfo.lvup_exps, info.exp);
    }

    public void setEquipButton(int index, string hasChar) {
        if (hasChar == "true") {
            equipButton.SetActive(true);
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
        }
    }

    public void setFriendlySlider(int lv, int[] lvUp_exp, int exp) {
        float offset = 0;
        switch(lv) {
            case 1:
                offset = 1 / lvUp_exp[0];
                break;
            case 2:
                offset = 1 / lvUp_exp[1];
                break;
        }
        friendlySlider.value = exp * offset;
    }

    public void setMainChar(int index, int lv) {
        Debug.Log("Index : " + index + ", LV : " + lv);
        if(prevMainChar != null) {
            Destroy(prevMainChar);
        }

        int arrIndex = index - 1;
        int arrSubIndex = lv - 1;
        GameObject charPref = Instantiate(Characters[arrIndex].Pref[arrSubIndex]);
        prevMainChar = charPref;
        charPref.transform.SetParent(mainStage.transform);
        charPref.transform.localPosition = new Vector3(0f, 40f, 0f);
        charPref.transform.localScale = Vector3.one;
        charPref.name = "Character";
    }

    public void setSideBar(int index) {
        UISprite sprite = sideBarGrid.transform.Find("Lv1Container/Sprite").GetComponent<UISprite>();
        sprite.atlas = atlasArr[index - 1];
        sprite.spriteName = index + "-1";

        sprite = sideBarGrid.transform.Find("Lv10Container/Sprite").GetComponent<UISprite>();
        sprite.atlas = atlasArr[index - 1];
        sprite.spriteName = index + "-2";

        sprite = sideBarGrid.transform.Find("Lv20Container/Sprite").GetComponent<UISprite>();
        sprite.atlas = atlasArr[index - 1];
        sprite.spriteName = index + "-3";
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
        //내 캐릭터 리스트 생성
        character_inventory[] myChars = charInvenStore.my_characters;
        int repCharIndex = charInvenStore.representChar.character_inventory.id;

        for (int i=0; i< myChars.Length; i++) {
            GameObject item = Instantiate(itemPref);
            item.transform.SetParent(itemGrid.transform);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;

            Info info = item.AddComponent<Info>();

            info.id = myChars[i].id;
            info.characterId = myChars[i].character;
            info.has_character = myChars[i].has_character;
            info.paid = myChars[i].paid;
            info.lv = myChars[i].lv;
            info.exp = myChars[i].exp;

            //대표 캐릭터인 경우
            if(info.characterId == equipedCharIndex) {
                selectedChar = item;
            }

            if (charInvenStore.all_characters.ContainsKey(info.characterId).Equals(true)) {
                all_characters tmp = charInvenStore.all_characters[info.characterId];

                sbInfo sbInfo = item.AddComponent<sbInfo>();
                sbInfo.name = tmp.name;
                sbInfo.desc = tmp.desc;
                sbInfo.cost = tmp.cost;
                sbInfo.lvup_exps = tmp.lvup_exps;

                item.transform.Find("Puzzles/Value").GetComponent<UILabel>().text = info.paid + " / " + sbInfo.cost.ToString();
            }

            if(info.has_character == "true") {
                item.transform.Find("DeactiveContainer").gameObject.SetActive(false);
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
