using Flux;
using UnityEngine;
using System.Collections;

public class Groups : Store<Actions> {
    public Groups(Dispatcher<Actions> _dispatcher) : base(_dispatcher) { }
    NetworkManager networkManager = NetworkManager.Instance;

    public Group[] myGroups;

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.GAME_START:
                getMyGroups(action as GameStartAction);
                
                break;

            case ActionTypes.COMMUNITY_SEARCH:
                CommunitySearchAction searchAct = action as CommunitySearchAction;
                if(searchAct.type == CommunitySearchAction.searchType.GROUP) {
                    search(searchAct);
                }
                break;
            case ActionTypes.COMMUNITY_DELETE:
                CommunityDeleteAction delAct = action as CommunityDeleteAction;
                if (delAct.type == CommunityDeleteAction.deleteType.GROUP) {
                    delete(delAct);
                }
                break;
        }
    }

    private void getMyGroups(GameStartAction act) {
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

    private void search(CommunitySearchAction act) {
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
    public string groupName;
    public int memberNum;
    public string location;

    public static Group fromJSON(string json) {
        return JsonUtility.FromJson<Group>(json);
    }
}
