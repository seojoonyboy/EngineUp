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

    private AudioSource source;

    void Start() {
        source = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void playEffectSound(int index) {
        source.clip = soundEffects[index];
        source.PlayOneShot(soundEffects[index]);
    }
}
