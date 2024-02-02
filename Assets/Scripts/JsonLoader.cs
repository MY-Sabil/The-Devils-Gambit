using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JsonLoader : MonoBehaviour
{
    private void Awake()
    {
        #if UNITY_WEBGL
            DontDestroyOnLoad(gameObject);
            StartCoroutine(ReadJson());
        #endif
    }

    [HideInInspector] public string jsonString = "";

    IEnumerator ReadJson()
    {
        UnityWebRequest json = UnityWebRequest.Get("https://raw.githubusercontent.com/MY-Sabil/A-Devil-s-Gambit/main/result.json");
        yield return json.SendWebRequest();
        if (json.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(json.error);
        }
        else
        {
            jsonString = json.downloadHandler.text;
        }

        json.Dispose();
    }
}
