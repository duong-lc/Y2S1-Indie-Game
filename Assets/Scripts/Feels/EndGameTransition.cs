using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Logging;
using DG.Tweening;
using TMPro;
using UnityEngine;
using EventType = Core.Events.EventType;

public class EndGameTransition : MonoBehaviour
{
    [Header("End Screen Values")]
    [SerializeField] private TMP_Text endScreen_Score;
    [SerializeField] private TMP_Text endScreen_Accuracy;
    [SerializeField] private TMP_Text endScreen_MaxCombo;
    [SerializeField] private TMP_Text endScreen_Perfect;
    [SerializeField] private TMP_Text endScreen_Early;
    [SerializeField] private TMP_Text endScreen_Late;
    [SerializeField] private TMP_Text endScreen_Miss;
    [SerializeField] private TMP_Text endScreen_Ratings;
    [SerializeField] private GameObject endScreen_HighScoreNotice;
    [SerializeField] private GameObject endScreen_FCNotice;

    [SerializeField] private GameObject button1;
    [SerializeField] private GameObject button2;
    
    public bool isHighScore;
    public bool isFullCombo;

    public float offScreenLocalY;
    public float inScreenLocalY;
    
    private void Awake()
    {
        this.AddListener(EventType.GameEndedEvent, param => UpdateEndGameProperties());
        this.AddListener(EventType.EndGameTransitionEvent, param => Transition());
    }

    private void Start()
    {
        isHighScore = false;
        isFullCombo = false;
        
        var hiddenPosition = transform.localPosition;
        transform.localPosition = new Vector3(hiddenPosition.x, offScreenLocalY, hiddenPosition.z);
    }
    
    private void Transition()
    {
        endScreen_HighScoreNotice.SetActive(false);
        endScreen_FCNotice.SetActive(false);
        endScreen_Accuracy.gameObject.SetActive(false);
        endScreen_MaxCombo.gameObject.SetActive(false);
        endScreen_Ratings.gameObject.SetActive(false);
        endScreen_Score.gameObject.SetActive(false);
        endScreen_Perfect.gameObject.SetActive(false);
        endScreen_Early.gameObject.SetActive(false);
        endScreen_Late.gameObject.SetActive(false);
        endScreen_Miss.gameObject.SetActive(false);
        

        GameSceneController.Instance.LoadEndScreenOverlay();
        //
        var mySequence = DOTween.Sequence().SetUpdate(UpdateType.Normal, true);;
        mySequence.Append(transform.DOLocalMoveY(inScreenLocalY, 1f).SetEase(Ease.Linear))
            .InsertCallback(1.5f, () => endScreen_Accuracy.gameObject.SetActive(true))
            .InsertCallback(2f, () => endScreen_MaxCombo.gameObject.SetActive(true))
            .InsertCallback(2.5f, () => endScreen_Perfect.gameObject.SetActive(true))
            .InsertCallback(2.5f, () => endScreen_Early.gameObject.SetActive(true))
            .InsertCallback(2.5f, () => endScreen_Late.gameObject.SetActive(true))
            .InsertCallback(2.5f, () => endScreen_Miss.gameObject.SetActive(true))
            .InsertCallback(3f, () => endScreen_Score.gameObject.SetActive(true));

        if(isHighScore)
            mySequence.InsertCallback(3.5f, () => endScreen_HighScoreNotice.SetActive(true));
        if(isFullCombo)
            mySequence.InsertCallback(3.5f, () => endScreen_FCNotice.gameObject.SetActive(true));

        mySequence.InsertCallback(3.5f, () => endScreen_Ratings.gameObject.SetActive(true))
            .InsertCallback(3.5f, () => button1.SetActive(true))
            .InsertCallback(3.5f, () => button2.SetActive(true));
        
        mySequence.Play();
    }

    private void UpdateEndGameProperties()
    {
        endScreen_Accuracy.text = $"Accuracy: " + ScoreManager.Instance.AccuracyFloat.ToString("#.##") + "%";
        endScreen_MaxCombo.text = $"Max Combo: {ScoreManager.Instance.MaxCombo}";
        endScreen_Ratings.text = $"{ScoreManager.Instance.GetRatings()}";
        endScreen_Score.text = $"{ScoreManager.Instance.CurrentScore}";
        endScreen_Perfect.text = $"Perfect: {ScoreManager.Instance.perfectHits}";
        endScreen_Early.text = $"Early: {ScoreManager.Instance.earlyHits}";
        endScreen_Late.text = $"Late: {ScoreManager.Instance.lateHits}";
        endScreen_Miss.text = $"Miss: {ScoreManager.Instance.missHits}";
    }
}
