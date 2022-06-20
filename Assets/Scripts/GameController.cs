using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TestTools;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public Action StartRound;
    [Header("dependencies")]
    public Character FirstCharacter;
    public Character SecondCharacter;
    public EndGamePanel EndGamePanel;
    [Header("settings")]
    public float ScoreToWin = 3;
    public float TimeBetweenRounds;
    [HideInInspector] public float TimeToRound;

    public enum GameStates
    {
        Prepare,
        RoundIn,
        Pause,
        End
    }
    public GameStates State
    {
        get => state;
        set
        {
            state = value;
            switch (value)
            {
                case GameStates.Prepare:
                    TimeToRound = TimeBetweenRounds;
                    FirstCharacter.gameObject.SetActive(true);
                    SecondCharacter.gameObject.SetActive(true);
                    StartRound?.Invoke();
                    return;
                case GameStates.End:
                    EndGame();
                    return;
            }
        }
    }
    private GameStates state;
    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.fixedUnscaledDeltaTime;
        StartGame();
    }

    public void StartGame()
    {
        FirstCharacter.Score = 0;
        SecondCharacter.Score = 0;
        State = GameStates.Prepare;
    }
    
    public void PlayerHit(Character p)
    {
        if (state!=GameStates.RoundIn)
        {
            return;
        }
        int score = 0;
        if (FirstCharacter != p)
        {
            FirstCharacter.Score++;
            score = FirstCharacter.Score;
        }
        else
        {
            SecondCharacter.Score++;
            score = SecondCharacter.Score;
        }

        if (score >= ScoreToWin)
        {
            State = GameStates.End;
        }
        else
        {
            State = GameStates.Prepare;
        }
    }

    void EndGame()
    {
        string winner = FirstCharacter.Score > SecondCharacter.Score ? "Player1" : "Player2";
        EndGamePanel.Winner = winner;
        EndGamePanel.gameObject.SetActive(true);
    }

    private void Update()
    {
        switch (State)
        {
            case GameStates.Prepare:
                if (TimeToRound > 0)
                {
                    TimeToRound -= Time.deltaTime;
                }
                else
                {
                    State = GameStates.RoundIn;
                }

                break;
            case GameStates.RoundIn:
                break;
            case GameStates.Pause:
                break;
            case GameStates.End:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}