using Fusion;
using UnityEngine;

public class SimplePlayer : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;

    [Header("Bullet")]
    [SerializeField] private NetworkPrefabRef bulletPrefab;
    [SerializeField] private Transform firePoint;

    [SerializeField] private float fireDistance = 20f;
    [SerializeField] private LayerMask hitMask;

    [Networked] private TickTimer FireCooldown { get; set; }
    [SerializeField] private float fireInterval = 0.2f;

    public override void FixedUpdateNetwork()
    {
        if (GetInput<FusionBootstrap.NetworkInputData>(out var inputData))
        {
            Vector3 move = new Vector3(inputData.move.x, 0f, inputData.move.y);

            if (move.sqrMagnitude > 1f)
                move.Normalize();

            transform.position += move * moveSpeed * Runner.DeltaTime;

            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotateSpeed * Runner.DeltaTime
                );
            }
        }

        // 발사
        if (inputData.buttons.IsSet((int)FusionBootstrap.InputButton.Fire))
        {
            if (FireCooldown.ExpiredOrNotRunning(Runner))
            {
                FireLagCompensated();
                FireCooldown = TickTimer.CreateFromSeconds(Runner, fireInterval);
            }
        }
    }

    private void Fire()
    {
        if (!Object.HasStateAuthority)
            return;

        Vector3 spawnPos = firePoint != null
            ? firePoint.position
            : transform.position + transform.forward + Vector3.up * 0.5f;

        Quaternion spawnRot = transform.rotation;

        NetworkObject bulletObj = Runner.Spawn(
            bulletPrefab,
            spawnPos,
            spawnRot,
            Object.InputAuthority
        );

        SampleBullet bullet = bulletObj.GetComponent<SampleBullet>();
        if(bullet != null)
        {
            bullet.Init(Object.InputAuthority);
        }
    }

    private void FireLagCompensated()
    {
        if (!Object.HasStateAuthority)
            return;

        Vector3 origin = firePoint != null ? firePoint.position : transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward;

        // Runner -> NetworkRunner => 게임의 네트워크 전체를 관리하는 관리자
        if (Runner.LagCompensation.Raycast(
            origin,
            direction,
            fireDistance,
            Object.InputAuthority,
            out LagCompensatedHit hit,
            hitMask
        ))
        {
            Debug.Log($"LagComp Hit : {hit.Hitbox.name}");
            RPC_PlayHitEffect(hit.Point, hit.Normal);
            Hitbox hitbox = hit.Hitbox;
            if(hitbox != null)
            {
                HealthTarget target = hitbox.GetComponentInParent<HealthTarget>();
                if (target != null)
                {
                    target.TakeDamage(1);
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayHitEffect(Vector3 pos, Vector3 normal)
    {
        if (EffectManager.instance == null) return;
        EffectManager.instance.PlayerWorldEffect(EffectManager.instance.HitEffect, pos, normal);
    }
}
