
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitsSort : MonoBehaviour
{
    private int grabLimit;
    private RaycastHit[] currentHits;
    private RaycastHit[] lastHits;
    private Trash trash;
    public RaycastHit[] SortingHits(RaycastHit[] hits, Vector3 hitPoint, int layerNum)
    {
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
        Debug.Log($"가장 가까운 레이어: {LayerMask.LayerToName(layerNum)}");

        // 해당 레이어와 일치하는 요소만 선택
        RaycastHit[] sameLayerHits = Array.FindAll(hits, h => h.collider.gameObject.layer == layerNum);

        int finalCount = Mathf.Min(sameLayerHits.Length, grabLimit);

        Debug.Log("sameLayerHits 개수 : " + sameLayerHits.Length);
        Debug.Log($"--- 결과 출력 (총 {finalCount}개) ---");
        RaycastHit[] finalCountLayerHits = new RaycastHit[finalCount];
        for (int i = 0; i < finalCount; i++)
        {
            //Debug.Log($"{i + 1}위: {sameLayerHits[i].collider.gameObject.name}, 거리: {sameLayerHits[i].distance:F2}");
            finalCountLayerHits[i] = sameLayerHits[i];
        }
        Debug.Log("finalCountLayerHits 개수 : " + finalCountLayerHits.Length);
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
                currentHitsOnOutline();
                lastHitsOffOutline();
            }
            lastHits = currentHits;
        }
    }
    public void currentHitsOnOutline()
    {
        for (int i = 0; i < currentHits.Length; i++)
        {
            currentHits[i].collider.gameObject.TryGetComponent<Trash>(out trash);
            trash.onOutline();
        }
    }
    public void lastHitsOffOutline()
    {
        if(lastHits == null)
        {
            return;
        }
        for (int i = 0; i < lastHits.Length; i++)
        {
            if(lastHits[i].collider == null || !lastHits[i].collider.gameObject.TryGetComponent<Trash>(out trash))
            {
                return;
            }
            trash.offOutline();
        }
        lastHits = null;
    }
}
