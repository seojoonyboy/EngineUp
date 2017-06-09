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
        boxNum;

    public Riding ridingStore;
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

    void Awake() {
        gm = GameManager.Instance;

        UIEventListener.Get(mapViewBtn).onPress += new UIEventListener.BoolDelegate(btnListener);
        UIEventListener.Get(confirmBtn).onPress += new UIEventListener.BoolDelegate(btnListener);
        UIEventListener.Get(recordViewBtn).onPress += new UIEventListener.BoolDelegate(btnListener);
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
    }

    public void onRidingListener() {
        //서버로 부터 callback으로 받은 필터적용된 위도 경도 값을 이용하여 라인을 그린다.
        if (ridingStore.eventType == ActionTypes.RIDING_END) {
            gameObject.SetActive(true);
            setResult(ridingStore.totalDist, ridingStore.totalTime, ridingStore.avgSpeed, ridingStore.maxSpeed, ridingStore.uphillDistance, ridingStore.boxes);

            MyInfo infoRefresh = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
            gm.gameDispatcher.dispatch(infoRefresh);
        }

        if(gameObject.activeSelf) {
            if (ridingStore.eventType == ActionTypes.RIDING_DETAILS) {
                if (ridingStore.storeStatus == storeStatus.NORMAL) {
                    _drawLine();
                }
            }
        }
    }

    public void setResult(float mDist, string mTime, float mAvgSpeed, float mMaxSpeed, float uphillDist, int boxNum) {
        totalDist.text = (Math.Round(mDist, 2, MidpointRounding.AwayFromZero)).ToString();

        //char delimeter = '.';
        totalTime.text = mTime.ToString();

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

        if(coords != null) {
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
