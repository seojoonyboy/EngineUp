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

    public void onUserStoreListener() { }

    public void onCharStoreListener() { }

    public void offPanel() {
        gameObject.SetActive(false);
        UIToggle toggle = UIToggle.GetActiveToggle(2);
        toggle.value = false;
    }
}
