using UnityEngine;
using System.Collections;

public class GroupSearchView : MonoBehaviour {
    public GroupViewController controller;

    public GameObject 
        container,
        emptyMessage;

    public UIGrid grid;
    // 이벤트 parameter를 생성하여 리턴.

    void OnEnable() {
        removeAllList();
        Group[] searchedGroups = controller.groupStore.searchedGroups;
        int length = searchedGroups.Length;
        if(length == 0) {
            emptyMessage.SetActive(true);
            return;
        }

        for(int i=0; i<length; i++) {
            GameObject item = Instantiate(container);

            item.transform.SetParent(grid.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;

            item.GetComponent<GroupIndex>().id = searchedGroups[i].id;
            item.transform.Find("LocationLabel").GetComponent<UILabel>().text = searchedGroups[i].locationDistrict + " " + searchedGroups[i].locationCity;
            item.transform.Find("GroupNameLabel").GetComponent<UILabel>().text = searchedGroups[i].name;
            item.transform.Find("MemberCountLabel").GetComponent<UILabel>().text = "멤버 " + searchedGroups[i].membersCount + "명";

            EventDelegate addEvent = new EventDelegate(this, "showDetail");
            GameObject detailBtn = item.transform.Find("GotoDetailBtn").gameObject;
            // 생성한 이벤트 parameter를 배열에 순서대로 입력.
            addEvent.parameters[0] = MakeParameter(detailBtn, typeof(GameObject));

            // m_BtnTest 변수와 연결 된 버튼에 이벤트 등록.
            EventDelegate.Add(detailBtn.GetComponent<UIButton>().onClick, addEvent);
        }
        containerInit();
    }

    private EventDelegate.Parameter MakeParameter(Object _value, System.Type _type) {
        EventDelegate.Parameter param = new EventDelegate.Parameter();  // 이벤트 parameter 생성.
        param.obj = _value;   // 이벤트 함수에 전달하고 싶은 값.
        param.expectedType = _type;    // 값의 타입.

        return param;
    }

    public void onBtnEvent(GameObject _obj, UIButton _btn) {
        Debug.Log("onBtnEvent = " + _obj.name);
        Debug.Log("onBtnEvent _btn = " + _btn.name);
    }

    void showDetail(GameObject _obj) {
        controller.onPanel(_obj);
    }

    void OnDisable() {
        emptyMessage.SetActive(false);
    }

    private void containerInit() {
        grid.repositionNow = true;
        grid.Reposition();
    }

    private void removeAllList() {
        grid.transform.DestroyChildren();
    }

    public void OffPanel() {
        gameObject.SetActive(false);
    }
}