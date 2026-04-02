using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class TitleScreenController : MonoBehaviour
{
    private Label _startLabel;
    private VisualElement _fadeOverlay;
    private bool _isStarting = false;

    [Header("Audio")]
    public AudioClip clickClip;
    public AudioClip titleJingle;

    private void OnEnable()
    {
        Time.timeScale = 1f; // Ensure time is moving
        var root = GetComponent<UIDocument>().rootVisualElement;
        _startLabel = root.Q<Label>("startLabel");
        _fadeOverlay = root.Q<VisualElement>("fadeOverlay");

        AudioManager.PlayBGM(titleJingle, false);

        // Start blinking effect
StartCoroutine(BlinkRoutine());
        
        // Start fade-in effect
        if (_fadeOverlay != null)
        {
            _fadeOverlay.style.opacity = 1f;
            _fadeOverlay.style.display = DisplayStyle.Flex;
            StartCoroutine(FadeInRoutine());
        }
    }

    private void Update()
    {
        if (_isStarting) return;

        // Using New Input System directly as per Guidance.txt
        // Changed to mouse click only as requested
        bool mouseClicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        if (mouseClicked)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        _isStarting = true;
        AudioManager.PlaySFX(clickClip);
        StopAllCoroutines();

        // Ensure label is visible when clicked
        _startLabel.style.opacity = 1f;
        
        StartCoroutine(FadeAndStartRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Ease out: 1 - (1-t)^2
            float easeT = 1f - Mathf.Pow(1f - t, 2f);
            _fadeOverlay.style.opacity = 1f - easeT;
            yield return null;
        }

        _fadeOverlay.style.opacity = 0f;
        _fadeOverlay.style.display = DisplayStyle.None;
    }

    private IEnumerator BlinkRoutine()
    {
        while (!_isStarting)
        {
            _startLabel.style.opacity = 1f;
            yield return new WaitForSeconds(0.3f);
            _startLabel.style.opacity = 0f;
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator FadeAndStartRoutine()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        // Ensure overlay is visible before starting fade-out
        if (_fadeOverlay != null)
        {
            _fadeOverlay.style.display = DisplayStyle.Flex;
            _fadeOverlay.style.opacity = 0f;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            if (_fadeOverlay != null) _fadeOverlay.style.opacity = t;
            yield return null;
        }

        if (_fadeOverlay != null) _fadeOverlay.style.opacity = 1f;
        
        // Load the Main scene (Index 1)
        SceneManager.LoadScene(1);
    }
}
