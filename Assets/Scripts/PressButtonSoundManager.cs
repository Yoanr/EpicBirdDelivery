using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButtonSoundManager : MonoBehaviour
{
    public void PressButton()
    {
        SoundManager.PlayBruitage("buttonPress");
    }
}