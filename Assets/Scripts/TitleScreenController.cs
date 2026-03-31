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

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _startLabel = root.Q<Label>("startLabel");
        _fadeOverlay = root.Q<VisualElement>("fadeOverlay");

        // Start blinking effect
        StartCoroutine(BlinkRoutine());
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
        StopAllCoroutines();
        
        // Ensure label is visible when clicked
        _startLabel.style.opacity = 1f;
        
        StartCoroutine(FadeAndStartRoutine());
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

        // Reset fade overlay opacity just in case
_fadeOverlay.style.opacity = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            _fadeOverlay.style.opacity = t;
            yield return null;
        }

        _fadeOverlay.style.opacity = 1f;
        
        // Load the Main scene (Index 1)
        SceneManager.LoadScene(1);
    }
}
