using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LootLocker.Requests;

public class UIManager : MonoBehaviour
{
    [Header("Audios Stuff")]
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip[] _audioClips;

    [Header("Pause Stuff")]
    [SerializeField] bool _gamePaused;
    [SerializeField] RectTransform _pauseMenu;
    [SerializeField] float _resumeVelocity;
    [SerializeField] AnimationCurve _openCurve;
    [SerializeField] AnimationCurve _closeCurve;
    [SerializeField] Vector2 _slidePositions;

    [Header("Lost Stuff")]
    [SerializeField] RectTransform _retryCard;
    [SerializeField] RectTransform _lostThingy;
    [SerializeField] float _lostCurveVelocity;
    [SerializeField] AnimationCurve _lostCurve;
    [SerializeField] Vector2 _lostPositions;
    [SerializeField] Vector2 _retryCardPos;
    [SerializeField] TextMeshProUGUI _lostText;
    [SerializeField] string[] _lostPhrases;

    [Header("Win Stuff")]
    [SerializeField] RectTransform _winCard;
    [SerializeField] float _winCurveVelocity;
    [SerializeField] AnimationCurve _winCurve;
    [SerializeField] Vector2 _winPositions;

    [SerializeField] GameObject _usernameCard;
    [SerializeField] TextMeshProUGUI _usernameField;
    [SerializeField] TextMeshProUGUI _usernameError;

    [SerializeField] GameObject _winInfoCard;
    [SerializeField] TextMeshProUGUI _playerUsername;
    [SerializeField] TextMeshProUGUI _playerTimer;

    [SerializeField] float _timerVelocity;
    [SerializeField] AnimationCurve _timerCurve;

    [Header("Input Buttons")]
    [SerializeField] GameObject[] _inputCards;
    [SerializeField] TextMeshProUGUI[] _inputTexts;

    public static UIManager Instance;
    [SerializeField] DisplayHighscore display;

    void Awake(){
        Instance = this;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            _gamePaused = !_gamePaused;
            ShowPauseUI(_gamePaused);
        }
    }

    public void UpdateInputText(bool qwerty){
        _inputTexts[0].text = qwerty ? "W" : "Z";
        _inputTexts[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = qwerty ? "W" : "Z";
        _inputTexts[1].text = qwerty ? "A" : "Q";
        _inputTexts[1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = qwerty ? "A" : "Q";
    }

    public void CleanUsername(){
        PlayerPrefs.SetString("nickname","");
    }

    public void PlaySound(int index){
        _audioSource.pitch = 1;
        _audioSource.PlayOneShot(_audioClips[index]);
    }

    public void PlaySoundPitch(int index){
        _audioSource.pitch = Random.Range(0.9f,1.1f);
        _audioSource.PlayOneShot(_audioClips[index]);
    }

    Coroutine _pauseCoroutine;
    public void ShowPauseUI(bool state){

        if(_pauseCoroutine != null)
            StopCoroutine(_pauseCoroutine);

        if(state){
            _pauseCoroutine = StartCoroutine(SlideXPosition(_pauseMenu,_slidePositions.x,_resumeVelocity,_openCurve));
            PlaySound(0);                  
            return;
        }

        _pauseCoroutine = StartCoroutine(SlideXPosition(_pauseMenu,_slidePositions.y,_resumeVelocity,_closeCurve));
        PlaySound(1);
        _gamePaused = false;
        
    }

    public void ShowLostUI(bool state){
        if(state){
            _lostText.text = _lostPhrases[Random.Range(0,_lostPhrases.Length)];
            StartCoroutine(SlideYPosition(_lostThingy,_lostPositions.y,_lostCurveVelocity,_lostCurve));
            StartCoroutine(SlideYPosition(_retryCard,_retryCardPos.y,_lostCurveVelocity/2,_lostCurve));
            return;
        }
        StartCoroutine(SlideYPosition(_lostThingy,_lostPositions.x,_lostCurveVelocity,_lostCurve));
        StartCoroutine(SlideYPosition(_retryCard,_retryCardPos.x,_lostCurveVelocity,_lostCurve));    
    }

    public void ShowWinUI(bool state){
        if(state){
            StartCoroutine(ShowWinUI());
            return;
        }
        StartCoroutine(SlideYPosition(_winCard,_winPositions.x,_winCurveVelocity,_winCurve));
    }

    public void ClearUI(){
        _gamePaused = false;
        ShowPauseUI(false);
        ShowLostUI(false);
        ShowWinUI(false);
    }

    public void InputHoldUI(int index,bool enabled){
        _inputCards[index].SetActive(enabled);
    }


    public IEnumerator SlideXPosition(RectTransform objToMove,float goingTo,float animVelocity,AnimationCurve curve){
        float time = 0;
        float animationTime = 0;
        float startPos = objToMove.anchoredPosition.x;
        while (animationTime < 1)
        {
            time += Time.deltaTime * animVelocity;
            animationTime = curve.Evaluate(time);
            objToMove.anchoredPosition = new Vector2(Mathf.Lerp(startPos,goingTo,animationTime),objToMove.anchoredPosition.y);
            yield return null;
        }
    }

    public IEnumerator SlideYPosition(RectTransform objToMove,float goingTo,float animVelocity,AnimationCurve curve){
        float time = 0;
        float animationTime = 0;
        float startPos = objToMove.anchoredPosition.y;
        while (animationTime < 1)
        {
            time += Time.deltaTime * animVelocity;
            animationTime = curve.Evaluate(time);
            objToMove.anchoredPosition = new Vector2(objToMove.anchoredPosition.x,Mathf.Lerp(startPos,goingTo,animationTime));
            yield return null;
        }
    }

    IEnumerator ShowWinUI(){ 

        _usernameCard.SetActive(false);
        _winInfoCard.SetActive(true);

        CheckUsername();     
        StartCoroutine(SlideYPosition(_winCard,_winPositions.y,_winCurveVelocity,_winCurve));

        while (CheckUsername())
        {
            yield return null;
        }

        StartCoroutine(SetWinValues());
        yield return display.SubmitScoreRoutine(GameManager.instance.GetTimer().timer);
    }

    bool CheckUsername(){
        var nick = PlayerPrefs.GetString("nickname");
        if(nick == ""){
            _winInfoCard.SetActive(false);
            _usernameCard.SetActive(true);
            return true;
        }
        return false;
    }

    public void ApplyUsername(){
        var nick = _usernameField.text.ToString();

        if(nick.Length > 3 && (nick.Contains(" ") || nick.Contains(" ") || nick.Contains(" ") || nick.Contains("\n"))){
            _usernameError.text = "Name contains space";
            _usernameError.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Name contains space";
            return;
        }

        if(nick.Contains(" ") || nick.Contains(" ") || nick.Contains(" ") || nick.Contains("\n")){
            _usernameError.text = "Empty name";
            _usernameError.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Name contains space";
            return;
        }

        if(nick.Length <= 3){
            _usernameError.text = "Less than 3 characters";
            _usernameError.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Less than 3 characters";
            return;
        }

        print("everything worked");
        PlayerPrefs.SetString("nickname",nick);
        LootLockerSDKManager.SetPlayerName(nick,(response) => {
            if(response.success){
                Debug.Log("Usernamem setted");
            }
            else{
                Debug.Log("Couldnt set username" + response.Error);
            }
        });
    }

    IEnumerator SetWinValues(){   

        _usernameCard.SetActive(false);
        _winInfoCard.SetActive(true);

        _playerUsername.text = PlayerPrefs.GetString("nickname");
        _playerUsername.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("nickname");

        var time = 0f;
        var animTime = 0f;
        var timerLerped = 0f;
        while (animTime != 1)
        {
            time += Time.deltaTime * _timerVelocity;
            animTime = _timerCurve.Evaluate(time);
            timerLerped = Mathf.Lerp(0,GameManager.instance.GetTimer().timer,animTime);
            _playerTimer.text = GameManager.instance.FormatTimer(timerLerped);
            _playerTimer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameManager.instance.FormatTimer(timerLerped);
            yield return null;
        }
    }

    void VerifyTimeLeaderboard(){
        var name = PlayerPrefs.GetString("nickname");
        var timer = GameManager.instance.GetTimer().timer;
        //Highscore.AddNewHighscore(name,timer);
        //PlayerPrefs.SetString("nickname","");
    }
}
