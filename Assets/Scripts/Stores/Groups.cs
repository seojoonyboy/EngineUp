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
    public bool addResult = false;

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.GROUP_MY_GROUPS:

                break;
            case ActionTypes.GROUP_DETAIL:
                Group_detail getMemberAct = action as Group_detail;
                getGroupDetail(getMemberAct);
                break;
            case ActionTypes.GROUP_SEARCH:
                Group_search searchAct = action as Group_search;
                searchGroups(searchAct);
                break;
            case ActionTypes.GROUP_ADD:
                Group_AddAction addAct = action as Group_AddAction;
                addGroup(addAct);
                break;
            case ActionTypes.GROUP_ON_PANEL:
                Debug.Log("그룹 하위패널 활성화 액션 발생");
                Group_OnPanel onGroupPanelAct = action as Group_OnPanel;
                int index = onGroupPanelAct.index;
                onPanel(index);
                break;
        }
        eventType = action.type;
    }

    private void getMyGroups(Group_myGroups payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups/")
                    .Append(payload.id);
                //networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log(payload.response.data);
                myGroups = JsonHelper.getJsonArray<Group>(payload.response.data);
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                break;
        }
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
                Group_OnPanel onGroupPanel = ActionCreator.createAction(ActionTypes.GROUP_ON_PANEL) as Group_OnPanel;
                onGroupPanel.index = 1;
                dispatcher.dispatch(onGroupPanel);
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                break;
        }
    }

    private void getGroupDetail(Group_detail payload) {
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
                Group_OnPanel onGroupPanel = ActionCreator.createAction(ActionTypes.GROUP_ON_PANEL) as Group_OnPanel;
                onGroupPanel.index = 7;
                dispatcher.dispatch(onGroupPanel);
                break;
            case NetworkAction.statusTypes.FAIL:

                break;
        }
    }

    private void addGroup(Group_AddAction payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups");
                //허용 글자 수 초과시
                if (payload.name.Length >= 25) {
                    payload.status = NetworkAction.statusTypes.FAIL;
                    return;
                }
                WWWForm form = new WWWForm();

                form.AddField("name", payload.name);
                form.AddField("locationDistrict", payload.district);
                form.AddField("locationCity", payload.city);

                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log(payload.response.data);
                addResult = true;
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                addResult = false;
                _emitChange();
                break;
        }
    }

    private void onPanel(int index) {
        sceneIndex = index;
        _emitChange();
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