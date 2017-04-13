using UnityEngine;

public class StatViewController : MonoBehaviour {
    public UILabel nickNameLabel;
    public Riding ridingStore;
    public User userStore;

    public UILabel[] 
        stats,
        profiles,
        totalRidings,
        monthlyRidings;

    private GameManager gm;

    void Awake() {
        gm = GameManager.Instance;
    }

    void OnEnable() {
        initialize();

    }

    public void onUserListener() {
        nickNameLabel.text = userStore.nickName;
        initialize();
    }

    void Start() {
        nickNameLabel.text = GameManager.Instance.userStore.nickName;
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }

    private void initialize() {
        setProfile();
        setStats();
        setTotalRidingRecord();
        setMonthlyRidingRecord();
    }

    private void setProfile() {
        UserData data = userStore.myData;
        profiles[0].text = data.nickName;

        status statData = data.status;
        profiles[1].text = statData.rank.ToString();
        //그룹

        stats[0].text = statData.strength.ToString();
        stats[1].text = statData.speed.ToString();
        stats[2].text = statData.endurance.ToString();
        stats[3].text = statData.regeneration.ToString();
    }

    private void setStats() {
        
    }

    private void setTotalRidingRecord() {
        
    }

    private void setMonthlyRidingRecord() {
        
    }
}