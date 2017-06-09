using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Awake() {
        gm = GameManager.Instance;
        gm.userStore.addListener(onUserListener);
        gm.bicycleInventStore.addListener(onBicycleInvenListener);

        userStore = gm.userStore;
        bi = gm.bicycleInventStore;
    }

    void Start() {
        getItems_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_INIT) as getItems_act;
        act._type = equip_act.type.ITEM;
        gm.gameDispatcher.dispatch(act);

        //튜토리얼 진행 여부 확인
        //int isFirstPlay = PlayerPrefs.GetInt("isFirstPlay");
        //if(isFirstPlay != 1) {
        //    tutorial.SetActive(true);
        //}
    }

    public void onUserListener() {
        if(userStore.eventType == ActionTypes.MYINFO) {
            if(userStore.storeStatus == storeStatus.NORMAL) {
                int charIndex = userStore.myData.represent_character.character_inventory.character;
                int lv = userStore.myData.represent_character.character_inventory.lv;
                charSprite.atlas = atlasArr[charIndex - 1];
                charSprite.spriteName = charIndex + "-" + lv + "-main";
                charSprite.MakePixelPerfect();
                charSprite.width = (int)(charSprite.width * 0.7);
                charSprite.height = (int)(charSprite.height * 0.7);
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
