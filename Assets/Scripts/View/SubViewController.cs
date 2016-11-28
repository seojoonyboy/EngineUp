using UnityEngine;
using System.Collections;

public class SubViewController : MonoBehaviour {
    public GameObject
        resultPanel;

    void Start() {

    }

    public void onResultPanel() {
        resultPanel.SetActive(true);
    }
}
