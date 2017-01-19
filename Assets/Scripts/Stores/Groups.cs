using Flux;
using UnityEngine;
using System.Collections;

public class Groups : Store<Actions> {
    public Groups(Dispatcher<Actions> _dispatcher) : base(_dispatcher) { }
    NetworkManager networkManager = NetworkManager.Instance;

    public Group[] myGroups;
    public ActionTypes eventType;
    public string msg;

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.COMMUNITY_INITIALIZE:
                getMyGroups(action as CommunityInitAction);
                //임시 dummy file 이용
                TextAsset myGroup = Resources.Load<TextAsset>("myGroup");
                myGroups = JsonHelper.getJsonArray<Group>(myGroup.text);
                break;

            case ActionTypes.COMMUNITY_SEARCH:
                CommunitySearchAction searchAct = action as CommunitySearchAction;
                if(searchAct.type == CommunitySearchAction.searchType.GROUP) {
                    search(searchAct);
                }
                break;
        }
        eventType = action.type;
    }

    private void getMyGroups(CommunityInitAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
            var strBuilder = GameManager.Instance.sb;
            strBuilder.Remove(0, strBuilder.Length);
            strBuilder.Append(networkManager.baseUrl)
                .Append("users/")
                .Append(GameManager.Instance.deviceId);
            _emitChange();
            //networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
            break;
            case NetworkAction.statusTypes.SUCCESS:
            //_emitChange();
            break;
            case NetworkAction.statusTypes.FAIL:
            break;
        }
    }

    private void search(CommunitySearchAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("users/")
                    .Append(GameManager.Instance.deviceId);
                //networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                msg = "그룹 검색";
                _emitChange();

                break;
            case NetworkAction.statusTypes.SUCCESS:
                //msg = act.response.data;
                //_emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                break;
        }
    }

    private void delete(CommunityDeleteAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
            var strBuilder = GameManager.Instance.sb;
            strBuilder.Remove(0, strBuilder.Length);
            strBuilder.Append(networkManager.baseUrl)
                .Append("users/")
                .Append(GameManager.Instance.deviceId);
            //networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
            break;
            case NetworkAction.statusTypes.SUCCESS:
            _emitChange();
            break;
            case NetworkAction.statusTypes.FAIL:
            break;
        }
    }
}

[System.Serializable]
public class Group {
    public string location;
    public int memberNum;
    public string name;

    public static Group fromJSON(string json) {
        return JsonUtility.FromJson<Group>(json);
    }
}
