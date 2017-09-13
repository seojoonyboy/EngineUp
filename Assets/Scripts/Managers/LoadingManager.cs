using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : Singleton<LoadingManager> {
    protected LoadingManager() { }

    public GameObject loadingModal;
    private GameManager gm;
    private User userstore;
    private BicycleItem_Inventory bi_store;
    private Char_Inventory ci_store;
    private Friends fr_store;
    private Groups gr_store;

    public void onLoading() {
        loadingModal.SetActive(true);
    }

    public void offLoading() {
        loadingModal.SetActive(false);
    }

    void Awake() {
        gm = GameManager.Instance;
        userstore = gm.userStore;
        bi_store = gm.bicycleInventStore;
        ci_store = gm.charInvenStore;
        fr_store = gm.friendsStore;
        gr_store = gm.groupStore;

        userstore.addListener(user_listener);
        bi_store.addListener(bi_listener);
        ci_store.addListener(ci_listener);
        fr_store.addListener(fr_listener);
        gr_store.addListener(gr_listener);
    }

    void user_listener() {
        if(userstore.storeStatus == storeStatus.WAITING_REQ) {
            onLoading();
        }
    }

    void bi_listener() {
        if (bi_store.storeStatus == storeStatus.WAITING_REQ) {
            onLoading();
        }
    }

    void ci_listener() {
        if (ci_store.storeStatus == storeStatus.WAITING_REQ) {
            onLoading();
        }
    }

    void fr_listener() {
        if (fr_store.storeStatus == storeStatus.WAITING_REQ) {
            onLoading();
        }
    }

    void gr_listener() {
        if (gr_store.storeStatus == storeStatus.WAITING_REQ) {
            onLoading();
        }
    }
}
