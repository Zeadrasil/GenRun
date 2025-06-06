using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour, ISubject<(int, int, KeyCode)>
{
    float runTimer = 0;
    [SerializeField] private BoolVariable paused;
    [SerializeField] private BoolVariable notInMenus;
    [SerializeField] private VoidEvent startEvent = null;
    [SerializeField] private VoidEvent dieEvent = null;
    [SerializeField] private VoidEvent generateEvent = null;
    [SerializeField] private VoidEvent pauseEvent = null;
    [SerializeField] private KeyCodeVariable pauseButton = null;
    [SerializeField] private Canvas pauseMenuScreen = null;
    [SerializeField] private Canvas mainMenuScreen = null;
    [SerializeField] private Canvas controlsMenuScreen = null;
    [SerializeField] private Canvas deathScreen = null;
    [SerializeField] private Canvas HUD = null;
    [SerializeField] private TMP_Text remainingTime = null;
    [SerializeField] private TMP_Text currentScore = null;
    [SerializeField] private TMP_Text finalScore = null;
    [SerializeField] private TMP_Text highScore = null;
    [SerializeField] private TMP_Text highScoreHUD = null;
    [SerializeField] private TMP_Text updatePause = null;
    [SerializeField] private TMP_Text updateLeft1 = null;
    [SerializeField] private TMP_Text updateRight1 = null;
    [SerializeField] private TMP_Text updateJump1 = null;
    [SerializeField] private TMP_Text updateGrapple1 = null;
    [SerializeField] private TMP_Text updateLeft2 = null;
    [SerializeField] private TMP_Text updateRight2 = null;
    [SerializeField] private TMP_Text updateJump2 = null;
    [SerializeField] private TMP_Text updateGrapple2 = null;
    [SerializeField] private KeyCodeVariable left1 = null;
    [SerializeField] private KeyCodeVariable right1 = null;
    [SerializeField] private KeyCodeVariable jump1 = null;
    [SerializeField] private KeyCodeVariable grapple1 = null;
    [SerializeField] private KeyCodeVariable left2 = null;
    [SerializeField] private KeyCodeVariable right2 = null;
    [SerializeField] private KeyCodeVariable jump2 = null;
    [SerializeField] private KeyCodeVariable grapple2 = null;
    private bool multiplayer = false;
    private int generatedTiles = -1;
    int controlToUpdate = -1;
    bool active = false;
    void Start()
    {
        pauseButton.value = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pause", KeyCode.Escape.ToString()));
        left1.value = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left1", KeyCode.LeftArrow.ToString()));
        Publish((0, 0, left1.value));
        left2.value = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left2", KeyCode.A.ToString()));
        Publish((1, 0, left2.value));
        right1.value = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right1", KeyCode.RightArrow.ToString()));
        Publish((0, 1, right1.value));
        right2.value = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right2", KeyCode.D.ToString()));
        Publish((1, 1, right2.value));
        jump1.value = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Jump1", KeyCode.Space.ToString()));
        Publish((0, 2, jump1.value));
        jump2.value = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Jump2", KeyCode.W.ToString()));
        Publish((1, 2, jump2.value));
        grapple1.value = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Grapple1", KeyCode.C.ToString()));
        Publish((0, 3, grapple1.value));
        grapple2.value = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Grapple2", KeyCode.J.ToString()));
        Publish((1, 3, grapple2.value));
        generateEvent.Subscribe(generateSection);
        dieEvent.Subscribe(onDeath);
    }

    void Update()
    {
        if (notInMenus)
        {
            if (!paused && active)
            {
                runTimer -= Time.deltaTime;
                remainingTime.text = $"{(runTimer):F2}";
                if (runTimer < 0)
                {
                    dieEvent.RaiseEvent();
                }
                if (Input.GetKeyDown(pauseButton.value))
                {
                    paused.value = true;
                    pauseMenuScreen.gameObject.SetActive(true);
                    pauseEvent.RaiseEvent();
                }
            }
            else if (active)
            {
                if (Input.GetKeyDown(pauseButton.value))
                {
                    unpause();
                }
            }
        }
        else if (controlToUpdate != -1)
        {

            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    switch (controlToUpdate)
                    {
                        case 0:
                            pauseButton.value = keyCode;
                            PlayerPrefs.SetString("Pause", keyCode.ToString());
                            updatePause.text = keyCode.ToString();
                            break;
                        case 1:
                            left1.value = keyCode;
                            PlayerPrefs.SetString("Left1", keyCode.ToString());
                            updateLeft1.text = keyCode.ToString();
                            break;
                        case 2:
                            right1.value = keyCode;
                            PlayerPrefs.SetString("Right1", keyCode.ToString());
                            updateRight1.text = keyCode.ToString();
                            break;
                        case 3:
                            jump1.value = keyCode;
                            PlayerPrefs.SetString("Jump1", keyCode.ToString());
                            updateJump1.text = keyCode.ToString();
                            break;
                        case 4:
                            grapple1.value = keyCode;
                            PlayerPrefs.SetString("Grapple1", keyCode.ToString());
                            updateGrapple1.text = keyCode.ToString();
                            break;
                        case 5:
                            left2.value = keyCode;
                            PlayerPrefs.SetString("Left2", keyCode.ToString());
                            updateLeft2.text = keyCode.ToString();
                            break;
                        case 6:
                            right2.value = keyCode;
                            PlayerPrefs.SetString("Right2", keyCode.ToString());
                            updateRight2.text = keyCode.ToString();
                            break;
                        case 7:
                            jump2.value = keyCode;
                            PlayerPrefs.SetString("Jump2", keyCode.ToString());
                            updateJump2.text = keyCode.ToString();
                            break;
                        case 8:
                            grapple2.value = keyCode;
                            PlayerPrefs.SetString("Grapple2", keyCode.ToString());
                            updateGrapple2.text = keyCode.ToString();
                            break;
                    }
                    Publish(((controlToUpdate - 1) / 4, (controlToUpdate - 1) % 4, keyCode));
                    controlToUpdate = -1;
                    break;
                }
            }
        }
    }

    public void startGameSingleplayer()
    {
        dieEvent.RaiseEvent();
        generatedTiles = -1;
        runTimer = 0;
        paused.value = false;
        notInMenus.value = true;
        multiplayer = false;
        HUD.gameObject.SetActive(true);
        mainMenuScreen.gameObject.SetActive(false);
        deathScreen.gameObject.SetActive(false);
        pauseMenuScreen.gameObject.SetActive(false);
        highScoreHUD.text = (PlayerPrefs.GetFloat("HighScore", 0f) * 100).ToString();
        startEvent.RaiseEvent();
    }

    public void startGameMultiplayer()
    {

    }

    public void unpause()
    {
        paused.value = false;
        pauseMenuScreen.gameObject.SetActive(false);
        pauseEvent.RaiseEvent();
    }

    public void mainMenu()
    {
        if (controlToUpdate == -1)
        {
            notInMenus.value = false;
            pauseMenuScreen.gameObject.SetActive(false);
            controlsMenuScreen.gameObject.SetActive(false);
            deathScreen.gameObject.SetActive(false);
            HUD.gameObject.SetActive(false);
            mainMenuScreen.gameObject.SetActive(true);
        }
    }

    public void quit()
    {
        Application.Quit();
    }

    private void generateSection()
    {
        active = true;
        generatedTiles++;
        runTimer += 15 * (1.0f - (generatedTiles * 0.001f));
        currentScore.text = (generatedTiles * 100).ToString();
    }

    public void restart()
    {
        if(multiplayer)
        {
            startGameMultiplayer();
        }
        else
        {
            startGameSingleplayer();
        }

    }

    public void toControls()
    {
        controlsMenuScreen.gameObject.SetActive(true);
        mainMenuScreen.gameObject.SetActive(false);
        updatePause.text = pauseButton.value.ToString();
        updateLeft1.text = left1.value.ToString();
        updateRight1.text = right1.value.ToString();
        updateJump1.text = jump1.value.ToString();
        updateGrapple1.text = grapple1.value.ToString();
        updateLeft2.text = left2.value.ToString();
        updateRight2.text = right2.value.ToString();
        updateJump2.text = jump2.value.ToString();
        updateGrapple2.text = grapple2.value.ToString();
    }

    private void onDeath()
    {
        if(PlayerPrefs.GetFloat("HighScore", 0f) < generatedTiles)
        {
            PlayerPrefs.SetFloat("HighScore", generatedTiles);
            highScore.text = generatedTiles * 100 + " New High Score!";
        }
        else
        {
            highScore.text = (PlayerPrefs.GetFloat("HighScore", 0f) * 100).ToString();
        }
        finalScore.text = (generatedTiles * 100).ToString();
        deathScreen.gameObject.SetActive(true);
        active = false;
        notInMenus.value = false;
        HUD.gameObject.SetActive(false);
    }

    public void updatePauseButton()
    {
        controlToUpdate = 0;
        updatePause.text = "Press any key";
    }
    public void updateLeft1Button()
    {
        controlToUpdate = 1;
        updateLeft1.text = "Press any key";
    }
    public void updateRight1Button()
    {
        controlToUpdate = 2;
        updateRight1.text = "Press any key";
    }
    public void updateJump1Button()
    {
        controlToUpdate = 3;
        updateJump1.text = "Press any key";
    }
    public void updateGrapple1Button()
    {
        controlToUpdate = 4;
        updateGrapple1.text = "Press any key";
    }
    public void updateLeft2Button()
    {
        controlToUpdate = 5;
        updateLeft2.text = "Press any key";
    }
    public void updateRight2Button()
    {
        controlToUpdate = 6;
        updateRight2.text = "Press any key";
    }
    public void updateJump2Button()
    {
        controlToUpdate = 7;
        updateJump2.text = "Press any key";
    }
    public void updateGrapple2Button()
    {
        controlToUpdate = 8;
        updateGrapple2.text = "Press any key";
    }

    private List<IObserver<(int, int, KeyCode)>> observers = new();

    public void Subscribe(IObserver<(int, int, KeyCode)> observer)
    {
        if(!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void Unsubscribe(IObserver<(int, int, KeyCode)> observer)
    {
        observers.Remove(observer);
    }

    public void Publish((int, int, KeyCode) value)
    {
        foreach(IObserver<(int, int, KeyCode)> observer in observers)
        {
            observer.Observe(value);
        }
    }
}
