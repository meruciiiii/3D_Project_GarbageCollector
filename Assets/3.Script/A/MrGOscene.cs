using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MrGOscene : MonoBehaviour
{
    [Header("활성화할 대상 오브젝트")]
    public GameObject targetObject;

    // 이 오브젝트가 파괴될 때 실행됩니다.
    private void OnDestroy()
    {
        // 1. 대상 오브젝트가 할당되어 있는지 확인 (에러 방지)
        if (targetObject != null)
        {
            // 2. 대상 오브젝트를 활성화합니다.
            targetObject.SetActive(true);
        }
    }
}
