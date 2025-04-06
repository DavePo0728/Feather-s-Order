using UnityEngine;
using System.Collections;

/// <summary>
/// Creates a glowing afterimage effect using a cloned mesh and custom material.
/// </summary>
namespace AfterimageFX
{
    public class GlowEffect : MonoBehaviour, IAfterimageEffect
    {
        private MeshRenderer meshRenderer;
        private Material[] afterimageMaterials;

        [Tooltip("Material used for the afterimage effect.")]
        public Material afterimageMaterial;

        [Tooltip("Starting value for the _Power shader property.")]
        public float StartPower = 0.5f;

        [Tooltip("Ending value for the _Power shader property.")]
        public float EndPower = 1.5f;

        [Tooltip("Optional scale for the afterimage clone.")]
        public Vector3 CloneScale = Vector3.one;

        /// <summary>
        /// Initializes the afterimage using a mesh snapshot and applies fading over time.
        /// </summary>
        /// <param name="snapshotMesh">Mesh used for the afterimage.</param>
        /// <param name="lifetime">Duration before the afterimage fades out and is destroyed.</param>
        public void InitializeAfterimage(Mesh snapshotMesh, float lifetime)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            MeshFilter meshFilter = GetComponent<MeshFilter>();

            if (meshRenderer == null || meshFilter == null)
            {
                Debug.LogError("GlowEffect requires both MeshRenderer and MeshFilter!");
                return;
            }

            // Set the scale for the clone
            transform.localScale = CloneScale;

            // Apply the snapshot mesh to the clone
            meshFilter.mesh = snapshotMesh;

            int submeshCount = snapshotMesh.subMeshCount;
            afterimageMaterials = new Material[submeshCount];

            // Clone the afterimage material for each submesh
            for (int i = 0; i < submeshCount; i++)
            {
                afterimageMaterials[i] = new Material(afterimageMaterial);
            }

            meshRenderer.materials = afterimageMaterials;

            // Start fading coroutine and destroy the object after the lifetime expires
            StartCoroutine(FadeOut(lifetime));
            Destroy(gameObject, lifetime);
        }

        /// <summary>
        /// Gradually fades out the afterimage by adjusting shader properties.
        /// </summary>
        /// <param name="lifetime">How long the fade should last.</param>
        private IEnumerator FadeOut(float lifetime)
        {
            float time = 0f;
            Vector3 initialScale = transform.localScale;
            Vector3 targetScale = initialScale * 0.8f;

            while (time < lifetime)
            {
                float t = time / lifetime;

                float alpha = Mathf.Lerp(1f, 0f, t);
                float power = Mathf.Lerp(StartPower, EndPower, t);
                float colorpos = Mathf.Lerp(0f, 1f, t);
                Vector3 currentScale = Vector3.Lerp(initialScale, targetScale, t);

                transform.localScale = currentScale;

                foreach (Material mat in afterimageMaterials)
                {
                    if (mat.HasProperty("_Alpha"))
                        mat.SetFloat("_Alpha", alpha);

                    if (mat.HasProperty("_Color_Position"))
                        mat.SetFloat("_Color_Position", colorpos);

                    if (mat.HasProperty("_Power"))
                        mat.SetFloat("_Power", power);
                }

                time += Time.deltaTime;
                yield return null;
            }
        }

    }
}