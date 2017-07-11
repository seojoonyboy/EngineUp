using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainViewController : MonoBehaviour {
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
        //getItems_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_INIT) as getItems_act;
        //act._type = equip_act.type.ITEM;
        //gm.gameDispatcher.dispatch(act);
        item_init initItemAct = ActionCreator.createAction(ActionTypes.ITEM_INIT) as item_init;
        initItemAct._type = equip_act.type.BOTH;
        gm.gameDispatcher.dispatch(initItemAct);

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
                int charIndex = userStore.myData.represent_character.character_inventory.character;
                int lv = userStore.myData.represent_character.character_inventory.lv;
                charSprite.atlas = atlasArr[charIndex - 1];
                charSprite.spriteName = charIndex + "-" + lv + "-main";
                charSprite.MakePixelPerfect();
                charSprite.width = (int)(charSprite.width * 0.4);
                charSprite.height = (int)(charSprite.height * 0.4);
            }
        }
    }

    public void onBicycleInvenListener() {
        if(bi.eventType == ActionTypes.GARAGE_ITEM_INIT) {
            if(bi.storeStatus == storeStatus.NORMAL) {
                RespGetItems[] items = bi.frameItems.ToArray(typeof(RespGetItems)) as RespGetItems[];
                foreach (RespGetItems item in items){
                    if(item.is_equiped == "true") {
                        UISprite sprite = bicycle.transform.Find("Frame").GetComponent<UISprite>();
                        sprite.atlas = bicycleAtlas;
                        sprite.spriteName = item.item.id.ToString();
                    }
                }

                items = bi.engineItems.ToArray(typeof(RespGetItems)) as RespGetItems[];
                foreach (RespGetItems item in items) {
                    if (item.is_equiped == "true") {
                        UISprite sprite = bicycle.transform.Find("Engine").GetComponent<UISprite>();
                        sprite.atlas = bicycleAtlas;
                        sprite.spriteName = item.item.id.ToString();
                    }
                }

                items = bi.wheelItems.ToArray(typeof(RespGetItems)) as RespGetItems[];
                foreach (RespGetItems item in items) {
                    if (item.is_equiped == "true") {
                        UISprite sprite = bicycle.transform.Find("Wheel").GetComponent<UISprite>();
                        sprite.atlas = bicycleAtlas;
                        sprite.spriteName = item.item.id.ToString();
                    }
                }
            }
        }
    }
}
