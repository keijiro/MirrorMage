using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class GameOverController : MonoBehaviour
{
    private Label _retryLabel;
    private VisualElement _fadeOverlay;
    private bool _isTransitioning = false;

    private void OnEnable()
{
        var root = GetComponent<UIDocument>().rootVisualElement;
        _retryLabel = root.Q<Label>(null, "click-to-retry");
        _fadeOverlay = root.Q<VisualElement>("fadeOverlay");

        // Reset timeScale here so animations in this scene play correctly
        Time.timeScale = 1f;

        StartCoroutine(BlinkRoutine());
        
        if (_fadeOverlay != null)
        {
            _fadeOverlay.style.display = DisplayStyle.Flex;
            _fadeOverlay.style.height = Length.Percent(100f);
            _fadeOverlay.style.top = Length.Percent(0f);
            _fadeOverlay.style.opacity = 1f;
            StartCoroutine(FadeInRoutine());
        }
    }

    private void Update()
    {
        if (_isTransitioning) return;

        bool mouseClicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        if (mouseClicked)
        {
            AudioManager.PlaySFX(AudioID.SFX_Click);
            StartCoroutine(ReturnToTitleRoutine());
        }
}

    private IEnumerator FadeInRoutine()
    {
        // Wait for a very short moment while black to ensure the player feels the scene transition
        yield return new WaitForSecondsRealtime(0.1f);

        float duration = 1.0f; // Increased to 1.0s
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            // Ease out: 1 - (1-t)^2
            float easeT = 1f - Mathf.Pow(1f - t, 2f);
            _fadeOverlay.style.opacity = 1f - easeT;
            yield return null;
        }

        _fadeOverlay.style.opacity = 0f;
        _fadeOverlay.style.display = DisplayStyle.None;
    }

    private IEnumerator ReturnToTitleRoutine()
    {
        _isTransitioning = true;
        
        if (_fadeOverlay != null)
        {
            _fadeOverlay.style.display = DisplayStyle.Flex;
            _fadeOverlay.style.opacity = 0f;
            float duration = 0.5f; // Let's also set fade-out to 0.5s for consistency
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                _fadeOverlay.style.opacity = t;
                yield return null;
            }
            _fadeOverlay.style.opacity = 1f;
        }

        Time.timeScale = 1f; // Restore time scale!
        SceneManager.LoadScene(0); // Load TitleScreen
    }

    private IEnumerator BlinkRoutine()
    {
        while (!_isTransitioning)
        {
            if (_retryLabel != null)
            {
                _retryLabel.style.opacity = 1f;
                yield return new WaitForSecondsRealtime(0.5f);
                _retryLabel.style.opacity = 0f;
                yield return new WaitForSecondsRealtime(0.5f);
            }
            else yield return null;
        }
    }
}