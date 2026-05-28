using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(Rigidbody))]
public class PickableBox : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;

    [Networked] private NetworkBool IsHeld { get; set; }
    [Networked] private PlayerRef Holder { get; set; }

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void Spawned()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        if (IsHeld && Runner.TryGetPlayerObject(Holder, out NetworkObject playerObj))
        {
            SimplePlayer player = playerObj.GetComponent<SimplePlayer>();
            if (player != null)
            {
                rb.isKinematic = true;
                transform.position = player.HoldPointPosition;
                transform.rotation = Quaternion.LookRotation(player.transform.forward, Vector3.up);
            }
        }
    }

    public void PickUp(PlayerRef holder)
    {
        if (!Object.HasStateAuthority)
            return;

        Holder = holder;
        IsHeld = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    public void Drop(Vector3 impulse)
    {
        if (!Object.HasStateAuthority) return;

        Holder = default;
        IsHeld = false;


        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.AddForce(impulse, ForceMode.VelocityChange);
        }
    }
}