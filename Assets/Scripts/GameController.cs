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
    [FormerlySerializedAs("FirstPlayer")] [Header("dependencies")]
    public Character FirstCharacter;
    [FormerlySerializedAs("SecondPlayer")] public Character SecondCharacter;
    public EndGamePanel EndGamePanel;
    [Header("settings")]
    public float ScoreToWin = 3;
    public float TimeBetweenRounds = 3;
    [HideInInspector] public float TimeToRound = 0;

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
        private set
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
                case GameStates.RoundIn:
                    return;
            }
        }
    }

    private GameStates state = GameStates.RoundIn;

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
        StartGame();
    }

    public void StartGame()
    {
        FirstCharacter.Score = 0;
        SecondCharacter.Score = 0;
        FirstCharacter.gameObject.SetActive(true);
        SecondCharacter.gameObject.SetActive(true);
        state = GameStates.Prepare;
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