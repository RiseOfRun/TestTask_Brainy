using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.TestTools;

public class GameController : MonoBehaviour
{
    public Player FirstPlayer;
    public Player SecondPlayer;
    public static GameController Instance;
    public float ScoreToWin = 3;
    public Action StartRound;
    public float TimeToRound = 3;

    private float timeToRound = 0;
    
    public enum GameStates
    {
        Prepare,
        RoundIn,
        Pause,
        End
    }
    public GameStates State = GameStates.RoundIn;

    void Start()
    {
        Instance = this;
        StartGame();
    }
    void StartGame()
    {
        FirstPlayer.Score = 0;
        SecondPlayer.Score = 0;
    }

    public void PlayerHit(Player p)
    {
        int score = 0;
        if (FirstPlayer != p)
        {
            FirstPlayer.Score++;
            score = FirstPlayer.Score;
        }
        else
        {
            SecondPlayer.Score++;
            score = SecondPlayer.Score;  
        }
        
        if (score>=ScoreToWin)
        {
            State = GameStates.End;
        }
        else
        {
            State = GameStates.Prepare;
            timeToRound = TimeToRound;
        }
    }
    
    private void Update()
    {
        switch (State)
        {
            case GameStates.Prepare:
                if (timeToRound>0)
                {
                    timeToRound -= Time.deltaTime;
                }
                else
                {
                    StartRound?.Invoke();
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
