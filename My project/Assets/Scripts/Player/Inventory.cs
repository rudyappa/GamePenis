using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int slotCount = 5;
    public Transform holdPoint;

    // Ссылки на UI-иконки (перетащишь в инспекторе)
    public Image[] slotIcons;

    private GameObject[] slots;
    public int SelectedSlot { get; private set; } = 0;

    void Awake()
    {
        slots = new GameObject[slotCount];
    }

    void Update()
    {
        // Выбор колёсиком мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) SelectSlot((SelectedSlot + 1) % slotCount);
        else if (scroll < 0f) SelectSlot((SelectedSlot - 1 + slotCount) % slotCount);

        // Выбор цифрами 1-5
        for (int i = 0; i < slotCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SelectSlot(i);
        }
    }

    // Добавить предмет в первый свободный слот
    public bool AddItem(GameObject item, PickableObject pickable)
    {
        for (int i = 0; i < slotCount; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = item;
                pickable.PickUp(holdPoint);
                item.SetActive(i == SelectedSlot);
                UpdateSlotUI(i); // обновляем иконку
                return true;
            }
        }
        return false; // инвентарь полон
    }

    public GameObject GetSelectedItem()
    {
        return slots[SelectedSlot];
    }

    public void ClearSelectedSlot()
    {
        slots[SelectedSlot] = null;
        UpdateSlotUI(SelectedSlot); // обновляем UI
    }

    public GameObject GetSlot(int index) => slots[index];

    void SelectSlot(int index)
    {
        // Скрываем текущий слот
        if (slots[SelectedSlot] != null)
            slots[SelectedSlot].SetActive(false);

        SelectedSlot = index;
        ShowSlot(index);
    }

    void ShowSlot(int index)
    {
        if (slots[index] != null)
            slots[index].SetActive(true);
    }

    // ==== НОВЫЙ МЕТОД ДЛЯ ОБНОВЛЕНИЯ UI ====
    void UpdateSlotUI(int slotIndex)
    {
        if (slotIcons == null || slotIndex >= slotIcons.Length) return;

        GameObject item = slots[slotIndex];
        if (item != null)
        {
            // Пытаемся найти компонент ItemData
            ItemData data = item.GetComponent<ItemData>();
            if (data != null && data.itemIcon != null)
            {
                slotIcons[slotIndex].sprite = data.itemIcon;
                slotIcons[slotIndex].color = Color.white;
                slotIcons[slotIndex].enabled = true;
            }
            else
            {
                // Если нет ItemData — используем цвет кубика
                Renderer rend = item.GetComponent<Renderer>();
                if (rend != null)
                {
                    slotIcons[slotIndex].sprite = null;
                    slotIcons[slotIndex].color = rend.material.color;
                    slotIcons[slotIndex].enabled = true;
                }
            }
        }
        else
        {
            // Слот пуст — убираем иконку
            slotIcons[slotIndex].sprite = null;
            slotIcons[slotIndex].enabled = false;
        }
    }
}