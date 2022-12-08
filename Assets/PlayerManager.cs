using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;

public class PlayerManager : MonoBehaviour
{
    IEnumerator Start(){
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) => {
            if(response.success){
                Debug.Log("Player Logged In");
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());

                done = true;
            }
            else{
                Debug.Log("Could not start sesh");
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }
}
