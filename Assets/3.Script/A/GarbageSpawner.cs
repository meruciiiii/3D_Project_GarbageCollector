using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GarbageSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject[] prefabsToSpawn;

    [Tooltip("스폰된 오브젝트들이 들어갈 부모 오브젝트입니다. 비워두면 이 오브젝트의 자식으로 들어갑니다.")]
    public Transform parentTransform;

    [Header("Spawn Settings")]
    public int spawnCount = 10;

    void Start()
    {
        // 부모를 지정하지 않았다면 이 스크립트가 붙은 오브젝트를 부모로 설정
        if (parentTransform == null)
        {
            parentTransform = this.transform;
        }

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

            // 2. 랜덤 회전값 생성 
            Quaternion randomRotation = Quaternion.Euler(
                Random.Range(0f, 360f),
                Random.Range(0f, 360f),
                Random.Range(0f, 360f)
            );

            Instantiate(prefabsToSpawn[randomIndex], spawnPosition, randomRotation, parentTransform);
        }
    }
}
