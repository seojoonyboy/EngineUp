using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GroupSettingChangeView : MonoBehaviour {
    public GroupViewController controller;
    private Animator animator;

    public Dropdown
        largeArea_dropMenu,
        detailArea_dropMenu;

    public InputField descInput;

    GameManager gm;

    public GameObject
        deActivePanel,
        descModifyButton,
        modal;

    private Groups groupStore;
    private Group group;
    
    void Awake() {
        animator = GetComponent<Animator>();
        gm = GameManager.Instance;
        groupStore = gm.groupStore;
    }

    void OnEnable() {
        Invoke("playSlideIn", 0.2f);
    }

    void playSlideIn() {
        animator.Play("SlideIn");
    }

    public void onBackButton() {
        animator.Play("SlideOut");
    }

    public void slideFinished(AnimationEvent animationEvent) {
        int boolParm = animationEvent.intParameter;

        //slider in
        if (boolParm == 1) {
            setProvinceList();
            descChange();
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
    }

    public void setProvinceList() {
        if (!gameObject.activeSelf) {
            return;
        }

        if(controller.locationStore.districts == null || controller.locationStore.districts.Length == 0) {
            GetDistrictsData distAct = ActionCreator.createAction(ActionTypes.GET_DISTRICT_DATA) as GetDistrictsData;
            gm.gameDispatcher.dispatch(distAct);
        }
        else {
            District[] districts = controller.locationStore.districts;
            largeArea_dropMenu.options.Clear();

            foreach (District data in districts) {
                largeArea_dropMenu.options.Add(new Dropdown.OptionData() { text = data.name });
            }

            for(int i=0; i<districts.Length; i++) {
                if(districts[i].name == groupStore.clickedGroup.locationDistrict) {
                    largeArea_dropMenu.value = i;
                    largeArea_dropMenu.transform.Find("Label").GetComponent<Text>().text = largeArea_dropMenu.options[i].text;
                }
            }
        }

        if(controller.locationStore.cities == null) {
            int index = largeArea_dropMenu.value;

            GetCityData getCityDataAct = ActionCreator.createAction(ActionTypes.GET_CITY_DATA) as GetCityData;
            getCityDataAct.id = index;
            gm.gameDispatcher.dispatch(getCityDataAct);
        }
        else {
            setCityList();
        }
    }

    public void setCityList() {
        if (!gameObject.activeSelf) {
            return;
        }

        Borough[] cities = controller.locationStore.borough;
        detailArea_dropMenu.options.Clear();
        foreach (Borough data in cities) {
            detailArea_dropMenu.options.Add(new Dropdown.OptionData() { text = data.name });
        }

        detailArea_dropMenu.value = 0;
        detailArea_dropMenu.transform.Find("Label").GetComponent<Text>().text = detailArea_dropMenu.options[0].text;

        for (int i=0; i<cities.Length; i++) {
            if(cities[i].name == groupStore.clickedGroup.locationCity) {
                detailArea_dropMenu.value = i;
                detailArea_dropMenu.transform.Find("Label").GetComponent<Text>().text = detailArea_dropMenu.options[i].text;
            }
        }
    }

    public void provinceSelected() {
        if (gameObject.activeSelf) {
            int index = largeArea_dropMenu.value;
            GetCityData getCityDataAct = ActionCreator.createAction(ActionTypes.GET_CITY_DATA) as GetCityData;
            getCityDataAct.id = index;
            gm.gameDispatcher.dispatch(getCityDataAct);
        }
    }

    public void descChange() {
        string desc = groupStore.clickedGroup.groupIntro;
        Debug.Log("소개글 : " + desc);
        if (!string.IsNullOrEmpty(desc)) {
            descInput.text = desc;
        }
    }

    //그룹 최종 수정 버튼 클릭시
    public void posting() {
        Group_AddAction editAct = ActionCreator.createAction(ActionTypes.GROUP_EDIT) as Group_AddAction;
        editAct.id = controller.detailView.id;
        editAct.desc = descInput.text;
        editAct.district = largeArea_dropMenu.options[largeArea_dropMenu.value].text;
        editAct.city = detailArea_dropMenu.options[detailArea_dropMenu.value].text;
        editAct.name = controller.detailView.groupName.text;

        gm.gameDispatcher.dispatch(editAct);
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }

    //소개글 수정 버튼 클릭
    public void onDescModifyPanel() {
        deActivePanel.SetActive(false);
        descModifyButton.SetActive(false);
    }
}
