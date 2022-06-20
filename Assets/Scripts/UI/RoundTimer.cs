using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundTimer : MonoBehaviour
{
    public TMP_Text text;

    void Update()
    {
        if (GameController.Instance.State == GameController.GameStates.Prepare)
        {
            text.gameObject.SetActive(true);
            text.text = $"{(int) GameController.Instance.TimeToRound + 1}";
        }
        else
        {
            text.gameObject.SetActive(false);
        }
    }
}