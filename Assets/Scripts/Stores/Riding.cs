using Flux;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

public class Riding : Store<Actions>{
    LocationInfo? _preLocation = null;
    LocationInfo[] postBuffer;
    int postBufferCounter;
    int ridingId;
    public bool isRiding;
    DateTime startTime;
    public float totalDist;
    public float curSpeed;
    public float avgSpeed;
    public float maxSpeed = 0;
    public TimeSpan totalTime;
    public StringBuilder resultData = new StringBuilder();
    private string dataFilePath;
    private const float EARTH_RADIUS = 6371;
    NetworkManager networkManager = NetworkManager.Instance;
    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();

    public ArrayList coordList;

    public Riding(Dispatcher<Actions> _dispatcher):base(_dispatcher){
        postBuffer = new LocationInfo[10];
        postBufferCounter = 0;
        coordList = new ArrayList();
    }

    void _gpsSend(){
        System.Text.StringBuilder _sb = GameManager.Instance.sb;
        _sb.Remove(0,_sb.Length);
        for (var i=0; i<postBufferCounter; i++){
            var loc = postBuffer[i];
            _sb.Append(loc.altitude).Append("|")
            .Append(loc.latitude).Append("|")
            .Append(loc.longitude).Append("|")
            .Append(loc.timestamp).Append("|")
            .Append(loc.horizontalAccuracy).Append("|")
            .Append(loc.verticalAccuracy).Append("@");
        }
        var coordData = _sb.ToString();

        _sb.Remove(0,_sb.Length);
        _sb.Append(networkManager.baseUrl)
            .Append("ridings/")
            .Append(ridingId)
            .Append("?deviceId=")
            .Append(GameManager.Instance.deviceId);
        WWWForm f = new WWWForm();
        f.AddField("coordData", coordData);

        networkManager.request("PUT", _sb.ToString(), f, _gpsSendCallback);
        postBufferCounter = 0;
    }

    void _gpsSendCallback(HttpResponse response){
        Debug.Log(response.responseCode);
        Debug.Log(response.data);
        RidingData ridingData = RidingData.fromJSON(response.data);

        totalDist = ridingData.distance;
        avgSpeed = ridingData.avgSpeed;
        maxSpeed = ridingData.maxSpeed;
    }

    private void _gpsOperation(LocationInfo loc){
        if(!_filter(loc)){ return; } // 필터 적용
        postBuffer[postBufferCounter] = loc;
        postBufferCounter++;

        coordData data = new coordData(loc.longitude,loc.latitude);
        coordList.Add(data);

        if(postBufferCounter >= postBuffer.Length){
            _gpsSend();
        }

        //첫 data
        if(_preLocation == null) {
            _preLocation = loc;
            return;
        }
        _preLocation = loc;
    }

    bool _filter(LocationInfo loc) {
        if( loc.horizontalAccuracy == 0 || loc.verticalAccuracy == 0 ) { return false; }
        if( loc.horizontalAccuracy > 100 || loc.horizontalAccuracy > 100) { return false; }
        if( loc.timestamp == 0 ) { return false; }
        if( _preLocation == null ) { return true; }
        if( loc.timestamp == _preLocation.Value.timestamp ) { return false; }

        return true;
    }

    void _writeData(LocationInfo loc){
        System.Text.StringBuilder _sb = GameManager.Instance.sb;
        _sb.Remove(0,_sb.Length);
        _sb.Append(loc.altitude).Append("|")
            .Append(loc.latitude).Append("|")
            .Append(loc.longitude).Append("|")
            .Append(loc.timestamp).Append("|")
            .Append(loc.horizontalAccuracy).Append("|")
            .Append(loc.verticalAccuracy).Append("\n");
        File.AppendAllText(dataFilePath, _sb.ToString(), Encoding.UTF8);
    }

    void _dataFileInit(DateTime now){
        System.Text.StringBuilder _sb = GameManager.Instance.sb;
        _sb.Remove(0,_sb.Length);
        _sb.Append(Application.persistentDataPath).Append("/");
        string d = now.ToString("s");
        int count = 0;
        d = d.Replace("T","")
            .Replace("-","")
            .Replace(":","");
        _sb.Append(d).Append(count);
        while(File.Exists(_sb.ToString())){
            count++;
            _sb.Remove(_sb.Length-1,1);
            _sb.Append(count);
        }
        dataFilePath = _sb.ToString();
        Debug.Log(dataFilePath);
    }

    void _readFile(string path) {
        byte[] bytes = File.ReadAllBytes(path);
        string data = System.Text.Encoding.UTF8.GetString(bytes);
        //resultData.Remove(0, resultData.Length);
        resultData.Append(data);
        resultData.Remove(0,1);
    }

    void ridingStart(RidingStartAction act){
        coordList.Clear();
        switch(act.status){
        case NetworkAction.statusTypes.REQUEST:
            _initRiding();
            var sb = GameManager.Instance.sb;
            sb.Remove(0,sb.Length)
                .Append(networkManager.baseUrl)
                .Append("ridings?deviceId=")
                .Append(GameManager.Instance.deviceId);
            WWWForm form = new WWWForm();
            form.AddField("distance", 0);
            networkManager.request("POST", sb.ToString(),
                form, ncExt.networkCallback(dispatcher, act));
            break;
        case NetworkAction.statusTypes.SUCCESS:
            Debug.Log("riding start success");
            RidingData ridingData = RidingData.fromJSON(act.response.data);
            ridingId = ridingData.id;
            break;
        case NetworkAction.statusTypes.FAIL:
            Debug.Log("riding start fail");
            Debug.Log(act.response.data);
            break;
        }
    }

    void _initRiding(){
        startTime = DateTime.Now;
        totalDist = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ridingId = 0;
        isRiding = true;
    }

    protected override void _onDispatch(Actions action){
        switch(action.type){
        case ActionTypes.RIDING_START:
            ridingStart(action as RidingStartAction);
            _emitChange();
            break;
        case ActionTypes.GET_GPS_DATA:
            totalTime = DateTime.Now - startTime;
            GetGPSDataAction _act = action as GetGPSDataAction;
            _gpsOperation(_act.GPSInfo);
            _emitChange();
            break;
        case ActionTypes.RIDING_END:
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            // _readFile(dataFilePath);
            _gpsSend();
            isRiding = false;
            _emitChange();
            break;
        }
    }


}

class RidingData {
    public int id;
    public float distance;
    public string runningTime;
    public float avgSpeed;
    public float maxSpeed;

    public static RidingData fromJSON(string json){
        return JsonUtility.FromJson<RidingData>(json);
    }
}

class coordData {
    public float longitude;
    public float latitude;

    public coordData(float longitude, float latitude) {
        this.longitude = longitude;
        this.latitude = latitude;
    }
}