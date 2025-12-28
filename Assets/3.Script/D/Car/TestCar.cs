using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TestCar : MonoBehaviour {
    [Header("Settings")]
    public SplineContainer splineContainer; // 아까 만든 경로
    public float speed = 10f; // 이동 속도
    public bool isLoop = true; // 반복 여부

    [Header("Debug Info")]
    [Range(0f, 1f)]
    public float progress = 0f; // 0(시작) ~ 1(끝) 사이의 위치 값
    private float splineLength; // 전체 길이

    void Start() {
        if (splineContainer == null) {
            Debug.LogError("Spline Container가 연결되지 않았습니다!");
            return;
        }

        // 스플라인의 전체 길이를 계산합니다.
        splineLength = splineContainer.CalculateLength();
    }

    void Update() {
        if (splineContainer == null) return;

        MoveCar();
    }

    void MoveCar() {
        // 1. 진행도(Progress) 업데이트
        // 거리 = 속도 * 시간. 전체 길이로 나누어 0~1 사이 비율로 변환
        float distanceTravelled = speed * Time.deltaTime;
        float progressChange = distanceTravelled / splineLength;

        progress += progressChange;

        // 2. 루프 처리 (1을 넘어가면 0으로 되돌림)
        if (isLoop) {
            if (progress > 1f) progress -= 1f;
        } else {
            progress = Mathf.Clamp01(progress); // 끝에 멈춤
        }

        // 3. 스플라인 위의 위치와 회전값 가져오기
        // Evaluate 함수는 0~1 사이의 t값에 해당하는 위치와 방향을 줍니다.
        Vector3 position = splineContainer.EvaluatePosition(progress);
        Vector3 tangent = splineContainer.EvaluateTangent(progress);
        Vector3 up = splineContainer.EvaluateUpVector(progress);

        // 4. 차량에 적용
        transform.position = position;

        // 진행 방향(tangent)을 바라보게 회전
        if (tangent != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(tangent, up);
        }
    }
}
