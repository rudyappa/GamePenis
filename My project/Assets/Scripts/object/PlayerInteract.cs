using UnityEngine;
using Fusion;

public class PlayerInteract : NetworkBehaviour
{
    public float interactRange = 3f;
    public Camera playerCamera;
    public Transform holdPoint;
    public Inventory inventory;

    void Update()
    {
        if (Object != null && !Object.HasInputAuthority) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        bool lookingAtPickable = false;
        GameObject target = null;
        PickableObject pickable = null;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            target = hit.collider.gameObject;
            pickable = target.GetComponent<PickableObject>();
            lookingAtPickable = pickable != null;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (lookingAtPickable)
            {
                inventory.AddItem(target, pickable);
            }
            else
            {
                GameObject held = inventory.GetSelectedItem();
                if (held != null)
                {
                    held.GetComponent<PickableObject>().PlaceDown();
                    inventory.ClearSelectedSlot();
                }
            }
        }
    }
}