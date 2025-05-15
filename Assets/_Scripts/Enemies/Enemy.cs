using UnityEngine;

namespace _Scripts.Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        [Header("Attack Variables")]
        [SerializeField]
        protected float detectionRange = 20f;
        
        [Header("Damage Animation")]
        [SerializeField]
        private float flashDuration = 0.3f;
        [SerializeField]
        private Color flashColor =new (0.6f, 0.1f, 0.1f, 1f);

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int SecondaryColor = Shader.PropertyToID("_Color");
        
        private Renderer[] renderers;
        private Color[] originalColors;
        protected Transform Player;

        private void Awake()
        {
            InitRenderers();
            FindReferences();
        }
        
        private void InitRenderers()
        {
            renderers = GetComponentsInChildren<Renderer>();
            originalColors = new Color[renderers.Length];

            for (var i = 0; i < renderers.Length; i++)
                originalColors[i] = GetRendererColor(renderers[i]);
        }

        private static Color GetRendererColor(Renderer renderer)
        {
            var material = renderer.material;
            if (material.HasProperty(BaseColor))
            {
                return material.GetColor(BaseColor);
            }
            return material.HasProperty(SecondaryColor) ? material.GetColor(SecondaryColor) : Color.white;
        }
        
        public void FlashRed()
        {
            foreach (var objectRenderer in renderers)
                SetRendererColor(objectRenderer, flashColor);

            Invoke(nameof(RestoreOriginalColors), flashDuration);
        }
        
        private static void SetRendererColor(Renderer objectRenderer, Color color)
        {
            var mat = objectRenderer.material;
            if (mat.HasProperty(BaseColor))
                mat.SetColor(BaseColor, color);
            else if (mat.HasProperty(SecondaryColor))
                mat.SetColor(SecondaryColor, color);
        }

        private void RestoreOriginalColors()
        {
            for (var i = 0; i < renderers.Length; i++)
                SetRendererColor(renderers[i], originalColors[i]);
        }
        
        private protected void RotateToPlayer()
        {
            var direction = Player.position - transform.position;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(direction);
        }
        
        private protected abstract void FindReferences();
        private protected abstract void Attack();

    }
}