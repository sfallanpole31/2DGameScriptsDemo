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

    //0�O�����B1�O�Ǫ����`�B2�O�L��
    public void PlaySfX(int id)
    {
        AudioS.PlayOneShot(AudioClips[id]);
    }

}
