using UnityEngine;
using System.Collections;

public class Community_VC : MonoBehaviour {
    public User userStore;
    public FeedViewController feedCtrler;
    public FriendsViewController friendsCtrler;

    public void onUserListener() {
        if(userStore.eventType == ActionTypes.GET_COMMUNITY_DATA) {
            if (!userStore.isSearch) {
                makeCommunityList();
                return;
            }

            if(userStore.community_req_type == GetCommunityAction.requestType.FRIENDS) {
                friendsCtrler.onSerchResult();
            }
            else if(userStore.community_req_type == GetCommunityAction.requestType.GROUP) {

            }
        }
    }

    private void makeCommunityList() {
        Debug.Log("Listen User Store in Community_VC");
        friendsCtrler.makeList();
        feedCtrler.makeList();
    }

    private void makeSearchedList() {

    }
}
