using System;
using System.Collections;
using UnityEngine;

public class TreasureBoxDrop : MonoBehaviour
{
    public event Action<string> OnDropCollected;

    public bool isCollected { get; private set; }

    private AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.instance.playerIsPickingAnItem = true;

            AnimatorClipInfo[] clipInfos = null;
            float pickingAnimLength = 0f;
            try
            {
                GameManager.instance.playerAnimator.SetTrigger("pickup");
                clipInfos = GameManager.instance.playerAnimator.GetCurrentAnimatorClipInfo(0);
            }
            catch (Exception e) { Debug.Log(e.Message); }

            if (clipInfos != null)
                pickingAnimLength = clipInfos[0].clip.length;

            StartCoroutine(CollectYield(pickingAnimLength - 0.1f)); //called few milisec before the animation is finished.
        }
    }

    public void SetCollectableValue(bool value)
    {
        isCollected = value;
    }

    private IEnumerator CollectYield(float waiting = 0.3f)
    {
        yield return new WaitForSeconds(waiting);
        isCollected = true;

        if (OnDropCollected != null)
            OnDropCollected.Invoke(gameObject.tag);

        GameManager.instance.playerIsPickingAnItem = false;
        Destroy(gameObject);
    }
}
