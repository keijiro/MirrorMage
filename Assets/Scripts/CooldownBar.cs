using UnityEngine;

public class CooldownBar : MonoBehaviour
{
    public SpriteRenderer frameSr;
    public Transform fillTransform;
    public PlayerController player;

    private void Update()
    {
        if (player == null) return;

        bool isOnCooldown = player.IsOnCooldown();
        float progress = player.GetCooldownProgress();

        // Show/hide visuals
        if (frameSr != null)
        {
            frameSr.enabled = isOnCooldown;
            // Maybe dim it?
            // frameSr.color = new Color(1, 1, 1, 0.7f);
        }

        if (fillTransform != null)
        {
            var fillSr = fillTransform.GetComponentInChildren<SpriteRenderer>();
            if (fillSr != null) fillSr.enabled = isOnCooldown;
            
            if (isOnCooldown)
            {
                // Scaler
                Vector3 currentScale = fillTransform.localScale;
                currentScale.x = progress;
                fillTransform.localScale = currentScale;
            }
        }
    }
}
