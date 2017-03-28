using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Result_VC : MonoBehaviour {

    public UILabel
        totalDist,
        totalTime,
        avgSpeed,
        maxSpeed,
        uphillDistanceLabel;
    public Riding ridingStore;
    public GameObject map;

    void OnEnable() {
        StartCoroutine("drawLine");
    }

    void OnDisable() {
        map.GetComponent<OnlineMaps>().RemoveAllDrawingElements();
        //임시로 직접 접근 (수정 필요)
        Debug.Log("filteredCoordsLists 비우기");
        ridingStore.filteredCoordsLists.Clear();

        totalDist.text = "0";
        avgSpeed.text = "0";
        maxSpeed.text = "0";
        uphillDistanceLabel.text = "0";
        totalTime.text = null;
        gameObject.SetActive(false);
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
            setResult(ridingStore.totalDist, ridingStore.totalTime, ridingStore.avgSpeed, ridingStore.maxSpeed, ridingStore.uphillDistance);
        }
    }

    public void setResult(float mDist, string mTime, float mAvgSpeed, float mMaxSpeed, float uphillDist) {
        totalDist.text = (Math.Round(mDist, 2, MidpointRounding.AwayFromZero)).ToString();

        //char delimeter = '.';
        totalTime.text = mTime.ToString();

        avgSpeed.text = (Math.Round(mAvgSpeed, 2, MidpointRounding.AwayFromZero)).ToString();
        maxSpeed.text = (Math.Round(mMaxSpeed, 2, MidpointRounding.AwayFromZero)).ToString();
        uphillDistanceLabel.text = (Math.Round(uphillDist, 2, MidpointRounding.AwayFromZero)).ToString();
    }

    public void offResultPanel() {
        OnDisable();
    }

    IEnumerator drawLine() {
        //딜레이를 주지 않으면 Map을 찾지 못하는 현상이 일어남.....ㅜㅜ
        yield return new WaitForSeconds(1.0f);
        //Vector2 tmp = new Vector2(127.7446f, 37.8704f);
        //OnlineMaps.instance.AddMarker(tmp);
        //Debug.Log("Count : " + ridingStore.filteredCoordsLists.Count);
        foreach (filteredCoords[] data in ridingStore.filteredCoordsLists) {
            List<Vector2> list = new List<Vector2>();
            for (int i = 0; i < data.Length; i++) {
                Debug.Log("그리기위한 Filter된 CoordLists " + i + "번째 위도 : " + data[i].latitude + ", 경도 : " + data[i].longitude);
                float lat = data[i].latitude;
                float lon = data[i].longitude;
                Vector2 val = new Vector2(lat, lon);
                //OnlineMaps.instance.AddMarker(val);
                list.Add(val);
                //Debug.Log("X : " + val.x + ", Y : " + val.y);
                }
            OnlineMaps.instance.AddDrawingElement(new OnlineMapsDrawingLine(list, Color.red, 3.0f));
        }
        //지도 위치 수정
        if(ridingStore.filteredCoordsLists.Count != 0) {
            filteredCoords[] lastData = (filteredCoords[])ridingStore.filteredCoordsLists[ridingStore.filteredCoordsLists.Count - 1];
            float lastLat = lastData[lastData.Length - 1].latitude;
            float lastLon = lastData[lastData.Length - 1].longitude;
            Vector2 lastVal = new Vector2(lastLat, lastLon);
            OnlineMaps.instance.position = lastVal;
        }
    }

    void test() {
        List<Vector2> list = new List<Vector2> {
            new Vector2(3, 3),
            new Vector2(5, 3),
            new Vector2(4, 4),
            new Vector2(9.3f, 6.5f)
        };
        //map.GetComponent<OnlineMaps>().AddDrawingElement(new OnlineMapsDrawingLine(list, Color.red, 3f));
    }
}
