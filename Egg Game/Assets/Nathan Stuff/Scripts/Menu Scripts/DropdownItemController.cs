using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropdownItemController : MonoBehaviour
{
    public GameObject passwordInputField; // Reference to the password input field (4th child)

    // Call this method to activate the password input field
    public void ActivatePasswordInputField()
    {
        if (passwordInputField != null)
        {
            passwordInputField.SetActive(true);
        }
    }
}
