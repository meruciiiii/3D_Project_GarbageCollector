using UnityEngine;

public class test3DSFX : MonoBehaviour
{
    private void Start()
    {
        AudioManager.instance.Play3DSFX("SFX3D1", this.transform);
    }
}
