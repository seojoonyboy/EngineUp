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

    public GameObject
        disabledUnlockButton,
        unlockButton,
        equipButton;

    public GameObject
        descModal;

    public UIAtlas[] atlasArr;
    public RuntimeAnimatorController[] animatorArr;

    public UIGrid
        itemGrid,
        sideBarGrid;

    public UILabel lvLabel;

    void Awake() {
        gm = GameManager.Instance;
        charInvenStore = gm.charInvenStore;
    }

    void Start() {
        getCharacters_act act = ActionCreator.createAction(ActionTypes.GARAGE_CHAR_INIT) as getCharacters_act;
        gm.gameDispatcher.dispatch(act);
    }

    public void onCharInvenStore() {
        ActionTypes charStoreEventType = charInvenStore.eventType;

        if (charStoreEventType == ActionTypes.GARAGE_CHAR_INIT) {
            makeList();
        }
    }

    public void onUserListener() {
        ActionTypes userStoreEventType = userStore.eventType;

        if (userStoreEventType == ActionTypes.MYINFO) {
            if (userStore.storeStatus == storeStatus.NORMAL) {
                character_inventory charInfo = userStore.myData.represent_character.character_inventory;
                setMainChar(charInfo.id);
                setSideBar(charInfo.id);
            }
        }
    }

    //내 캐릭터중 하나 선택시
    public void charSelected(GameObject obj) {
        selectedChar = obj;

        Info info = obj.GetComponent<Info>();
        Debug.Log("ID : " + info.id);
        setMainChar(info.characterId);
        setSideBar(info.characterId);
        
        //setSlot();
        setButton(info);
        //Debug.Log(index);
    }

    public void setMainChar(int index) {
        //mainStageChar.GetComponent<Animator>().runtimeAnimatorController = animatorArr[0];
        UISprite sprite = mainStageChar.GetComponent<UISprite>();
        sprite.atlas = atlasArr[index - 1];
        sprite.spriteName = index + "-1-main";
        sprite.MakePixelPerfect();
        sprite.gameObject.transform.localScale = new Vector3(0.5f, 0.5f);
    }

    public void setSideBar(int index) {
        UISprite sprite = sideBarGrid.transform.Find("Lv1Container/Sprite").GetComponent<UISprite>();
        sprite.atlas = atlasArr[index - 1];
        sprite.spriteName = index + "-1";

        sprite = sideBarGrid.transform.Find("Lv10Container/Sprite").GetComponent<UISprite>();
        sprite.spriteName = index + "-2";

        sprite = sideBarGrid.transform.Find("Lv20Container/Sprite").GetComponent<UISprite>();
        sprite.spriteName = index + "-3";
    }

    //캐릭터 장착하기
    public void equipCharButton() {
        equip_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_EQUIP) as equip_act;
        act._type = equip_act.type.CHAR;
        act.id = selectedChar.GetComponent<Info>().id;
        gm.gameDispatcher.dispatch(act);
    }

    //해제되지 않은 캐릭터 처리
    private void setLocked() {
        
    }

    //해금 가능 조건인지 판단
    private void setButton(Info info) {
        //이미 해금이 되어있는 경우
        if (info.has_character == "true") {
            equipButton.SetActive(true);
            unlockButton.SetActive(false);
        }
        else if(info.has_character == "false") {
            equipButton.SetActive(false);
            unlockButton.SetActive(true);
            if (charInvenStore.all_characters.ContainsKey(info.characterId).Equals(true)) {
                all_characters tmp = charInvenStore.all_characters[info.characterId];
                //해금이 불가능한 경우 (조각이 부족한 경우)
                if(info.paid < tmp.cost) {
                    unlockButton.GetComponent<UIButton>().isEnabled = false;
                    Debug.Log("조각이 불충분합니다.");
                }
                //해금이 가능한 경우 (조각이 모두 있는 경우)
                else if (info.paid >= tmp.cost) {
                    unlockButton.GetComponent<UIButton>().isEnabled = true;
                    Debug.Log("조각이 충분합니다.");
                }
            }

        }

    }

    //캐릭터 해금하기
    public void unlockChar() {
        garage_unlock_char act = ActionCreator.createAction(ActionTypes.CHAR_OPEN) as garage_unlock_char;
        act.id = selectedChar.GetComponent<Info>().id;
        gm.gameDispatcher.dispatch(act);
    }

    public void offCharDesc() {
        descModal.SetActive(false);
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

            if(info.id == repCharIndex) {
                selectedChar = item;
                if (info.has_character == "true") {
                    equipButton.SetActive(true);
                    item.transform.Find("DeactiveContainer").gameObject.SetActive(false);
                }
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

    public void onDetailModal() {
        descModal.SetActive(true);
        Info info = selectedChar.GetComponent<Info>();
        GameObject modal = descModal.transform.Find("Modal").gameObject;
        UILabel nameLabel = modal.transform.Find("Name").GetComponent<UILabel>();
        UILabel descLabel = modal.transform.Find("Desc").GetComponent<UILabel>();

        int index = info.id;
        sbInfo sbinfo = selectedChar.GetComponent<sbInfo>();
        nameLabel.text = "이름 : " + sbinfo.name;
        descLabel.text = "설명 : " + sbinfo.desc;
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
