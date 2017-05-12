using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HistoryDetailViewController : MonoBehaviour {
    public BoxCollider[] collider;
    public GameObject map;
    private Vector2 preMapScale;
    public int id;
    private GameManager gm;
    private Riding ridingStore;
    private Vector3 preMapPos;

    public UILabel
        dist,
        avgSpeed,
        uphill,
        time,
        maxSpeed,
        boxNum,
        nickName,
        date;

    OnlineMapsDrawingLine _line;

    void Awake() {
        gm = GameManager.Instance;
        ridingStore = gm.ridingStore;
    }

    //지도 사용을 위해 다른 collider를 꺼준다.
    void OnEnable() {
        foreach(Collider col in collider) {
            col.enabled = false;
        }

        GetRidingRecords act = ActionCreator.createAction(ActionTypes.RIDING_DETAILS) as GetRidingRecords;
        act.id = id;
        gm.gameDispatcher.dispatch(act);
    }

    void OnDisable() {
        foreach(Collider col in collider) {
            col.enabled = true;
        }
    }

    public void setMap(RidingDetails data) {
        map.SetActive(true);
        OnlineMaps _map = map.GetComponent<OnlineMaps>();
        map.transform.localScale = new Vector3(1.31f, 1.0f, 1.31f);
        preMapPos = map.transform.localPosition;
        map.transform.localPosition = new Vector3(-1953f, 380f, -1127f);
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
        }
        _map.zoom = 18;

        OnlineMapsControlBase.instance.OnMapZoom += zooming;
    }

    public void onPanel() {
        gameObject.SetActive(true);
    }

    public void offPanel() {
        OnlineMaps _map = map.GetComponent<OnlineMaps>();
        map.transform.localScale = Vector3.one;
        map.transform.localPosition = preMapPos;
        _map.RemoveAllDrawingElements();

        gameObject.SetActive(false);
        map.SetActive(false);
    }

    public void setInfo(RidingDetails data) {
        dist.text = (Math.Round(data.distance, 2, MidpointRounding.AwayFromZero)) + " KM";
        avgSpeed.text = (Math.Round(data.avgSpeed, 2, MidpointRounding.AwayFromZero)) + " KM/h";
        uphill.text = (Math.Round(data.uphillDistance, 2, MidpointRounding.AwayFromZero)) + " M";
        time.text = data.runningTime.Split('.')[0];
        maxSpeed.text = (Math.Round(data.maxSpeed, 2, MidpointRounding.AwayFromZero)) + " KM/h";
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
}
