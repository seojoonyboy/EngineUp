using Flux;
using UnityEngine;
using System.Collections;

public class Groups : AjwStore {
    public Groups(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }

    NetworkManager networkManager = NetworkManager.Instance;
    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();

    public Group[] 
        myGroups,
        searchedGroups;

    public Group clickedGroup;
    public ActionTypes eventType;
    public int sceneIndex = -1;

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.GROUP_GET_MEMBERS:
                Group_getMemberAction getMemberAct = action as Group_getMemberAction;
                getGroupDetail(getMemberAct);
                break;
            case ActionTypes.GROUP_SEARCH:
                Group_search searchAct = action as Group_search;
                searchGroups(searchAct);
                break;
            case ActionTypes.GROUP_ON_PANEL:
                _emitChange();
                break;
        }
        eventType = action.type;
    }

    private void searchGroups(Group_search payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups?name=")
                    .Append(payload.keyword);
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log(payload.response.data);
                searchedGroups = JsonHelper.getJsonArray<Group>(payload.response.data);
                onPanel(1);
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                break;
        }
    }

    private void getGroupDetail(Group_getMemberAction payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups/")
                    .Append(payload.id);
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                clickedGroup = Group.fromJSON(payload.response.data);
                onPanel(7);
                break;
            case NetworkAction.statusTypes.FAIL:

                break;
        }
    }

    private void onPanel(int index) {
        sceneIndex = index;

        Group_OnPanel onMemberPanel = ActionCreator.createAction(ActionTypes.GROUP_ON_PANEL) as Group_OnPanel;
        dispatcher.dispatch(onMemberPanel);
    }
}

[System.Serializable]
public class Group {
    public int id;
    public string name;
    public string groupIntro;
    public string locationDistrict;
    public string locationCity;
    public int membersCount;

    public static Group fromJSON(string json) {
        return JsonUtility.FromJson<Group>(json);
    }
}

[System.Serializable]
public class Member {

}