using Flux;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Riding : AjwStore{
    public storeStatus storeStatus = storeStatus.NORMAL;
    public string message;

    public ActionTypes eventType;
    coordData _preLocation = null;
    coordData[] postBuffer;
    int postBufferCounter;
    int ridingId;
    public float totalDist;
    public float curSpeed;
    public float avgSpeed;
    public float maxSpeed = 0;
    public float uphillDistance;
    public string totalTime;
    NetworkManager networkManager = NetworkManager.Instance;
    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();

    public ArrayList coordList;
    public ArrayList filteredCoordsLists = new ArrayList();
    public Riding(QueueDispatcher<Actions> _dispatcher):base(_dispatcher){
        postBuffer = new coordData[10];
        postBufferCounter = 0;
        coordList = new ArrayList();
    }

    void _gpsSend(GPSSendAction payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
                Debug.Log("GPS 전송");
                System.Text.StringBuilder _sb = GameManager.Instance.sb;
                _sb.Remove(0, _sb.Length);
                for (var i = 0; i < postBufferCounter; i++) {
                    var loc = postBuffer[i];
                    _sb.Append(loc.altitude).Append("|")
                    .Append(loc.latitude).Append("|")
                    .Append(loc.longitude).Append("|")
                    .Append(loc.timeStamp).Append("|")
                    .Append(loc.horizontalAcuracy).Append("|")
                    .Append(loc.verticalAcuracy).Append("@");
                }
                var coordData = _sb.ToString();

                _sb.Remove(0, _sb.Length);
                _sb.Append(networkManager.baseUrl)
                    .Append("ridings/")
                    .Append(ridingId);
                WWWForm f = new WWWForm();
                f.AddField("coordData", coordData);
                if(payload.isStop) {
                    Debug.Log("일시정지함");
                    //f.AddField("isPaused", "true");
                    f.AddField("isPaused", 1);
                }

                Debug.Log("전송 CoordData : " + coordData);
                networkManager.request("PUT", _sb.ToString(), f, ncExt.networkCallback(dispatcher, payload));
                postBufferCounter = 0;
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                Debug.Log("callback CoordData : " + payload.response.data);
                RidingData ridingData = RidingData.fromJSON(payload.response.data);

                totalDist = ridingData.distance;
                avgSpeed = ridingData.avgSpeed;
                maxSpeed = ridingData.maxSpeed;
                uphillDistance = ridingData.uphillDistance;
                filteredCoordsLists.Add(ridingData.filteredCoords);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                Debug.Log(payload.response.data);
                _emitChange();
                break;
        }
    }

    private void _gpsOperation(GetGPSDataAction act){
        coordData loc = act.GPSInfo;
        if (act.isStop) {
            Debug.Log("라이딩 일시 정지. gps 좌표를 모두 전송합니다.");
            GPSSendAction _act = ActionCreator.createAction(ActionTypes.GPS_SEND) as GPSSendAction;
            _act.isStop = true;
            dispatcher.dispatch(_act);
            return;
        }

        if (!_filter(loc)) { return; } // 필터 적용
        postBuffer[postBufferCounter] = loc;
        postBufferCounter++;

        //coordData data = new coordData(loc.longitude,loc.latitude);
        //coordList.Add(data);

        if(postBufferCounter >= postBuffer.Length){
            GPSSendAction _act = ActionCreator.createAction(ActionTypes.GPS_SEND) as GPSSendAction;
            dispatcher.dispatch(_act);
        }

        //첫 data
        if(_preLocation == null) {
            _preLocation = loc;
            return;
        }
        _preLocation = loc;
    }

    public bool _filter(coordData loc) {
        if( loc.timeStamp == 0 ) { return false; }
        if( _preLocation == null ) { return true; }
        if( loc.timeStamp <= _preLocation.timeStamp) { return false; }

        return true;
    }

    void ridingStart(RidingStartAction act){
        coordList.Clear();
        switch(act.status){
        case NetworkAction.statusTypes.REQUEST:
            storeStatus = storeStatus.WAITING_REQ;
            
            _initRiding();
            var sb = GameManager.Instance.sb;
            sb.Remove(0,sb.Length)
                .Append(networkManager.baseUrl)
                .Append("ridings");
            WWWForm form = new WWWForm();
            //form.AddField("distance", 0);
            networkManager.request("POST", sb.ToString(),
                form, ncExt.networkCallback(dispatcher, act));
            break;
        case NetworkAction.statusTypes.SUCCESS:
            storeStatus = storeStatus.NORMAL;

            Debug.Log("riding start success");
            RidingData ridingData = RidingData.fromJSON(act.response.data);
            ridingId = ridingData.id;
            break;
        case NetworkAction.statusTypes.FAIL:
            storeStatus = storeStatus.ERROR;

            Debug.Log("riding start fail");
            Debug.Log(act.response.data);
            break;
        }
    }

    void _initRiding(){
        totalDist = 0;
        curSpeed = 0;
        maxSpeed = 0;
        avgSpeed = 0;
        uphillDistance = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ridingId = 0;
    }

    protected override void _onDispatch(Actions action){
        switch(action.type){
        case ActionTypes.RIDING_START:
            ridingStart(action as RidingStartAction);
            _emitChange();
            break;
        case ActionTypes.GET_GPS_DATA:
            GetGPSDataAction _act = action as GetGPSDataAction;
            _gpsOperation(_act);
            totalTime = _act.timeText;
            _emitChange();
            break;
        case ActionTypes.RIDING_END:
            Screen.sleepTimeout = SleepTimeout.SystemSetting;

            GPSSendAction sendAct = ActionCreator.createAction(ActionTypes.GPS_SEND) as GPSSendAction;
            dispatcher.dispatch(sendAct);

            _emitChange();
            break;
        case ActionTypes.GPS_SEND:
            GPSSendAction _sendAct = action as GPSSendAction;
            _gpsSend(_sendAct);
            break;
        }
        eventType = action.type;
    }
}

public class RidingData {
    public int id = -1;
    public float distance = 0;
    public string runningTime = null;
    public float avgSpeed = 0;
    public float maxSpeed = 0;
    public float uphillDistance = 0;
    public filteredCoords[] filteredCoords;

    public static RidingData fromJSON(string json){
        return JsonUtility.FromJson<RidingData>(json);
    }
}

public class coordData {
    public float longitude;
    public float latitude;
    public float altitude;
    public double timeStamp;
    public float horizontalAcuracy;
    public float verticalAcuracy;

    public coordData(float longitude, float latitude, float altitude, double timeStamp, float horAcc, float vertAcc) {
        this.longitude = longitude;
        this.latitude = latitude;
        this.altitude = altitude;
        this.timeStamp = timeStamp;
        this.horizontalAcuracy = horAcc;
        this.verticalAcuracy = vertAcc;
    }
}

[System.Serializable]
public class filteredCoords {
    public int id = -1;
    public float altitude = 0;
    public float latitude = 0;
    public float longitude = 0;
    public string timestamp = null;
    public float horizontalAccuracy = 0;
    public float verticalAccuracy = 0;
}