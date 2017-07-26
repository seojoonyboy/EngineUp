using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Result_VC : MonoBehaviour {
    public Text
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
    OnlineMapsMarker endMarker;

    SoundManager sm;

    private GameManager gm;
    private Vector3 
        preMapScale,
        preMapPos;

    public GameObject 
        mapViewBtn,
        confirmBtn,
        recordViewBtn;

    public Slider 
        lvSlider,
        friendlySlider;

    public Text
        LvHeader,
        FrHeader,
        LvExp,
        FrExp;

    public CanvasGroup canvas;

    private UserData preData;
    private Exp[] exps;

    private static int 
        maxLv = 99,
        maxExp = 99000,
        maxCharLv = 3;

    private TweenPosition tP;
    public Texture2D markerTexture;

    void Awake() {
        gm = GameManager.Instance;
        sm = SoundManager.Instance;
    }

    void Start() {
        TextAsset file = (TextAsset)Resources.Load("Exp");
        exps = JsonHelper.getJsonArray<Exp>(file.text);

        tP = GetComponent<TweenPosition>();
    }
    void offPanel() {
        totalDist.text = "0";
        avgSpeed.text = "0";
        maxSpeed.text = "0";
        uphillDistanceLabel.text = "0";
        totalTime.text = null;

        mapViewBtn.SetActive(true);
        recordViewBtn.SetActive(false);

        tP.ResetToBeginning();

        gameObject.SetActive(false);
    }

    public void tPFinished() {
        offPanel();
    }

    void zooming() {
        if(line == null) {
            return;
        }
        int level = OnlineMaps.instance.zoom;
        //Debug.Log("zoom Level : " + level);
        if(level > 10) {
            line.weight = 1.0f;
            endMarker.scale = 0.3f;
        }
        else {
            line.weight = 3.0f;
            endMarker.scale = 0.5f;
        }
    }

    public void onRidingListener() {
        //서버로 부터 callback으로 받은 필터적용된 위도 경도 값을 이용하여 라인을 그린다.
        if (ridingStore.eventType == ActionTypes.RIDING_END) {
            gameObject.SetActive(true);
            setResult(ridingStore.totalDist, ridingStore.totalTime, ridingStore.avgSpeed, ridingStore.maxSpeed, ridingStore.uphillDistance, ridingStore.boxes);

            GetRidingRecords act = ActionCreator.createAction(ActionTypes.RIDING_DETAILS) as GetRidingRecords;
            act.id = ridingStore.ridingId;
            act.type = GetRidingRecords.callType.RIDING_RESULT;
            gm.gameDispatcher.dispatch(act);

            MyInfo infoRefresh = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
            infoRefresh._type = MyInfo.type.RIDING_END;
            gm.gameDispatcher.dispatch(infoRefresh);
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

        twSlider.GetComponent<Slider>().value = (float)data.status.exp / lvSliderOffset;
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

        twSlider.GetComponent<Slider>().value = (float)currCharExp / friendlyOffset; 

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
        preEndurance.text = _preEndur.ToString();

        var stat = data.status;
        if(_preStr == stat.strength) {
            incStrength.text = "";
        }
        else {
            incStrength.text = "+ " + (stat.strength - _preStr);
            Debug.Log("근력 증가");
        }

        if(_preReco == stat.regeneration) {
            incRecovery.text = "";
        }
        else {
            incRecovery.text = "+ " + (stat.regeneration - _preReco);
            Debug.Log("회복력 증가");
        }

        if(_preSpeed == stat.speed) {
            incSpeed.text = "";
        }
        else {
            incSpeed.text = "+ " + (stat.speed - _preSpeed);
            Debug.Log("스피드 증가");
        }

        if(_preEndur == stat.endurance) {
            incEndurance.text = "";
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

    public void tweenPos() {
        tP.PlayForward();
    }

    public void offResultPanel() {
        offPanel();
    }

    void _drawLine() {
        OnlineMaps.instance.zoom = 18;
        
        RidingDetails details = ridingStore.ridingDetails;
        if(details != null) {
            innerRidingDetails[] coords = details.coords;

            List<Vector2> list = new List<Vector2>();

            //좌표들의 평균값으로 맵 시작위치 지정
            float sumLat = 0;
            float sumLon = 0;

            foreach (innerRidingDetails coord in coords) {
                float lat = coord.latitude;
                float lon = coord.longitude;
                sumLat += lat;
                sumLon += lon;
                Vector2 val = new Vector2(lat, lon);
                list.Add(val);
            }

            if(coords.Length == 0) {
                OnlineMaps.instance.position = new Vector2(127.74437f, 37.87998f);
            }

            else {
                float avgLat = sumLat / (float)coords.Length;
                float avgLon = sumLon / (float)coords.Length;
                Vector2 mapPos = new Vector2(avgLat, avgLon);
                OnlineMaps.instance.position = mapPos;
            }

            line = new OnlineMapsDrawingLine(list, Color.red, 2.0f);
            OnlineMaps.instance.AddDrawingElement(line);
        }
    }

    void _drawMarker() {
        //OnlineMaps.instance.AddMarker()
        RidingDetails details = ridingStore.ridingDetails;
        Vector2 tmp = new Vector2(0, 0);
        if (details == null) {
            return;
        }

        innerRidingDetails[] coords = details.coords;

        if (coords.Length > 0) {
            //도착점 마커
            float lat = coords[coords.Length - 1].latitude;
            float lon = coords[coords.Length - 1].longitude;
            Vector2 pos = new Vector2(lat, lon);

            endMarker = OnlineMaps.instance.AddMarker(pos, markerTexture, "");
            endMarker.scale = 0.5f;

            //시작점 마커
            lat = coords[0].latitude;
            lon = coords[0].longitude;
            pos = new Vector2(lat, lon);
            OnlineMaps.instance.AddMarker(pos, markerTexture, "");
        }
    }

    public void onMapPanel() {
        mapPanel.SetActive(true);
        map.SetActive(true);

        preMapScale = map.transform.localScale;
        preMapPos = map.transform.localPosition;

        map.transform.localScale = new Vector3(1.57f, 1f, 1.57f);
        map.transform.localPosition = new Vector3(0, 0, 1724);

        OnlineMapsControlBase.instance.OnMapZoom += zooming;
        
        canvas.blocksRaycasts = false;

        _drawLine();
        _drawMarker();
    }

    public void offMapPanel() {
        mapPanel.SetActive(false);

        map.SetActive(false);
        map.transform.localScale = preMapScale;

        canvas.blocksRaycasts = true;

        if (OnlineMaps.instance != null) {
            OnlineMaps.instance.RemoveAllDrawingElements();
            OnlineMaps.instance.RemoveAllMarkers();
            
        }
    }

    public void OnButtonPressed(GameObject obj) {
        obj.transform.Find("OnImg").gameObject.SetActive(true);
        obj.transform.Find("OffImg").gameObject.SetActive(false);
        obj.transform.Find("OnLabel").gameObject.SetActive(true);
        obj.transform.Find("OffLabel").gameObject.SetActive(false);
    }

    public void OnButtonReleased(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;

        switch (index) {
            case 0:
                onMapPanel();

                recordViewBtn.SetActive(true);
                mapViewBtn.SetActive(false);
                break;
            case 1:
                offMapPanel();
                tweenPos();
                break;
            case 2:
                offMapPanel();

                recordViewBtn.SetActive(false);
                mapViewBtn.SetActive(true);
                break;
        }

        obj.transform.Find("OnImg").gameObject.SetActive(false);
        obj.transform.Find("OffImg").gameObject.SetActive(true);
        obj.transform.Find("OnLabel").gameObject.SetActive(false);
        obj.transform.Find("OffLabel").gameObject.SetActive(true);
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
