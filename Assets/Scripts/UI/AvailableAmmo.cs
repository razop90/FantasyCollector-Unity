using UnityEngine;
using UnityEngine.UI;

public class AvailableAmmo : MonoBehaviour
{
    public int Ammo;
    public MaxAmmoAvailable maxAmmo;

    private Text textObj;
    private Color textColor;

    public void Start()
    {
        InitProps();
    }

    public void DecreaseAmmo(int amount)
    {
        InitProps();

        Ammo -= amount;
        if (Ammo <= 0)
        {
            textObj.color = Color.red;
            Ammo = 0;
        }

        textObj.text = Ammo.ToString();
    }

    public void IncreaseAmmo(int amount)
    {
        InitProps();

        Ammo += amount;
        if (Ammo > maxAmmo.Ammo)
        {
            Ammo = maxAmmo.Ammo;
        }

        textObj.color = textColor;
        textObj.text = Ammo.ToString();
    }

    public void SetAmount(int amount)
    {
        InitProps();

        Ammo = amount;
        if (Ammo > maxAmmo.Ammo)
        {
            Ammo = maxAmmo.Ammo;
        }

        textObj.color = textColor;
        textObj.text = Ammo.ToString();
    }

    public bool IsAmmoAvailable()
    {
        if (Ammo <= 0)
        {
            //play no ammo animation on text
            return false;
        }

        return true;
    }

    private void InitProps()
    {
        if (textObj == null)
        {
            textObj = GetComponent<Text>();

            textObj = GetComponent<Text>();
            textColor = textObj.color;
        }
    }
}
