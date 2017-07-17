using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainViewController : MonoBehaviour {
    public GameObject map;
    public UISprite charSprite;
    private GameManager gm;
    private User userStore;
    private BicycleItem_Inventory bi;
    public UIAtlas[] atlasArr;
    public UIAtlas bicycleAtlas;

    public GameObject 
        bicycle,
        tutorial;

    public Sprite[] 
        ranks,
        partner_busts_m,
        Bicycles_items_slot,
        Bicycles_items_stage;

    public SpritesArr[] 
        characters_busts_sm,
        characters_entire_body;

    void Awake() {
        gm = GameManager.Instance;
        gm.userStore.addListener(onUserListener);
        gm.bicycleInventStore.addListener(onBicycleInvenListener);

        userStore = gm.userStore;
        bi = gm.bicycleInventStore;
    }

    void Start() {
        item_init initItemAct = ActionCreator.createAction(ActionTypes.ITEM_INIT) as item_init;
        initItemAct._type = equip_act.type.ITEM;
        gm.gameDispatcher.dispatch(initItemAct);

        item_init _act = ActionCreator.createAction(ActionTypes.ITEM_INIT) as item_init;
        _act._type = equip_act.type.CHAR;
        gm.gameDispatcher.dispatch(_act);

        MyInfo myInfoAct = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
        gm.gameDispatcher.dispatch(myInfoAct);

        //튜토리얼 진행 여부 확인
        int isFirstPlay = PlayerPrefs.GetInt("isFirstPlay");
        if (isFirstPlay != 1) {
            tutorial.SetActive(true);
            charSprite.gameObject.SetActive(false);
        }
    }

    public void onUserListener() {
        if(userStore.eventType == ActionTypes.MYINFO) {
            if(userStore.storeStatus == storeStatus.NORMAL) {
                //int charIndex = userStore.myData.represent_character.character_inventory.character;
                //int lv = userStore.myData.represent_character.character_inventory.lv;
            }
        }
    }

    public void onBicycleInvenListener() {
        if(bi.eventType == ActionTypes.ITEM_INIT) {
            if (bi.storeStatus == storeStatus.NORMAL) {
                //UISprite sprite = null;

                //sprite = bicycle.transform.Find("Wheel").GetComponent<UISprite>();
                //sprite.atlas = bicycleAtlas;
                //if (bi.equipedItemIndex[0] != null) {
                //    sprite.spriteName = bi.equipedItemIndex[0].item.id.ToString();
                //}
                //else {
                //    sprite.spriteName = "6";
                //}

                //sprite = bicycle.transform.Find("Frame").GetComponent<UISprite>();
                //sprite.atlas = bicycleAtlas;
                //if (bi.equipedItemIndex[1] != null) {
                //    sprite.spriteName = bi.equipedItemIndex[1].item.id.ToString();
                //}
                //else {
                //    sprite.spriteName = "3";
                //}

                //sprite = bicycle.transform.Find("Engine").GetComponent<UISprite>();
                //sprite.atlas = bicycleAtlas;
                //if (bi.equipedItemIndex[2] != null) {
                //    sprite.spriteName = bi.equipedItemIndex[2].item.id.ToString();
                //}
                //else {
                //    sprite.spriteName = "9";
                //}
            }
        }
    }
}
