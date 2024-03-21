using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class Quiz : MonoBehaviour
{
    [Header("Question")]
    QuestionSO currentQuestion;                                                 // scriptable object to store questions and answers
    [SerializeField]List<QuestionSO> questions = new List<QuestionSO>();
    List<string> insults = new List<string>();
    [SerializeField] TextMeshProUGUI questionText;                              // text field to display question

    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons;                                // list of buttons to store answers
    int correctAnswerIndex;
    bool hasAnsweredEarly = false;
    bool answerShown = false;

    [Header("Button Colors")]
    [SerializeField] Sprite correctAnswerSprite;
    [SerializeField] Sprite defaultAnswerSprite;

    [Header("Timer")]
    [SerializeField] Image timerImage;
    [SerializeField] Timer timer;

    [Header("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    [Header("Progress Bar")]
    [SerializeField] Slider progressBar;
    bool isComplete = false;

    void Awake()
    {
        // update question on the canvas
        AddInsults();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        progressBar.maxValue = questions.Count;
        progressBar.minValue = 0;
    }

    void Update()
    {
        UpdateTimerImage();
        if (timer.loadNextQuestion)
        {
            GetNextQuestion();
            timer.loadNextQuestion = false;
        }
        else if (!hasAnsweredEarly && !timer.isAnsweringQuestion)
        {
            if (!answerShown)
            {
                DisplayAnswer(-1);
            }
            SetButtonState(false);
        }
    }

    void UpdateTimerImage()
    {
        timerImage.fillAmount = timer.fillFraction;
    }

    private void AddInsults()
    {
        insults.Add("you suck!");
        insults.Add("my grandma could answer that,");
        insults.Add("wrong as expected,");
        insults.Add("surely you're joking,");
        insults.Add("...");
    }

    private string RandomInsultGenerator()
    {
        int randomIndex = Random.Range(1, insults.Count);
        return insults[randomIndex];
    }

    public void OnAnswerSelected(int index)
    {
        hasAnsweredEarly = true;
        DisplayAnswer(index);
        SetButtonState(false);
        timer.CancelTimer();
    }

    void DisplayAnswer(int index)
    {
        correctAnswerIndex = currentQuestion.GetCorrectAnswerIndex();
        Image buttonImage;
        if (index == correctAnswerIndex)
        {
            questionText.text = "eh, that was an easy one";
            buttonImage = answerButtons[index].GetComponent<Image>();
            buttonImage.sprite = correctAnswerSprite;
            scoreKeeper.IncremenentCorrectAnswer();
        }
        else
        {
            questionText.text = RandomInsultGenerator() + " the answer was: \n" + currentQuestion.GetAnswer(correctAnswerIndex);
            buttonImage = answerButtons[correctAnswerIndex].GetComponent<Image>();
            buttonImage.sprite = correctAnswerSprite;
        }
        answerShown = true;
        scoreText.text = "Score: " + scoreKeeper.calculateScore() + "%";
        //UpdateIsCompleteCheck();
    }

    void UpdateIsCompleteCheck() 
    {
        if (progressBar.value == progressBar.maxValue)
        {
            isComplete = true;
        }
    }

    void GetNextQuestion()
    {
        UpdateIsCompleteCheck();
        if (!isComplete)
        {
            UpdateValues();
            GetRandomQuestion();
            DisplayQuestion();
        }
    }

    private void UpdateValues()
    {
        hasAnsweredEarly = false;
        answerShown = false;
        SetButtonState(true);
        scoreKeeper.IncrementQuestionsSeen();
        SetDefaultButonSprites();
        progressBar.value++;
    }

    private void GetRandomQuestion()
    {
        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];

        if (questions.Contains(currentQuestion))
        {
            questions.Remove(currentQuestion);
        }
        else
        {
            throw new KeyNotFoundException();
        }
        
    }

    void DisplayQuestion()
    {
        questionText.text = currentQuestion.GetQuestion();

        TextMeshProUGUI buttonText;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = currentQuestion.GetAnswer(i);
        }
    }

    void SetButtonState(bool state)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            Button button = answerButtons[i].GetComponent<Button>();
            button.interactable = state;
        }
    }

    void SetDefaultButonSprites()
    {
        Image buttonImage;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            buttonImage = answerButtons[i].GetComponent<Image>();
            buttonImage.sprite = defaultAnswerSprite;
        }
    }

    public bool IsComplete() 
    {
        return isComplete;
    }
}
