
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIsFaint : MonoBehaviour
{
    [SerializeField] private float fallDuration = 1.5f;
    [SerializeField] private Vector3 fallPositionOffset = new Vector3(0, 0f, 0.5f); // 바닥으로 내려가고 살짝 앞으로
    [SerializeField] private Vector3 fallRotationOffset = new Vector3(10f, 0, 85f);   // 옆으로 픽 쓰러지는 각도 (Z축 중요)
    [SerializeField] private AnimationCurve fallCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    //[SerializeField] private ShowerSystem showerSystem;
    [SerializeField] private GameObject Sceneload_fade;
    [SerializeField] private VignetteController vignetteController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Camera camera;
    public void StartPassOutEffect()
    {
        DoNotControl(true);
        StartCoroutine(PassOutRoutine());
        DoNotControl(false);
    }

    private IEnumerator PassOutRoutine()
    {
        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;

        Vector3 targetPos = startPos + fallPositionOffset;
        Quaternion targetRot = Quaternion.Euler(fallRotationOffset);

        float elapsed = 0f;
        DoNotControl(true);
        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = fallCurve.Evaluate(elapsed / fallDuration);

            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }


        Debug.Log("연출 끝: 리스폰 로직 실행");
        Sceneload_fade.SetActive(true);
        yield return new WaitForSeconds(1f);
        HandleResetPlayer();
        playerInput.onPickUp();
        DoNotControl(false);
    }

    private void HandleResetPlayer()
    {
        transform.localPosition = new Vector3(-1.22f, -0.69f, -13.496f);
        transform.localRotation = Quaternion.Euler(0,-90f,0);
        camera.transform.rotation = Quaternion.Euler(0, 0, 0);
        GameManager.instance.P_CurrentHP = GameManager.instance.P_MaxHP;
        GameManager.instance.P_Weight = 0;
        vignetteController.OnWash();
    }
    private void DoNotControl(bool isFaint)
    {
        PlayerController playerController;
        TryGetComponent<PlayerController>(out playerController);
        Rigidbody Player_r;
        TryGetComponent<Rigidbody>(out Player_r);
        if (Player_r != null)
        {
            Player_r.linearVelocity = Vector3.zero;
            Player_r.angularVelocity = Vector3.zero;
        }
        playerController.enabled = !isFaint;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
