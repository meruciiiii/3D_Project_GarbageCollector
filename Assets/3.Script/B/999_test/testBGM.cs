using UnityEngine;

public class testBGM : MonoBehaviour
{
    private void Start()
    {
        AudioManager.instance.PlayBGM("BGM1");
        AudioManager.instance.PlaySFX("SFX1");
    }
}
