using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class TitleScreenController : MonoBehaviour
{
    private Label _startLabel;
    private VisualElement _fadeOverlay;
    private VisualElement _instructionCard;
    private bool _isStarting = false;

    private static bool _hasShownInstructions = false;
    private bool _showingInstructions = false;

    private IEnumerator Start()
    {
        Time.timeScale = 1f; // Ensure time is moving
        var root = GetComponent<UIDocument>().rootVisualElement;
        _startLabel = root.Q<Label>("startLabel");
        _fadeOverlay = root.Q<VisualElement>("fadeOverlay");
        _instructionCard = root.Q<VisualElement>("instructionCard");

        // Wait for AudioManager to be available
        while (AudioManager.Instance == null)
        {
            yield return null;
        }

        AudioManager.PlayBGM(AudioID.BGM_Title, false);

        // Always start with the start label visible and blinking
        _showingInstructions = false;
        if (_instructionCard != null) _instructionCard.style.display = DisplayStyle.None;
        _startLabel.style.display = DisplayStyle.Flex;
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

        bool mouseClicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        if (mouseClicked)
        {
            if (_showingInstructions)
            {
                // Second click: Proceed to game
                _hasShownInstructions = true;
                StartGame();
            }
            else
            {
                // First click: Check if we need to show instructions
                if (!_hasShownInstructions && _instructionCard != null)
                {
                    ShowInstructions();
                }
                else
                {
                    StartGame();
                }
            }
        }
    }

    private void ShowInstructions()
    {
        _showingInstructions = true;
        AudioManager.PlaySFX(AudioID.SFX_Click);
        
        if (_instructionCard != null) 
        {
            _instructionCard.style.display = DisplayStyle.Flex;
            
            // Wait a frame to ensure display: flex is applied before starting transition
            _instructionCard.schedule.Execute(() => {
                _instructionCard.AddToClassList("instruction-card--visible");
            }).StartingIn(1);
        }
        if (_startLabel != null) _startLabel.style.display = DisplayStyle.None;
    }

    private void StartGame()
    {
        _isStarting = true;
        AudioManager.PlaySFX(AudioID.SFX_Click);
        StopAllCoroutines();

        // Ensure visible elements stay visible during fade-out
        if (_startLabel != null && !_showingInstructions)
        {
            _startLabel.style.display = DisplayStyle.Flex;
            _startLabel.style.opacity = 1f;
        }
        
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
        while (!_isStarting && !_showingInstructions)
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
