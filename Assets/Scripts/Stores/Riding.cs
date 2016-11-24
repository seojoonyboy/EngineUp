using Flux;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class Riding : Store<Actions>{
    LocationInfo? _preLocation = null;
    DateTime startTime;
    public float totalDist;
    public float curSpeed;
    public float avgSpeed;
    public float maxSpeed;
    public TimeSpan totalTime;

    private string dataFilePath;
    private const float EARTH_RADIUS = 6371;

    public Riding(Dispatcher<Actions> _dispatcher):base(_dispatcher){}

    private void _gpsOperation(LocationInfo loc){
        totalTime = DateTime.Now - startTime;
        _writeData(loc);
        if(_preLocation == null){
            _preLocation = loc;
            return;
        } else {
            if(_preLocation.Value.timestamp == loc.timestamp){return;}
            float curDistance = calcDist(_preLocation.Value, loc);
            curDistance = float.IsNaN(curDistance)?0:curDistance;
            totalDist += curDistance;
            float intervalTime = (float)(loc.timestamp - _preLocation.Value.timestamp);
            curSpeed = (curDistance / intervalTime) * 3600f;
            avgSpeed = totalDist / (float)totalTime.TotalHours;
        }
        _preLocation = loc;
        // Debug.Log(loc.timestamp);
    }

    float calcDist(LocationInfo prePos, LocationInfo curPos) {
        float newLatitude = curPos.latitude;
        float newLongitude = curPos.longitude;
        float lastLatitude = prePos.latitude;
        float lastLongitude = prePos.longitude;

        float deltaLatitude = (newLatitude - lastLatitude) * Mathf.Deg2Rad;
        float deltaLongitude = (newLongitude - lastLongitude) * Mathf.Deg2Rad;

        float a = Mathf.Pow(Mathf.Sin(deltaLongitude / 2), 2)
            + Mathf.Cos(lastLatitude * Mathf.Deg2Rad) * Mathf.Cos(newLatitude * Mathf.Deg2Rad)
            * Mathf.Pow(Mathf.Sin(deltaLongitude / 2), 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        return EARTH_RADIUS * c;
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
    protected override void _onDispatch(Actions action){
        switch(action.type){
        case ActionTypes.RIDING_START:
            startTime = DateTime.Now;
            totalDist = 0;
            _dataFileInit(startTime);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            break;
        case ActionTypes.GET_GPS_DATA:
            GetGPSDataAction _act = action as GetGPSDataAction;
            _gpsOperation(_act.GPSInfo);
            break;
        case ActionTypes.RIDING_END:
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            break;
        }
        _emmetChange();
    }
}