using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    public static AmmoUI Instance;
    [Tooltip("Reference to a TMP_Text component that displays ammo (format: \"mag / reserve\")")]
    public TMP_Text ammoText;

    [Tooltip("Reference to a TMP_Text component that displays reload hint (e.g. \"Press R to reload\")")]
    public TMP_Text reloadHintText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // start hidden — only show when a weapon calls SetAmmo
        if (ammoText != null) ammoText.gameObject.SetActive(false);
        if (reloadHintText != null) reloadHintText.gameObject.SetActive(false);
    }

    // call this to show/hide the whole ammo UI (e.g. when player has no weapon)
    public void SetVisible(bool visible)
    {
        if (ammoText != null) ammoText.gameObject.SetActive(visible);
        if (reloadHintText != null) reloadHintText.gameObject.SetActive(visible && reloadHintText.gameObject.activeSelf);
    }

    public void SetAmmo(int magazine, int reserve)
    {
        // show UI when ammo is updated (equipping a weapon)
        if (ammoText != null && !ammoText.gameObject.activeSelf) ammoText.gameObject.SetActive(true);

        if (ammoText != null)
            ammoText.text = $"{magazine} / {reserve}";

        // show reload hint when magazine is empty and there's reserve ammo
        if (reloadHintText == null) return;

        if (magazine <= 0 && reserve > 0)
        {
            reloadHintText.gameObject.SetActive(true);
            reloadHintText.text = "Press R to reload";
        }
        else if (magazine <= 0 && reserve <= 0)
        {
            reloadHintText.gameObject.SetActive(true);
            reloadHintText.text = "Out of ammo";
        }
        else
        {
            reloadHintText.gameObject.SetActive(false);
        }
    }

    public void ShowReloadHint(bool show, string text = "Press R to reload")
    {
        if (reloadHintText == null) return;
        if (!ammoText.gameObject.activeSelf && show) ammoText.gameObject.SetActive(true);
        reloadHintText.gameObject.SetActive(show);
        if (show) reloadHintText.text = text;
    }
}