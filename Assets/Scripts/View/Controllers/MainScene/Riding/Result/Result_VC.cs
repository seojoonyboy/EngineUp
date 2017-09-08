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
        boxNum;

    public Riding ridingStore;
    public User userStore;

    public GameObject
        map,
        mapPanel,
        resultPanel;

    OnlineMapsDrawingLine line;
    OnlineMapsMarker 
        endMarker,
        startMarker;

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
        friendlySlider,
        boxSlider;

    public Text
        LvHeader,
        FrHeader,
        LvExp,
        FrExp;

    public Text[] specs;

    public CanvasGroup canvas;

    private UserData preData;
    private Exp[]
        exps,
        AccExps;

    private static int 
        maxLv = 99,
        maxExp = 99000,
        maxCharLv = 3;

    private TweenPosition tP;
    public Texture2D[] markerTextures;
    private bool tmp = false;

    void Awake() {
        gm = GameManager.Instance;
        sm = SoundManager.Instance;
    }

    void Start() {
        TextAsset file = (TextAsset)Resources.Load("Exp");
        exps = JsonHelper.getJsonArray<Exp>(file.text);

        tP = GetComponent<TweenPosition>();

        file = (TextAsset)Resources.Load("AccExp");
        AccExps = JsonHelper.getJsonArray<Exp>(file.text);
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
            line.weight = 3f;
        }
        else {
            line.weight = 5f;
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
        TweenSlider tS = lvSlider.GetComponent<TweenSlider>();
        tS.ResetToBeginning();
        //레벨 증가량 계산
        int preLv = preData.status.rank;
        int currLv = data.status.rank;

        //누적경험치가 아닌 현재 레벨업 구간에서의 경험치
        int preRefineExp = 0;
        int refineExp = 0;
        //레벨업에 필요한 경험치(기준)
        int standardExp = 0;
        //레벨업이 일어나지 않은 경우
        if(currLv > 10) {
            standardExp = AccExps[AccExps.Length - 1].exp + 100 * (currLv - AccExps[AccExps.Length - 1].lv + 1);
            refineExp = data.status.exp - (AccExps[AccExps.Length - 1].exp + 100 * (currLv - AccExps[AccExps.Length - 1].lv));
        }
        else if (currLv == 10) {
            standardExp = exps[exps.Length - 1].exp;
            refineExp = data.status.exp - AccExps[currLv].exp;
        }
        else {
            standardExp = exps[currLv + 1].exp;
            refineExp = data.status.exp - AccExps[currLv].exp;
        }

        lvSlider.maxValue = standardExp;

        if(preLv == currLv) {
            if(currLv > 10) {
                preRefineExp = preData.status.exp - (AccExps[AccExps.Length - 1].exp + 100 * (currLv - AccExps[AccExps.Length - 1].lv));
            }
            else {
                preRefineExp = preData.status.exp - AccExps[currLv].exp;
            }
        }

        //레벨업이 일어난 경우
        else {
            refineExp = data.status.exp - AccExps[currLv].exp;
        }

        tS.to = refineExp;
        tS.from = preRefineExp;
        tS.PlayForward();

        LvHeader.text = "Rank " + currLv;
        LvExp.text = preRefineExp + " + " + (refineExp - preRefineExp);

        tS = friendlySlider.GetComponent<TweenSlider>();
        //친밀도 증가량 계산
        int currCharLv = data.represent_character.character_inventory.lv;
        int currCharExp = data.represent_character.character_inventory.exp;
        int[] maxExps = { 10, 20 };

        int preCharLv = preData.represent_character.character_inventory.lv;
        int preExp = preData.represent_character.character_inventory.exp;

        float friendlyOffset = 0;
        if (currCharLv == 1) {
            friendlyOffset = maxExps[0];
        }
        else if (currCharLv >= 2) {
            friendlyOffset = maxExps[1];
        }

        preRefineExp = 0;
        refineExp = 0;

        //레벨업 발생하지 않음
        if(currCharLv == preCharLv) {
            if(currCharLv == 1) {
                preRefineExp = preExp;
                refineExp = currCharExp;
            }
            else if(currCharLv > 1) {
                preRefineExp = (int)(preExp - maxExps[currCharLv - 2]);
                refineExp = (int)(currCharExp - maxExps[currCharLv - 2]);
            }
        }
        //레벨업 발생
        else {
            preRefineExp = 0;
        }

        friendlySlider.maxValue = friendlyOffset;

        tS.from = preRefineExp;
        tS.to = refineExp;
        tS.PlayForward();

        if(currCharExp == maxExps[1]) {
            FrExp.text = "MAX";
        }
        else {
            FrExp.text = preRefineExp + " + " + (refineExp - preRefineExp);
        }
        FrHeader.text = "친밀도 Lv " + currCharLv;

        if(data.status.strength == 0) {
            specs[0].text = "00";
        }
        else {
            specs[0].text = data.status.strength.ToString();
        }

        if(data.status.endurance == 0) {
            specs[1].text = "00";
        }
        else {
            specs[1].text = data.status.endurance.ToString();
        }

        if(data.status.regeneration == 0) {
            specs[2].text = "00";
        }
        else {
            specs[2].text = data.status.regeneration.ToString();
        }

        if(data.status.speed == 0) {
            specs[3].text = "00";
        }
        else {
            specs[3].text = data.status.speed.ToString();
        }

        //공구함 슬라이더
        int expIncreased = data.status.exp - preData.status.exp;
        
        tS = boxSlider.GetComponent<TweenSlider>();

        int remainderExp = data.status.exp % 10;
        if (expIncreased == 0) {
            boxSlider.value = remainderExp;
        }
        else {
            if(remainderExp == 0) {
                tS.from = preData.status.exp;
                tS.to = boxSlider.maxValue;
                boxSlider.value = 0;
                tmp = true;
            }
            else {
                tS.from = preData.status.exp;
                tS.to = data.status.exp;
            }
        }

        tS.PlayForward();

        Text sliderText = boxSlider.transform.Find("Num").GetComponent<Text>();
        sliderText.text = remainderExp + "/10";
    }

    public void needResetSlider() {
        if(tmp) {
            boxSlider.value = 0;
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
        RidingDetails details = ridingStore.ridingDetails;
        if(details != null) {
            innerRidingDetails[] coords = details.coords;

            List<Vector2> list = new List<Vector2>();
            foreach (innerRidingDetails coord in coords) {
                float lat = coord.latitude;
                float lon = coord.longitude;
                Vector2 val = new Vector2(lat, lon);
                list.Add(val);
            }
            var _map = OnlineMaps.instance;
            //입력받은 좌표가 없는 경우
            if (coords.Length == 0) {
                OnlineMaps.instance.position = new Vector2(127.74437f, 37.87998f);
            }
            //입력받은 좌표가 있는 경우
            else {
                var maxRect = ridingStore.maxCoord;
                var minRect = ridingStore.minCoord;

                if (IsRectCoordValid(minRect, maxRect)) {
                    float centerX = (maxRect.x + minRect.x) / 2.0f;
                    float centerY = (maxRect.y + minRect.y) / 2.0f;

                    _map.position = new Vector2(centerY, centerX);

                    double dx;
                    double dy;
                    OnlineMapsUtils.DistanceBetweenPoints(minRect.x, minRect.y, maxRect.x, maxRect.y, out dx, out dy);
                    //1km 이하인 경우
                    double maxDist = dx;
                    if (dx < dy) {
                        maxDist = dy;
                    }
                    Debug.Log("Dist X : " + dx);
                    int zoomLv = 18;
                    int offset = (int)(maxDist / 0.2);
                    if (offset <= 1) {
                        _map.zoom = zoomLv;
                    }
                    else {
                        zoomLv = zoomLv - (offset - 1);
                        if (zoomLv <= 10) {
                            zoomLv = 10;
                        }
                        else {
                            _map.zoom = zoomLv;
                        }
                    }
                }
                else {
                    Vector2 pos = new Vector2(coords[coords.Length - 1].latitude, coords[coords.Length - 1].longitude);
                    _map.position = pos;
                }
            }

            line = new OnlineMapsDrawingLine(list, Color.red, 3.0f);
            OnlineMaps.instance.AddDrawingElement(line);
        }
    }

    bool IsRectCoordValid(_Rect min, _Rect max) {
        bool result = true;
        if(min.x == 0 || min.y == 0 || max.x == 0 || max.y == 0) {
            result = false;
        }
        return result;
    }

    void _drawMarker() {
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

            endMarker = OnlineMaps.instance.AddMarker(pos, markerTextures[1], "");
            endMarker.align = OnlineMapsAlign.Center;
            endMarker.scale = 0.8f;

            //시작점 마커
            lat = coords[0].latitude;
            lon = coords[0].longitude;
            pos = new Vector2(lat, lon);
            startMarker = OnlineMaps.instance.AddMarker(pos, markerTextures[0], "");
            startMarker.align = OnlineMapsAlign.Center;
            startMarker.scale = 0.8f;
        }
    }

    public void onMapPanel() {
        mapPanel.SetActive(true);
        map.SetActive(true);

        map.transform.Find("Texture").GetComponent<RectTransform>().sizeDelta = new Vector2(1024, 1024);

        map.GetComponent<RectTransform>().offsetMin = new Vector2(0, 146);
        map.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        OnlineMapsControlBase.instance.OnMapZoom += zooming;

        _drawLine();
        _drawMarker();
    }

    public void offMapPanel() {
        map.SetActive(false);
        mapPanel.SetActive(false);

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
