using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Cinemachine; 패키지 설치후 사용, 그런데 이거 다 같이 설치해야 돼는거 아닌지(main에서?)

public class UI_CamarMove : MonoBehaviour
{
    //2차 이동로직(transform)
    [Header("Camera 이동")]
    [SerializeField] private GameObject camera_ob;
    [SerializeField] private Transform cameraStartTransform;      // 시작 위치
    [SerializeField] private Transform camaraTargetTransform;     // 목표 위치
    [SerializeField] private float moveSpeed;      // 이동 속도
    [SerializeField] private float rotationSpeed;      // 회전 속도
    private bool moveCamera = false;

    [SerializeField] private GameObject currentUIGroup; // 현재 보여주는 UI 그룹
    [SerializeField] private GameObject nextUIGroup;    // 다음에 보여줄 UI 그룹

    private void Start()
    {
        // 카메라 위치/회전을 cameraStartTransform의 값으로 즉시 변경
        camera_ob.transform.position = cameraStartTransform.position;
        camera_ob.transform.rotation = cameraStartTransform.rotation;

        // 게임 시작 시: 현재 UI는 키고, 다음 UI는 끕니다.
        if (currentUIGroup != null)
            currentUIGroup.SetActive(true);

        if (nextUIGroup != null)
            nextUIGroup.SetActive(false);
    }

    // 화면 전환과 버튼에 넣을 메서드 public
    public void SwitchUIWithCamera()
    {
        // UI 이동 중에는 현재 UI 숨기기
        if (currentUIGroup != null)
            currentUIGroup.SetActive(false);

        // 배경 이동 시작
        StartCoroutine(MoveCameraRoutine());

        // 이동 완료후 UI 전환 코루틴 시작
        StartCoroutine(WaitForBgAndShowUI());
    }

    private IEnumerator MoveCameraRoutine()
    {
        if (moveCamera) yield break;
        moveCamera = true;

        // 시작 상태 저장
        Vector3 startPos = camera_ob.transform.position;
        Quaternion startRot = camera_ob.transform.rotation;

        // 목표 상태 저장 (Start/Target 교체 전의 현재 목표값)
        Vector3 targetPos = camaraTargetTransform.position;
        Quaternion targetRot = camaraTargetTransform.rotation;

        float elapsedTime = 0f;
        float duration = 1.0f / moveSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            camera_ob.transform.position = Vector3.Lerp(startPos, targetPos, t);
            camera_ob.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        // 최종 위치 확정
        camera_ob.transform.position = targetPos;
        camera_ob.transform.rotation = targetRot;

        // 목표와 시작 위치/회전 교체 재활용을 위함
        Vector3 tempPos = cameraStartTransform.position;
        Quaternion tempRot = cameraStartTransform.rotation;
        cameraStartTransform.position = targetPos;
        cameraStartTransform.rotation = targetRot;
        camaraTargetTransform.position = tempPos;
        camaraTargetTransform.rotation = tempRot;

        moveCamera = false;
    }

    // UI 전환 대기 코루틴
    private IEnumerator WaitForBgAndShowUI()
    {
        // 카메라 이동이 끝날 때까지 기다림 (MoveCameraRoutine이 moveCamera를 false로 만들 때까지)
        while (moveCamera)
            yield return null;

        // 이동 완료 후 다음 UI 보여주기
        if (nextUIGroup != null)
            nextUIGroup.SetActive(true);
        GameObject temp = currentUIGroup;
        currentUIGroup = nextUIGroup;
        nextUIGroup = temp;//재사용 하기 위해 ui 교체, position 교체와 동일.
        Debug.Log("대기및 ui교체");
    }
}
