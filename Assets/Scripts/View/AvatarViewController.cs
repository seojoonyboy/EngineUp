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
        GameManager gameManager = GameManager.Instance;
        string nickName = menuPanel.transform.Find("NickNameBox").GetComponent<UIInput>().value;
        //EditNickNameAction action = (EditNickNameAction)ActionCreator.createAction(ActionTypes.EDIT_NICKNAME);
        //action.nickname = nickName;
        //GameManager.Instance.gameDispatcher.dispatch(action);
        UserCreateAction action = ActionCreator.createAction(ActionTypes.USER_CREATE) as UserCreateAction;
        action.nickName = nickName;
        action.deviceId = gameManager.deviceId;
        gameManager.gameDispatcher.dispatch(action);
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

    public void CustomBtnClick(int type) {
        moveCam(type);
    }

    public void moveCam(int type) {
        int arrIndex = type;

        if(arrIndex != -1) {
            Camera.main.transform.position = avatarElementPos[arrIndex].transform.position;
        }
    }

    public void returnToOrigin() {
        //StopCoroutine("move");
        Camera.main.transform.position = cameraOriginPos;
    }

    //Camera 자연스러운 이동을 위한 code
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
