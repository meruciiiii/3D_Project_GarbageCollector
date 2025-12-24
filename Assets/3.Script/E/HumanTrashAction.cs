
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanTrashAction : MonoBehaviour
{
    private Trash currentTrash;
    Vector3 startPos;
    Vector3 targetPos;
    Quaternion startRot;
    Quaternion targetRot;
    private bool isHolding => currentTrash != null;
    public bool IsHolding => isHolding;

    private Transform child_human;
    [SerializeField] private AnimationCurve liftCurve;
    [SerializeField] private float liftDuration = 0.5f;
    [SerializeField] private LayerMask groundMask;
    //public void TryInteractWithBigTrash(Trash trash)
    //{
    //    if (IsHolding)
    //    {
    //        DrobGarbage(trash);
    //    }
    //    else
    //    {
    //        Hold(trash);
    //    }
    //}
    public void Hold(Trash trash)
    {
        if (CanHold(trash.Data.getrequiredStrength(trash.TrashNum)))
            return;
        currentTrash = trash;
        StartCoroutine(ChangePositionWithCurve());
        Debug.Log("실행했나요?");
        StartCoroutine(ChangeRotationWithCurve());
        trash.Trash_c.enabled = false;
        trash.Trash_r.constraints = RigidbodyConstraints.FreezeAll;
        GameManager.instance.BigGarbageWeight = trash.Data.getBigTrashWeight(trash.TrashNum);
        GameManager.instance.isGrabBigGarbage = true;
    }
    public bool CanHold(int strength)
    {
        if (GameManager.instance.P_Str < strength)
            return true; //들 수 있다.
        else
            return false; //들 수 없다.
    }
    public void DrobGarbage()                                                                //수정해야 할 것
    {
        currentTrash.transform.SetParent(null);

        startPos = currentTrash.transform.position;
        targetPos = GetGroundPoint(startPos);

        startRot = currentTrash.transform.rotation;
        targetRot = Quaternion.identity;







        child_human = transform.GetChild(7).gameObject.transform;
        Trash trash = currentTrash;
        Debug.Log("is it Droping");
        Transform trashRotation = trash.transform;                                                                //trash
        trashRotation.rotation = child_human.rotation;
        Vector3 direction = (transform.forward + transform.right * 0.5f).normalized;
        trashRotation.position = transform.position + direction * 1.5f;
        trashRotation.rotation = transform.rotation;

        
        //trash.gameObject.SetActive(true);                                                                //수정해야 할 것
        trash.Trash_r.isKinematic = false;
        trash.Trash_r.useGravity = true;
        trash.Trash_r.AddForce(direction * 1f, ForceMode.Impulse);
        trash.Trash_r.AddTorque(Vector3.Cross(Vector3.up, direction) * 1f);
        //Debug.Log(transform.position);
        GameManager.instance.isGrabBigGarbage = false;
        currentTrash = null;
    }
    private IEnumerator ChangePositionWithCurve()
    {
        child_human = transform.GetChild(7).gameObject.transform;
        startPos = currentTrash.transform.position;
        targetPos = transform.GetChild(7).gameObject.transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < liftDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / liftDuration);
            float curveValue = liftCurve.Evaluate(normalizedTime);

            currentTrash.transform.position = Vector3.Lerp(startPos, targetPos, curveValue);
            //Debug.Log($"Camera.fieldOfView: {Camera.fieldOfView}");
            yield return null;
        }
        AttachNPC();
    }
    private IEnumerator ChangeRotationWithCurve()
    {
        child_human = transform.GetChild(7).gameObject.transform;
        startRot = currentTrash.transform.rotation;
        targetRot = transform.GetChild(7).gameObject.transform.rotation;
        float elapsedTime = 0f;
        while (elapsedTime < liftDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / liftDuration);
            float curveValue = liftCurve.Evaluate(normalizedTime);

            currentTrash.transform.rotation = Quaternion.Lerp(startRot, targetRot, curveValue);
            //Debug.Log($"Camera.fieldOfView: {Camera.fieldOfView}");
            yield return null;
        }
    }
    private void AttachNPC()
    {
        currentTrash.transform.SetParent(this.gameObject.transform);
        currentTrash.transform.localPosition = Vector3.zero;
        currentTrash.transform.localRotation = Quaternion.identity;
    }
    private Vector3 GetGroundPoint(Vector3 from)
    {
        Ray ray = new Ray(from + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 5f, groundMask))
        {
            return hit.point;
        }
        return from;
    }
    public void SellHeldTrash()
    {
        if (currentTrash == null)
            return;
        Destroy(currentTrash.gameObject);
        currentTrash = null;
    }
}
