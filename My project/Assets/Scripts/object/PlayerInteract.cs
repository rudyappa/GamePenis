using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactRange = 3f;
    public Camera playerCamera;
    public Transform holdPoint;
    public Inventory inventory;

    void Update()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.red);

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
                    PickableObject heldPickable = held.GetComponent<PickableObject>();
                    heldPickable.PlaceDown();
                    inventory.ClearSelectedSlot();
                }
            }
        }
    }
}