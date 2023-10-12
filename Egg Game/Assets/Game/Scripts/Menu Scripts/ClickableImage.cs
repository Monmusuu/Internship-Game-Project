using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickableImage : MonoBehaviour
{
    public UnityEvent onClick;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SimulateClick()
    {
        onClick.Invoke();
    }
}