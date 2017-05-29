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
        uphillDistanceLabel;

    public Text boxText;

    public Riding ridingStore;
    public GameObject 
        map,
        mapHeader;
    OnlineMapsDrawingLine line;

    public Collider[] colliders;

    private GameManager gm;
    private Vector3 preMapScale;
    void Awake() {
        gm = GameManager.Instance;
    }

    void OnEnable() {
        map.SetActive(true);
        preMapScale = map.transform.localScale;
        map.transform.localScale = new Vector3(1.13f, 1.0f, 1.13f);
        mapHeader.SetActive(true);
        OnlineMapsControlBase.instance.OnMapZoom += zooming;

        _drawLine();

        foreach(Collider coll in colliders) {
            coll.enabled = false;
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
        map.GetComponent<OnlineMaps>().RemoveAllDrawingElements();
        map.SetActive(false);
        map.transform.localScale = preMapScale;
        ridingStore.filteredCoordsLists.Clear();

        totalDist.text = "0";
        avgSpeed.text = "0";
        maxSpeed.text = "0";
        uphillDistanceLabel.text = "0";
        totalTime.text = null;
        gameObject.SetActive(false);
        mapHeader.SetActive(false);

        foreach (Collider coll in colliders) {
            coll.enabled = true;
        }

    }

    public void onRidingListener() {
        //서버로 부터 callback으로 받은 필터적용된 위도 경도 값을 이용하여 라인을 그린다.
        //라인 누적
        if(ridingStore.eventType == ActionTypes.GPS_SEND) {
            if(ridingStore.storeStatus == storeStatus.NORMAL) {
                Debug.Log("리스트에 좌표 배열을 담습니다.");
            }
        }
        if (ridingStore.eventType == ActionTypes.RIDING_END) {
            gameObject.SetActive(true);
            setResult(ridingStore.totalDist, ridingStore.totalTime, ridingStore.avgSpeed, ridingStore.maxSpeed, ridingStore.uphillDistance, ridingStore.boxes);

            MyInfo infoRefresh = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
            gm.gameDispatcher.dispatch(infoRefresh);
        }
    }

    public void setResult(float mDist, string mTime, float mAvgSpeed, float mMaxSpeed, float uphillDist, int boxNum) {
        totalDist.text = (Math.Round(mDist, 2, MidpointRounding.AwayFromZero)).ToString();

        //char delimeter = '.';
        totalTime.text = mTime.ToString();

        avgSpeed.text = (Math.Round(mAvgSpeed, 2, MidpointRounding.AwayFromZero)).ToString();
        maxSpeed.text = (Math.Round(mMaxSpeed, 2, MidpointRounding.AwayFromZero)).ToString();
        uphillDistanceLabel.text = (Math.Round(uphillDist, 2, MidpointRounding.AwayFromZero)).ToString();

        boxText.text = "상자 " + boxNum + "개를 획득하셨습니다.";
    }

    public void offResultPanel() {
        OnDisable();
    }

    void _drawLine() {
        foreach (filteredCoords[] data in ridingStore.filteredCoordsLists) {
            List<Vector2> list = new List<Vector2>();
            for (int i = 0; i < data.Length; i++) {
                Debug.Log("그리기위한 Filter된 CoordLists " + i + "번째 위도 : " + data[i].latitude + ", 경도 : " + data[i].longitude);
                float lat = data[i].latitude;
                float lon = data[i].longitude;
                Vector2 val = new Vector2(lat, lon);
                list.Add(val);
            }
            line = new OnlineMapsDrawingLine(list, Color.red, 2.0f);
            OnlineMaps.instance.AddDrawingElement(line);
        }
        List<Vector2> tmp = new List<Vector2> {
            new Vector2(3, 3),
            new Vector2(5, 3)
        };
        OnlineMapsDrawingLine _tmp = new OnlineMapsDrawingLine(tmp, Color.gray, 3.0f);
        OnlineMaps.instance.AddDrawingElement(_tmp);

        //지도 위치 수정
        if (ridingStore.filteredCoordsLists.Count != 0) {
            filteredCoords[] lastData = (filteredCoords[])ridingStore.filteredCoordsLists[ridingStore.filteredCoordsLists.Count - 1];
            Debug.Log(lastData.Length);
            if (lastData.Length != 0) {
                float lastLat = lastData[0].latitude;
                float lastLon = lastData[0].longitude;
                Vector2 lastVal = new Vector2(lastLat, lastLon);
                OnlineMaps.instance.position = lastVal;
                OnlineMaps.instance.zoom = 18;
            }
        }
        else {
            OnlineMaps.instance.position = new Vector2(127.74437f, 37.87998f);
            OnlineMaps.instance.zoom = 18;
        }
    }
}
