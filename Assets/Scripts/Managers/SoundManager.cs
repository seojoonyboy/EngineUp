using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> {
    protected SoundManager() { }

    /// 효과음
    //0. 버튼 클릭음
    //1. 팝업
    //2. 튜토리얼 터치음
    //3. 아이템 판매음
    //4. 유저, 파트너 레벨업
    //5. 공구함 열기
    //6. 아이템 획득

    /// 배경음
    //
    public AudioClip[] 
        soundEffects,
        bgm;

    public int 
        isBGMON,
        isESON,
        isSilentMode;

    private AudioSource source;

    void Start() {
        source = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);

        isBGMON = PlayerPrefs.GetInt("BGM");
        isESON = PlayerPrefs.GetInt("ES");
        isSilentMode = PlayerPrefs.GetInt("SILENT");
    }

    public void playEffectSound(int index) {
        if (isESON == 0) {
            source.mute = true;
            AudioListener.volume = 0;
            //Debug.Log("효과음 무음");
        }
        else {
            resetSetting();
        }
        source.clip = soundEffects[index];
        source.PlayOneShot(soundEffects[index]);
    }

    public void playBGMSound(int index) {
        if (isBGMON == 0) {
            source.mute = true;
            AudioListener.volume = 0;
        }
        else {
            resetSetting();
        }
        source.clip = bgm[index];
        source.PlayOneShot(bgm[index]);
        //resetSetting();
    }

    public void setSoundSetting(int index, bool isOn = false) {
        switch(index) {
            case 0:
                if(isOn) {
                    isBGMON = 1;
                }
                else {
                    isBGMON = 0;
                }
                break;
            case 1:
                if (isOn) {
                    isESON = 1;
                }
                else {
                    isESON = 0;
                }
                break;
            case 2:
                
                break;
        }
    }

    void resetSetting() {
        source.mute = false;
        AudioListener.volume = 1;
    }
}
