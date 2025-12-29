
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitsSort : MonoBehaviour
{
    private int grabLimit;
    private RaycastHit[] currentHits = Array.Empty<RaycastHit>();
    private RaycastHit[] lastHits = Array.Empty<RaycastHit>();
    private Trash trash;
    public RaycastHit[] SortingHits(RaycastHit[] hits, RaycastHit hit, Vector3 hitPoint, int layerNum)
    {
        if (layerNum.Equals(7) || layerNum.Equals(9))
        {
            grabLimit = 1;
        }
        else
            grabLimit = GameManager.instance.grab_limit;
        if (hits == null || hits.Length == 0)
        {
            Debug.Log("RaycastHit에 입력된 대상이 없습니다.");
            if (lastHits != null)
            {
                lastHitsOffOutline();
            }
            return null;
        }
        // 거리 순으로 정렬 
        Array.Sort(hits, (hitsA, hitsB) =>
        {
            float distanceA = (hitPoint - hitsA.point).sqrMagnitude;
            float distanceB = (hitPoint - hitsB.point).sqrMagnitude;
            return distanceA.CompareTo(distanceB);
        });

        // 가장 가까운 요소의 레이어 파악
        //Debug.Log($"가장 가까운 레이어: {LayerMask.LayerToName(layerNum)}");

        // 해당 레이어와 일치하는 요소만 선택
        RaycastHit[] sameLayerHits = Array.FindAll(hits, h => h.collider.gameObject.layer == layerNum);

        int finalCount = Mathf.Min(sameLayerHits.Length, grabLimit);

        //Debug.Log("sameLayerHits 개수 : " + sameLayerHits.Length);
        //Debug.Log($"--- 결과 출력 (총 {finalCount}개) ---");
        RaycastHit[] finalCountLayerHits = new RaycastHit[finalCount];
        finalCountLayerHits[0] = hit;
        for (int i = 1; i < finalCount; i++)
        {
            //Debug.Log($"{i + 1}위: {sameLayerHits[i].collider.gameObject.name}, 거리: {sameLayerHits[i].distance:F2}");
            finalCountLayerHits[i] = sameLayerHits[i];
        }
        //Debug.Log("finalCountLayerHits 개수 : " + finalCountLayerHits.Length);
        lastHistCheck(finalCountLayerHits);
        return finalCountLayerHits;
    }
    public void lastHistCheck(RaycastHit[] finalCountLayerHits)
    {
        currentHits = finalCountLayerHits;
        if (lastHits == null)
        {
            currentHitsOnOutline();
            lastHits = currentHits;
        }
        else
        {
            if (lastHits == currentHits)
            {
            }
            else
            {
                lastHitsOffOutline();
                currentHitsOnOutline();
            }
            lastHits = currentHits;
        }
    }
    public void currentHitsOnOutline()
    {
        int playerBag = GameManager.instance.P_Maxbag;
        int nowWeight = GameManager.instance.P_Weight;
        for (int i = 0; i < currentHits.Length; i++)
        {
            if (currentHits[i].collider.gameObject.TryGetComponent<Outline>(out Outline outline))
            {
                if (Checking(currentHits[i], playerBag, nowWeight))
                {
                    ChangeGray(outline);
                    if (currentHits[i].collider.TryGetComponent<IInteractable>(out var interactable))
                    {
                        nowWeight += interactable.TrashWeight();
                    }
                }
                else
                {
                    ChangeRed(outline);
                }
                outline.enabled = true;
            }
        }
    }
    public void lastHitsOffOutline()
    {
        for (int i = 0; i < lastHits.Length; i++)
        {
            if (lastHits[i].collider.gameObject.TryGetComponent<Outline>(out Outline outline))
            {
                outline.enabled = false;
            }
        }
    }
    public void CatchandEmpty()
    {
        currentHits = Array.Empty<RaycastHit>();
        lastHits = Array.Empty<RaycastHit>();
    }
    public void ChangeRed(Outline outline)
    {
        outline.OutlineColor = new Color(130 / 255f, 0f, 0f);
    }
    public void ChangeGray(Outline outline)
    {
        outline.OutlineColor = new Color(130 / 255f, 130 / 255f, 130 / 255f);
    }
    public bool Checking(RaycastHit check, int Bag, int now)
    {

        if (check.collider.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            now += interactable.TrashWeight();
        }
        else
        {
            Debug.Log("IInteractable 컴포넌트가 없습니다.");
        }
        if (now < Bag)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}