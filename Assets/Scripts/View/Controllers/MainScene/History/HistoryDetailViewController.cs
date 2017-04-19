using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryDetailViewController : MonoBehaviour {
    public BoxCollider[] collider;
    public GameObject map;
    private Vector2 preMapScale;
    public int id;
    private GameManager gm;
    private Riding ridingStore;

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
        OnlineMaps _map = map.GetComponent<OnlineMaps>();
        
        preMapScale = _map.tilesetSize;
        _map.tilesetSize = new Vector2(540, 600);
        innerRidingDetails[] coords = data.coords;
        if(coords.Length == 0) {
            _map.position = new Vector2(127.74437f, 37.87998f);
        }
        else {
            _map.position = new Vector2(coords[0].altitude, coords[0].latitude);
            List<Vector2> list = new List<Vector2>();

            for(int i=0; i<coords.Length; i++) {
                Vector2 coord = new Vector2(coords[i].altitude, coords[i].latitude);
                list.Add(coord);
            }
            _line = new OnlineMapsDrawingLine(list, Color.red, 3.0f);
            _map.AddDrawingElement(_line);
        }
        _map.zoom = 18;

        map.SetActive(true);

        OnlineMapsControlBase.instance.OnMapZoom += zooming;
    }

    public void onPanel() {
        gameObject.SetActive(true);
    }

    public void offPanel() {
        OnlineMaps _map = map.GetComponent<OnlineMaps>();
        _map.tilesetSize = preMapScale;
        _map.RemoveAllDrawingElements();

        gameObject.SetActive(false);
        map.SetActive(false);
    }

    public void setInfo(RidingDetails data) {
        dist.text = data.distance + " KM";
        avgSpeed.text = data.avgSpeed + " KM/h";
        uphill.text = data.uphillDistance + " M";
        time.text = data.runningTime;
        maxSpeed.text = data.maxSpeed + " KM/h";
        boxNum.text = "x " + data.get_boxes;
    }

    public void setNickName(string data) {
        nickName.text = data + "님의 라이딩 기록";
    }

    public void setDate(string time) {
        string[] split = time.Split('T');

        date.text = split[0];
    }

    private void zooming() {
        if (_line != null) {
            int level = OnlineMaps.instance.zoom;
            if (level > 10) {
                _line.weight = 0.5f;
            }
            else {
                _line.weight = 1f;
            }
        }
    }
}
