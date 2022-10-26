using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class Keypad : MonoBehaviour
{
    [SerializeField] private int maxDigits;
    [SerializeField] private int combination;
    [SerializeField] private TextMeshProUGUI screen;
    public void KeypadDigitInput(int number)
    {
        if (screen.text.ToCharArray().Length < maxDigits)
        {
            StringBuilder sb = new();
            sb.Append(screen.text).Append(number);
            screen.text = sb.ToString();
        }
    }

    public void KeypadDeleteInput()
    {
        if (screen.text != null && screen.text != "")
        {
            screen.text = screen.text.Remove(screen.text.Length - 1, 1);
        }
    }

    public void KeypadEnterInput()
    {
        StringBuilder sb = new();
        sb.Append("FUCK");
        if (screen.text.Equals(combination.ToString()))
        {
            sb.Append(" YEAH");
        }
        sb.Append("!!!");
        Debug.Log(sb.ToString());
    }
}
