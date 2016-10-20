using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class AvatarViewCtrler : MonoBehaviour {
    public GameObject 
        selectPanel,
        menuPanel;

    public void OnSelectPanel() {
        selectPanel.SetActive(true);
    }

    public void OffSelectPanel() {
        selectPanel.SetActive(false);
    }

    public void OffMenuPanel() {
        menuPanel.SetActive(false);
    }
}
