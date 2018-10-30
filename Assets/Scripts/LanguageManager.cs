using FromTheBard.Lang;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public void FrenchButtonEffect()
    {
        Lang.SetLang(LanguagesEnum.Francais);
        SoundManager.PlayMusique("titleScreen");
    }

    public void EnglishButtonEffect()
    {
        Lang.SetLang(LanguagesEnum.English);
        SoundManager.PlayMusique("titleScreen");
    }
}