using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonShinanigans : MonoBehaviour
{
    public void Pressing(bool state){
        GetComponent<Animator>().SetBool("Pressing",true);
    }

    public void PlayAudio(int index){
        UIManager.Instance.PlaySoundPitch(index);
    }
}
