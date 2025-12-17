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

        Debug.Log("카메라가 시작 위치로 즉시 설정되었습니다.");

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

        moveCamera = true; // 이동 시작 (WaitForBgAndShowUI를 위해)

        // 위치와 회전 중 하나라도 목표에 도달하지 않았으면 계속 반복합니다.
        float positionThreshold = 0.05f; // 위치 이동 완료 임계값 (원래 0.2f였으나 더 작게 조정)
        float rotationThreshold = 0.3f; // 회전 이동 완료 임계값 (원래와 동일)

        while (Vector3.Distance(camera_ob.transform.position, camaraTargetTransform.position) > positionThreshold ||
               Quaternion.Angle(camera_ob.transform.rotation, camaraTargetTransform.rotation) > rotationThreshold)
        {
            // 1. 위치 이동 (Lerp로 감속 효과 적용)
            camera_ob.transform.position = Vector3.Lerp(
                camera_ob.transform.position,
                camaraTargetTransform.position,
                moveSpeed * Time.deltaTime);

            // 2. 회전 이동 (Slerp로 감속 효과 적용)
            camera_ob.transform.rotation = Quaternion.Slerp(
                camera_ob.transform.rotation,
                camaraTargetTransform.rotation,
                rotationSpeed * Time.deltaTime);

            yield return null; // 한 프레임 대기
        }

        // **이동 완료 후 스냅 (오차 제거)**
        camera_ob.transform.position = camaraTargetTransform.position;
        camera_ob.transform.rotation = camaraTargetTransform.rotation;

        // **목표와 시작 위치/회전 교체** (다음 이동을 위한 준비)
        Vector3 tempPos = cameraStartTransform.position;
        Quaternion tempRot = cameraStartTransform.rotation;

        cameraStartTransform.position = camaraTargetTransform.position;
        cameraStartTransform.rotation = camaraTargetTransform.rotation;

        camaraTargetTransform.position = tempPos;
        camaraTargetTransform.rotation = tempRot;

        moveCamera = false; // 이동 종료
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
