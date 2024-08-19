using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    private float timeBetweenShots;
    private float timeSinceLastShot = 0.0f;
    bool shooting;
    // Start is called before the first frame update
    void Start()
    {
        timeBetweenShots = 1 / (700 / 60.0f);
    }
    public void GetShootInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            shooting = true;
        }
        if (context.canceled)
        {
            shooting = false;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        timeSinceLastShot += Time.deltaTime;
        if ( shooting && timeSinceLastShot >= timeBetweenShots)
        {
            GameObject bullet = BulletPool.poolInstance.GetPlayerPooledObject();
            if (bullet != null)
            {
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.SetActive(true);
            }
            timeSinceLastShot = 0.0f;
        }
    }
}
