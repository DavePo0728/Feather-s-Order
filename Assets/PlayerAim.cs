using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAim : MonoBehaviour
{
    [SerializeField]
    Camera playerCamera;
    [SerializeField]
    GameObject aimmingImage1, aimmingImage2, aimmingImage3,lockImage;
    [SerializeField]
    GameObject lockedEnemy;
    [SerializeField]
    GameObject emptyAimObject1, emptyAimObject2, emptyAimObject3;
    public GameObject _lockedEnemy => lockedEnemy;
    float lockCountTimer;
    //[SerializeField]
    //float fillDuration;
    //[SerializeField]
    //float scaleDuration;
    //[SerializeField]
    //float aimmedScale;
    bool aimmed = false;
    [SerializeField]
    LayerMask raycastIgnore;
    private void Awake()
    {
        //RaycastIgnore = LayerMask.GetMask("Bullet");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        aimmingImage1.transform.position = playerCamera.WorldToScreenPoint(emptyAimObject1.transform.position);
        aimmingImage2.transform.position = playerCamera.WorldToScreenPoint(emptyAimObject2.transform.position);
        aimmingImage3.transform.position = playerCamera.WorldToScreenPoint(emptyAimObject3.transform.position);
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(emptyAimObject3.transform.position), out hit, Mathf.Infinity, raycastIgnore))
        {

            if(hit.collider.gameObject.tag == "Enemy")
            {
                Debug.DrawRay(transform.position, emptyAimObject3.transform.position * hit.distance, Color.red);
                lockedEnemy = hit.collider.gameObject;
                lockImage.SetActive(true);
                lockImage.transform.position = playerCamera.WorldToScreenPoint(lockedEnemy.transform.position);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(emptyAimObject3.transform.position), Color.green);
            lockedEnemy = null;
            lockImage.SetActive(false);
        }

    }
    //IEnumerator FillAndScale()
    //{
    //    aimmingImage.transform.localScale = Vector3.one;
    //    // Fill the image
    //    float elapsedTime = 0f;
    //    while (elapsedTime < fillDuration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        aimmingImage.fillAmount = Mathf.Clamp01(elapsedTime / fillDuration);
    //        yield return null;
    //    }

    //    // Ensure fill amount is exactly 1
    //    aimmingImage.fillAmount = 1f;

    //    elapsedTime = 0f;
    //    while (elapsedTime < scaleDuration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        float scale = Mathf.Lerp(1f, 1.2f, elapsedTime / (scaleDuration / 2));
    //        aimmingImage.transform.localScale = Vector3.one * scale;
    //        yield return null;
    //    }

    //    elapsedTime = 0f;
    //    while (elapsedTime < scaleDuration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        float scale = Mathf.Lerp(1.2f, 0.9f, elapsedTime / (scaleDuration / 2));
    //        aimmingImage.transform.localScale = Vector3.one * scale;
    //        yield return null;
    //    }

    //    // Ensure the scale is exactly 0.9
    //    aimmingImage.transform.localScale = Vector3.one * 0.9f;
    //    aimmed = true;
    //}
}
