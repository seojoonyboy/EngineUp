using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GroupAddView : MonoBehaviour {
    public GroupViewController controller;
    private Animator animator;

    public InputField nameInput;

    GameManager gm;

    public GameObject modal;

    public Dropdown
        largeArea_dropMenu,
        detailArea_dropMenu;

    void Awake() {
        animator = GetComponent<Animator>();
        gm = GameManager.Instance;
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
            GetDistrictsData distAct = ActionCreator.createAction(ActionTypes.GET_DISTRICT_DATA) as GetDistrictsData;
            gm.gameDispatcher.dispatch(distAct);
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
    }

    public void setProvinceList() {
        //District[] districts = controller.locationStore.districts;
        //provinceMenu.fontSize = 40;
        //provinceMenu.items = new List<string>();
        //for (int i=0; i< districts.Length; i++) {
        //    provinceMenu.items.Add(districts[i].name);
        //}
        //provinceMenu.value = provinceMenu.items[0];
    }

    public void provinceSelected() {
        if (gameObject.activeSelf) {
            //if (!string.IsNullOrEmpty(provinceMenu.value)) {
            //    GetCityData getCityDataAct = ActionCreator.createAction(ActionTypes.GET_CITY_DATA) as GetCityData;
            //    int index = provinceMenu.items.IndexOf(provinceMenu.value) + 1;
            //    getCityDataAct.id = index;
            //    gm.gameDispatcher.dispatch(getCityDataAct);
            //}
            int index = largeArea_dropMenu.value;
            Debug.Log("Drop Menu Index : " + index);
            GetCityData getCityDataAct = ActionCreator.createAction(ActionTypes.GET_CITY_DATA) as GetCityData;
            getCityDataAct.id = index;
            gm.gameDispatcher.dispatch(getCityDataAct);
        }
    }

    public void setCityList() {
        Borough[] cities = controller.locationStore.borough;
        detailArea_dropMenu.options.Clear();
        foreach (Borough data in cities) {
            detailArea_dropMenu.options.Add(new Dropdown.OptionData() { text = data.name });
        }
        detailArea_dropMenu.value = 0;
        detailArea_dropMenu.transform.Find("Label").GetComponent<Text>().text = detailArea_dropMenu.options[0].text;
    }

    //그룹 최종 생성 버튼 클릭시
    public void posting() {
        Group_AddAction addAct = ActionCreator.createAction(ActionTypes.GROUP_ADD) as Group_AddAction;

        addAct.name = nameInput.text;
        addAct.district = largeArea_dropMenu.options[largeArea_dropMenu.value].text;
        addAct.city = detailArea_dropMenu.options[detailArea_dropMenu.value].text;

        gm.gameDispatcher.dispatch(addAct);
    }

    public void offPanel() {
        //nameInput.value = provinceMenu.items[0];
        gameObject.SetActive(false);
    }

    public void onGroupStoreListener() {
        Groups groupStore = controller.groupStore;
        ActionTypes type = groupStore.eventType;
        if (type == ActionTypes.GET_DISTRICT_DATA) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                setProvinceList();
            }
        }

        if(type == ActionTypes.GET_CITY_DATA) {
            setCityList();
        }

        if (type == ActionTypes.GROUP_ADD) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                gameObject.SetActive(false);

                Group_myGroups getMyGroupAct = ActionCreator.createAction(ActionTypes.MY_GROUP_PANEL) as Group_myGroups;
                gm.gameDispatcher.dispatch(getMyGroupAct);
            }
            else if(groupStore.storeStatus == storeStatus.ERROR) {

            }
            modal.SetActive(true);
            modal.transform.Find("Modal/Label").GetComponent<UILabel>().text = groupStore.message;
        }
    }

    public void init() {
        if(gameObject.activeSelf) {
            District[] districts = controller.locationStore.districts;
            largeArea_dropMenu.options.Clear();

            foreach (District data in districts) {
                largeArea_dropMenu.options.Add(new Dropdown.OptionData() {text = data.name});
            }
            largeArea_dropMenu.value = 0;
            largeArea_dropMenu.transform.Find("Label").GetComponent<Text>().text = largeArea_dropMenu.options[0].text;
        }
    }


}
