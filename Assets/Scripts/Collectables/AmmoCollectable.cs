using Assets.Scripts;
using Assets.Scripts.Collectables;
using UnityEngine;

public class AmmoCollectable : Collectable
{
    protected override void DisplayPickedData(int pickedAmount)
    {
        base.DisplayPickedData(pickedAmount);

        FloatingTextHandler.CreateFloatingText(transform.position, "+" + pickedAmount.ToString(), Color.cyan, 25);
    }
}
