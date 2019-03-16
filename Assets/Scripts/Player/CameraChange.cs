using System.Collections;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
    public GameObject ThirdCam;
    public GameObject BackCam;
    public int CamMode;
    public BasicBehaviour behaviourManager;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Camera"))
        {
            if (CamMode == 1)
            {
                CamMode = 0;
            }
            else
            {
                CamMode += 1;
            }
            StartCoroutine(CamChange());
        }
    }

    IEnumerator CamChange()
    {
        yield return new WaitForSeconds(0.01f);

        if (!GameManager.instance.aim.aim)
        {
            if (CamMode == 0)
            {
                behaviourManager.playerCamera = ThirdCam.transform;
                GameManager.instance.aim.isAimEnabled = true;

                ThirdCam.SetActive(true);
                BackCam.SetActive(false);
            }
            if (CamMode == 1)
            {
                behaviourManager.playerCamera = BackCam.transform;
                GameManager.instance.aim.isAimEnabled = false;

                BackCam.SetActive(true);
                ThirdCam.SetActive(false);
            }

            behaviourManager.OnMainCameraChanged();
        }
    }


}
