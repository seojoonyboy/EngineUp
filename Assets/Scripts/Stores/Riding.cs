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
    //라이딩시 server에 배정되는 id
    public int ridingId;
    public float totalDist;
    public float curSpeed;
    public float avgSpeed;
    public float maxSpeed = 0;
    public float uphillDistance;
    public string totalTime = "00:00:00";
    public int boxes = 0;
    NetworkManager networkManager = NetworkManager.Instance;
    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();

    public ArrayList coordList;
    public ArrayList filteredCoordsLists = new ArrayList();

    public RidingRecords[] ridingRecords;
    public RidingDetails ridingDetails;

    public string postsCallbackHeader;
    public GetRidingRecords.callType callRecType;

    public Riding(QueueDispatcher<Actions> _dispatcher):base(_dispatcher){
        postBuffer = new coordData[10];
        postBufferCounter = 0;
        coordList = new ArrayList();
    }

    void _gpsSend(GPSSendAction payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
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
                boxes = ridingData.get_boxes;
                ridingId = ridingData.id;

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
        Debug.Log(loc.latitude);
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
            RidingData ridingData = RidingData.fromJSON(act.response.data);
            ridingId = ridingData.id;
            break;
        case NetworkAction.statusTypes.FAIL:
            storeStatus = storeStatus.ERROR;
            Debug.Log(act.response.data);
            break;
        }
    }

    //라이딩 기록 목록 불러오기
    //pagination included
    void getRecords(GetRidingRecords payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;

                callRecType = payload.type;

                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl).Append("ridings");
                if(!payload.isFirst) {
                    if(postsCallbackHeader != null && postsCallbackHeader.Contains("next")) {
                        int startIndex = postsCallbackHeader.IndexOf('?');
                        int endIndex = postsCallbackHeader.IndexOf('>');
                        string str = postsCallbackHeader.Substring(startIndex, endIndex - startIndex);
                        strBuilder.Append(str);
                    }
                    else {
                        Debug.Log("다음 글 없음");
                        return;
                    }
                }
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                postsCallbackHeader = payload.response.header;
                ridingRecords = JsonHelper.getJsonArray<RidingRecords>(payload.response.data);
                Debug.Log(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                Debug.Log(payload.response.data);
                _emitChange();
                break;
        }
    }

    void getRidingHistoryDetails(GetRidingRecords payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder
                    .Append(networkManager.baseUrl)
                    .Append("ridings/")
                    .Append(payload.id);
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                ridingDetails = RidingDetails.fromJSON(payload.response.data);
                Debug.Log(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                Debug.Log(payload.response.data);
                _emitChange();
                break;
        }
    }

    void removeRecords(RidingRecordsRmv payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder
                    .Append(networkManager.baseUrl)
                    .Append("ridings/all");
                networkManager.request("DELETE", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                ridingDetails = RidingDetails.fromJSON(payload.response.data);
                Debug.Log(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                Debug.Log(payload.response.data);
                _emitChange();
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
        case ActionTypes.GET_RIDING_RECORDS:
            getRecords(action as GetRidingRecords);
            break;
        case ActionTypes.RIDING_DETAILS:
            getRidingHistoryDetails(action as GetRidingRecords);
            break;
        case ActionTypes.RIDING_RECORDS_REMOVE:
            removeRecords(action as RidingRecordsRmv);
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
    public int get_boxes;

    public static RidingData fromJSON(string json){
        return JsonUtility.FromJson<RidingData>(json);
    }
}

[System.Serializable]
public class RidingRecords : RidingData {
    public string createDate;
}

[System.Serializable]
public class RidingDetails : RidingData {
    public innerRidingDetails[] coords;
    public string createDate;

    public static RidingDetails fromJSON(string json) {
        return JsonUtility.FromJson<RidingDetails>(json);
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

[System.Serializable]
public class innerRidingDetails : filteredCoords {
    public string isPaused;
    public string createDate;
}