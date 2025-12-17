using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [Header("World")]
    public Transform PlayerTransform;
    public Transform DowmLeftTransform;
    public Transform UPRightTransform;

    [Header("UI")]
    public RectTransform mapRect; //지도
    public RectTransform playerSymbol;//symbol 지도의 자식

    private Vector2 WorldSize;
    private Vector2 minimapSize;

    private void Start()
    {
        WorldSize = new Vector2
            (UPRightTransform.position.x - DowmLeftTransform.position.x, 
            UPRightTransform.position.z - DowmLeftTransform.position.z);
        minimapSize = mapRect.sizeDelta;
    }

    private void Update()
    {
        // 1. 플레이어의 상대적 위치 계산 (0 ~ 1 사이 값)
        float normX = (PlayerTransform.position.x - DowmLeftTransform.position.x) / WorldSize.x;
        float normY = (PlayerTransform.position.z - DowmLeftTransform.position.z) / WorldSize.y;

        // 2. UI 좌표로 변환 (Pivot이 0,0일 때 기준, 0.5일 경우 -mapSize/2 필요)
        // 맵 UI의 Pivot이 (0.5, 0.5)라면 아래와 같이 계산합니다.
        float uiX = (normX - 0.5f) * minimapSize.x;
        float uiY = (normY - 0.5f) * minimapSize.y;
        playerSymbol.anchoredPosition = new Vector2(uiX, uiY);
    }
}
