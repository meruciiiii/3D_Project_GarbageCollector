using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchUI : MonoBehaviour
{
    [SerializeField] private GameObject currentUI_GameObject;
    [SerializeField] private GameObject nextUI_GameObject;
    public void SwitchUIObject()
    {
        if (currentUI_GameObject != null) currentUI_GameObject.SetActive(false);

        if (nextUI_GameObject != null) nextUI_GameObject.SetActive(true);
        else Debug.Log("UI_Botton_change nextUIObject 이 없습니다. SerializeField이기에 컴포넌트에 GameObject를 연결해주세요");

        //교체 해서 재사용하겠습니다.
        GameObject tempObject = currentUI_GameObject;
        currentUI_GameObject = nextUI_GameObject;
        nextUI_GameObject = tempObject;
        Debug.Log("ShowNextUIObjectandSwitch");
    }
}
