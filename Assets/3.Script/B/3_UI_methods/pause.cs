using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pause : MonoBehaviour
{
    [SerializeField] private GameObject ScreenHider;//화면 가리개

    [SerializeField] private GameObject InGame_UI;
    [SerializeField] private GameObject Pause_UI;

    private PlayerInput input;
    private bool ispause = false;

    private void Awake()
    {
        input = FindAnyObjectByType<PlayerInput>();
        ScreenHider.SetActive(false);
    }

    private void Start()
    {
        //input.onPickUp += pasue; //차후 ESC 인풋 이벤트에 등록
    }

    private void pasue()
    {
        if (ispause) 
        { 
            Time.timeScale = 0;
            InGame_UI.SetActive(false);
            Pause_UI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            InGame_UI.SetActive(true);
            Pause_UI.SetActive(false);
        }
    }
}
