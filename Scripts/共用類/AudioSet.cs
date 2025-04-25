using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSet : MonoBehaviour
{
    AudioSource AudioS;
    public AudioClip[] AudioClips;

    // Start is called before the first frame update
    void Start()
    {

        AudioS = GetComponent<AudioSource>();
    }

    //0是攻擊、1是怪物死亡、2是過關
    public void PlaySfX(int id)
    {
        AudioS.PlayOneShot(AudioClips[id]);
    }

}
