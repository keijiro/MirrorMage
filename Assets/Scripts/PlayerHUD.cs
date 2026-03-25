using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHUD : MonoBehaviour
{
    public PlayerController player;
    private VisualElement _healthBarFill;
    private VisualElement _xpBarFill;
    private Label _levelLabel;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _healthBarFill = root.Q<VisualElement>("healthBarFill");
        _xpBarFill = root.Q<VisualElement>("xpBarFill");
        _levelLabel = root.Q<Label>("levelText");
    }

    private void Update()
    {
        if (player == null) return;

        if (_healthBarFill != null)
        {
            float hpProgress = player.GetHealthProgress();
            _healthBarFill.style.width = new Length(hpProgress * 100f, LengthUnit.Percent);
        }

        if (_xpBarFill != null)
        {
            float xpProgress = player.GetXPProgress();
            _xpBarFill.style.width = new Length(xpProgress * 100f, LengthUnit.Percent);
        }

        if (_levelLabel != null)
        {
            _levelLabel.text = "Lv. " + player.currentLevel;
        }
    }
}
