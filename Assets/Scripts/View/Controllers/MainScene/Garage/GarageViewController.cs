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

    public void onBicycleStoreListener() {
        ActionTypes bicycleItemStoreEventType = bicycleItemStore.eventType;
        if(bicycleItemStoreEventType == ActionTypes.GARAGE_ITEM_INIT) {
            if(bicycleItemStore.storeStatus == storeStatus.NORMAL) {
                bicycleController.makeList();
            }
        }

        if (bicycleItemStoreEventType == ActionTypes.GARAGE_SELL) {
            if (bicycleItemStore.storeStatus == storeStatus.NORMAL) {
                bicycleController.makeList();
            }
        }
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }
}
