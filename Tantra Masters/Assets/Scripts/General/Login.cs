using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField password;

    public void LoginClicked()
    {
        StartCoroutine(OnLogin());
    }

    IEnumerator OnLogin()
    {
        string uri = Globals.login + "user=" + username.text + "&pass=" + password.text;
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
            if (www.downloadHandler.text == "2")
            {
                Debug.Log("Username does not exist");
                yield break;
            }
            else if (www.downloadHandler.text == "1")
            {
                Debug.Log("Incorrect Password");
                yield break;
            }
            PlayerData.instance.userDataAPI = new();
            PlayerData.instance.userDataAPI = JsonConvert.DeserializeObject<UserDataAPI>(www.downloadHandler.text);
            PlayerData.instance.LoadData();
        }
    }
}
