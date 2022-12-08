using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    float timer = 0;
    bool raceEnded = true;
    bool timerStarted = false;
    [SerializeField] TextMeshProUGUI timerText;
    string format = @"ss\.fff";

    public static GameManager instance;

    [Header("REFS")]
    [SerializeField] BikeHandling bHandling;
    [SerializeField] BikeMovement bMove;

    AudioSource _audioSource;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        timerStarted = false;
        raceEnded = false;
        lostMatch = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)){
            if(!GameManager.instance.RunRunning()) return;
            RestartRun(); 
        }
    }

    void FixedUpdate()
    {
        if(!raceEnded && timerStarted && !lostMatch)
            timer += Time.deltaTime * 1000;

        timerText.text = TimeSpan.FromMilliseconds(timer).ToString(format);
    }

    public void EndRace(){
        raceEnded = true;
        _audioSource.Play();
        UIManager.Instance.ShowWinUI(true);
    }

    public void StartTimer(){
        timerStarted = true;
    }
    
    bool lostMatch = false;
    public void EndRun(){
        if(lostMatch || raceEnded) return;
        lostMatch = true;
        UIManager.Instance.ShowLostUI(true);
    }

    public void RestartRun(){
       bMove.ResetValues();
       bHandling.ResetValues();
       ResetValues();
       UIManager.Instance.ClearUI();
    }
    
    void ResetValues(){
        timer = 0;
        timerText.text = TimeSpan.FromMilliseconds(timer).ToString(format);
        timerStarted = false;
        raceEnded = false;
        lostMatch = false;
    }

    void OnTriggerEnter(Collider other)
    {
        EndRace();
    }
    
    public (float timer,string timerFormated) GetTimer(){
        return (timer,TimeSpan.FromMilliseconds(timer).ToString(format));
    }

    public string FormatTimer(float timer){
        return TimeSpan.FromMilliseconds(timer).ToString(format);
    }

    public bool RunRunning(){
        return !raceEnded;
    }

    public bool RunStarted(){
        return timerStarted && !lostMatch && !raceEnded;
    }

}
