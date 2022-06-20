using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public int LogCount;
    public float LogRate;
    public TMP_Text Text;
    [HideInInspector] public List<Actions> PerformedActions;

    public enum Actions
    {
        Move,
        Rotate,
        Shoot
    }

    private readonly Queue<string> messages = new Queue<string>();
    private float timeToLog;

    void Start()
    {
        timeToLog = LogRate;
    }

    void Update()
    {
        timeToLog -= Time.deltaTime;
        if (timeToLog <= 0)
        {
            PushMessages();
        }
    }

    public void PushMessages()
    {
        timeToLog = LogRate;
        if (PerformedActions.Count == 0)
        {
            return;
        }

        List<string> words = new List<string>();
        if (PerformedActions.Contains(Actions.Move))
        {
            words.Add("Move");
        }

        if (PerformedActions.Contains(Actions.Rotate))
        {
            words.Add("Rotate");
        }

        if (PerformedActions.Contains(Actions.Shoot))
        {
            int shootCount = PerformedActions.Count(x => x == Actions.Shoot);
            string word = "Shoot";
            if (shootCount > 1)
            {
                word += $" {shootCount} times";
            }

            words.Add(word);
        }

        PerformedActions.Clear();
        words[words.Count - 1] += "\n";
        if (words.Count > 1)
        {
            words[words.Count - 1] = "and " + words[words.Count - 1];
        }

        string message = String.Join(" ", words);
        if (messages.Count < LogCount)
        {
            messages.Enqueue(message);
        }
        else
        {
            messages.Dequeue();
            messages.Enqueue(message);
        }

        Text.text = "";
        foreach (var m in messages)
        {
            Text.text += m;
        }
    }
}