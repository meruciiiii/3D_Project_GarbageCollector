
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitsSort : MonoBehaviour
{
    private int grabLimit = 1;
    public RaycastHit[] SortingHits(RaycastHit[] hits, Vector3 hitPoint)
    {
        if (hits == null || hits.Length == 0)
        {
            Debug.Log("RaycastHit에 입력된 대상이 없습니다.");
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
        int targetLayer = hits.collider.gameObject.layer;
        Debug.Log($"가장 가까운 레이어: {LayerMask.LayerToName(targetLayer)}");

        // 해당 레이어와 일치하는 요소만 선택
        RaycastHit[] sameLayerHits = Array.FindAll(hits, h => h.collider.gameObject.layer == targetLayer);

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
        return finalCountLayerHits;
    }
}
