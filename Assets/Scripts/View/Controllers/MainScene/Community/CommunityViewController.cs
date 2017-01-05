using UnityEngine;
using System.Collections;

public class CommunityViewController : MonoBehaviour {
    FeedViewController feedCtrler;
    FriendsViewController friendsCtrler;

    void Start() {
        connectChildCtrler();
        initialize();
    }

    private void connectChildCtrler() {
        GameObject tmp = transform.Find("FeedPanel").gameObject;
        feedCtrler = tmp.GetComponent<FeedViewController>();
        tmp = transform.Find("FriendsPanel").gameObject;
        friendsCtrler = tmp.GetComponent<FriendsViewController>();
    }

    private void initialize() {
        friendsCtrler.makeList();
    }
}
