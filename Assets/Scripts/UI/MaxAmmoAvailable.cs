using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxAmmoAvailable : MonoBehaviour
{
    public int Ammo;

    private Text textObj;

    public void SetAmount(int amount)
    {
        if (textObj == null)
            textObj = GetComponent<Text>();

        Ammo = amount;
        textObj.text = amount.ToString();
    }
}
