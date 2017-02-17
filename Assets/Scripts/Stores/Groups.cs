using Flux;
using UnityEngine;
using System.Collections;

public class Groups : AjwStore {
    public Groups(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }

    NetworkManager networkManager = NetworkManager.Instance;
    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();

    public Group[] myGroups;
    public ActionTypes eventType;
    public string msg;

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.GROUP_GET_MEMBERS:
                Group_getMemberAction getMemberAct = action as Group_getMemberAction;
                getMembers(getMemberAct);
                break;
        }
        eventType = action.type;
    }

    private void getMembers(Group_getMemberAction payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("friends/requested?");
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:

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
