using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class LevelUpUI : MonoBehaviour
{
    private PlayerController _player;
    private VisualElement _root;
    private VisualElement _overlay;
    private Label _titleLabel;
    private List<Button> _optionButtons = new List<Button>();
    private Coroutine _flashCoroutine;

    private const string HIDDEN_CLASS = "option-card--hidden";
    private const string SELECTED_CLASS = "option-card--selected";
    private const string DIMMED_CLASS = "option-card--dimmed";

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _overlay = _root.Q<VisualElement>("overlay");
        _titleLabel = _root.Q<Label>(className: "level-up-title");
        
        _overlay.style.display = DisplayStyle.None;

        SetupButton("btnMoveSpeed", OnMoveSpeedSelected);
        SetupButton("btnChargeSpeed", OnChargeSpeedSelected);
        SetupButton("btnBarrierStrength", OnBarrierStrengthSelected);
    }

    private void SetupButton(string name, System.Action callback)
    {
        Button btn = _root.Q<Button>(name);
        if (btn != null)
        {
            btn.clicked += () => StartCoroutine(HandleSelection(btn, callback));
            _optionButtons.Add(btn);
        }
    }

    public void Show(PlayerController player)
    {
        _player = player;
        Time.timeScale = 0f;
        _overlay.style.display = DisplayStyle.Flex;

        // Start C# flashing animation
        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(FlashTitleColor());

        // Apply hidden class initially for entry animation
        foreach (var btn in _optionButtons)
        {
            btn.AddToClassList(HIDDEN_CLASS);
            btn.RemoveFromClassList(SELECTED_CLASS);
            btn.RemoveFromClassList(DIMMED_CLASS);
        }

        // Trigger entry animation after a very short delay
        StartCoroutine(TriggerEntryAnimation());
    }

    private IEnumerator TriggerEntryAnimation()
    {
        yield return null; // Wait for layout/frame
        
        float delay = 0f;
        foreach (var btn in _optionButtons)
        {
            // Staggered entry (optional, but adds juice)
            StartCoroutine(RemoveHiddenDelayed(btn, delay));
            delay += 0.05f;
        }
    }

    private IEnumerator RemoveHiddenDelayed(Button btn, float delay)
    {
        if (delay > 0) yield return new WaitForSecondsRealtime(delay);
        btn.RemoveFromClassList(HIDDEN_CLASS);
    }

    private IEnumerator HandleSelection(Button selectedBtn, System.Action applyUpgrade)
    {
        // Apply visual states
        foreach (var btn in _optionButtons)
        {
            if (btn == selectedBtn)
            {
                btn.AddToClassList(SELECTED_CLASS);
            }
            else
            {
                btn.AddToClassList(DIMMED_CLASS);
            }
        }

        // Wait for animation to finish (using realtime because timescale is 0)
        yield return new WaitForSecondsRealtime(0.4f);

        applyUpgrade?.Invoke();
        Hide();
    }

    private void Hide()
    {
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
            _flashCoroutine = null;
        }

        _overlay.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }

    private IEnumerator FlashTitleColor()
    {
        Color color1 = Color.white;
        Color color2 = Color.yellow;
        float speed = 12f;

        while (true)
        {
            float t = (Mathf.Sin(Time.unscaledTime * speed) + 1f) / 2f;
            if (_titleLabel != null)
            {
                _titleLabel.style.color = Color.Lerp(color1, color2, t);
            }
            yield return null;
        }
    }

    private void OnMoveSpeedSelected()
    {
        if (_player != null)
        {
            _player.moveSpeed *= 1.15f;
            Debug.Log("Move Speed Upgraded: " + _player.moveSpeed);
        }
    }

    private void OnChargeSpeedSelected()
    {
        if (_player != null)
        {
            _player.barrierCooldown *= 0.85f;
            _player.barrierCooldown = Mathf.Max(_player.barrierCooldown, 1f);
            Debug.Log("Barrier Cooldown Upgraded: " + _player.barrierCooldown);
        }
    }

    private void OnBarrierStrengthSelected()
    {
        if (_player != null)
        {
            _player.barrierDuration *= 1.2f;
            if (_player.barrierObject != null)
            {
                _player.barrierObject.transform.localScale *= 1.15f;
            }
            Debug.Log("Barrier Strength Upgraded: Duration " + _player.barrierDuration);
        }
    }
}
