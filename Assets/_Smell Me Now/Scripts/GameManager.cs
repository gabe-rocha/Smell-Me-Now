using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get => instance; set => instance = value; }

    void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            Application.targetFrameRate = 60;

        } else {
            Destroy(this);
        }
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Space))
        {
            EventManager.Instance.TriggerEvent(EventManager.Events.OnLevelStarted);
        }
    }
}