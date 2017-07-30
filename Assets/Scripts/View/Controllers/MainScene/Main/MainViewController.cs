using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainViewController : MonoBehaviour {
    public GameObject map;
    public SpritesManager spriteManager;

    //메인 화면 상의 캐릭터
    public Image charSprite;

    private GameManager gm;
    private User userStore;
    private BicycleItem_Inventory bi;
    private Char_Inventory ci;

    private bool 
        isCharLoded = false,
        isBicycleLoaded = false;

    public GameObject
        tutorial,
        tutorialChar,                           //튜토리얼에 등장하는 캐릭터 이미지
        bicycleSprite,                          //메인 화면 상의 자전거 객체(하위 : 프레임, 엔진, 바퀴)
        loadingModal;                           //로딩 화면

    public Sprite[] 
        ranks,
        partner_busts_m,
        Bicycles_items_slot,
        Bicycles_items_stage;

    public SpritesArr[] 
        characters_slots,
        characters_entire_body;

    void Awake() {
        gm = GameManager.Instance;
        gm.userStore.addListener(onUserListener);
        gm.bicycleInventStore.addListener(onBicycleInvenListener);
        gm.charInvenStore.addListener(onCharInvenListener);

        userStore = gm.userStore;
        bi = gm.bicycleInventStore;
        ci = gm.charInvenStore;

        //Application.targetFrameRate = 60;
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
        //int isFirstPlay = PlayerPrefs.GetInt("isFirstPlay");
        //if (isFirstPlay != 1) {
        //    tutorial.SetActive(true);
        //    tutorialChar.SetActive(true);
        //}
    }

    public void onUserListener() {
        
    }

    public void onBicycleInvenListener() {
        if(bi.eventType == ActionTypes.ITEM_INIT) {
            if (bi.storeStatus == storeStatus.NORMAL) {
                Image sprite = bicycleSprite.transform.Find("Wheel").GetComponent<Image>();
                if (bi.equipedItemIndex[0] != null) {
                    if(spriteManager.stage_items[bi.equipedItemIndex[0].item.id - 1] != null) {
                        sprite.sprite = spriteManager.stage_items[bi.equipedItemIndex[0].item.id - 1];
                    }
                    else {
                        sprite.sprite = spriteManager.stage_items[53];
                    }
                }
                else {
                    sprite.sprite = spriteManager.stage_items[53];
                }

                sprite = bicycleSprite.transform.Find("Frame").GetComponent<Image>();
                if (bi.equipedItemIndex[1] != null) {
                    if(spriteManager.stage_items[bi.equipedItemIndex[1].item.id - 1] != null) {
                        sprite.sprite = spriteManager.stage_items[bi.equipedItemIndex[1].item.id - 1];
                    }

                    else {
                        sprite.sprite = spriteManager.stage_items[0];
                    }
                }
                else {
                    sprite.sprite = spriteManager.stage_items[0];
                }

                sprite = bicycleSprite.transform.Find("Engine").GetComponent<Image>();
                if (bi.equipedItemIndex[2] != null) {
                    if (spriteManager.stage_items[bi.equipedItemIndex[2].item.id - 1] != null) {
                        sprite.sprite = spriteManager.stage_items[bi.equipedItemIndex[2].item.id - 1];
                    }

                    else {
                        sprite.sprite = spriteManager.stage_items[85];
                    }
                }
                else {
                    sprite.sprite = spriteManager.stage_items[85];
                }

                isBicycleLoaded = true;
                isAllLoaded();
            }
        }
    }

    public void onCharInvenListener() {
        if(ci.eventType == ActionTypes.ITEM_INIT) {
            if(ci.storeStatus == storeStatus.NORMAL) {
                charSprite.sprite = characters_entire_body[ci.repCharacter.character - 1].images[ci.repCharacter.lv - 1];

                isCharLoded = true;
                isAllLoaded();
            }
        }
    }

    private void isAllLoaded() {
        if(isCharLoded && isBicycleLoaded) {
            loadingModal.SetActive(false);
        }
    }
}
