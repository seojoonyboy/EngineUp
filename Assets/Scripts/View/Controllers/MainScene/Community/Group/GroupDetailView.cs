using UnityEngine;
using System.Collections;

public class GroupDetailView : MonoBehaviour {
    public GroupViewController controller;
    public UILabel 
        groupName, 
        groupLocation,
        groupDesc,
        memberCount;
    // 이벤트 parameter를 생성하여 리턴.

    void OnEnable() {
        Group group = controller.groupStore.clickedGroup;
        groupName.text = group.name;
        groupLocation.text = group.locationDistrict + " " + group.locationCity;
        memberCount.text = group.membersCount + " 명";
        groupDesc.text = group.groupIntro;
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }
}
