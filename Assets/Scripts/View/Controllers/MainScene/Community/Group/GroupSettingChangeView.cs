using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupSettingChangeView : MonoBehaviour {
    public GroupViewController controller;
    public UIPopupList
        provinceMenu,
        cityMenu;

    public UIInput descInput;

    GameManager gm;

    private bool 
        isEditDesc,
        isFirstSetDistrict;

    public GameObject
        deActivePanel,
        descModifyButton;

    private Groups groupStore;
    private Group group;
    void Awake() {
        gm = GameManager.Instance;
    }

    void OnEnable() {
        isFirstSetDistrict = true;
        groupStore = controller.groupStore;
        group = groupStore.clickedGroup;
        descInput.value = group.groupIntro;
        setProvinceList();
    }

    void OnDisable() {
        deActivePanel.SetActive(true);
        descModifyButton.SetActive(true);
        isEditDesc = false;
    }

    public void setProvinceList() {
        District[] districts = controller.locationStore.districts;
        provinceMenu.fontSize = 40;
        provinceMenu.items = new List<string>();
        for (int i = 0; i < districts.Length; i++) {
            provinceMenu.items.Add(districts[i].name);
        }
    }

    public void provinceSelected() {
        //Debug.Log("!!");
        if (isFirstSetDistrict) {
            provinceMenu.value = group.locationDistrict;
            cityMenu.value = group.locationCity;
            //Debug.Log("첫 할당 : " + controller.groupStore.clickedGroup.locationCity);
            isFirstSetDistrict = false;
        }
        else {
            //Debug.Log("새로운 할당");
            GetCityData getCityDataAct = ActionCreator.createAction(ActionTypes.GET_CITY_DATA) as GetCityData;
            int index = provinceMenu.items.IndexOf(provinceMenu.value) + 1;
            //Debug.Log("Index : " + index);
            getCityDataAct.id = index;
            gm.gameDispatcher.dispatch(getCityDataAct);
        }
    }

    public void setCityList() {
        Borough[] cities = controller.locationStore.borough;
        cityMenu.fontSize = 40;
        cityMenu.items = new List<string>();
        for (int i = 0; i < cities.Length; i++) {
            cityMenu.items.Add(cities[i].name);
        }
        cityMenu.value = cityMenu.items[0];
    }

    //그룹 최종 수정 버튼 클릭시
    public void posting() {
        Group_AddAction addAct = ActionCreator.createAction(ActionTypes.GROUP_ADD) as Group_AddAction;
        addAct.district = provinceMenu.value;
        addAct.city = cityMenu.value;
        if (isEditDesc) {
            
        }
        //Debug.Log("province val : " + provinceMenu.value);
        gm.gameDispatcher.dispatch(addAct);
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }

    //소개글 수정 버튼 클릭
    public void onDescModifyPanel() {
        deActivePanel.SetActive(false);
        descModifyButton.SetActive(false);
        isEditDesc = true;
    }
}
