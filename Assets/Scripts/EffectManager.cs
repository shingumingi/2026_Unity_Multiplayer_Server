using UnityEngine;

public class EffectManager : MonoBehaviour
{

    public static EffectManager instance { get; private set; }

    [Header("Effects")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject muzzleEffect;

    public void Awake()
    {
        instance = this;
    }

    public GameObject HitEffect => hitEffect;
    public GameObject MuzzleEffect => muzzleEffect;

    public void PlayLocalEffect(GameObject prefab, Vector3 pos, Vector3 normal)
    {
        if (prefab == null) return;

        Quaternion rot = normal.sqrMagnitude > 0.001f
            ? Quaternion.LookRotation(normal)
            : Quaternion.identity;

        GameObject fx = Instantiate(prefab, pos, rot);
        Destroy(fx, 2f);
    }

    public void PlayerWorldEffect(GameObject prefab, Vector3 pos, Vector3 normal)
    {
        PlayLocalEffect(prefab, pos, normal);
    }
}
