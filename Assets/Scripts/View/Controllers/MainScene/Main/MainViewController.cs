using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainViewController : MonoBehaviour {
    public UISprite charSprite;
    private GameManager gm;
    private User userStore;
    public UIAtlas[] atlasArr;

    void Awake() {
        gm = GameManager.Instance;
        gm.userStore.addListener(onUserListener);
        userStore = gm.userStore;
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
}
