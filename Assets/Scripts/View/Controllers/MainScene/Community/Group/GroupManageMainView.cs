using UnityEngine;
using System.Collections;

public class GroupManageMainView : MonoBehaviour {
    public GroupDetailView controller;

    GameManager gm;
    void Start() {
        gm = GameManager.Instance;
    }

    public void offPanel() {
        gameObject.SetActive(false);

        Group_detail getGroupDetailAct = ActionCreator.createAction(ActionTypes.GROUP_DETAIL) as Group_detail;
        getGroupDetailAct.id = controller.id;
        gm.gameDispatcher.dispatch(getGroupDetailAct);
    }

    public void onGroupStoreListener() {

    }
}