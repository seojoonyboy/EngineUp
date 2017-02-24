using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GroupAddViewController : MonoBehaviour {
    public GroupViewController controller;
    public UIPopupList
        provinceMenu,
        cityMenu;
    public UIInput nameInput;

    GameManager gm;

    void Awake() {
        gm = GameManager.Instance;
    }

    void OnEnable() {
        setProvinceList();
    }

    public void setProvinceList() {
        District[] districts = controller.locationStore.districts;
        provinceMenu.fontSize = 40;
        provinceMenu.items = new List<string>();
        for (int i=0; i< districts.Length; i++) {
            provinceMenu.items.Add(districts[i].name);
        }
    }

    public void provinceSelected() {
        GetCityData getCityDataAct = ActionCreator.createAction(ActionTypes.GET_CITY_DATA) as GetCityData;
        int index = provinceMenu.items.IndexOf(provinceMenu.value) + 1;
        getCityDataAct.id = index;
        gm.gameDispatcher.dispatch(getCityDataAct);
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

    //그룹 최종 생성 버튼 클릭시
    public void posting() {
        Group_AddAction addAct = ActionCreator.createAction(ActionTypes.GROUP_ADD) as Group_AddAction;
        addAct.name = nameInput.value;
        addAct.district = provinceMenu.value;
        addAct.city = cityMenu.value;
        //Debug.Log("province val : " + provinceMenu.value);
        gm.gameDispatcher.dispatch(addAct);
    }

    //그룹 생성 실패시 모달
    public void onModal() {

    }

    public void offPanel() {
        nameInput.value = provinceMenu.items[0];
        gameObject.SetActive(false);
    }
}
