using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LootLocker.Requests;

public class DisplayHighscore : MonoBehaviour
{
    const int leaderboardID = 9345;

    public TextMeshProUGUI[] highscoreText;
    public GameObject[] highscoreField;
    
    List<Highscores> highscoresList = new List<Highscores>();

    public void OnHighscoresDownloaded(){
        for (int i = 0; i < highscoreText.Length; i++)
        {
            highscoreText[i].text = i+1 + ". ";
            highscoreText[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = i+1 + ". ";
            if(highscoresList.Count> i){
                highscoreField[i].SetActive(true);
                highscoreText[i].text += highscoresList[i].username + " - " + highscoresList[i].time.ToString("00:000").Replace(":",".");
                highscoreText[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text += highscoresList[i].username + " - " + highscoresList[i].time.ToString("00:000").Replace(":",".");;
            }
            else{
                highscoreField[i].SetActive(false);
            }
        }
    }

    IEnumerator RefresHighscores(){  

        highscoresList.Clear();
          
        bool done = false;
        LootLockerSDKManager.GetScoreList(leaderboardID, 15, 0, (response) => {
            if(response.success){

                LootLockerLeaderboardMember[] members = response.items;
                for(int i = 0; i < members.Length; i++){
                    Highscores newScore = new Highscores();
                    newScore.username = members[i].player.name;
                    newScore.time = members[i].score;
                    highscoresList.Add(newScore);         
                }

                OnHighscoresDownloaded();
                done = true;
            }
            else{
                Debug.Log("Failed" + response.Error);
                done =  true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator SubmitScoreRoutine(float scoreToUpload){

        int score = (int)scoreToUpload;

        bool done = false;
        string playerID = PlayerPrefs.GetString("PlayerID");
        LootLockerSDKManager.SubmitScore(playerID,score,leaderboardID,(response) =>{
            if(response.success){
                Debug.Log("Sucess");
                done = true;
                StartCoroutine(RefresHighscores());
            }

            else{
                Debug.Log("Failed" + response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }

}

public struct Highscores {
    public string username;
    public float time;
    public Highscores(string _username,float _time){
        username = _username;
        time = _time;
    }
}
