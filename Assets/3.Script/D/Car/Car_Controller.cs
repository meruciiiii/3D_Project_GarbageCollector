using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Car_Controller : MonoBehaviour {
    [Header("스플라인 설정")]
    public SplineContainer route;
    public float speed = 10f; // 이동 속도
    public bool isLoop = true; // 반복 여부
    public bool isRotateNeed = false;
    private float rotation_value = 90f; //어떤 차량은 90도 회전해있는 문제가 있음... 임의의 값을 추가하여 회전하기.
    
    [Header("차량 모델")]
    public bool isChangeModel = true; // 외형 변경 여부
    public GameObject[] car_models;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    //[Header("Car List")]
    //public GameObject[] Car

    [Header("Debug Info")]
    [Range(0f, 1f)]
    public float progress = 0f; // 0(시작) ~ 1(끝) 사이의 위치 값
    private float splineLength; // 전체 길이

	private void Awake() {
		if(isChangeModel) {
            TryGetComponent(out meshFilter);
            TryGetComponent(out meshRenderer);
            TryGetComponent(out meshCollider);
            Random_Model();
        }
	}

	private void Start() {
        if (route == null) {
            Debug.LogError("Spline Container가 연결되지 않았습니다!");
            return;
        }

        // 스플라인의 전체 길이를 계산합니다.
        splineLength = route.CalculateLength();
    }

    private void Update() {
        if (route == null) return;

        MoveCar();
    }

    private void MoveCar() {
        // 진행도(Progress) 업데이트
        // 거리 = 속도 * 시간. 전체 길이로 나누어 0~1 사이 비율로 변환
        float distanceTravelled = speed * Time.deltaTime;
        float progressChange = distanceTravelled / splineLength;

        progress += progressChange;

        // 루프 처리 (1을 넘어가면 0으로 되돌림)
        if (isLoop) {
            if (progress > 1f) { 
                progress -= 1f; 
                if(isChangeModel) {
                    Random_Model();
				}
            }
        } else {
            progress = Mathf.Clamp01(progress); // 끝에 멈춤
        }

        // 스플라인 위의 위치와 회전값 가져오기
        // Evaluate 함수는 0~1 사이의 t값에 해당하는 위치와 방향을 줍니다.
        Vector3 position = route.EvaluatePosition(progress);
        Vector3 rotate = route.EvaluateTangent(progress);
        Vector3 up = route.EvaluateUpVector(progress);

        // 차량에 적용
        transform.position = position;
        // 진행 방향(tangent)을 바라보게 회전
        if (rotate != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation(rotate, up);
            if(isRotateNeed) {
                transform.rotation *= Quaternion.Euler(0f, rotation_value, 0f);
			}
		}
	}

    private void Random_Model() {
        int index = Random.Range(0, car_models.Length);
        GameObject car = car_models[index];
        if(index.Equals(1) || index.Equals(3)) {
            isRotateNeed = true;
		} else {
            isRotateNeed = false;
		}
       
        if(car.TryGetComponent(out MeshFilter mesh)) {
            this.meshFilter.sharedMesh = mesh.sharedMesh;
            this.meshCollider.sharedMesh = mesh.sharedMesh;
        }
        if (car.TryGetComponent(out MeshRenderer meshRenderer)) {
            this.meshRenderer.sharedMaterials = meshRenderer.sharedMaterials;
        }
    }
}
