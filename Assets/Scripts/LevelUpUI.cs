using UnityEngine;
using UnityEngine.UIElements;

public class LevelUpUI : MonoBehaviour
{
    private PlayerController _player;
    private VisualElement _root;
    private VisualElement _overlay;

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _overlay = _root.Q<VisualElement>("overlay");
        
        // Hide by default
        _overlay.style.display = DisplayStyle.None;

        // Register button callbacks
        _root.Q<Button>("btnMoveSpeed").clicked += OnMoveSpeedSelected;
        _root.Q<Button>("btnChargeSpeed").clicked += OnChargeSpeedSelected;
        _root.Q<Button>("btnBarrierStrength").clicked += OnBarrierStrengthSelected;
    }

    public void Show(PlayerController player)
    {
        _player = player;
        Time.timeScale = 0f;
        _overlay.style.display = DisplayStyle.Flex;
    }

    private void Hide()
    {
        _overlay.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }

    private void OnMoveSpeedSelected()
    {
        if (_player != null)
        {
            _player.moveSpeed *= 1.15f;
            Debug.Log("Move Speed Upgraded: " + _player.moveSpeed);
        }
        Hide();
    }

    private void OnChargeSpeedSelected()
    {
        if (_player != null)
        {
            _player.barrierCooldown *= 0.85f;
            _player.barrierCooldown = Mathf.Max(_player.barrierCooldown, 1f); // Min cooldown
            Debug.Log("Barrier Cooldown Upgraded: " + _player.barrierCooldown);
        }
        Hide();
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
            Debug.Log("Barrier Strength Upgraded: Duration " + _player.barrierDuration + ", Scale " + _player.barrierObject.transform.localScale.x);
        }
        Hide();
    }
}
