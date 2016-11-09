using UnityEngine;
using System.Collections;

public class MainViewController : MonoBehaviour {
    public GameObject
        ridingPanel;

    public void onRidingPanel() {
        ridingPanel.SetActive(true);
    }

    public void offRidingPanel() {
        ridingPanel.SetActive(false);
    }
}