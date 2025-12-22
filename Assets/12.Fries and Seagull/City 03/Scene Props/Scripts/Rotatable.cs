#if UNITY_EDITOR
using UnityEditor;
#endif
using Seagull.City_03.Inspector;
using UnityEngine;

namespace Seagull.City_03.SceneProps
{
    public enum Axis
    {
        x, y, z
    }

    public class Rotatable : MonoBehaviour
    {
        [SerializeField] private float startAngle;
        [SerializeField] private float endAngle;
        [SerializeField] private Axis rotationAxis;

        [Range(0f, 1f)] public float rotation;
        private float lastRotation = -1;

        // Start: 게임이 시작될 때 한 번 실행됩니다.
        private void Start()
        {
            // 원인 제거: startAngle과 endAngle을 강제로 바꾸던 로직을 삭제했습니다.
            // 인스펙터에 입력한 값을 그대로 믿고 사용합니다.

            updateAngle();
            lastRotation = rotation;
        }

        // FixedUpdate: 물리 프레임마다 호출되며 rotation 값의 변화를 감지합니다.
        private void FixedUpdate()
        {
            if (lastRotation == -1)
            {
                lastRotation = rotation;
                return;
            }

            if (lastRotation == rotation) return;

            updateAngle();
            lastRotation = rotation;
        }

        // OnValidate: 에디터에서 값을 수정할 때마다 즉시 반영합니다.
        private void OnValidate()
        {
            updateAngle();
            lastRotation = rotation;
        }

        // 실제 각도를 계산하여 적용하는 핵심 함수입니다.
        private void updateAngle()
        {
            // 0과 1 사이로 값을 제한합니다.
            rotation = Mathf.Clamp01(rotation);

            // 각도 계산: 시작 각도 + (진행도 * 총 회전량)
            float totalRange = endAngle - startAngle;
            float currentAngle = (rotation * totalRange) + startAngle;

            // 회전축 설정
            Vector3 axis = Vector3.zero;
            if (rotationAxis == Axis.x)
            {
                axis = Vector3.right;
            }
            else if (rotationAxis == Axis.y)
            {
                axis = Vector3.up;
            }
            else if (rotationAxis == Axis.z)
            {
                axis = Vector3.forward;
            }

            // 로컬 회전값 적용
            transform.localRotation = Quaternion.AngleAxis(currentAngle, axis);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Rotatable))]
    public class RotatableEditor : AnInspector { }
#endif
}