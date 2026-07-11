using UnityEngine;

public class ItemData : MonoBehaviour
{
    public Sprite itemIcon;
    public string itemName;

    // ПОЗА В РУКЕ (для каждого предмета своя)
    public Vector3 holdPosition = Vector3.zero;   // смещение относительно HoldPoint
    public Vector3 holdRotation = Vector3.zero;   // поворот относительно HoldPoint
}