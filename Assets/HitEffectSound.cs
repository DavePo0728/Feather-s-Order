using UnityEngine;

public class HitEffectSound : MonoBehaviour
{
    public AudioClip hitSFX;
    public float volume = 1f;

    void OnEnable()
    {
        if (hitSFX != null)
        {
            AudioSource.PlayClipAtPoint(hitSFX, transform.position, volume);
        }
    }
}
