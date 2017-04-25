using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterViewControlller : MonoBehaviour {
    GameManager gm;
    Char_Inventory charInvenStore;
    public User userStore;
    //character_inventory[] characters;

    public GameObject
        mainStageChar,
        lv1Slot,
        lv10Slot,
        lv20Slot,
        itemPref,
        selectedChar;

    public GameObject equipButton;

    public UIAtlas[] atlasArr;
    public RuntimeAnimatorController[] animatorArr;

    public UIGrid
        itemGrid,
        sideBarGrid;

    public UILabel 
        lvLabel,
        charName,
        desc;

    void Awake() {
        gm = GameManager.Instance;
        charInvenStore = gm.charInvenStore;
    }

    void OnEnable() {
        getCharacters_act act = ActionCreator.createAction(ActionTypes.GARAGE_CHAR_INIT) as getCharacters_act;
        gm.gameDispatcher.dispatch(act);
    }

    void OnDisable() {
        selectedChar = null;
    }

    public void onCharInvenStore() {
        ActionTypes charStoreEventType = charInvenStore.eventType;

        if (charStoreEventType == ActionTypes.GARAGE_CHAR_INIT) {
            if (charInvenStore.storeStatus == storeStatus.NORMAL) {
                makeList();
                int index = 0;
                if (selectedChar != null) {
                    index = selectedChar.GetComponent<Info>().characterId;
                }
                else {
                    character_inventory charInfo = userStore.myData.represent_character.character_inventory;
                    index = charInfo.character;
                }

                if (charInvenStore.all_characters.ContainsKey(index).Equals(true)) {
                    all_characters tmp = charInvenStore.all_characters[index];
                    desc.text = tmp.desc;
                    charName.text = tmp.name;
                }
            }
        }
    }

    public void onUserListener() {
        ActionTypes userStoreEventType = userStore.eventType;

        if (userStoreEventType == ActionTypes.MYINFO) {
            if (userStore.storeStatus == storeStatus.NORMAL) {
                character_inventory charInfo = userStore.myData.represent_character.character_inventory;
                setMainChar(charInfo.character, charInfo.lv);
                setSideBar(charInfo.character);
                equipButton.SetActive(true);
                equipButton.transform.Find("Box/Check").gameObject.SetActive(true);
                lvLabel.text = "Lv. " + charInfo.lv.ToString();
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
        setDesc(sbInfo.desc);
        
        lvLabel.text = "Lv. " + info.lv.ToString();
        //Debug.Log(index);
        charName.text = sbInfo.name;
    }

    public void setEquipButton(int index, string hasChar) {
        if (hasChar == "true") {
            equipButton.SetActive(true);
            character_inventory charInfo = userStore.myData.represent_character.character_inventory;
            if (index == charInfo.character) {
                equipButton.transform.Find("Box/Check").gameObject.SetActive(true);
            }
            else {
                equipButton.transform.Find("Box/Check").gameObject.SetActive(false);
            }
        }
        else {
            equipButton.SetActive(false);
        }
    }

    public void setDesc(string data) {
        desc.text = data;
    }

    public void setMainChar(int index, int lv) {
        //mainStageChar.GetComponent<Animator>().runtimeAnimatorController = animatorArr[0];
        UISprite sprite = mainStageChar.GetComponent<UISprite>();
        //Debug.Log("index : " + index);
        sprite.atlas = atlasArr[index - 1];
        sprite.spriteName = index + "-" + (lv + 1) + "-main";
        sprite.MakePixelPerfect();
        sprite.gameObject.transform.localScale = new Vector3(0.5f, 0.5f);
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
        UISprite sprite = mainStageChar.GetComponent<UISprite>();
        sprite.spriteName = name + "-main";
    }

    //캐릭터 장착하기
    public void equipCharButton() {
        equip_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_EQUIP) as equip_act;
        act._type = equip_act.type.CHAR;
        act.id = selectedChar.GetComponent<Info>().id;
        gm.gameDispatcher.dispatch(act);

        equipButton.transform.Find("Box/Check").gameObject.SetActive(true);
    }

    //해제되지 않은 캐릭터 처리
    private void setLocked() {
        
    }

    //캐릭터 해금하기
    public void unlockChar() {
        if(selectedChar == null) {
            return;
        }
        garage_unlock_char act = ActionCreator.createAction(ActionTypes.CHAR_OPEN) as garage_unlock_char;
        act.id = selectedChar.GetComponent<Info>().id;
        gm.gameDispatcher.dispatch(act);
    }

    public void makeList() {
        removeList();
        //내 캐릭터 리스트 생성
        character_inventory[] myChars = charInvenStore.my_characters;
        int repCharIndex = charInvenStore.representChar.character_inventory.id;
        //character_inventory[] allChars = charInvenStore.all_characters;
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

            if (charInvenStore.all_characters.ContainsKey(info.characterId).Equals(true)) {
                all_characters tmp = charInvenStore.all_characters[info.characterId];

                sbInfo sbInfo = item.AddComponent<sbInfo>();
                sbInfo.name = tmp.name;
                sbInfo.desc = tmp.desc;
                sbInfo.cost = tmp.cost;

                item.transform.Find("DeactiveContainer/puzzles").GetComponent<UILabel>().text = info.paid + " / " + sbInfo.cost.ToString();
            }

            if(info.has_character == "true") {
                item.transform.Find("DeactiveContainer").gameObject.SetActive(false);
            }

            UISprite sprite = item.transform.Find("Portrait").GetComponent<UISprite>();
            int atlasIndex = info.characterId;
            sprite.atlas = atlasArr[atlasIndex - 1];
            sprite.spriteName = atlasIndex + "-1";

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
        public int characterId;
        public string has_character;
    }

    private class sbInfo : MonoBehaviour {
        public string name;
        public string desc;
        public int cost;
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }
}
