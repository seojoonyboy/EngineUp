﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class HistoryDetailViewController : MonoBehaviour {
    public GameObject 
        map,
        mapHeader;
    private Vector3 
        preMapScale,
        preMapPos;
    public int id;
    private GameManager gm;
    private Riding ridingStore;

    public Text
        dist,
        avgSpeed,
        uphill,
        time,
        maxSpeed,
        boxNum;
    public Text nickName, date;
    OnlineMapsDrawingLine _line;

    public HistoryViewController parentController;

    private TweenPosition tP;
    private bool 
        isReverse_tp,
        isTweening = false;

    public CanvasGroup canvas;
    void Awake() {
        gm = GameManager.Instance;
        ridingStore = gm.ridingStore;

        tP = GetComponent<TweenPosition>();
    }

    void OnEnable() {
        tweenPos();
        isReverse_tp = false;
    }

    public void tweenPos() {
        if (isTweening) {
            return;
        }
        isTweening = true;
        if (!isReverse_tp) {
            tP.PlayForward();
        }
        else {
            offMap();
            //swap
            Vector3 tmp;
            tmp = tP.to;
            tP.to = tP.from;
            tP.from = tmp;

            tP.ResetToBeginning();
            tP.PlayForward();
        }
    }

    public void tpFinished() {
        isTweening = false;
        //패널 닫기시
        if (isReverse_tp) {
            gameObject.SetActive(false);
            gameObject.transform.Find("TopPanel").gameObject.SetActive(false);
        }
        //패널 활성화시
        else {
            gameObject.transform.Find("TopPanel").gameObject.SetActive(true);

            GetRidingRecords act = ActionCreator.createAction(ActionTypes.RIDING_DETAILS) as GetRidingRecords;
            act.id = id;
            act.type = GetRidingRecords.callType.HISTORY;
            gm.gameDispatcher.dispatch(act);
        }

        isReverse_tp = true;
    }

    public void setMap(RidingDetails data) {
        canvas.blocksRaycasts = false;

        mapHeader.SetActive(true);
        map.SetActive(true);

        preMapScale = map.transform.localScale;
        preMapPos = map.transform.localPosition;

        map.transform.localScale = new Vector3(1f, 1f, 0.8f);
        map.transform.localPosition = new Vector3(0.08f, 0, 1793.24f);

        OnlineMaps _map = OnlineMaps.instance;
        OnlineMapsControlBase.instance.OnMapZoom += zooming;

        canvas.blocksRaycasts = false;

        innerRidingDetails[] coords = data.coords;
        if(coords.Length == 0) {
            _map.position = new Vector2(127.74437f, 37.87998f);
        }
        else {
            _map.position = new Vector2(coords[0].latitude, coords[0].longitude);
            List<Vector2> list = new List<Vector2>();

            for(int i=0; i<coords.Length; i++) {
                Vector2 coord = new Vector2(coords[i].latitude, coords[i].longitude);
                list.Add(coord);
            }
            _line = new OnlineMapsDrawingLine(list, Color.red, 2.0f);
            _map.AddDrawingElement(_line);

            if(coords.Length == 1) {
                //도착마크만 표시
                Vector2 markerPos = new Vector2(coords[0].latitude, coords[0].longitude);
                _map.AddMarker(markerPos);
            }
            else {
                //출발 도착 마커 모두 표시
                Vector2 startPos = new Vector2(coords[0].latitude, coords[0].longitude);
                _map.AddMarker(startPos);

                Vector2 endPos = new Vector2(coords[coords.Length - 1].latitude, coords[coords.Length - 1].longitude);
                _map.AddMarker(endPos);
            }
        }
        _map.zoom = 18;
    }

    public void setInfo(RidingDetails data) {
        dist.text = (Math.Round(data.distance, 2, MidpointRounding.AwayFromZero)).ToString();
        avgSpeed.text = (Math.Round(data.avgSpeed, 2, MidpointRounding.AwayFromZero)).ToString();
        uphill.text = (Math.Round(data.uphillDistance, 2, MidpointRounding.AwayFromZero)).ToString();
        time.text = data.runningTime.Split('.')[0];
        maxSpeed.text = (Math.Round(data.maxSpeed, 2, MidpointRounding.AwayFromZero)).ToString();
        boxNum.text = data.get_boxes.ToString();
    }

    public void setNickName(string data) {
        nickName.text = data + "님의 라이딩 기록";
    }

    public void setDate(string time) {
        string[] split = time.Split('T');
        string[] _date = split[0].Split('-');
        date.text = _date[0] + " . " + _date[1] + " . " + _date[2];
    }

    private void zooming() {
        if (_line != null) {
            int level = OnlineMaps.instance.zoom;
            if (level > 10) {
                _line.weight = 1f;
            }
            else {
                _line.weight = 3f;
            }
        }
    }

    private void offMap() {
        OnlineMaps _map = map.GetComponent<OnlineMaps>();
        map.transform.localScale = Vector3.one;
        map.transform.localPosition = preMapPos;

        _map.RemoveAllDrawingElements();
        _map.RemoveAllMarkers();

        map.SetActive(false);

        canvas.blocksRaycasts = true;
        mapHeader.SetActive(false);
    }
}
