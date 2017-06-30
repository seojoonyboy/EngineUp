using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideMove : MonoBehaviour {
    SoundManager sm;
    public float speed = 1f;

    private bool 
        isOn = false,
        isFinished = false;
    Vector3 leftPos = new Vector3(-122, 0, 0);
    Vector3 rightPos = new Vector3(122, 0, 0);
    float time;
    UILabel label;

    private int index = -1;
    void Awake() {
        sm = SoundManager.Instance;
        label = gameObject.transform.Find("InnerImage/Label").GetComponent<UILabel>();
    }

    public void onPressed() {
        if(!isFinished) {
            isOn = GetComponent<boolIndex>().isOn;
            time = 0;
            StartCoroutine("Slide");
        }
    }

    void OnEnable() {
        index = GetComponent<ButtonIndex>().index;

        switch (index) {
            case 0:
                int isBGMOn = PlayerPrefs.GetInt("BGM");
                if (isBGMOn == 1) {
                    transform.localPosition = leftPos;
                    label.text = "ON";
                    GetComponent<boolIndex>().isOn = true;
                }
                else {
                    transform.localPosition = rightPos;
                    label.text = "OFF";
                    GetComponent<boolIndex>().isOn = false;
                }
                break;
            case 1:
                int isESOn = PlayerPrefs.GetInt("ES");
                if (isESOn == 1) {
                    transform.localPosition = leftPos;
                    label.text = "ON";
                    GetComponent<boolIndex>().isOn = true;
                }
                else {
                    transform.localPosition = rightPos;
                    label.text = "OFF";
                    GetComponent<boolIndex>().isOn = false;
                }
                break;
            case 2:
                int isSilent = PlayerPrefs.GetInt("SILENT");
                if (isSilent == 1) {
                    transform.localPosition = leftPos;
                    label.text = "ON";
                    GetComponent<boolIndex>().isOn = true;
                }
                else {
                    transform.localPosition = rightPos;
                    label.text = "OFF";
                    GetComponent<boolIndex>().isOn = false;
                }
                break;
        }
    }

    IEnumerator Slide() {
        while (!isFinished) {
            Vector3 pos = transform.localPosition;
            time += Time.deltaTime;

            if (!isOn) {
                switch (index) {
                    case 0:
                        PlayerPrefs.SetInt("BGM", 1);
                        break;
                    case 1:
                        PlayerPrefs.SetInt("ES", 1);
                        Debug.Log("효과음 활성화 요청");
                        break;
                    case 2:
                        PlayerPrefs.SetInt("SILENT", 1);
                        break;
                }
                sm.setSoundSetting(index, true);

                transform.localPosition = Vector3.Lerp(rightPos, leftPos, time * speed);
                if (pos.x <= leftPos.x) {
                    StopCoroutine("Slide");
                    GetComponent<boolIndex>().isOn = true;
                    label.text = "ON";
                }
            }
            else {
                transform.localPosition = Vector3.Lerp(leftPos, rightPos, time * speed);
                switch (index) {
                    case 0:
                        PlayerPrefs.SetInt("BGM", 0);
                        break;
                    case 1:
                        PlayerPrefs.SetInt("ES", 0);
                        Debug.Log("효과음 비활성화 요청");
                        break;
                    case 2:
                        PlayerPrefs.SetInt("SILENT", 0);
                        break;
                }
                sm.setSoundSetting(index, false);

                if (pos.x >= rightPos.x) {
                    StopCoroutine("Slide");
                    GetComponent<boolIndex>().isOn = false;
                    label.text = "OFF";
                }
                
            }
            yield return 0;
        }
    }
}