using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class AvatarViewController : MonoBehaviour {
    public GameObject 
        selectPanel,
        menuPanel;

    public Transform[] avatarElementPos;
    private Vector3 cameraOriginPos;
    private float 
        startTime,
        speed = 3f;

    private bool canZoom = true;

    void Start() {
        cameraOriginPos = Camera.main.transform.position;
        startTime = Time.time;
        Debug.Log(startTime);
    }

    public void OnConfirm() {
        string nickName = menuPanel.transform.Find("NickNameBox").GetComponent<UIInput>().value;
        EditNickNameAction action = (EditNickNameAction)ActionCreator.createAction(ActionTypes.EDIT_NICKNAME);
        action.nickname = nickName;
        GameManager.Instance.gameDispatcher.dispatch(action);
        //Debug.Log(GameManager.Instance.userStore.nickName);
    }

    public void OnSelectPanel() {
        selectPanel.SetActive(true);
    }

    public void OffSelectPanel() {
        selectPanel.SetActive(false);
    }

    public void OffMenuPanel() {
        menuPanel.SetActive(false);
    }

    public void moveToHair() {
        StartCoroutine("move", avatarElementPos[0].transform.position);
    }

    public void moveToHead() {
        StartCoroutine("move", avatarElementPos[1].transform.position);
    }

    public void moveToBody() {
        StartCoroutine("move", avatarElementPos[2].transform.position);
        Debug.Log("endPos : " + avatarElementPos[2].transform.position);
    }

    public void returnToOrigin() {
        StopCoroutine("move");
        Camera.main.transform.position = cameraOriginPos;
    }

    IEnumerator move(Vector3 endPos) {
        float journeyLength = Vector3.Distance(cameraOriginPos, endPos);
        endPos.z -= 1f;
        float _time = 0;
        Debug.Log("endPos : " + endPos);
        while (canZoom) {
            _time += Time.deltaTime;
            float disCovered = (_time - startTime) * speed;
            float fracJourney = disCovered / journeyLength;
            Camera.main.transform.position = Vector3.Lerp(cameraOriginPos, endPos, fracJourney);
            if (Camera.main.transform.position.z >= endPos.z) {
                canZoom = false;
            }
            yield return null;
        }
        canZoom = true;
    }
}
