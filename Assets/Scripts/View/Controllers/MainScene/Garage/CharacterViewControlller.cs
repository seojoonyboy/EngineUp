using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterViewControlller : MonoBehaviour {
    GameManager gm;
    Char_Inventory charInvenStore;
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
        unlockButton;

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

    void OnEnable() {
        getCharacters_act act = ActionCreator.createAction(ActionTypes.GARAGE_CHAR_INIT) as getCharacters_act;
        gm.gameDispatcher.dispatch(act);
    }

    //내 캐릭터중 하나 선택시
    public void charSelected(GameObject obj) {
        selectedChar = obj;

        Info info = obj.GetComponent<Info>();
        Debug.Log("ID : " + info.id);
        setMainChar(info.id);
        setSideBar(info.id);
        //setSlot();
        //setButton();
        //Debug.Log(index);
    }

    private void setMainChar(int index) {
        //mainStageChar.GetComponent<Animator>().runtimeAnimatorController = animatorArr[0];
        UISprite sprite = mainStageChar.GetComponent<UISprite>();
        sprite.atlas = atlasArr[index - 1];
        sprite.spriteName = index.ToString();
    }

    private void setSideBar(int index) {
        UISprite sprite = sideBarGrid.transform.Find("Lv1Container/Sprite").GetComponent<UISprite>();
        sprite.atlas = atlasArr[index - 1];
        sprite.spriteName = index.ToString();
    }

    private void setSlot() {
        //for(int i=0; i<characters.Length; i++) {
        //    GameObject item = Instantiate(itemPref);
        //    item.transform.SetParent(itemGrid.transform);
        //}
        setLocked();
    }

    //해제되지 않은 캐릭터 처리
    private void setLocked() {
        
    }

    //해금 가능 조건인지 판단
    private void setButton() {
        //해금이 가능한 경우
        //해금이 불가능한 경우
        
    }

    //캐릭터 해금하기
    public void unlockChar() {
        
    }

    public void offCharDesc() {
        descModal.SetActive(false);
    }

    public void makeList() {
        removeList();
        //내 캐릭터 리스트 생성
        all_characters[] allChars = charInvenStore.all_characters;
        //character_inventory[] allChars = charInvenStore.all_characters;
        for (int i=0; i<allChars.Length; i++) {
            GameObject item = Instantiate(itemPref);
            item.transform.SetParent(itemGrid.transform);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;

            Info info = item.AddComponent<Info>();
            info.id = allChars[i].id;
            //info.paid = allChars[i].paid;
            //info.lv = allChars[i].lv;
            //info.exp = allChars[i].exp;
            //info.user = allChars[i].user;
            //info.character = allChars[i].character;

            UISprite sprite = item.transform.Find("Portrait").GetComponent<UISprite>();
            sprite.atlas = atlasArr[i];
            sprite.spriteName = (i + 1).ToString();

            UIDragScrollView dScrollView = item.AddComponent<UIDragScrollView>();
            dScrollView.scrollView = itemGrid.transform.parent.GetComponent<UIScrollView>();

            EventDelegate.Parameter param = new EventDelegate.Parameter();
            EventDelegate onClick = new EventDelegate(this, "charSelected");
            param.obj = item;
            onClick.parameters[0] = param;
            EventDelegate.Add(item.GetComponent<UIButton>().onClick, onClick);
            selectedChar = item;
        }
        //sidebar 갱신

        //메인화면 캐릭터 갱신
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
        all_characters[] allChars = charInvenStore.all_characters;
        Debug.Log(allChars.Length);
        for (int i=0; i< allChars.Length; i++) {
            if(allChars[i].id == index) {
                nameLabel.text = "이름 : " + allChars[i].name;
                descLabel.text = "설명 : " + allChars[i].desc;
            }
        }
    }

    private class Info : MonoBehaviour {
        public int id;
        public int paid;
        public int lv;
        public int exp;
        public int user;
        public int character;
    }
}
