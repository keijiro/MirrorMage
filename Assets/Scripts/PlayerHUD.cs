using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHUD : MonoBehaviour
{
    public PlayerController player;
    private VisualElement _healthBarFill;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _healthBarFill = root.Q<VisualElement>("healthBarFill");
    }

    private void Update()
    {
        if (player == null || _healthBarFill == null) return;

        float progress = player.GetHealthProgress();
        _healthBarFill.style.width = new Length(progress * 100f, LengthUnit.Percent);
    }
}
