using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float clock = 0.0f;
    public static int hp = 3;

    public static bool hasLost = false;

    public static float highScore = 0.0f;

    [SerializeField]
    private Text strclock;

    [SerializeField]
    private Text strhp;

    [SerializeField]
    private GameController player1;

    [SerializeField]
    private GameController player2;

    [SerializeField]
    private Slider staminaSlider1;

    [SerializeField]
    private Slider staminaSlider2;

    [SerializeField]
    private GameObject gameOverPopUp;

    [SerializeField]
    private GameObject Level;

    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private Text secondTextTime;

    [SerializeField]
    private Text highScoreText;

    [SerializeField]
    private GameObject playerLINK;

    private bool levelRunning = false;

    // Update is called once per frame
    private void Update()
    {
        if (levelRunning)
        {
            if (hp <= 0 || player1.stamina <= 0 || player2.stamina <= 0)
            {
                Lose();
            }
            clock += Time.deltaTime;
            UpdateHUD();
            UpdateStamina();
        }
    }

    private void UpdateHUD()
    {
        strclock.text = clock.ToString(".00");
        strhp.text = hp.ToString("0.");
    }

    private void UpdateStamina()
    {
        staminaSlider1.value = player1.stamina;
        staminaSlider2.value = player2.stamina;
    }

    public void LaunchLevel()
    {
        clock = 0;
        hp = 3;
        player1.Initialize();
        player2.Initialize();
        levelManager.Initialize();
        playerLINK.SetActive(false);

        levelRunning = true;
        UpdateHUD();
        UpdateStamina();
    }

    private void Lose()
    {
        if (!hasLost)
        {
            secondTextTime.text = strclock.text;
            float currentScore = float.Parse(strclock.text);
            if (currentScore > highScore)
                highScore = currentScore;
            highScoreText.text = highScore.ToString(".00");
            SoundManager.PlayBruitage("lose");
            hasLost = true;
            StartCoroutine(LoseCoroutine());
        }
    }

    private IEnumerator LoseCoroutine()
    {
        yield return new WaitForSeconds(3);
        levelRunning = false;
        Level.SetActive(false);
        gameOverPopUp.SetActive(true);
        hasLost = false;
    }
}