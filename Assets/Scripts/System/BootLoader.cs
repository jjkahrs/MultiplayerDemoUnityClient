using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootLoader : MonoBehaviour
{
    public void Awake() {
        App app = GameObject.FindObjectOfType<App>();
        if(app == null) {
            Debug.Log("App not found. Loading preload scene...");
            GameObject check = GameObject.Find("__app");
            if (check==null) { 
                UnityEngine.SceneManagement.SceneManager.LoadScene("_PreloadScene");
            }            
        }
    }
}
