using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimapfollow : MonoBehaviour//미니맵 서브 카메라에 같이 붙을 컴포넌트입니다. 카메라가 움직임!
{
    [SerializeField] private Transform player;
    private float height = 250f;

    private void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position;
            newPosition.y += height;
            transform.position = newPosition;

            // 회전은 항상 아래를 바라보게 고정 (지도가 안 돌아감)
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
