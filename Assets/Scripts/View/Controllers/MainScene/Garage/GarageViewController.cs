using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageViewController : MonoBehaviour {
    public CharacterViewControlller charController;
    public BicycleViewController bicycleController;

    private GameManager gm;
    
    private BicycleItem_Inventory bicycleItemStore;
    private Char_Inventory charItemStore;
    public User userStore;

    void Awake() {
        gm = GameManager.Instance;

        bicycleItemStore = gm.bicycleInventStore;
        charItemStore = gm.charInvenStore;
    }

    public void onUserStoreListener() {
        ActionTypes userStoreEventType = userStore.eventType;

        if (userStoreEventType == ActionTypes.MYINFO) {
            if(userStore.storeStatus == storeStatus.NORMAL) {
                character_inventory charInfo = userStore.myData.represent_character.character_inventory;
                charController.setMainChar(charInfo.id);
                charController.setSideBar(charInfo.id);
            }
        }
    }

    public void onBicycleStoreListener() {
        ActionTypes bicycleItemStoreEventType = bicycleItemStore.eventType;

        if (bicycleItemStoreEventType == ActionTypes.GARAGE_ITEM_INIT) {
            if (bicycleItemStore.storeStatus == storeStatus.NORMAL) {
                bicycleController.makeList();
            }
        }

        else if (bicycleItemStoreEventType == ActionTypes.GARAGE_SELL) {
            if (bicycleItemStore.storeStatus == storeStatus.NORMAL) {
                bicycleController.makeList();
            }
        }
    }

    public void onCharStoreListener() {
        ActionTypes charStoreEventType = charItemStore.eventType;

        if(charStoreEventType == ActionTypes.GARAGE_CHAR_INIT) {
            charController.makeList();
            Debug.Log("?!");
        }

        //if(charStoreEventType == ActionTypes.GARAGE_ITEM_EQUIP) {
        //    Debug.Log("캐릭터 장착");
        //}
    }

    public void offPanel() {
        gameObject.SetActive(false);
        UIToggle toggle = UIToggle.GetActiveToggle(2);
        toggle.value = false;
    }
}
