using UnityEngine;
using System.Collections;

public class MainViewController : MonoBehaviour {
    public GameObject
        ridingPanel;

    public void onRidingPanel() {
        ridingPanel.SetActive(true);
    }
}