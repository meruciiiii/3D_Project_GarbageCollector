
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    [SerializeField] private Rigidbody[] ragdollBodies;
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
        if (!trash.isRuntimeInstance)
        {
            currentTrash = CloneTrashRuntime();
            currentTrash.isRuntimeInstance = true;
        }
        SetRagdollKinematic(true);
        //trash.Trash_c.enabled = false;
        trash.Trash_r.isKinematic = true;
        trash.Trash_r.useGravity = false;
        currentTrash.Trash_r.constraints = RigidbodyConstraints.FreezeAll;
        StartCoroutine(LiftWithCurve());
        GameManager.instance.BigGarbageWeight = trash.Data.getBigTrashWeight(trash.TrashNum);
        GameManager.instance.isGrabBigGarbage = true;
        //trash.Trash_r.constraints = RigidbodyConstraints.FreezeAll;
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
        if (currentTrash == null)
        {
            return;
        }
        if (currentTrash.TryGetComponent(out Animator Ani))
        {
            currentTrash.Trash_r.constraints = RigidbodyConstraints.None;
            //currentTrash.Trash_r.constraints = RigidbodyConstraints.FreezePositionY;
            Ani.enabled = false;
        }
        StopCoroutine(DropWithCurve());
        currentTrash.transform.SetParent(null);
        StartCoroutine(DropWithCurve());
        SetRagdollKinematic(false);
        GameManager.instance.isGrabBigGarbage = false;
    }
    private IEnumerator LiftWithCurve()
    {
        Transform hand = transform.GetChild(7);
        Trash trash = currentTrash;
        trash.transform.SetParent(null);
        SetRagdollKinematic(true);

        trash.transform.SetParent(hand, true);
        startPos = trash.transform.position;
        startRot = trash.transform.rotation;

        targetPos = hand.position;
        targetRot = hand.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < liftDuration)
        {
            float t = Mathf.Clamp01(elapsedTime / liftDuration);
            float curveValue = liftCurve.Evaluate(t);

            trash.transform.position =
                Vector3.Lerp(startPos, targetPos, curveValue);
            trash.transform.rotation =
                Quaternion.Lerp(startRot, targetRot, curveValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        trash.transform.position = targetPos;
        trash.transform.rotation = targetRot;
        // 완전히 도착 후 Parent 설정
        trash.transform.localPosition = Vector3.zero;
        trash.transform.localRotation = Quaternion.identity;
    }
    private IEnumerator DropWithCurve()
    {
        Trash trash = currentTrash;

        trash.Trash_r.isKinematic = false;
        trash.Trash_r.useGravity = true;

        startPos = trash.transform.position;
        targetPos = GetGroundPoint(startPos);

        startRot = trash.transform.rotation;
        targetRot = Quaternion.identity;

        float elapsedTime = 0f;

        while (elapsedTime < liftDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / liftDuration);
            float curveValue = liftCurve.Evaluate(t);

            trash.transform.position = Vector3.Lerp(startPos, targetPos, curveValue);
            trash.transform.rotation = Quaternion.Lerp(startRot, targetRot, curveValue);

            yield return null;
        }
        trash.Trash_r.isKinematic = false;
        trash.Trash_r.useGravity = true;
        trash.Trash_c.enabled = true;

        GameManager.instance.isGrabBigGarbage = false;
        currentTrash = null;
    }
    private Vector3 GetGroundPoint(Vector3 from)
    {
        Ray ray = new Ray(from + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 5f, groundMask))
        {
            return hit.point;
        }
        return from += Vector3.up * 0.02f;
    }
    public void SellHeldTrash()
    {
        if (currentTrash == null)
            return;
        Destroy(currentTrash.gameObject);
        currentTrash = null;
    }
    public Trash CloneTrashRuntime()
    {
        GameObject cloneGO = Instantiate(currentTrash.gameObject);
        Trash cloneTrash = cloneGO.GetComponent<Trash>();
        CloneSetting(cloneTrash);
        if (currentTrash.gameObject.TryGetComponent(out Stage3_NPC npc_script)) {
            npc_script.ChangeState(npc_script.becomeTrashState);
        }
        return cloneTrash;
    }
    private void CloneSetting(Trash cloneTrash)
    {
        if (cloneTrash.TryGetComponent(out NavMeshAgent Nav))
        {
            Nav.enabled = false;
        }
        if (cloneTrash.TryGetComponent(out Stage3_NPC stageNpc))
        {
            stageNpc.enabled = false;
        }
        if (cloneTrash.TryGetComponent(out NPC_Create_Trash create_Trash))
        {
            create_Trash.enabled = false;
        }
        if (cloneTrash.TryGetComponent(out NPC_Random_Mesh random_Mesh))
        {
            random_Mesh.enabled = false;
        }
        
    }
    private void SetRagdollKinematic(bool value)
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = value;
            rb.useGravity = !value;
        }
    }
}
