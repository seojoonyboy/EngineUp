using UnityEngine;
using UnityEngine.UI;

public class StatViewController : MonoBehaviour {
    public UILabel 
        nickNameLabel,
        mainLvLabel,
        mainTitleLabel,
        mainGearLabel;

    public Riding ridingStore;
    public User userStore;
    public Locations locationStore;

    public Text[] 
        stats,
        myInfoes,
        totalRidings,
        monthlyRidings,
        profiles;

    private GameManager gm;
    
    public UISlider mainSlider;
    public int mainSliderOffset = 100;

    public GameObject 
        districtPref,
        bicyclePref,
        callenderPref,
        inputObj;

    public GameObject 
        distGrid,
        bicycleTypeGrid;

    public GameObject[] editModals;

    public Transform canvas;
    bool isSelWHNow = false;

    private TweenPosition tP;
    private bool isReverse_tp;

    public bool InputFieldActive = false;
    public RectTransform childRectTransform;

    void Awake() {
        gm = GameManager.Instance;
        tP = GetComponent<TweenPosition>();
    }

    void OnEnable() {
        Debug.Log("!!");
        tweenPos();
        isReverse_tp = false;
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(isSelWHNow) {
                onSubmit(inputObj);
                Debug.Log("모바일 뒤로가기 버튼 클릭");
            }
        }
    }

    public void tweenPos() {
        if (!isReverse_tp) {
            tP.PlayForward();
        }
        else {
            //swap
            Vector3 tmp;
            tmp = tP.to;
            tP.to = tP.from;
            tP.from = tmp;

            tP.ResetToBeginning();
            tP.PlayForward();
        }
    }

    public void tPFinished() {
        if (isReverse_tp) {
            offPanel();
            gameObject.transform.Find("TopPanel").gameObject.SetActive(false);
        }
        else {
            gameObject.transform.Find("TopPanel").gameObject.SetActive(true);

            MyInfo act = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
            gm.gameDispatcher.dispatch(act);
        }

        isReverse_tp = true;
    }

    void OnDisable() {
        tP.ResetToBeginning();
    }

    public void onUserListener() {
        mainTitleLabel.text = userStore.userTitle;
        nickNameLabel.text = userStore.nickName;

        if (userStore.eventType == ActionTypes.MYINFO) {
            if(userStore.storeStatus == storeStatus.NORMAL) {
                initialize();
                mainLvLabel.text = "Lv " + userStore.myData.status.rank.ToString();
                mainGearLabel.text = userStore.myData.gears.ToString();
                int exp = userStore.myData.status.exp;
                //레벨업 환산 후 남은 경험치
                float extraExp = (float)(exp % mainSliderOffset);
                //슬라이더가 실제적으로 입력되는 값
                //slider 최댓값 100 기준
                float sliderVal = extraExp / mainSliderOffset;
                mainSlider.value = sliderVal;
                mainSlider.transform.Find("Val").GetComponent<UILabel>().text = extraExp + " / " + mainSliderOffset + " Km";
            }
        }

        if(userStore.eventType == ActionTypes.USER_BICYCLETYPES) {
            if(userStore.storeStatus == storeStatus.NORMAL) {
                makeBicycleTypeList();
            }
        }
    }

    public void onLocationStore() {
        if(locationStore.eventType == ActionTypes.GET_DISTRICT_DATA) {
            if(locationStore.storeStatus == storeStatus.NORMAL) {
                makeDistrictsList();
            }
        }
    }

    private void initialize() {
        setMyInfo();
        setProfile();
        setTotalRidingRecord();
        setMonthlyRidingRecord();
    }

    private void makeBicycleTypeList() {
        foreach (Transform child in bicycleTypeGrid.transform) {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < userStore.userBicycleTypes.Length; i++) {
            GameObject item = Instantiate(bicyclePref);

            item.transform.SetParent(bicycleTypeGrid.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;

            item.transform.Find("Name").GetComponent<Text>().text = userStore.userBicycleTypes[i].name;
            item.GetComponent<InputIndex>().type = "bicycle";

            item.GetComponent<Button>().onClick.AddListener(() => onSubmit(item));
        }
    }

    private void makeDistrictsList() {
        foreach(Transform child in distGrid.transform) {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < locationStore.districts.Length; i++) {
            GameObject item = Instantiate(districtPref);
            item.transform.SetParent(distGrid.transform);
            //item.transform.SetParent(districtGrid.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;

            item.transform.Find("Name").GetComponent<Text>().text = locationStore.districts[i].name;
            item.GetComponent<InputIndex>().type = "district";

            item.GetComponent<Button>().onClick.AddListener(() => onSubmit(item));
        }
    }

    private void setMyInfo() {
        UserData data = userStore.myData;
        myInfoes[0].text = data.nickName;

        status statData = data.status;
        myInfoes[1].text = statData.rank.ToString();
        myInfoes[2].text = userStore.userTitle;

        //그룹
        stats[0].text = statData.strength.ToString();
        stats[1].text = statData.speed.ToString();
        stats[2].text = statData.endurance.ToString();
        stats[3].text = statData.regeneration.ToString();
    }

    private void setProfile() {
        UserData data = userStore.myData;
        if(!string.IsNullOrEmpty(data.country) || !string.IsNullOrEmpty(data.district)) {
            profiles[0].text = data.country + " / " + data.district;
        }
        if(!string.IsNullOrEmpty(data.bicycle)) {
            profiles[1].text = data.bicycle;
        }
        if(!string.IsNullOrEmpty(data.birthday)) {
            profiles[2].text = data.birthday;
        }
        if(!string.IsNullOrEmpty(data.weight) || !string.IsNullOrEmpty(data.height)) {
            profiles[3].text = data.weight + " Kg / " + data.height + " Cm";
        }
        if(!string.IsNullOrEmpty(data.gender)) {
            if(data.gender == "W") {
                profiles[4].text = "여";
            }
            else if(data.gender == "M") {
                profiles[4].text = "남";
            }
        }
    }



    private void setTotalRidingRecord() {
        UserData data = userStore.myData;
        record rec = data.record;

        totalRidings[0].text = rec.count + " 회";
        totalRidings[1].text = rec.riding_time.Split('.')[0];
        totalRidings[2].text = rec.total_distance.ToString();
        totalRidings[3].text = rec.max_speed.ToString();
        totalRidings[4].text = rec.avg_speed.ToString();
    }

    private void setMonthlyRidingRecord() {
        UserData data = userStore.myData;
        record rec = data.record_this_month;

        monthlyRidings[0].text = rec.count + " 회";
        monthlyRidings[1].text = rec.riding_time.Split('.')[0];
        monthlyRidings[2].text = rec.total_distance.ToString();
        monthlyRidings[3].text = rec.max_speed.ToString();
        monthlyRidings[4].text = rec.avg_speed.ToString();
    }

    public void WHInput(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        
        editModals[7].SetActive(true);
        InputField inputField = inputObj.GetComponent<InputField>();
        inputField.ActivateInputField();
        inputField.Select();
        Text unit = inputObj.transform.Find("Unit").GetComponent<Text>();
        switch (index) {
            //몸무게 입력
            case 0:
                unit.text = "Kg";
                inputObj.GetComponent<InputIndex>().type = "weight";
                inputField.text = userStore.myData.weight;
                break;
            //키 입력
            case 1:
                unit.text = "Cm";
                inputObj.GetComponent<InputIndex>().type = "height";
                inputField.text = userStore.myData.height;
                break;
        }
    }

    //최종 수정(선택)완료시
    public void onSubmit(GameObject obj) {
        
        string type = obj.GetComponent<InputIndex>().type;
        Debug.Log(type);
        EditProfileAction profileEditAct = ActionCreator.createAction(ActionTypes.EDIT_PROFILE) as EditProfileAction;
        object value = null;
        switch (type) {
            case "country":
                profileEditAct.type = EditProfileAction.profileType.COUNTRY;
                break;
            case "district":
                profileEditAct.type = EditProfileAction.profileType.DISTRICT;

                value = obj.transform.Find("Name").GetComponent<Text>().text;
                editModals[5].SetActive(false);
                editModals[6].SetActive(false);
                Debug.Log(value);
                break;
            case "Birthday":
                profileEditAct.type = EditProfileAction.profileType.BIRTHDAY;
                break;
            case "weight":
                obj.transform.parent.gameObject.SetActive(false);
                inputObj.transform.parent.gameObject.SetActive(false);
                value = inputObj.GetComponent<InputField>().text;
                editModals[3].transform.Find("Weight").GetComponent<Text>().text = (string)value;
                profileEditAct.type = EditProfileAction.profileType.WEIGHT;
                break;
            case "height":
                obj.transform.parent.gameObject.SetActive(false);
                inputObj.transform.parent.gameObject.SetActive(false);
                value = inputObj.GetComponent<InputField>().text;
                editModals[3].transform.Find("Height").GetComponent<Text>().text = (string)value;
                profileEditAct.type = EditProfileAction.profileType.HEIGHT;
                break;
            case "woman":
                profileEditAct.type = EditProfileAction.profileType.GENDER;
                value = obj.GetComponent<InputIndex>().type;
                break;
            case "man":
                profileEditAct.type = EditProfileAction.profileType.GENDER;
                value = obj.GetComponent<InputIndex>().type;
                break;
            case "bicycle":
                editModals[1].SetActive(false);
                editModals[6].SetActive(false);
                profileEditAct.type = EditProfileAction.profileType.BICYCLE;
                value = obj.transform.Find("Name").GetComponent<Text>().text;
                Debug.Log(value);
                break;
        }
        profileEditAct.value = value;
        gm.gameDispatcher.dispatch(profileEditAct);
    }

    //지역, 자전거종류, 생년월일, 몸무게 / 키, 성별 입력(수정)
    public void onProfileEdit(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        if(index != 2) {
            editModals[index].transform.parent.gameObject.SetActive(true);
            editModals[index].SetActive(true);
        }
        //지역 상세 선택
        switch(index) {
            case 0:
                editModals[index].transform.Find("District").GetComponent<Text>().text = userStore.myData.district;
                break;
            //자전거 종류
            case 1:
                GetBicycleTypes bicycleType = ActionCreator.createAction(ActionTypes.USER_BICYCLETYPES) as GetBicycleTypes;
                gm.gameDispatcher.dispatch(bicycleType);
                break;
            //생년월일
            case 2:
                editModals[6].SetActive(true);
                GameObject calender = Instantiate(callenderPref);
                calender.transform.SetParent(editModals[6].transform);
                calender.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 340f);
                break;
            //WH
            case 3:
                editModals[index].transform.Find("Height").GetComponent<Text>().text = userStore.myData.height;
                editModals[index].transform.Find("Weight").GetComponent<Text>().text = userStore.myData.weight;
                break;
            case 5:
                editModals[0].SetActive(false);
                GetDistrictsData data = ActionCreator.createAction(ActionTypes.GET_DISTRICT_DATA) as GetDistrictsData;
                gm.gameDispatcher.dispatch(data);
                break;
        }
    }

    public void offProfileEdit(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        editModals[index].transform.parent.gameObject.SetActive(false);
        editModals[index].SetActive(false);
    }

    public void offInput(GameObject obj) {
        obj.SetActive(true);
        isSelWHNow = false;
    }

    public void calenderSelEnd(string result) {
        string[] date = result.Split(' ')[0].Split('/');
        Debug.Log(date[0]);
        Debug.Log(date[1]);
        Debug.Log(date[2]);
        editModals[6].SetActive(false);

        string value = date[2] + "-" + date[0] + "-" + date[1];
        EditProfileAction profileEditAct = ActionCreator.createAction(ActionTypes.EDIT_PROFILE) as EditProfileAction;
        profileEditAct.type = EditProfileAction.profileType.BIRTHDAY;
        profileEditAct.value = value;
        gm.gameDispatcher.dispatch(profileEditAct);
    }

    [System.Serializable]
    class bicyleType {
        public string id;
        public string name;

        public static bicyleType fromJSON(string json) {
            return JsonUtility.FromJson<bicyleType>(json);
        }
    }

    [System.Serializable]
    class district {
        public string id;
        public string name;

        public static district fromJSON(string json) {
            return JsonUtility.FromJson<district>(json);
        }
    }
}