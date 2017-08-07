using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BicycleDetailViewController : MonoBehaviour {
    private GameManager gm;
    private SoundManager sm;
    private BicycleItem_Inventory bicycleItemStore;
    private User userStore;

    private Animator animator;
    private int id = -1;

    public BicycleListViewController parent;

    public SpritesManager spriteManager;

    public GameObject[] specs;

    public GameObject 
        notifyModal,
        equipButton,
        unEquipButton,
        tierImg,
        tierGrid;

    private Color32 increaseColor = new Color32(249, 168, 37, 255);
    private Color32 decreaseColor = new Color32(2, 154, 173, 255);
    private int[] limitRank = new int[4] { 1, 10, 20, 30 };

    void Awake() {
        gm = GameManager.Instance;
        sm = SoundManager.Instance;
        userStore = gm.userStore;
        bicycleItemStore = gm.bicycleInventStore;

        animator = GetComponent<Animator>();
    }

    void OnEnable() {
        Invoke("playSlideIn", 0.2f);
    }

    void OnDisable() {
        init();
    }

    void playSlideIn() {
        animator.Play("SlideIn");
    }

    public void slideFinished(AnimationEvent animationEvent) {
        int boolParm = animationEvent.intParameter;

        //slider in
        if (boolParm == 1) {
            Info info = parent.selectedItem.GetComponent<Info>();

            if(info == null) { return; }

            id = info.id;
            int grade = info.grade;
            for(int i=0; i<grade; i++) {
                GameObject tier = Instantiate(tierImg);
                tier.transform.SetParent(tierGrid.transform, false);
            }

            gameObject.transform.Find("Name").GetComponent<Text>().text = info.name;
            var type = info.parts;

            if(type == "FR") {
                Image img = gameObject.transform.Find("Image_FR").GetComponent<Image>();
                var tmp = spriteManager.stage_items[info.imageId - 1];
                if (tmp != null) {
                    img.enabled = true;
                    img.sprite = tmp;
                }
                else {
                    img.enabled = false;
                }
            }
            else if(type == "WH") {
                Image img = gameObject.transform.Find("Image_WH_mask/Image_WH").GetComponent<Image>();
                var tmp = spriteManager.stage_items[info.imageId - 1];
                if(tmp != null) {
                    img.enabled = true;
                    img.sprite = tmp;
                }
                else {
                    img.enabled = false;
                }

            }
            else if(type == "DS") {
                Image img = gameObject.transform.Find("Image_EG").GetComponent<Image>();
                var tmp = spriteManager.stage_items[info.imageId - 1];
                if (tmp != null) {
                    img.enabled = true;
                    img.sprite = tmp;
                }
                else {
                    img.enabled = false;
                }
            }

            spec diffSpecs = diffSpec(info);

            int diffStr = diffSpecs.diff_strength;
            int diffSpeed = diffSpecs.diff_speed;
            int diffEndurance = diffSpecs.diff_endurance;
            int diffRecovery = diffSpecs.diff_recovery;

            int pre_str = diffSpecs.pre_str;
            int pre_speed = diffSpecs.pre_speed;
            int pre_end = diffSpecs.pre_end;
            int pre_rec = diffSpecs.pre_rec;

            Text incStr = specs[0].transform.Find("Diff").GetComponent<Text>();
            Text incRec = specs[1].transform.Find("Diff").GetComponent<Text>();
            Text incSpeed = specs[2].transform.Find("Diff").GetComponent<Text>();
            Text incEnd = specs[3].transform.Find("Diff").GetComponent<Text>();

            //이전보다 근력 증가
            if (diffStr >= 0) {
                specs[0].transform.Find("Diff/Inc").gameObject.SetActive(true);
                incStr.color = increaseColor;
            }
            else {
                specs[0].transform.Find("Diff/Dec").gameObject.SetActive(true);
                incStr.color = decreaseColor;
            }

            if(diffRecovery >= 0) {
                specs[1].transform.Find("Diff/Inc").gameObject.SetActive(true);
                incRec.color = increaseColor;
            }
            else {
                specs[1].transform.Find("Diff/Dec").gameObject.SetActive(true);
                incRec.color = decreaseColor;
            }

            if (diffSpeed >= 0) {
                specs[2].transform.Find("Diff/Inc").gameObject.SetActive(true);
                incSpeed.color = increaseColor;
            }
            else {
                specs[2].transform.Find("Diff/Dec").gameObject.SetActive(true);
                incSpeed.color = decreaseColor;
            }

            if (diffEndurance >= 0) {
                specs[3].transform.Find("Diff/Inc").gameObject.SetActive(true);
                incEnd.color = increaseColor;
            }
            else {
                specs[3].transform.Find("Diff/Dec").gameObject.SetActive(true);
                incEnd.color = decreaseColor;
            }

            incStr.transform.Find("Val").GetComponent<Text>().text = pre_str.ToString();
            incRec.transform.Find("Val").GetComponent<Text>().text = pre_rec.ToString();
            incSpeed.transform.Find("Val").GetComponent<Text>().text = pre_speed.ToString();
            incEnd.transform.Find("Val").GetComponent<Text>().text = pre_end.ToString();

            specs[0].transform.Find("Diff").GetComponent<Text>().text = System.Math.Abs(diffStr).ToString();
            specs[1].transform.Find("Diff").GetComponent<Text>().text = System.Math.Abs(diffRecovery).ToString();
            specs[2].transform.Find("Diff").GetComponent<Text>().text = System.Math.Abs(diffSpeed).ToString();
            specs[3].transform.Find("Diff").GetComponent<Text>().text = System.Math.Abs(diffEndurance).ToString();

            if(info.is_equiped) {
                unEquipButton.SetActive(true);
            }
            else {
                equipButton.SetActive(true);
            }
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
    }

    public void onBackButton() {
        animator.Play("SlideOut");
    }

    public void OnEquipButton() {
        Info info = parent.selectedItem.GetComponent<Info>();
        int itemGrade = info.grade;
        int myGrade = userStore.myData.status.rank;
        if(canEquip(itemGrade, myGrade)) {
            equip_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_EQUIP) as equip_act;
            act._type = equip_act.type.ITEM;
            act.id = info.id;
            gm.gameDispatcher.dispatch(act);

            close();
        }
        else {
            notifyModal.SetActive(true);
            notifyModal.transform.Find("InnerModal/Text").GetComponent<Text>().text = "등급이 낮아 장착할 수 없습니다.";
        }
    }

    private bool canEquip(int grade, int userRank) {
        bool result = false;
        if(limitRank[grade - 1] <= userRank) {
            result = true;
        }
        else {
            result = false;
        }
        return result;
    }

    public void OnSellButton() {
        Info info = parent.selectedItem.GetComponent<Info>();
        garage_sell_act act = ActionCreator.createAction(ActionTypes.GARAGE_SELL) as garage_sell_act;
        act.id = info.id;
        gm.gameDispatcher.dispatch(act);

        notifyModal.SetActive(true);
        notifyModal.transform.Find("InnerModal/Text").GetComponent<Text>().text = "총 " + info.gear  + " 기어를 획득하였습니다.";
    }

    public void OnUnequipButton() {
        unequip_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_UNEQUIP) as unequip_act;
        Info info = parent.selectedItem.GetComponent<Info>();
        int index = info.id;
        act.id = index;
        gm.gameDispatcher.dispatch(act);

        close();
    }

    public void close() {
        onBackButton();
        parent.onBackButton();
    }

    public void offNotifyModal() {
        notifyModal.SetActive(false);
        close();
    }

    private void init() {
        gameObject.transform.Find("Image_EG").GetComponent<Image>().enabled = false;
        gameObject.transform.Find("Image_FR").GetComponent<Image>().enabled = false;
        gameObject.transform.Find("Image_WH_mask/Image_WH").GetComponent<Image>().enabled = false;

        foreach(GameObject obj in specs) {
            obj.transform.Find("Diff/Inc").gameObject.SetActive(false);
            obj.transform.Find("Diff/Dec").gameObject.SetActive(false);
        }

        unEquipButton.SetActive(false);
        equipButton.SetActive(false);
        notifyModal.SetActive(false);

        foreach(Transform child in tierGrid.transform) {
            Destroy(child.gameObject);
        }
    }

    private spec diffSpec(Info info) {
        spec specs = new spec();
        int selected_spec_str = info.strength;
        int selected_spec_end = info.endurance;
        int selected_spec_speed = info.speed;
        int selected_spec_rec = info.recovery;

        int pre_str = 0;
        int pre_end = 0;
        int pre_speed = 0;
        int pre_rec = 0;

        RespGetItems equipedItem;
        if (info.parts == "WH") {
            equipedItem = bicycleItemStore.equipedItemIndex[0];
            if (equipedItem != null) {
                pre_str = equipedItem.item.strength;
                pre_rec = equipedItem.item.regeneration;
                pre_speed = equipedItem.item.speed;
                pre_end = equipedItem.item.endurance;
            }
        }

        else if(info.parts == "FR") {
            equipedItem = bicycleItemStore.equipedItemIndex[1];
            if (equipedItem != null) {
                pre_str = equipedItem.item.strength;
                pre_rec = equipedItem.item.regeneration;
                pre_speed = equipedItem.item.speed;
                pre_end = equipedItem.item.endurance;
            }
        }

        else if(info.parts == "DS") {
            equipedItem = bicycleItemStore.equipedItemIndex[2];
            if (equipedItem != null) {
                pre_str = equipedItem.item.strength;
                pre_rec = equipedItem.item.regeneration;
                pre_speed = equipedItem.item.speed;
                pre_end = equipedItem.item.endurance;
            }
        }
        //Debug.Log("이전 장착 Spec");
        //Debug.Log(pre_str);
        //Debug.Log(pre_end);
        //Debug.Log(pre_rec);
        //Debug.Log(pre_speed);

        //Debug.Log("선택한 아이템 Spec");
        //Debug.Log(selected_spec_str);
        //Debug.Log(selected_spec_end);
        //Debug.Log(selected_spec_rec);
        //Debug.Log(selected_spec_speed);

        specs.diff_strength = selected_spec_str - pre_str;
        specs.diff_endurance = selected_spec_end - pre_end;
        specs.diff_recovery = selected_spec_rec - pre_rec;
        specs.diff_speed = selected_spec_speed - pre_speed;

        specs.pre_str = pre_str;
        specs.pre_speed = pre_speed;
        specs.pre_rec = pre_rec;
        specs.pre_end = pre_end;

        return specs;
    }

    private class spec {
        public int diff_strength = 0;
        public int diff_speed = 0;
        public int diff_endurance = 0;
        public int diff_recovery = 0;

        public int pre_str = 0;
        public int pre_end = 0;
        public int pre_speed = 0;
        public int pre_rec = 0;
    }
}
