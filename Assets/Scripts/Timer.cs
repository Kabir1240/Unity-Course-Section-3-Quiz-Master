using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] float timeToCompleteQuestion = 30f;
    [SerializeField] float timetoShowCorrectAnswer = 10f;

    public bool loadNextQuestion;
    public bool isAnsweringQuestion;
    public float fillFraction;

    float timerValue;
    void Update()
    {
        updateTimer();
    }

    public void CancelTimer()
    {
        timerValue = 0;
    }

    void updateTimer()
    {
        timerValue -= Time.deltaTime;
        // Debug.Log(timerValue + " " + isAnsweringQuestion);

        if (isAnsweringQuestion)
        {
            if (timerValue > 0)
            {
                fillFraction = timerValue / timeToCompleteQuestion;
            }

            else
            {
                timerValue = timetoShowCorrectAnswer;
                isAnsweringQuestion = false;
            }
        }

        else if (!isAnsweringQuestion)
        {
            if (timerValue > 0)
            {
                fillFraction = timerValue / timetoShowCorrectAnswer;
            }

            else
            {
                timerValue = timeToCompleteQuestion;
                isAnsweringQuestion = true;
                loadNextQuestion = true;
            }
        }
    }
}
