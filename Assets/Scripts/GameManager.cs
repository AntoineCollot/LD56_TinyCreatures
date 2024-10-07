using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public bool gameIsOver { get; private set; }
    public bool gameHasStarted { get; private set; }
    public bool GameIsPlaying => !gameIsOver && gameHasStarted;
    public bool autoStart = true;

    public float gameTime;

    public UnityEvent onGameStart = new UnityEvent();
    public UnityEvent onGameOver = new UnityEvent();
    public UnityEvent onGameWin = new UnityEvent();

    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
        if (autoStart)
            StartGame();
    }

    private void Update()
    {
        if (GameIsPlaying)
            gameTime += Time.deltaTime;
    }

    public void StartGame()
    {
        if (gameHasStarted)
            return;
        gameHasStarted = true;
        onGameStart.Invoke();

        SFXManager.PlaySound(GlobalSFX.DefendTheCheese);
    }

    public void GameOver()
    {
        if (gameIsOver)
            return;
        gameIsOver = true;
        onGameOver.Invoke();
        SFXManager.PlaySound(GlobalSFX.GameOver);
    }

    public void ClearLevel()
    {
        if (gameIsOver)
            return;

        gameIsOver = true;
        onGameWin.Invoke();
    }
}
