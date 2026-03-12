using UnityEngine;

public class MyDoor : MonoBehaviour
{
    [SerializeField] private MyButton button;
    private bool isOpen = false;

    void Start()
    {
        button.OnPressed += OpenDoor;
    }

    private void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;
        Debug.Log("문이 열린다.");
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
    }
}