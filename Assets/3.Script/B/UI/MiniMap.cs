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
    public RectTransform playerSymbol;

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
        // 1. 플레이어의 상대적 위치 비율 계산 (0 ~ 1 사이 값)
        float normX = (PlayerTransform.position.x - DowmLeftTransform.position.x) / WorldSize.x;
        float normY = (PlayerTransform.position.z - DowmLeftTransform.position.z) / WorldSize.y;

        // 2. 미니맵 UI의 화면상 네 모서리 좌표 가져오기
        Vector3[] corners = new Vector3[4];
        mapRect.GetWorldCorners(corners);

        // corners[0]은 좌하단(Min), corners[2]는 우상단(Max)
        Vector2 minCorner = corners[0];
        Vector2 maxCorner = corners[2];

        // 3. X축과 Y축 각각 따로 계산 (Lerp의 원리 이용)
        float finalX = Mathf.Lerp(minCorner.x, maxCorner.x, normX);
        float finalY = Mathf.Lerp(minCorner.y, maxCorner.y, normY);

        // 4. 최종 위치 적용
        playerSymbol.position = new Vector2(finalX, finalY);
    }
}
