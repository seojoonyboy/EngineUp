using UnityEngine;
using System.Collections;

public class CustomizingController : MonoBehaviour {
    public GameObject avatar;
    public GameObject[] 
        charParts,
        bicycleParts;
    public Material[] mats;

    public void SetParts(int index) {
        charParts[index].GetComponent<Renderer>().material = mats[1];
    }

    public void ButtonMessage(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        SetParts(index);
    }
}
