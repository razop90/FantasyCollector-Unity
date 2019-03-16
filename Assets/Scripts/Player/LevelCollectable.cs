using System;
using System.Collections;
using UnityEngine;

public class LevelCollectable : MonoBehaviour
{
    public bool isBossItemCollected { get; private set; }

    public string bossItemTagName = "BossItem";
    public AudioClip collectedSound;

    private GameObject item;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == bossItemTagName)
        {
            GameManager.instance.playerIsPickingAnItem = true;

            if (audioSource != null && collectedSound != null)
                audioSource.PlayOneShot(collectedSound);

            item = other.gameObject;
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

            StartCoroutine(AnimationYield(pickingAnimLength)); //called after the animation is finished.
            StartCoroutine(CollectYield(pickingAnimLength - 0.1f)); //called few milisec before the animation is finished.
        }
    }

    public void SetCollectableValue(bool value)
    {
        isBossItemCollected = value;
    }

    private IEnumerator AnimationYield(float waiting = 0.3f)
    {
        yield return new WaitForSeconds(waiting);
        GameManager.instance.playerIsPickingAnItem = false;
    }

    private IEnumerator CollectYield(float waiting = 0.3f)
    {
        yield return new WaitForSeconds(waiting);
        isBossItemCollected = true;
        Destroy(item);
    }
}
