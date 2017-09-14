using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GroupMemberView : MonoBehaviour {
    //View Controller로부터 그룹의 id를 할당받는다.
    public GroupViewController controller;
    private GameManager gm;
    private Animator animator;

    public Member[] members;
    public GameObject container;

    public GameObject 
        content,
        ownerContainer;

    void Awake() {
        animator = GetComponent<Animator>();
        gm = GameManager.Instance;
    }

    void OnEnable() {
        Invoke("playSlideIn", 0.2f);
    }

    void playSlideIn() {
        animator.Play("SlideIn");
    }

    public void onBackButton() {
        animator.Play("SlideOut");
    }

    public void slideFinished(AnimationEvent animationEvent) {
        int boolParm = animationEvent.intParameter;

        //slider in
        if (boolParm == 1) {
            Group_getMemberAction act = ActionCreator.createAction(ActionTypes.GROUP_GET_MEMBERS) as Group_getMemberAction;
            act.id = controller.detailView.id;
            gm.gameDispatcher.dispatch(act);
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
    }

    public void makeList() {
        members = controller.groupStore.groupMembers;
        removeAllList();
        for (int i = 0; i < members.Length; i++) {
            if (members[i].memberState != "MB") {
                continue;
            }
            else {
                if (members[i].memberGrade == "GO") {
                    //item.transform.SetParent(owner_grid.transform);
                    ownerContainer.transform.Find("InnerContainer/Nickname").GetComponent<Text>().text = members[i].user.nickName;
                    //ownerContainer.transform.Find("InnerContainer/Dist").GetComponent<Text>().text = members[i].
                }
                else if (members[i].memberGrade == "GM") {
                    GameObject item = Instantiate(container);

                    item.transform.SetParent(content.transform, false);
                    item.transform.Find("InnerContainer/Nickname").GetComponent<Text>().text = members[i].user.nickName;
                }
            }
        }
        //containerInit();
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }

    private void removeAllList() {
        foreach(Transform child in content.transform) {
            if(child.tag == "locked") {
                continue;
            }
            else {
                Destroy(child.gameObject);
            }
        }
    }
}


