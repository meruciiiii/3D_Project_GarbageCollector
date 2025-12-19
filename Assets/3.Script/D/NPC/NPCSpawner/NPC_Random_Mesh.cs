using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PrivateLT.CharacterCustomization;

public class NPC_Random_Mesh : MonoBehaviour {
    public Customization_Data customizationData;
    [SerializeField] private List<CustomizationPart> _customizationParts;

    private void Awake() {
        ApplyRandomLook();
    }

    public void ApplyRandomLook() {
        if (customizationData == null) return;

        foreach (var part in _customizationParts) {
            // [중요] 기존 코드의 이벤트를 강제로 정리하는 로직
            // Initialise를 하기 전에 미리 빼줘야 MissingReference를 피할 수 있습니다.
            var data = customizationData.CosmeticDatas.FirstOrDefault(x => x.Type == part.SkinnedMeshRenderer.name.Split('_')[0]);
            if (data != null) {
                // 이전 NPC들이 등록해놨을지 모를 이벤트들을 모두 제거 (안전장치)
                // 현재 구조상 '모든' 구독자가 빠지지만, NPC는 각자 Shuffle을 직접 호출하므로 문제없습니다.
                data.OnShuffle = null;
                data.OnRefresh = null;
                data.OnClear = null;
                data.OnSetMeshWithIndex = null;
            }

            // 이제 안전하게 초기화
            part.Initialise(customizationData);
        }

        Shuffle();
    }

    // NPC 전용 셔플 (기존과 동일)
    private void Shuffle(List<string> alreadyUsedParts = null) {
        alreadyUsedParts ??= new List<string>();
        List<CustomizationPart> parts = new();

        foreach (var p in _customizationParts) {
            if (alreadyUsedParts.Contains(p.Type)) continue;
            parts.Add(p);
        }

        if (parts.Count <= 0) return;

        var chosenPart = parts[Random.Range(0, parts.Count)];

        // 여기서 실제로 Mesh가 바뀌고, 내부에서 Unfittable 처리를 위해 다른 파츠의 Clear()를 호출할 수 있음
        chosenPart.Shuffle();
        alreadyUsedParts.Add(chosenPart.Type);

        if (!string.IsNullOrEmpty(chosenPart.CosmeticsData.UnfittableTypes)) {
            var unfittables = chosenPart.CosmeticsData.UnfittableTypes.Split(',').Select(s => s.Trim());
            foreach (var u in unfittables) {
                if (!alreadyUsedParts.Contains(u)) alreadyUsedParts.Add(u);
            }
        }

        Shuffle(alreadyUsedParts);
    }

    // NPC가 파괴될 때도 깔끔하게 비워줍니다.
    private void OnDestroy() {
        if (_customizationParts == null) return;
        foreach (var part in _customizationParts) {
            part.Close();
        }
    }
}