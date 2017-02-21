using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GroupAddViewController : MonoBehaviour {
    public GroupViewController controller;
    public UIPopupList
        provinceMenu,
        cityMenu;
    GameManager gm = GameManager.Instance;

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
    }

    //그룹 생성 버튼 클릭시 그룹을 생성할 수 있는 조건인지 검사
    public void checkCanAdd() {
        //조건에 만족하면
        GetDistrictsData getDistDataAct = ActionCreator.createAction(ActionTypes.GET_DISTRICT_DATA) as GetDistrictsData;
        gm.gameDispatcher.dispatch(getDistDataAct);
    }
}
