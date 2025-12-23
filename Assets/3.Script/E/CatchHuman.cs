using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchHuman : MonoBehaviour
{
    //사람을 들어버린다.
    //
    //Vector3(0.620000005,0.860000014,0.970000029)
    //Vector3(344,250,91.0000229)
    //Quaternion(-0.634525776,-0.511627376,0.325215936,0.479425818)
    [SerializeField] private Vector3 liftPositionOffset = new Vector3(0.62f, 0.86f, 0.97f); // 바닥으로 내려가고 살짝 앞으로
    [SerializeField] private Vector3 liftRotationOffset = new Vector3(344f, 250f, 91f);   // 옆으로 픽 쓰러지는 각도 (Z축 중요)
    private AnimationCurve liftCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private float liftDuration = 0.5f;




    private GameObject trash;
    private int trashNum;
    private Vector3 PlayerPosition;
    private Rigidbody trash_r;
    private Transform trashRotation;
    [SerializeField] private CleanPlayer cleanPlayer;
    private float porce;

    private void Awake()
    {
    }
    public void CatchMan(GameObject trash)
    {
        //지정된 위치 회전값으로 변경
        //GameManager.instance.isGrabBigGarbage = true;
        this.trash = trash;
        PassOutRoutine();
    }

    private IEnumerator PassOutRoutine()
    {
        Debug.Log("휴먼 코루틴 실행 했음다");
        Vector3 startPos = trash.transform.localPosition;
        Debug.Log(startPos);
        Quaternion startRot = trash.transform.localRotation;
        Debug.Log(startRot);

        Vector3 targetPos = transform.localPosition + liftPositionOffset;
        Quaternion targetRot = Quaternion.Euler(liftRotationOffset);

        float elapsed = 0f;
        while (elapsed < liftDuration)
        {
            elapsed += Time.deltaTime;
            float t = liftCurve.Evaluate(elapsed / liftDuration);

            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        Debug.Log("연출 끝: 리스폰 로직 실행");
        //playerInput.onPickUp();
    }
    public void DestroyHuman()
    {
        Destroy(trash);
        transform.GetChild(trashNum + 1).gameObject.SetActive(false);
        trash = null;
    }
    public void DrobHuman()
    {
        if (GameManager.instance.isGrabBigGarbage)
        {
            trashRotation = trash.transform;
            trashRotation.rotation = transform.GetChild(trashNum + 1).gameObject.transform.rotation;
            transform.GetChild(trashNum + 1).gameObject.SetActive(false);
            PlayerPosition = transform.position;
            Vector3 direction = (transform.forward + transform.right * 0.5f).normalized;
            trash.transform.position = PlayerPosition + direction * 1.5f;
            trash.transform.rotation = trashRotation.rotation;
            trash.SetActive(true);
            trash.TryGetComponent<Rigidbody>(out trash_r);
            trash_r.isKinematic = false;
            trash_r.useGravity = true;
            trash_r.AddForce(direction * porce, ForceMode.Impulse);
            trash_r.AddTorque(Vector3.Cross(Vector3.up, direction) * 1f);
            Debug.Log(trash.transform.position);
            GameManager.instance.isGrabBigGarbage = false;
        }
        else
        {
            return;
        }

    }
}