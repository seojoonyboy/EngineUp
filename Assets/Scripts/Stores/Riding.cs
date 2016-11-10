using Flux;
using UnityEngine;
using System;

public class Riding : Store<Actions>{
    LocationInfo? _preLocation = null;
    DateTime startTime;
    public float totalDist;
    public float curSpeed;
    public float avgSpeed;
    public TimeSpan totalTime;


    private const float EARTH_RADIUS = 6371;

    public Riding(Dispatcher<Actions> _dispatcher):base(_dispatcher){}

    private void _gpsOperation(LocationInfo loc){
        totalTime = DateTime.Now - startTime;
        if(_preLocation == null){
            _preLocation = loc;
            return;
        } else {
            float curDistance = calcDist(_preLocation.Value, loc);
            totalDist += curDistance;
            float intervalTime = (float)(loc.timestamp - _preLocation.Value.timestamp);
            curSpeed = curDistance / intervalTime / 3600;
            avgSpeed = totalDist / (float)totalTime.TotalHours;
        }
        _preLocation = loc;
        Debug.Log("gps");
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
    protected override void _onDispatch(Actions action){
        switch(action.type){
        case ActionTypes.RIDING_START:
            startTime = DateTime.Now;
            totalDist = 0;
            break;
        case ActionTypes.GET_GPS_DATA:
            GetGPSDataAction _act = action as GetGPSDataAction;
            _gpsOperation(_act.GPSInfo);
            break;
        }
        _emmetChange();
    }
}