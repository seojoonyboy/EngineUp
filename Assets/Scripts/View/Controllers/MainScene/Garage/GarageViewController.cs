using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageViewController : MonoBehaviour {
    public CharacterViewControlller charController;
    public BicycleViewController bicycleController;

    private GameManager gm;
    
    private BicycleItem_Inventory bicycleItemStore;
    public Char_Inventory charItemStore;
    void Awake() {
        gm = GameManager.Instance;
        bicycleItemStore = gm.bicycleInventStore;
        charItemStore = gm.charInvenStore;
    }

    public void onStoreListener() {
        ActionTypes bicycleItemStoreEventType = bicycleItemStore.eventType;
        ActionTypes charStoreEventType = charItemStore.eventType;
        if (bicycleItemStoreEventType == ActionTypes.GARAGE_ITEM_INIT) {
            if(bicycleItemStore.storeStatus == storeStatus.NORMAL) {
                bicycleController.makeList();
            }
        }

        else if (bicycleItemStoreEventType == ActionTypes.GARAGE_SELL) {
            if (bicycleItemStore.storeStatus == storeStatus.NORMAL) {
                bicycleController.makeList();
            }
        }

        else if (charStoreEventType == ActionTypes.GARAGE_CHAR_INIT) {
            if (charItemStore.storeStatus == storeStatus.NORMAL) {
                charController.makeList();
            }
        }
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }
}
