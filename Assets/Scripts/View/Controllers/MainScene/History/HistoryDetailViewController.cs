using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryDetailViewController : MonoBehaviour {
    public BoxCollider[] collider;
    public GameObject map;
    private Vector2 preMapScale;

    //지도 사용을 위해 다른 collider를 꺼준다.
    void OnEnable() {
        foreach(Collider col in collider) {
            col.enabled = false;
        }
        setMap();
        map.SetActive(true);
    }

    void OnDisable() {
        foreach(Collider col in collider) {
            col.enabled = true;
        }
        map.GetComponent<OnlineMaps>().tilesetSize = preMapScale;
    }

    void setMap() {
        preMapScale = map.GetComponent<OnlineMaps>().tilesetSize;
        map.GetComponent<OnlineMaps>().tilesetSize = new Vector2(540, 600);
    }

}
