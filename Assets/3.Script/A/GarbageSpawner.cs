using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GarbageSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject[] prefabsToSpawn;

    [Header("Spawn Settings")]
    public int spawnCount = 10;

    void Start()
    {
        // 1. 렌더러만 비활성화 (오브젝트는 파괴하지 않음)
        if (TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
        {
            meshRenderer.enabled = false;
        }
        SpawnObjects();
    }

    void SpawnObjects()
    {
        if (prefabsToSpawn.Length == 0) return;

        // Plane의 기본 크기는 10x10입니다. 
        // 따라서 Scale에 5를 곱한 값이 중심으로부터의 거리가 됩니다.
        float halfWidth = transform.lossyScale.x * 5f;
        float halfLength = transform.lossyScale.z * 5f;

        for (int i = 0; i < spawnCount; i++)
        {
            int randomIndex = Random.Range(0, prefabsToSpawn.Length);

            // Plane의 중심(transform.position) 기준 랜덤 위치 계산
            float randomX = Random.Range(-halfWidth, halfWidth);
            float randomZ = Random.Range(-halfLength, halfLength);

            // 높이(Y)는 Plane의 현재 높이로 고정
            Vector3 spawnPosition = transform.position + new Vector3(randomX, 0, randomZ);

            Instantiate(prefabsToSpawn[randomIndex], spawnPosition, Quaternion.identity);
        }
    }
}
