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
    OnlineMapsMarker endMarker, startMarker;

    public HistoryViewController parentController;

    public CanvasGroup canvas;
    public Texture2D markerTexture;
    private Animator animator;

    public float markerOffset;

    private _Rect
        minCoord,
        maxCoord;

    void Awake() {
        gm = GameManager.Instance;
        ridingStore = gm.ridingStore;

        animator = GetComponent<Animator>();
    }


    void OnEnable() {
        Invoke("playSlideIn", 0.2f);
    }

    void playSlideIn() {
        animator.Play("SlideIn");
    }

    public void onBackButton() {
        animator.Play("SlideOut");
        offMap();
    }

    public void slideFinished(AnimationEvent animationEvent) {
        int boolParm = animationEvent.intParameter;

        //slider in
        if (boolParm == 1) {
            GetRidingRecords act = ActionCreator.createAction(ActionTypes.RIDING_DETAILS) as GetRidingRecords;
            act.id = id;
            act.type = GetRidingRecords.callType.HISTORY;
            gm.gameDispatcher.dispatch(act);
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
    }

    public void setMap(RidingDetails data) {
        minCoord = new _Rect();
        maxCoord = new _Rect();

        canvas.blocksRaycasts = false;

        mapHeader.SetActive(true);
        map.SetActive(true);

        preMapScale = map.transform.localScale;
        preMapPos = map.transform.localPosition;

        map.transform.localScale = new Vector3(1f, 1f, 0.8f);
        map.transform.localPosition = new Vector3(0.08f, 0, 1791f);

        OnlineMaps _map = OnlineMaps.instance;
        OnlineMapsControlBase.instance.OnMapZoom += zooming;

        innerRidingDetails[] coords = data.coords;

        //var testMarker = OnlineMaps.instance.AddMarker(new Vector2(127.74437f, 37.87998f));
        //testMarker.texture = markerTexture;
        //testMarker.scale = 0.5f;
        //OnlineMaps.instance.markers[0].align = OnlineMapsAlign.Top;
        OnlineMaps.instance.OnChangePosition += changePos;
        if (coords.Length == 0) {
            _map.position = new Vector2(127.74437f, 37.87998f);
        }
        else {
            List<Vector2> list = new List<Vector2>();
            //좌표들의 평균값으로 맵 시작위치 지정
            for(int i=0; i<coords.Length; i++) {
                Vector2 coord = new Vector2(coords[i].latitude, coords[i].longitude);
                list.Add(coord);

                if(minCoord.x == 0) {
                    minCoord.x = coords[i].longitude;
                }
                else {
                    if (coords[i].longitude < minCoord.x) {
                        minCoord.x = coords[i].longitude;
                    }
                }
                if(minCoord.y == 0) {
                    minCoord.y = coords[i].latitude;
                }
                else {
                    if (coords[i].latitude < minCoord.y) {
                        minCoord.y = coords[i].latitude;
                    }
                }

                if(coords[i].longitude > maxCoord.x) {
                    maxCoord.x = coords[i].longitude;
                }
                if(coords[i].latitude > maxCoord.y) {
                    maxCoord.y = coords[i].latitude;
                }
            }

            //Debug.Log("최소 X값 : " + minCoord.x);
            //Debug.Log("최소 y값 : " + minCoord.y);
            //Debug.Log("최대 X값 : " + maxCoord.x);
            //Debug.Log("최대 y값 : " + maxCoord.y);

            if (IsRectCoordValid(minCoord, maxCoord)) {
                float centerX = (float)((minCoord.x + maxCoord.x) / 2.0f);
                float centerY = (float)((minCoord.y + maxCoord.y) / 2.0f);

                _map.position = new Vector2(centerY, centerX);

                double dx;
                double dy;
                OnlineMapsUtils.DistanceBetweenPoints(minCoord.x, minCoord.y, maxCoord.x, maxCoord.y, out dx, out dy);
                //1km 이하인 경우
                double maxDist = dx;
                if (dx < dy) {
                    maxDist = dy;
                }
                //Debug.Log("Dist X : " + dx);
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
                //Vector2 pos = new Vector2(coords[coords.Length - 1].latitude, coords[coords.Length - 1].longitude);
                //_map.position = pos;
                //_map.zoom = 18;
            }
            else {
                Vector2 pos = new Vector2(coords[coords.Length - 1].latitude, coords[coords.Length - 1].longitude);
                _map.position = pos;
            }

            _line = new OnlineMapsDrawingLine(list, Color.red, 3.0f);
            _map.AddDrawingElement(_line);

            if(coords.Length == 1) {
                //도착마크만 표시
                Vector2 markerPos = new Vector2(coords[0].latitude, coords[0].longitude);
                endMarker = _map.AddMarker(markerPos);
                endMarker.align = OnlineMapsAlign.BottomLeft;
            }
            else {
                //출발 도착 마커 모두 표시
                Vector2 startPos = new Vector2(coords[0].latitude, coords[0].longitude);
                startMarker = _map.AddMarker(startPos, markerTexture, "");
                startMarker.align = OnlineMapsAlign.BottomLeft;

                Vector2 endPos = new Vector2(coords[coords.Length - 1].latitude, coords[coords.Length - 1].longitude);
                endMarker = _map.AddMarker(endPos, markerTexture, "");
                endMarker.align = OnlineMapsAlign.BottomLeft;
            }
        }
    }

    bool IsRectCoordValid(_Rect min, _Rect max) {
        bool result = true;
        if (min.x == 0 || min.y == 0 || max.x == 0 || max.y == 0) {
            result = false;
        }
        return result;
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
                _line.weight = 3f;
            }
            else {
                _line.weight = 5f;
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

    private void changePos() {
        double marker_lng;
        double marker_lat;
        //OnlineMaps.instance.markers[0].GetPosition(out marker_lng, out marker_lat);

        double tlx; //좌측 상단 x
        double tly; //좌측 상단 y
        double brx; //우측 하단 x
        double bry; //우측 하단 y

        OnlineMaps.instance.GetCorners(out tlx, out tly, out brx, out bry);

        //if (tlx + markerOffset * 0.0001 >= marker_lng || tly - markerOffset * 0.0001 <= marker_lat || brx - markerOffset * 0.0001 <= marker_lng || bry + markerOffset * 0.0001 >= marker_lat) {
        //    //Debug.Log("영역밖");
        //    OnlineMaps.instance.markers[0].enabled = false;
        //}
        //else {
        //    OnlineMaps.instance.markers[0].enabled = true;
        //}
    }
}
