using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField]
    PlayerSlashAttack playerSlashAttack;
    [SerializeField]
    GunShoot gunPoint1, gunPoint2;
    [SerializeField]
    SpriteRenderer gunPoint1Img, gunPoint2Img;
	[SerializeField]
	MeshRenderer gunPoint1MeshRenderer, gunPoint2MeshRenderer;
	private float timeBetweenShots;
    private float timeSinceLastShot = 0.0f;
    [SerializeField]
    AudioSource gunSound;
    bool shooting;
    float timeSinceLastShooting = 0f;   
    void Start()
    {
        timeBetweenShots = 1 / (800 / 60.0f);
        gunPoint1Img.enabled = false;
        gunPoint2Img.enabled = false;
        gunSound = GetComponent<AudioSource>();
    }

    public void GetShootInput(InputAction.CallbackContext context)
    {
        if (context.performed&&playerSlashAttack.slashState == PlayerSlashAttack.SlashState.Idle)
        {
            StartShooting();
        }
        if (context.canceled && playerSlashAttack.slashState == PlayerSlashAttack.SlashState.Idle)
        {
            StopShooting();
        }
    }
    public void StartShooting()
    {
        //Debug.Log(playerSlashAttack.slashState);
		//Debug.Log("StartShooting");
		shooting = true;
        gunPoint1Img.enabled = true;
		gunPoint2Img.enabled = true;
		StartCoroutine(AnimateRangeUp(gunPoint1MeshRenderer.material, 0.2f));
		StartCoroutine(AnimateRangeUp(gunPoint2MeshRenderer.material, 0.2f));
	}
    public void StopShooting()
    {
        //Debug.Log("StopShooting");
		shooting = false;
        gunPoint1Img.enabled = false;

		gunPoint2Img.enabled = false;
		StartCoroutine(AnimateRangeDown(gunPoint1MeshRenderer.material, 0.2f));
		StartCoroutine(AnimateRangeDown(gunPoint2MeshRenderer.material, 0.2f));
		gunSound.Stop();
    }
    void FixedUpdate()
    {
        timeSinceLastShot += Time.deltaTime;
        //Debug.Log("Shooting: "+shooting);
        if (shooting && timeSinceLastShot >= timeBetweenShots)
        {
            if (playerSlashAttack != null)
            {
                if (playerSlashAttack.slashState == PlayerSlashAttack.SlashState.Dashing
                    || playerSlashAttack.slashState == PlayerSlashAttack.SlashState.Arrived
                    || playerSlashAttack.slashState == PlayerSlashAttack.SlashState.Attacking)
                {
                    //Debug.Log("StopShooting");
                    StopShooting();
                    return;
                }
            }
            gunSound.Play();
            gunPoint1.Shoot();
            gunPoint2.Shoot();
            timeSinceLastShot = 0.0f;
        }
    }
	// <summary>
	/// 在指定的時間內將材質的「_Range」屬性從 0 動畫化為 1。
	public IEnumerator AnimateRangeDown(Material material, float duration)
	{
       //Debug.Log("AnimateRangeDown");
		float elapsed = 0f;
		while (elapsed < duration)
		{
			float value = Mathf.Lerp(0f, 1f, elapsed / duration);
			material.SetFloat("_Range", value);
			elapsed += Time.deltaTime;
			yield return null;
		}
		material.SetFloat("_Range", 1f); // 確保最後設為1
	}
	// <summary>
	///在指定的時間內將材質的「_Range」屬性從 1 動畫化為 0。
	public IEnumerator AnimateRangeUp(Material material, float duration)
	{
        //Debug.Log("AnimateRangeUp");
		float elapsed = 0f;
		while (elapsed < duration)
		{
			float value = Mathf.Lerp(1f, 0f, elapsed / duration);
			material.SetFloat("_Range", value);
			elapsed += Time.deltaTime;
			yield return null;
		}
		material.SetFloat("_Range", 0f); // 確保最後設為0
	}
}
