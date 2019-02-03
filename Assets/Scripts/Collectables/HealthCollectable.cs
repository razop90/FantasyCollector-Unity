using Assets.Scripts;
using Assets.Scripts.Collectables;
using UnityEngine;

public class HealthCollectable : Collectable
{
    protected override void DisplayPickedData(int pickedAmount)
    {
        base.DisplayPickedData(pickedAmount);

        FloatingTextHandler.CreateFloatingText(transform.position, "+" + pickedAmount.ToString(), Color.green, 25);
    }
}
