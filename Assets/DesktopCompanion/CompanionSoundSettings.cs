using System;
using UnityEngine;

public sealed class CompanionSoundSettings : MonoBehaviour
{
    private const string SoundEnabledKey = "DesktopCompanion.SoundEffectsEnabled.v1";
    private static bool loaded;
    private static bool enabledValue;

    private Texture2D soundOnIcon;
    private Texture2D soundOffIcon;
    private float feedbackEndsAt;

    public static event Action<bool> Changed;

    public static bool Enabled
    {
        get
        {
            if (!loaded)
            {
                enabledValue = PlayerPrefs.GetInt(SoundEnabledKey, 1) != 0;
                loaded = true;
            }
            return enabledValue;
        }
    }

    private void Awake()
    {
        _ = Enabled;
        soundOnIcon = CreateIcon(true);
        soundOffIcon = CreateIcon(false);
    }

    private void Update()
    {
        if (!Application.isFocused || !Input.GetKeyDown(KeyCode.S)) return;
        enabledValue = !Enabled;
        PlayerPrefs.SetInt(SoundEnabledKey, enabledValue ? 1 : 0);
        PlayerPrefs.Save();
        Changed?.Invoke(enabledValue);
        feedbackEndsAt = Time.unscaledTime + 1.6f;
    }

    private void OnGUI()
    {
        if (Time.unscaledTime >= feedbackEndsAt) return;
        var previous = GUI.color;
        var x = (Screen.width - 58f) * 0.5f;
        var y = Screen.height - 73f;
        GUI.color = new Color(0f, 0f, 0f, 0.68f);
        GUI.DrawTexture(new Rect(x, y, 58f, 58f), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.DrawTexture(new Rect(x + 3f, y + 3f, 52f, 52f),
            Enabled ? soundOnIcon : soundOffIcon);
        GUI.color = previous;
    }

    private void OnDestroy()
    {
        if (soundOnIcon != null) Destroy(soundOnIcon);
        if (soundOffIcon != null) Destroy(soundOffIcon);
    }

    private static Texture2D CreateIcon(bool soundOn)
    {
        const int size = 64;
        var pixels = new Color32[size * size];
        var white = new Color32(255, 255, 255, 255);
        for (var y = 0; y < size; y++)
        for (var x = 0; x < size; x++)
        {
            var speakerBar = x >= 10 && x <= 21 && y >= 26 && y <= 38;
            var speakerCone = x >= 22 && x <= 37 &&
                Mathf.Abs(y - 32) <= 8f + (x - 22) * 0.8f;
            var distance = Mathf.Sqrt((x - 30) * (x - 30) + (y - 32) * (y - 32));
            var soundWaves = soundOn && x >= 40 &&
                (Mathf.Abs(distance - 16f) <= 1.7f || Mathf.Abs(distance - 25f) <= 1.7f);
            var muteSlash = !soundOn && x >= 40 && x <= 56 &&
                Mathf.Abs(y - (13 + (x - 40) * 2f)) <= 2f;
            if (speakerBar || speakerCone || soundWaves || muteSlash)
                pixels[y * size + x] = white;
        }

        var icon = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.DontSave
        };
        icon.SetPixels32(pixels);
        icon.Apply(false, true);
        return icon;
    }
}
