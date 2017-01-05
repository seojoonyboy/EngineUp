using UnityEngine;
using System.Collections;

public class Community_VC : MonoBehaviour {
    public User userStore;
    FeedViewController feedCtrler;
    FriendsViewController friendsCtrler;

    public void onUserListener() {
        initialize();
    }

    private void connectChildCtrler() {
        GameObject tmp = transform.Find("FeedPanel").gameObject;
        feedCtrler = tmp.GetComponent<FeedViewController>();
        tmp = transform.Find("FriendsPanel").gameObject;
        friendsCtrler = tmp.GetComponent<FriendsViewController>();
    }

    private void initialize() {
        Debug.Log("Listen User Store in Community_VC");
        connectChildCtrler();
        friendsCtrler.makeList();
        feedCtrler.makeList();
    }
}
