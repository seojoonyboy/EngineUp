﻿using Flux;
using UnityEngine;
using System.Collections;

public class Locations : AjwStore {
    //store status
    public storeStatus storeStatus = storeStatus.NORMAL;
    //store message
    public string message;

    public Locations(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }

    NetworkManager networkManager = NetworkManager.Instance;
    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();

    public ActionTypes eventType;

    public District[] districts;
    public City cities;
    public Borough[] borough;
    // Use this for initialization
    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.GET_DISTRICT_DATA:
                GetDistrictsData distAct = action as GetDistrictsData;
                getDistrictsData(distAct);
                break;
            case ActionTypes.GET_CITY_DATA:
                GetCityData cityAct = action as GetCityData;
                getCityData(cityAct);
                break;
        }
        eventType = action.type;
    }

    private void getDistrictsData(GetDistrictsData payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("countries/1/districts");
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;

                //Debug.Log(payload.response.data);
                districts = JsonHelper.getJsonArray<District>(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;

                Debug.Log(payload.response.data);
                break;
        }
    }

    private void getCityData(GetCityData payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;

                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("countries/1/districts/")
                    .Append(payload.id + 1);
                //Debug.Log(strBuilder);
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;

                Debug.Log(payload.response.data);
                cities = City.fromJSON(payload.response.data);
                borough = cities.cities;
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                
                Debug.Log(payload.response.data);
                break;
        }
    }
}

[System.Serializable]
public class District {
    public int id;
    public string name;

    public static District fromJSON(string json) {
        return JsonUtility.FromJson<District>(json);
    }
}

[System.Serializable]
public class City {
    public int id;
    public string name;
    public Borough[] cities;

    public static City fromJSON(string json) {
        return JsonUtility.FromJson<City>(json);
    }
}

[System.Serializable]
public class Borough {
    public int id;
    public string name;
}