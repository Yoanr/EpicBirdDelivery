using FromTheBard.Lang;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarPanel : MonoBehaviour
{
    public string[] scenarKeys = new string[3];

    public Sprite[] scenarSprites = new Sprite[3];

    [SerializeField]
    private Text scenarText;

    [SerializeField]
    private Image scenarImage;

    [SerializeField]
    private GameObject level;

    private GameManager gameManager;

    private int state = 0;

    public void Awake()
    {
        state = 0;
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Next();
        SoundManager.PlayMusique("level");
    }

    public void Next()
    {
        if (state == scenarKeys.Length)
        {
            gameObject.SetActive(false);
            level.SetActive(true);
            gameManager.LaunchLevel();
            state = -1;
        }
        else
        {
            if (scenarKeys[state] != "")
            {
                scenarText.gameObject.SetActive(true);
                scenarText.text = Lang.Get(DictionariesEnum.All, scenarKeys[state]);
                scenarText.font = Lang.GetFont(FontsEnum.Main);
            }
            else
                scenarText.gameObject.SetActive(false);

            if (scenarSprites[state] != null)
            {
                scenarImage.gameObject.SetActive(true);
                scenarImage.sprite = scenarSprites[state];
            }
            else
                scenarImage.gameObject.SetActive(false);
        }
        state++;
    }
}