using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml.Linq;
using System.IO;

public class DreamloHighscore : MonoBehaviour
{
    const string privateCode = "FikbRRGRn0WLa2Oy4LmO-AaSRd6i54SUCsl55riwRNYA";
    const string publicCode = "63869f1f8f40bb11045b5fa0";
    const string webURL = "http://dreamlo.com/lb/";
    public Highscores[] highscoresList;
    DisplayHighscore displayHighscore;
    static DreamloHighscore instance;

    void Awake(){
        instance = this;
        displayHighscore = GetComponent<DisplayHighscore>();
    }

    public static void AddNewHighscore(string username,float time){
        instance.StartCoroutine(instance.UploadNewHighscore(username,time));
    }

    IEnumerator UploadNewHighscore(string username,float time){
        UnityWebRequest www = new UnityWebRequest(webURL + privateCode + "/add/" + UnityWebRequest.EscapeURL(username) + "/"+ -time +"/"+ time);
        yield return www.SendWebRequest();
        if(string.IsNullOrEmpty(www.error)){
            //print("Foi caralho");
            DownloadHighscores();
        }
        else{
            print("Deu BO: " + www.error);
        }
    }

    public void DownloadHighscores(){
        StartCoroutine(GetHighscores());
    }

    IEnumerator GetHighscores(){
        UnityWebRequest www = new UnityWebRequest(webURL + publicCode + "/xml-seconds-asc");
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();
        if(string.IsNullOrEmpty(www.error)){
            FormatHighscores(www.downloadHandler.text);
            //displayHighscore.OnHighscoresDownloaded(highscoresList);
        }
        else{
            print("Deu BO: " + www.error);
        }
    }

    void FormatHighscores(string textStream){

        //LE A CARALHA DO XML
        var doc = XDocument.Parse(textStream);  
        var allDict = doc.Element("dreamlo").Element("leaderboard").Elements("entry");
        List<Dictionary<string,string>> allInfo = new List<Dictionary<string,string>>();
        foreach (var oneDict in allDict)
        {
            var name = oneDict.Element("name").ToString().Replace("<name>","").Replace("</name>","");
            var time = oneDict.Element("seconds").ToString().Replace("<seconds>","").Replace("</seconds>","");
            Dictionary<string,string> dic = new Dictionary<string, string>();
            dic.Add("name",name);
            dic.Add("time",time);
            allInfo.Add(dic);
        } 

        //Faze o formatinho para salvar no site lá.
        highscoresList = new Highscores[allInfo.Count];
        for (int i = 0; i < allInfo.Count; i++)
        {
            string username = allInfo[i]["name"];
            float time =  float.Parse(allInfo[i]["time"]);
            highscoresList[i] = new Highscores(username,time);
        }
        
    }
}

// public struct Highscores {
//     public string username;
//     public float time;
//     public Highscores(string _username,float _time){
//         username = _username;
//         time = _time;
//     }
// }