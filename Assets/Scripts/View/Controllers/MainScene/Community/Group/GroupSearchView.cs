using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GroupSearchView : MonoBehaviour {
    public GroupViewController parent;
    private GameManager gm;
    private TweenPosition tP;

    public GameObject 
        container,
        emptyMessage,
        content;

    public bool
        isReverse_tp = false,
        isTweening = false;

    void Awake() {
        tP = GetComponent<TweenPosition>();
        gm = GameManager.Instance;
    }

    void OnEnable() {
        tweenPos();
    }

    void OnDisable() {
        //removeList();
        isReverse_tp = false;
    }

    public void tweenPos() {
        if (isTweening) {
            return;
        }
        isTweening = true;

        if (!isReverse_tp) {
            tP.ResetToBeginning();
            tP.PlayForward();
        }
        else {
            //swap
            Vector3 tmp;
            tmp = tP.to;
            tP.to = tP.from;
            tP.from = tmp;

            tP.ResetToBeginning();
            tP.PlayForward();
        }
    }

    public void tPFinished() {
        isTweening = false;

        if (isReverse_tp) {
            gameObject.SetActive(false);

            Vector3 tmp;
            tmp = tP.to;
            tP.to = tP.from;
            tP.from = tmp;
        }
        else {
            Debug.Log("TP FInished");
            isReverse_tp = true;

            if (parent.searchInput.text == null) {
                return;
            }

            Group_search searchAct = ActionCreator.createAction(ActionTypes.GROUP_SEARCH) as Group_search;
            searchAct.keyword = parent.searchInput.text;
            gm.gameDispatcher.dispatch(searchAct);

            parent.searchInput.text = null;
        }
    }

    public void makeList() {
        removeAllList();

        Group[] searchedGroups = parent.groupStore.searchedGroups;
        int length = searchedGroups.Length;
        if (length == 0) {
            emptyMessage.SetActive(true);
            return;
        }

        for (int i = 0; i < length; i++) {
            GameObject item = Instantiate(container);

            item.transform.SetParent(content.transform, false);

            item.GetComponent<GroupIndex>().id = searchedGroups[i].id;
            item.transform.Find("InnerContainer/District").GetComponent<Text>().text = searchedGroups[i].locationDistrict + " " + searchedGroups[i].locationCity;
            item.transform.Find("InnerContainer/Title").GetComponent<Text>().text = searchedGroups[i].name;
            item.transform.Find("InnerContainer/Member").GetComponent<Text>().text = "멤버 " + searchedGroups[i].membersCount + "명";

            Button detailBtn = item.transform.Find("DetailButton").GetComponent<Button>();
            detailBtn.onClick.AddListener(() => showDetail(item));
        }
    }

    void removeAllList() {
        foreach(Transform child in content.transform) {
            Destroy(child.gameObject);
        }
    }

    void showDetail(GameObject obj) {
        parent.showDetail(obj);
    }

    public void OffPanel() {
        gameObject.SetActive(false);
    }

    public void onGroupStoreListener() {
        Groups groupStore = parent.groupStore;
        ActionTypes groupStoreEventType = groupStore.eventType;

        if(groupStoreEventType == ActionTypes.GROUP_SEARCH) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                makeList();
            }
            else {
                emptyMessage.SetActive(true);
            }
        }
    }
}