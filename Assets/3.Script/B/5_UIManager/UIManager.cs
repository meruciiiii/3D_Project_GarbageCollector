using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //이벤트를 만들어서 호출시켜 Text UI, 청결도 UI를 그때만 바꾸겠습니다.
    // Update로 매번 UI text를 검사하고 있는데 그 부분을 메서드로 바꿔버리고
    // 이벤트 호출시키면 해당 메서드 실행하게 하겠습니다.

    public event Action UIValueChange;
    [SerializeField] private PlayerInput input;

    public static UIManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        input.onPickUp += change_Value;
    }

    public void change_Value()
    {
        if(UIValueChange != null) UIValueChange();
    }
}
