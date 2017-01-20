using Flux;
using UnityEngine;
using System.Collections;

public class Friends : Store<Actions> {
    public Friends(Dispatcher<Actions> _dispatcher) : base(_dispatcher) { }
    NetworkManager networkManager = NetworkManager.Instance;

    public Friend[] myFriends;
    public string
        msg,
        keyword;
    public ActionTypes eventType;
    public GameObject targetItem;

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.COMMUNITY_INITIALIZE:
                getMyFriends(action as CommunityInitAction);
                //임시 dummy file 이용
                TextAsset friends = Resources.Load<TextAsset>("myFriends");
                myFriends = JsonHelper.getJsonArray<Friend>(friends.text);
                break;

            case ActionTypes.COMMUNITY_SEARCH:
                CommunitySearchAction searchAct = action as CommunitySearchAction;
                if (searchAct.type == CommunitySearchAction.searchType.FRIEND) {
                    search(searchAct);
                }
                break;

            case ActionTypes.COMMUNITY_DELETE:
                CommunityDeleteAction delAct = action as CommunityDeleteAction;
                if (delAct.type == CommunityDeleteAction.deleteType.FRIEND) {
                    delete(delAct);
                }
                break;
        }
        eventType = action.type;
    }

    private void getMyFriends(CommunityInitAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("users/")
                    .Append(GameManager.Instance.deviceId);
                //networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));                
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

    private void search(CommunitySearchAction act) {
        keyword = act.keyword;
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("users/")
                    .Append(GameManager.Instance.deviceId);
                //networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                msg = keyword + " 로 검색 결과";
                _emitChange();
                break;
            case NetworkAction.statusTypes.SUCCESS:
                //msg = keyword + " 로 검색 결과";
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
                targetItem = act.targetGameObj;
                _emitChange();
                //networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                //msg = keyword + " 로 검색 결과";
                //_emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                break;
        }
    }
}

[System.Serializable]
public class Friend {
    public string id;
    public string Level;
    public string[] Active;

    public static Friend fromJSON(string json) {
        return JsonUtility.FromJson<Friend>(json);
    }
}