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
    public ScrollSnapRect scrollSnapRect;

    SoundManager sm;

    public GameObject
        mainStage,
        lv1Slot,
        lv2Slot,
        lv3Slot,
        itemPref,
        selectedChar,
        scroll_pagePref,
        scroll_pageIconPref,
        specs;

    public ScrollSnapRect sR;

    public GameObject 
        equipButton,
        nonepossessionButton,
        itemGrid,
        sideBarGrid,
        pageIconGrid,
        descModal;

    public Text[] stats;

    public Text 
        lvLabel,
        charName;

    public Slider friendlySlider;

    public int
        per_str,
        per_speed,
        per_endurance,
        per_recovery;

    public GameObject changeSpecViewButton;

    public string rep_name;
    public int rep_id;

    void Awake() {
        gm = GameManager.Instance;
        sm = SoundManager.Instance;

        charInvenStore = gm.charInvenStore;
    }

    void OnEnable() {
        setInfo(true);
        setStat();
        setMainChar(charInvenStore.repCharacter.imageId, charInvenStore.repCharacter.lv);
    }

    void OnDisable() {
        selectedChar = null;
        nonepossessionButton.SetActive(false);
        scrollSnapRect.enabled = false;
    }

    //친밀도 Slider
    public void OnSliderChanged(GameObject slider) {
        Slider _slider = slider.GetComponent<Slider>();
        slider.transform.Find("Percentage").GetComponent<Text>().text = (_slider.value / _slider.maxValue).ToString() + "%";
    }

    public void onCharInvenStore() {
        if (gameObject.activeSelf) {
            ActionTypes charStoreEventType = charInvenStore.eventType;

            if (charStoreEventType == ActionTypes.ITEM_INIT) {
                if (charInvenStore.storeStatus == storeStatus.NORMAL) {
                    //makeList();

                    sR.init();
                    setInfo(true);
                    //character_inventory charInfo = charInvenStore.repCharacter;
                    //setMainChar(charInfo.character, charInfo.lv);
                    //setEquipButton(charInfo.character, charInfo.has_character);
                    //setSideBar();
                    setStat();
                }
            }
        }
    }

    public void onUserListener() {
        ActionTypes userStoreEventType = userStore.eventType;

    }

    private void setInfo(bool isRepChar = false) {
        Slider slider = mainStage.transform.Find("FR_Slider").GetComponent<Slider>();
        
        Text name = mainStage.transform.Find("Name").GetComponent<Text>();
        Text frLv = slider.transform.Find("Header").GetComponent<Text>();
        Text frPercentage = slider.transform.Find("Percentage").GetComponent<Text>();

        int lv = 0;
        if (isRepChar) {
            name.text = charInvenStore.repCharacter.name;
            lv = charInvenStore.repCharacter.lv;
            slider.maxValue = charInvenStore.repCharacter.lvup_exps[lv - 1];
            slider.value = charInvenStore.repCharacter.exp;

            //Debug.Log("대표 캐릭터 정보 갱신");
        }
        else {
            
        }
        frLv.text = "친밀도 Lv" + lv;
        frPercentage.text = ((slider.value / slider.maxValue) * 100f).ToString() + "%";
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
    }

    public void setStat() {
        initStat();

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

        for (int i = 0; i < 4; i++) {
            stats[i].transform.parent.GetComponent<Text>().enabled = true;
        }
        changeSpecViewButton.GetComponent<boolIndex>().isOn = false;
    }

    public void changeSpecViewType() {
        bool isOn = changeSpecViewButton.GetComponent<boolIndex>().isOn;
        var mySpec = userStore.myData.status;

        if (isOn) {
            setStat();

            for (int i = 0; i < 4; i++) {
                stats[i].transform.parent.GetComponent<Text>().enabled = true;
            }
        }
        else {
            stats[0].text = ((int)(per_str * mySpec.strength / 100)).ToString();
            stats[1].text = ((int)(per_endurance * mySpec.endurance / 100)).ToString();
            stats[2].text = ((int)(per_speed * mySpec.speed / 100)).ToString();
            stats[3].text = ((int)(per_recovery * mySpec.regeneration / 100)).ToString();

            for (int i = 0; i < 4; i++) {
                stats[i].transform.parent.GetComponent<Text>().enabled = false;
            }
        }

        changeSpecViewButton.GetComponent<boolIndex>().isOn = !isOn;
    }

    private void initStat() {
        
    }
}
