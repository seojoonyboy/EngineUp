using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GroupViewController : MonoBehaviour {
    public GameObject[] subPanels;
    public Groups groupStore;
    public Locations locationStore;
    private GameManager gm;

    public InputField searchInput;

    public GroupAddView addViewCtrler;
    public GroupDetailView detailView;
    public GroupSearchView searchView;
    public GroupDelView groupDelView;
    public GroupMemberView memberView;
    
    public GameObject 
        container,
        addContainer,
        content;

    public GameObject modal;
    public GameObject nullMessage;

    public GameObject[] group_setting_subpanels;    //그룹 설정 내 하위 View들
    void Awake() {
        gm = GameManager.Instance;
        groupStore = gm.groupStore;
        locationStore = gm.locationStore;
    }

    void OnEnable() {
        //내 그룹 목록을 불러온다.
        Group_myGroups getMyGroupAct = ActionCreator.createAction(ActionTypes.MY_GROUP_PANEL) as Group_myGroups;
        gm.gameDispatcher.dispatch(getMyGroupAct);
    }

    void OnDisable() {
        //for(int i=0; i<subPanels.Length; i++) {
        //    subPanels[i].SetActive(false);
        //}
    }

    public void onPanel(GameObject obj) {
        int index = obj.GetComponent<GroupSceneIndex>().index;
        sendReq(index, obj);
    }

    //그룹 설정 하위 패널
    public void onSubPanel(GameObject obj) {
        int index = obj.GetComponent<GroupSceneIndex>().index;
        group_setting_subpanels[index].SetActive(true);
    }

    public void sendReq(int sceneIndex, GameObject obj = null) {
        Debug.Log("Scene Index : " + sceneIndex);
        switch (sceneIndex) {
            //그룹찾기
            case 0:
                break;
            case 1:
                break;
            //그룹 설정 메인
            case 2:
                break;
            //그룹 설정 변경
            case 4:
                //그룹추가

                break;
            //그룹원 보기
            case 5:
                break;
            case 6:
                break;
            //그룹 상세보기
            case 7:
                int id = obj.GetComponent<GroupIndex>().id;
                subPanels[sceneIndex].GetComponent<GroupDetailView>().id = id;
                break;
            case 8:
                break;
            case 9:
                
                break;
        }
        subPanels[sceneIndex].SetActive(true);
    }

    public void onGroupStoreListener() {
        ActionTypes groupStoreEventType = groupStore.eventType;
        if (groupStoreEventType == ActionTypes.MY_GROUP_PANEL) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                makeList(groupStore.myGroups);
            }
        }

        else if(groupStoreEventType == ActionTypes.GROUP_DETAIL) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                detailView.refreshTxt();
            }
        }

        else if(groupStoreEventType == ActionTypes.GROUP_CHECK_MY_STATUS) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                if(groupStore.isGroupMember) {
                    if(groupStore.myInfoInGroup[0].memberGrade == "GO") {
                        detailView.setViewMode("OWNER");
                    }
                    else if(groupStore.myInfoInGroup[0].memberGrade == "GM") {
                        detailView.setViewMode("MEMBER");
                    }
                }
                else {
                    detailView.setViewMode("VISITOR");
                }
            }
        }

        else if(groupStoreEventType == ActionTypes.GROUP_GET_MEMBERS) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                memberView.makeList();
                group_setting_subpanels[1].GetComponent<GroupMemberManageView>().makeList();
            }
        }

        else if(groupStoreEventType == ActionTypes.GROUP_BAN) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                detailView.refreshTxt();
                string msg = "그룹원을 강퇴시켰습니다.";
                onModal(msg);
            }
        }

        else if(groupStoreEventType == ActionTypes.GET_DISTRICT_DATA) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                addViewCtrler.init();
                group_setting_subpanels[0].GetComponent<GroupSettingChangeView>().setProvinceList();
            }
        }

        else if(groupStoreEventType == ActionTypes.GET_CITY_DATA) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                addViewCtrler.setCityList();
                group_setting_subpanels[0].GetComponent<GroupSettingChangeView>().setCityList();
            }
        }

        else if(groupStoreEventType == ActionTypes.GROUP_SEARCH) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                searchView.makeList();
            }
        }

        else if(groupStoreEventType == ActionTypes.GROUP_EDIT) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                detailView.refreshTxt();
                string msg = "그룹 정보가 갱신되었습니다.";
                onModal(msg);
            }
        }

        else if(groupStoreEventType == ActionTypes.GROUP_DESTROY) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                string msg = "그룹이 해체되었습니다.";
                onModal(msg);

                //내그룹 갱신
                Group_myGroups getMyGroupAct = ActionCreator.createAction(ActionTypes.MY_GROUP_PANEL) as Group_myGroups;
                gm.gameDispatcher.dispatch(getMyGroupAct);

                //그룹 삭제 화면 닫기
                if (group_setting_subpanels[2].activeSelf) {
                    group_setting_subpanels[2].GetComponent<GroupDelView>().onBackButton();
                }
                //그룹 상세보기 화면 닫기
                if (detailView.gameObject.activeSelf) {
                    detailView.onBackButton();
                }
                //그룹 설정 화면 닫기
                if (subPanels[1].activeSelf) {
                    subPanels[1].GetComponent<GroupSettingView>().onBackButton();
                }
            }
        }

        //Debug.Log(groupStore.storeStatus);
        if (groupStore.storeStatus == storeStatus.WAITING_REQ) {
            LoadingManager.Instance.onLoading();
        }
        else {
            LoadingManager.Instance.offLoading();
        }
    }

    public void onModal(string msg) {
        modal.SetActive(true);
        modal.transform.Find("InnerModal/Text").GetComponent<Text>().text = msg;
    }

    private void makeList(Group[] myGroups) {
        Debug.Log("내 그룹 목록 갱신");
        removeAllList();

        Button addBtn = addContainer.GetComponent<Button>();
        addBtn.onClick.AddListener(() => sendReq(4));

        if (myGroups == null || myGroups.Length == 0) {
            nullMessage.SetActive(true);
            return;
        }
        nullMessage.SetActive(false);
        for (int i = 0; i < myGroups.Length; i++) {
            GameObject item = Instantiate(container);

            item.transform.SetParent(content.transform, false);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;

            item.GetComponent<GroupIndex>().id = myGroups[i].id;
            item.transform.Find("InnerContainer/Title").GetComponent<Text>().text = myGroups[i].name;
            item.transform.Find("InnerContainer/District").GetComponent<Text>().text = myGroups[i].locationDistrict + " " + myGroups[i].locationCity;
            item.transform.Find("InnerContainer/Member").GetComponent<Text>().text = "멤버 " + myGroups[i].membersCount + "명";

            Button detailBtn = item.transform.Find("DetailButton").GetComponent<Button>();
            detailBtn.onClick.AddListener(() => showDetail(item));
        }
    }

    public void showDetail(GameObject obj) {
        sendReq(7, obj);
        Debug.Log("Show Detail");
    }

    private void removeAllList() {
        foreach(Transform child in content.transform) {
            Destroy(child.gameObject);
        }
    }

    public void offModal() {
        modal.SetActive(false);
    }
}
