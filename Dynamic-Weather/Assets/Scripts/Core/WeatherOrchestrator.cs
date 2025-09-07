using UnityEngine;
using UnityEngine.VFX;

public class WeatherOrchestrator : MonoBehaviour
{
    [Header("--- MASTER CONTROL ---")]
    [Tooltip("The master control for the entire weather system. 0 is calm, 1 is a full storm.")]
    [Range(0f, 1f)]
    public float stormIntensity = 0f;

    [Header("--- COMPONENT TOGGLES ---")]
    public bool isTornadoActive = true;
    public bool isRainActive = true;
    public bool isLightningActive = true;
    public bool isWindActive = true; // One toggle for both wind types

    [Header("--- SCRIPT REFERENCES ---")]
    [Tooltip("Drag the GameObject that has the TornadoController script on it here.")]
    public TornadoController tornadoController; // Reference to your tornado movement/physics script

    [Header("--- VFX REFERENCES ---")]
    public VisualEffect tornadoVFX;
    public VisualEffect dustCloudVFX;
    public VisualEffect circularWindVFX; // Looping Wind
    public VisualEffect directionalWindVFX; // Straight Wind
    public VisualEffect rainVFX;
    public VisualEffect midSkyLightningVFX; // Assuming this is one of your lightning VFX
    public VisualEffect highSkyLightningVFX; // And this is the other

    void Update()
    {
        // --- TORNADO SYSTEM ---
        HandleTornadoSystem();

        // --- RAIN SYSTEM ---
        HandleRainSystem();

        // --- LIGHTNING SYSTEM ---
        HandleLightningSystem();

        // --- WIND SYSTEM ---
        HandleWindSystem();
    }

    private void HandleTornadoSystem()
    {
        if (tornadoController == null) return;

        // Define the threshold at which the tornado should start appearing.
        float activationThreshold = 0.2f;
        bool systemShouldBeActive = isTornadoActive && stormIntensity > activationThreshold;

        // 1. ACTIVATE / DEACTIVATE the main Tornado GameObject.
        if (tornadoController.gameObject.activeSelf != systemShouldBeActive)
        {
            tornadoController.gameObject.SetActive(systemShouldBeActive);
        }

        // If the system is off, do nothing further.
        if (!systemShouldBeActive) return;

        // 2. CALCULATE TORNADO POWER. This 0-1 value is our perfect animation driver.
        float tornadoPower = Mathf.Clamp01((stormIntensity - activationThreshold) / (1f - activationThreshold));

        // 3. THIS IS THE FIX: Drive the animation curves directly using our new parameter.
        // The tornado's visual state (scale, dissolve, etc.) is now 100% synced to its power.
        if (tornadoVFX != null)
        {
            tornadoVFX.SetFloat("AnimationTime", tornadoPower);
        }

        // 4. CONTROL THE TORNADO'S SCRIPT PARAMETERS
        tornadoController.movementSpeed = Mathf.Lerp(1f, 5f, tornadoPower);
        tornadoController.maxAttractionForce = Mathf.Lerp(0f, 50f, tornadoPower);

        // 5. CONTROL CHILD VFX (Dust and Circular Wind)
        if (dustCloudVFX != null)
        {
            dustCloudVFX.SetFloat("Dust Spawn Rate", tornadoPower * 250f);
        }
        if (circularWindVFX != null)
        {
            circularWindVFX.SetFloat("Loop Amplitude", Mathf.Lerp(5f, 30f, tornadoPower));
            circularWindVFX.SetFloat("Wind Spawn Rate", tornadoPower * 5f);
        }
    }

    private void HandleRainSystem()
    {
        if (rainVFX == null) return;

        bool systemShouldBeActive = isRainActive && stormIntensity > 0f;
        if (rainVFX.gameObject.activeSelf != systemShouldBeActive)
        {
            rainVFX.gameObject.SetActive(systemShouldBeActive);
        }
        if (!systemShouldBeActive) return;

        // Rain intensity scales directly with the storm
        rainVFX.SetFloat("Rain Drop Rate", stormIntensity * 20000f);
        rainVFX.SetFloat("Turbulence Intensity", stormIntensity * 10f);
    }

    private void HandleLightningSystem()
    {
        // Mid-Sky Lightning
        if (midSkyLightningVFX != null)
        {
            bool midLightningShouldBeActive = isLightningActive && stormIntensity > 0.5f;
            if (midSkyLightningVFX.gameObject.activeSelf != midLightningShouldBeActive)
            {
                midSkyLightningVFX.gameObject.SetActive(midLightningShouldBeActive);
            }
            if (midLightningShouldBeActive)
            {
                float midLightningPower = Mathf.Clamp01((stormIntensity - 0.5f) / 0.5f);
                midSkyLightningVFX.SetFloat("Lightning Spawn Rate", Mathf.Lerp(0.05f, 0.7f, midLightningPower));
            }
        }

        // High-Sky Lightning (depends on mid-sky)
        if (highSkyLightningVFX != null)
        {
            bool highLightningShouldBeActive = isLightningActive && stormIntensity > 0.75f;
            if (highSkyLightningVFX.gameObject.activeSelf != highLightningShouldBeActive)
            {
                highSkyLightningVFX.gameObject.SetActive(highLightningShouldBeActive);
            }
            if (highLightningShouldBeActive)
            {
                float highLightningPower = Mathf.Clamp01((stormIntensity - 0.75f) / 0.25f);
                highSkyLightningVFX.SetFloat("Lightning Spawn Rate", Mathf.Lerp(0.05f, 0.5f, highLightningPower));
            }
        }
    }

    private void HandleWindSystem()
    {
        if (!isWindActive)
        {
            if (circularWindVFX != null && circularWindVFX.gameObject.activeSelf) circularWindVFX.gameObject.SetActive(false);
            if (directionalWindVFX != null && directionalWindVFX.gameObject.activeSelf) directionalWindVFX.gameObject.SetActive(false);
            return;
        }

        // Circular Wind (Child of Tornado)
        if (circularWindVFX != null)
        {
            float tornadoPower = Mathf.Clamp01((stormIntensity - 0.2f) / 0.8f);
            circularWindVFX.SetFloat("Loop Amplitude", Mathf.Lerp(5f, 30f, tornadoPower));
            circularWindVFX.SetFloat("Wind Spawn Rate", tornadoPower * 5f);
        }

        // Directional Wind
        if (directionalWindVFX != null)
        {
            bool windShouldBeActive = stormIntensity > 0.1f;
            if (directionalWindVFX.gameObject.activeSelf != windShouldBeActive)
            {
                directionalWindVFX.gameObject.SetActive(windShouldBeActive);
            }
            if (windShouldBeActive)
            {
                directionalWindVFX.SetFloat("Wind Spawn Rate", stormIntensity * 5f);
                directionalWindVFX.SetFloat("Turbulence Intensity", stormIntensity * 100f);
            }
        }
    }
}