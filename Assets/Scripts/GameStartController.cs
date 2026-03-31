using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class GameStartController : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float instructionDuration = 3.0f;
    [SerializeField] private float blinkInterval = 0.3f;

    private VisualElement _fadeOverlay;
    private VisualElement _instructionContainer;
    private Label _instructionLabel;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _fadeOverlay = root.Q<VisualElement>("fadeOverlay");
        _instructionContainer = root.Q<VisualElement>("instructionContainer");
        _instructionLabel = root.Q<Label>("instructionLabel");

        if (_instructionLabel != null)
        {
            _instructionLabel.text = "REFLECT MAGIC TO DEFEAT ENEMIES!";
        }

        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        // 1. Initial State: Height 100%, Top 0%, Overlay opaque (1.0)
        if (_fadeOverlay != null)
        {
            _fadeOverlay.style.height = Length.Percent(100f);
            _fadeOverlay.style.top = Length.Percent(0f);
            _fadeOverlay.style.opacity = 1f;
        }
        if (_instructionContainer != null) _instructionContainer.style.visibility = Visibility.Hidden;

        // Start shrinking (with opacity fade)
        // Animates height 100% -> 15% AND opacity 1.0 -> 0.6
        Coroutine shrinkInitial = StartCoroutine(AnimateHeightAndOpacity(100f, 15f, 1f, 0.6f, fadeDuration));
        
        // Wait a bit before starting the message (e.g., half of fadeDuration or fixed 0.5s)
        yield return new WaitForSeconds(0.5f);
        
        // Start blinking
        Coroutine blink = StartCoroutine(BlinkInstruction(instructionDuration));

        // Wait for blinking to finish
        yield return blink;

        // 2. Final Shrink: 15% -> 0% (opacity also goes to 0)
        yield return StartCoroutine(AnimateHeightAndOpacity(15f, 0f, 0.6f, 0f, 0.5f));

        // Cleanup
        if (_fadeOverlay != null) _fadeOverlay.style.display = DisplayStyle.None;
    }

    private IEnumerator AnimateHeightAndOpacity(float fromH, float toH, float fromO, float toO, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Ease out: 1 - (1-t)^2
            float easeT = 1f - Mathf.Pow(1f - t, 2f);
            
            float currentHeight = Mathf.Lerp(fromH, toH, easeT);
            float currentOpacity = Mathf.Lerp(fromO, toO, easeT);
            
            if (_fadeOverlay != null)
            {
                float currentTop = (100f - currentHeight) / 2f;
                _fadeOverlay.style.height = Length.Percent(currentHeight);
                _fadeOverlay.style.top = Length.Percent(currentTop);
                _fadeOverlay.style.opacity = currentOpacity;
            }
            yield return null;
        }
        
        if (_fadeOverlay != null)
        {
            _fadeOverlay.style.height = Length.Percent(toH);
            _fadeOverlay.style.top = Length.Percent((100f - toH) / 2f);
            _fadeOverlay.style.opacity = toO;
        }
    }

    private IEnumerator BlinkInstruction(float totalDuration)
    {
        if (_instructionContainer != null) _instructionContainer.style.visibility = Visibility.Visible;

        float elapsed = 0f;
        // Adjust blink speed based on blinkInterval
        // visiblePart : hiddenPart = 2 : 1
        float visiblePart = blinkInterval * 2f; 
        float hiddenPart = blinkInterval;  

        while (elapsed < totalDuration)
        {
            // Visible
            if (_instructionLabel != null) _instructionLabel.style.opacity = 1f;
            float vTime = Mathf.Min(visiblePart, totalDuration - elapsed);
            yield return new WaitForSeconds(vTime);
            elapsed += vTime;

            if (elapsed >= totalDuration) break;

            // Hidden
            if (_instructionLabel != null) _instructionLabel.style.opacity = 0f;
            float hTime = Mathf.Min(hiddenPart, totalDuration - elapsed);
            yield return new WaitForSeconds(hTime);
            elapsed += hTime;
        }

        // Final state: Hidden
        if (_instructionLabel != null) _instructionLabel.style.opacity = 0f;
        if (_instructionContainer != null) _instructionContainer.style.visibility = Visibility.Hidden;
    }
    }
