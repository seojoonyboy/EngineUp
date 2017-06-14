using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Result_VC : MonoBehaviour {
    public UILabel
        totalDist,
        totalTime,
        avgSpeed,
        maxSpeed,
        uphillDistanceLabel,
        boxNum,
        preStrength,
        incStrength,
        preEndurance,
        incEndurance,
        preSpeed,
        incSpeed,
        preRecovery,
        incRecovery;

    public Riding ridingStore;
    public User userStore;

    public GameObject
        map,
        mapPanel,
        resultPanel;

    OnlineMapsDrawingLine line;

    public Collider[] colliders;

    private GameManager gm;
    private Vector3 preMapScale;

    public GameObject 
        mapViewBtn,
        confirmBtn,
        recordViewBtn;

    public UISlider 
        lvSlider,
        friendlySlider;

    public UILabel
        LvHeader,
        FrHeader,
        LvExp,
        FrExp;

    private UserData preData;
    private Exp[] exps;

    private static int 
        maxLv = 99,
        maxExp = 99000,
        maxCharLv = 3;

    public GameObject
        StrIcon,
        EndIcon,
        SpeedIcon,
        RecoIcon;

    void Awake() {
        gm = GameManager.Instance;
        UIEventListener.Get(mapViewBtn).onPress += new UIEventListener.BoolDelegate(btnListener);
        UIEventListener.Get(confirmBtn).onPress += new UIEventListener.BoolDelegate(btnListener);
        UIEventListener.Get(recordViewBtn).onPress += new UIEventListener.BoolDelegate(btnListener);
    }

    void Start() {
        TextAsset file = (TextAsset)Resources.Load("Exp");
        exps = JsonHelper.getJsonArray<Exp>(file.text);
    }

    void btnListener(GameObject obj, bool state) {
        //Debug.Log(state);
        int index = obj.GetComponent<ButtonIndex>().index;
        if(state) {
            obj.transform.Find("ActiveImg").gameObject.SetActive(true);
            obj.transform.Find("DeactiveImg").gameObject.SetActive(false);
            obj.transform.Find("ActiveLabel").gameObject.SetActive(true);
            obj.transform.Find("DeactiveLabel").gameObject.SetActive(false);
        }
        else {
            obj.transform.Find("ActiveImg").gameObject.SetActive(false);
            obj.transform.Find("DeactiveImg").gameObject.SetActive(true);
            obj.transform.Find("ActiveLabel").gameObject.SetActive(false);
            obj.transform.Find("DeactiveLabel").gameObject.SetActive(true);

            switch (index) {
                //지도화면 보기 버튼
                case 0:
                    onMapPanel();
                    mapViewBtn.SetActive(false);
                    recordViewBtn.SetActive(true);
                    break;
                //확인 버튼
                case 1:
                    OnDisable();
                    break;
                //결과화면 보기 버튼
                case 2:
                    offMapPanel();
                    mapViewBtn.SetActive(true);
                    recordViewBtn.SetActive(false);
                    break;
            }
        }
    }

    void zooming() {
        if(line == null) {
            return;
        }
        int level = OnlineMaps.instance.zoom;
        //Debug.Log("zoom Level : " + level);
        if(level > 10) {
            line.weight = 1.0f;
        }
        if(level < 5) {
            line.weight = 3.0f;
        }
    }

    void OnDisable() {
        totalDist.text = "0";
        avgSpeed.text = "0";
        maxSpeed.text = "0";
        uphillDistanceLabel.text = "0";
        totalTime.text = null;
        gameObject.SetActive(false);

        foreach (Collider coll in colliders) {
            coll.enabled = true;
        }

        mapViewBtn.SetActive(true);
        recordViewBtn.SetActive(false);

        StrIcon.SetActive(true);
        EndIcon.SetActive(true);
        SpeedIcon.SetActive(true);
        RecoIcon.SetActive(true);
    }

    public void onRidingListener() {
        //서버로 부터 callback으로 받은 필터적용된 위도 경도 값을 이용하여 라인을 그린다.
        if (ridingStore.eventType == ActionTypes.RIDING_END) {
            gameObject.SetActive(true);
            setResult(ridingStore.totalDist, ridingStore.totalTime, ridingStore.avgSpeed, ridingStore.maxSpeed, ridingStore.uphillDistance, ridingStore.boxes);

            MyInfo infoRefresh = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
            infoRefresh._type = MyInfo.type.RIDING_END;
            gm.gameDispatcher.dispatch(infoRefresh);
        }

        if(gameObject.activeSelf) {
            if (ridingStore.eventType == ActionTypes.RIDING_DETAILS) {
                if (ridingStore.storeStatus == storeStatus.NORMAL) {
                    _drawLine();
                    _drawMarker();
                }
            }
        }
    }

    public void onUserListener() {
        if(userStore.eventType == ActionTypes.MYINFO) {
            if(userStore.storeStatus == storeStatus.NORMAL) {
                if(userStore.MyInfoType == MyInfo.type.RIDING_START) {
                    preData = userStore.myData;
                }
                else if(userStore.MyInfoType == MyInfo.type.RIDING_END) {
                    difference(userStore.myData);
                }
            }
        }
    }

    void difference(UserData data) {
        //레벨 증가량 계산
        int preLv = preData.status.rank;
        int currLv = data.status.rank;
        float lvSliderOffset = exps[currLv].exp;
        TweenSlider twSlider = lvSlider.GetComponent<TweenSlider>();

        twSlider.GetComponent<UISlider>().value = (float)data.status.exp / lvSliderOffset;
        //레벨 업
        if (preLv != currLv) {
            twSlider.from = 0;
        }
        else {
            twSlider.from = preData.status.exp / lvSliderOffset;
        }
        twSlider.to = data.status.exp / lvSliderOffset;
        twSlider.ResetToBeginning();

        //친밀도 증가량 계산
        twSlider = friendlySlider.GetComponent<TweenSlider>();
        int currCharLv = data.represent_character.character_inventory.lv;
        int currCharExp = data.represent_character.character_inventory.exp;
        int[] maxExps = { 200, 500 };

        int preCharLv = preData.represent_character.character_inventory.lv;
        int preExp = preData.represent_character.character_inventory.exp;

        float friendlyOffset = 0;
        if (currCharLv == 1) {
            friendlyOffset = maxExps[0];
        }
        else if (currCharLv == 2) {
            friendlyOffset = maxExps[1];
        }

        twSlider.GetComponent<UISlider>().value = (float)currCharExp / friendlyOffset; 

        //레벨업
        if (preCharLv != currCharLv) {
            twSlider.from = 0;
        }
        else {
            twSlider.from = preExp / friendlyOffset;
        }
        twSlider.to = currCharExp / friendlyOffset;
        twSlider.ResetToBeginning();

        //능력치 증가량
        //근력, 지구력, 스피드, 회복력
        int _preStr = preData.status.strength;
        int _preEndur = preData.status.endurance;
        int _preSpeed = preData.status.speed;
        int _preReco = preData.status.regeneration;

        preStrength.text = _preStr.ToString();
        preRecovery.text = _preReco.ToString();
        preSpeed.text = _preSpeed.ToString();
        preRecovery.text = _preReco.ToString();

        var stat = data.status;
        if(_preStr == stat.strength) {
            incStrength.text = "";
            StrIcon.SetActive(false);
        }
        else {
            incStrength.text = "+ " + (stat.strength - _preStr);
            Debug.Log("근력 증가");
        }

        if(_preReco == stat.regeneration) {
            incRecovery.text = "";
            RecoIcon.SetActive(false);
        }
        else {
            incRecovery.text = "+ " + (stat.regeneration - _preReco);
            Debug.Log("회복력 증가");
        }

        if(_preSpeed == stat.speed) {
            incSpeed.text = "";
            SpeedIcon.SetActive(false);
        }
        else {
            incSpeed.text = "+ " + (stat.speed - _preSpeed);
            Debug.Log("스피드 증가");
        }

        if(_preEndur == stat.endurance) {
            incEndurance.text = "";
            EndIcon.SetActive(false);
        }
        else {
            incEndurance.text = "+ " + (stat.endurance - _preEndur);
            Debug.Log("지구력 증가");
        }

        //레벨 및 경험치 라벨 설정
        LvHeader.text = "레벨 " + currLv;
        FrHeader.text = "친밀도 " + currCharLv;
        if(currLv == maxLv && data.status.exp == maxExp) {
            LvExp.text = "MAX";
        }
        else {
            LvExp.text = data.status.exp + " / " + lvSliderOffset;
        }

        if(currLv == maxCharLv && currCharExp == maxExps[1]) {
            FrExp.text = "MAX";
        }
        else {
            FrExp.text = currCharExp + " / " + friendlyOffset;
        }
    }

    public void setResult(float mDist, string mTime, float mAvgSpeed, float mMaxSpeed, float uphillDist, int boxNum) {
        totalDist.text = (Math.Round(mDist, 2, MidpointRounding.AwayFromZero)).ToString();

        //char delimeter = '.';
        if(mTime != null) {
            totalTime.text = mTime.ToString();
        }

        avgSpeed.text = (Math.Round(mAvgSpeed, 2, MidpointRounding.AwayFromZero)).ToString();
        maxSpeed.text = (Math.Round(mMaxSpeed, 2, MidpointRounding.AwayFromZero)).ToString();
        uphillDistanceLabel.text = (Math.Round(uphillDist, 2, MidpointRounding.AwayFromZero)).ToString();

        this.boxNum.text = boxNum.ToString();
    }

    public void offResultPanel() {
        OnDisable();


    }

    void _drawLine() {
        OnlineMaps.instance.zoom = 18;
        
        RidingDetails details = ridingStore.ridingDetails;
        innerRidingDetails[] coords = details.coords;

        if(coords.Length > 0) {
            float endLat = coords[coords.Length - 1].latitude;
            float endLon = coords[coords.Length - 1].longitude;
            Vector2 endPos = new Vector2(endLat, endLon);
            OnlineMaps.instance.position = endPos;
        }
        else {
            OnlineMaps.instance.position = new Vector2(127.74437f, 37.87998f);
        }

        List<Vector2> list = new List<Vector2>();
        foreach(innerRidingDetails coord in coords) {
            float lat = coord.latitude;
            float lon = coord.longitude;
            Vector2 val = new Vector2(lat, lon);
            list.Add(val);
        }
        line = new OnlineMapsDrawingLine(list, Color.red, 2.0f);
        OnlineMaps.instance.AddDrawingElement(line);
    }

    void _drawMarker() {
        //OnlineMaps.instance.AddMarker()
        RidingDetails details = ridingStore.ridingDetails;
        innerRidingDetails[] coords = details.coords;

        if (coords.Length > 0) {
            //도착점 마커
            float lat = coords[coords.Length - 1].latitude;
            float lon = coords[coords.Length - 1].longitude;
            Vector2 pos = new Vector2(lat, lon);
            OnlineMaps.instance.AddMarker(pos);

            //시작점 마커
            lat = coords[0].latitude;
            lon = coords[0].longitude;
            pos = new Vector2(lat, lon);
            OnlineMaps.instance.AddMarker(pos);
        }
    }

    public void onMapPanel() {
        map.SetActive(true);
        mapPanel.SetActive(true);

        preMapScale = map.transform.localScale;
        map.transform.localScale = new Vector3(2.025f, 1.0f, 2.025f);
        map.transform.localPosition = new Vector3(-1953f, 380f, -1266f);
        OnlineMapsControlBase.instance.OnMapZoom += zooming;

        GetRidingRecords act = ActionCreator.createAction(ActionTypes.RIDING_DETAILS) as GetRidingRecords;
        act.id = ridingStore.ridingId;
        gm.gameDispatcher.dispatch(act);
    }

    public void offMapPanel() {
        map.SetActive(false);
        mapPanel.SetActive(false);

        map.transform.localScale = preMapScale;
    }
}

[System.Serializable]
class Exp {
    public int lv;
    public int exp;

    public static Exp fromJSON(string json) {
        return JsonUtility.FromJson<Exp>(json);
    }
}
