using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterViewControlller : MonoBehaviour {
    GameManager gm;
    Char_Inventory charInvenStore;
    public User userStore;
    public MainViewController mV_controller;

    SoundManager sm;

    public GameObject mainStage;

    public Slider friendlySlider;
    public CharacterListViewController childPanel;

    public GameObject changeSpecViewButton;
    public MainViewController mV;

    public string rep_name;
    public int rep_id;
    public Text[] stats;
    public int
        per_str,
        per_speed,
        per_endurance,
        per_recovery;
    public Text specHeader;
    void Awake() {
        gm = GameManager.Instance;
        sm = SoundManager.Instance;

        charInvenStore = gm.charInvenStore;
    }

    void OnEnable() {
        setInfo(true);
        Character_inventory charInfo = charInvenStore.repCharacter;
        setMainChar(charInfo.imageId, charInfo.lv);
        setStat();
    }

    //친밀도 Slider
    public void OnSliderChanged(Slider slider) {
        slider.transform.Find("Percentage").GetComponent<Text>().text = (slider.value / slider.maxValue).ToString() + "%";
    }

    public void onCharInvenStore() {
        if (gameObject.activeSelf) {
            ActionTypes charStoreEventType = charInvenStore.eventType;

            if (charStoreEventType == ActionTypes.ITEM_INIT) {
                if (charInvenStore.storeStatus == storeStatus.NORMAL) {
                    //makeList();
                    setInfo(true);
                    Character_inventory charInfo = charInvenStore.repCharacter;
                    setMainChar(charInfo.imageId, charInfo.lv);
                    //setEquipButton(charInfo.character, charInfo.has_character);
                    //setSideBar();
                    setStat();
                }
            }
            if (charStoreEventType == ActionTypes.ITEM_INIT) {
                if (charInvenStore.storeStatus == storeStatus.NORMAL) {
                    childPanel.makeList();
                }
            }

            if(charStoreEventType == ActionTypes.GARAGE_ITEM_EQUIP) {
                if(charInvenStore.storeStatus == storeStatus.WAITING_REQ) {
                    mV.loadingModal.SetActive(true);
                }
            }
        }
    }

    public void onUserListener() {
        ActionTypes userStoreEventType = userStore.eventType;

    }

    private void setInfo(bool isRepChar = false) {
        int[] maxExps = { 0, 10, 20 };

        Text name = mainStage.transform.Find("Name").GetComponent<Text>();
        Text frLv = friendlySlider.transform.Find("Header").GetComponent<Text>();
        Text frPercentage = friendlySlider.transform.Find("Percentage").GetComponent<Text>();

        name.text = charInvenStore.repCharacter.name;
        int lv = charInvenStore.repCharacter.lv;
        friendlySlider.maxValue = charInvenStore.repCharacter.lvup_exps[lv - 1];
        friendlySlider.value = charInvenStore.repCharacter.exp - maxExps[lv - 1];

        frLv.text = "친밀도 Lv" + lv;
        frPercentage.text = ((friendlySlider.value / friendlySlider.maxValue) * 100f).ToString() + "%";
    }

    public void setMainChar(int index, int lv) {
        int arrIndex = index - 1;
        int arrSubIndex = lv - 1;
        if (arrSubIndex == -1) {
            arrSubIndex = 0;
        }

        var character = mainStage.transform.Find("Image").gameObject;
        var charImg = character.GetComponent<Image>();
        charImg.sprite = mV_controller.characters_entire_body[arrIndex].images[arrSubIndex];

        mV.loadingModal.SetActive(false);
    }

    public void setStat() {
        //아이템 장착, 파트너 장착에 따른 Spec 변화
        var itemSpects = userStore.itemSpects;

        //파트너 장착 효과
        int char_str = itemSpects.Char_strength;
        int char_end = itemSpects.Char_endurance;
        int char_reg = itemSpects.Char_regeneration;
        int char_speed = itemSpects.Char_speed;

        //아이템 장착 효과
        int item_str = itemSpects.Item_strength;
        int item_end = itemSpects.Item_endurance;
        int item_speed = itemSpects.Item_speed;
        int item_reg = itemSpects.Item_regeneration;

        //파트너 + 아이템 장착효과
        per_endurance = char_end + item_end;
        per_speed = char_speed + item_speed;
        per_str = char_str + item_str;
        per_recovery = char_reg + item_reg;

        stats[0].text = (per_str).ToString();
        stats[1].text = (per_endurance).ToString();
        stats[2].text = (per_speed).ToString();
        stats[3].text = (per_recovery).ToString();

        specHeader.text = "파트너 능력치 %";

        changeSpecViewButton.GetComponent<boolIndex>().isOn = false;
    }

    public void changeSpecViewType() {
        bool isOn = changeSpecViewButton.GetComponent<boolIndex>().isOn;
        var mySpec = userStore.myData.status;

        if (isOn) {
            setStat();
            specHeader.text = "파트너 능력치 %";
        }
        else {
            stats[0].text = ((int)(per_str * mySpec.strength / 100)).ToString();
            stats[1].text = ((int)(per_endurance * mySpec.endurance / 100)).ToString();
            stats[2].text = ((int)(per_speed * mySpec.speed / 100)).ToString();
            stats[3].text = ((int)(per_recovery * mySpec.regeneration / 100)).ToString();

            specHeader.text = "파트너 능력치";
        }

        changeSpecViewButton.GetComponent<boolIndex>().isOn = !isOn;
    }
}