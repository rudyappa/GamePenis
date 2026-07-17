using UnityEngine;
using Fusion;

public class PickableObject : NetworkBehaviour, IStateAuthorityChanged
{
    [Networked] public NetworkBool IsHeld { get; set; }
    [Networked] public PlayerRef HolderPlayer { get; set; }

    private Rigidbody rb;
    private Collider col;
    private NetworkTransform _netTransform;
    private ChangeDetector _changeDetector;
    private Transform _localHoldPoint;
    private bool _pendingPickup;
    private bool _pendingPlaceDown;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        _netTransform = GetComponent<NetworkTransform>();
    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            if (change == nameof(IsHeld))
            {
                ApplyHeldVisuals();
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (IsHeld && Object.HasStateAuthority && _localHoldPoint != null)
        {
            transform.position = _localHoldPoint.position;
            transform.rotation = _localHoldPoint.rotation;
        }
    }

    public void PickUp(Transform holdPoint)
    {
        _localHoldPoint = holdPoint;

        if (Object.HasStateAuthority)
        {
            DoPickUp();
        }
        else
        {
            _pendingPickup = true;
            Object.RequestStateAuthority();
        }
    }

    public void PlaceDown()
    {
        if (Object.HasStateAuthority)
        {
            DoPlaceDown();
        }
        else
        {
            _pendingPlaceDown = true;
            Object.RequestStateAuthority();
        }
    }

    // Вызывается автоматически Fusion'ом, когда права реально перешли к нам
    public void StateAuthorityChanged()
    {
        if (Object.HasStateAuthority)
        {
            if (_pendingPickup)
            {
                _pendingPickup = false;
                DoPickUp();
            }
            if (_pendingPlaceDown)
            {
                _pendingPlaceDown = false;
                DoPlaceDown();
            }
        }
    }

    private void DoPickUp()
    {
        IsHeld = true;
        HolderPlayer = Runner.LocalPlayer;
        ApplyHeldVisuals();
    }

    private void DoPlaceDown()
    {
        IsHeld = false;
        HolderPlayer = PlayerRef.None;
        _localHoldPoint = null;
        ApplyHeldVisuals();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void ApplyHeldVisuals()
    {
        if (col != null) col.isTrigger = IsHeld;

        if (IsHeld)
        {
            if (rb != null) { rb.isKinematic = true; rb.useGravity = false; }
        }
    }
}