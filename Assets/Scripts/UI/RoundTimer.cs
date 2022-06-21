using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundTimer : MonoBehaviour
{
    public TMP_Text text;

    void Update()
    {
        switch (GameController.Instance.State)
        {
            case GameController.GameStates.Pause:
                return;
            case GameController.GameStates.Prepare:
                text.gameObject.SetActive(true);
                text.text = $"{(int) GameController.Instance.TimeToRound + 1}";
                break;
            default:
                text.gameObject.SetActive(false);
                break;
        }
    }
}