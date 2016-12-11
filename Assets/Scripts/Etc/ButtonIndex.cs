using UnityEngine;
using System.Collections;

public class ButtonIndex : MonoBehaviour {
    public int index = -1;
    public GameObject viewController;

    public void OnClick() {
        viewController.GetComponent<AvatarViewController>().CustomBtnClick(index);
    }
}
