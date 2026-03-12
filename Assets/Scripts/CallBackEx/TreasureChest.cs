using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    private bool isOpen = false;

    public void OpenChest()
    {
        if (isOpen) return;
        Debug.Log("보물 상자가 열렸다.");
        transform.rotation = Quaternion.Euler(-30f, 0f, 0f);
    }
}
