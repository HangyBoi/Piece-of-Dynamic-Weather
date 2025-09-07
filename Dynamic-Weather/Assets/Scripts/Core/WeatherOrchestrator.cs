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
    public bool isWindActive = true;

    [Header("--- SCRIPT & VFX REFERENCES ---")]
    public TornadoController tornadoController;
    public VisualEffect tornadoVFX;
    public VisualEffect dustCloudVFX;
    public VisualEffect circularWindVFX;
    public VisualEffect directionalWindVFX;
    public VisualEffect rainVFX;
    public VisualEffect midSkyLightningVFX;
    public VisualEffect highSkyLightningVFX;

    // --- Exposed Settings for Tweaking in the Inspector ---

    [Header("--- TORNADO SETTINGS ---")]
    [Tooltip("At what storm intensity (0-1) the tornado begins to form.")]
    public float tornadoActivationThreshold = 0.2f;
    [Tooltip("The scale of the tornado GameObject at minimum and maximum power.")]
    public Vector2 minMaxTornadoScale = new Vector2(0.5f, 1.5f);
    [Tooltip("The movement speed of the tornado at full power.")]
    public float maxTornadoSpeed = 5f;
    [Tooltip("The physics attraction force of the tornado at full power.")]
    public float maxTornadoForce = 50f;
    [Tooltip("The spawn rate of the dust cloud at full tornado power.")]
    public float maxDustSpawnRate = 250f;

    [Header("--- RAIN SETTINGS ---")]
    public float maxRainDropRate = 20000f;
    public float maxRainTurbulence = 10f;

    [Header("--- LIGHTNING SETTINGS ---")]
    [Tooltip("At what storm intensity (0-1) the first lightning appears.")]
    public float midLightningThreshold = 0.5f;
    [Tooltip("At what storm intensity (0-1) the second layer of lightning appears.")]
    public float highLightningThreshold = 0.75f;
    public float maxMidLightningRate = 0.7f;
    public float maxHighLightningRate = 0.5f;
    [Tooltip("The min/max lifetime of lightning trails at maximum storm intensity.")]
    public float minMidLightningTrailLifetime = 0.3f;
    public float maxMidLightningTrailLifetime = 0.7f;
    public float minHighLightningTrailLifetime = 0.5f;
    public float maxHighLightningTrailLifetime = 1f;

    [Header("--- WIND SETTINGS ---")]
    // Directional Wind
    [Tooltip("The spawn rate for directional wind at full storm intensity.")]
    public float maxDirectionalWindRate = 5f;
    public float maxDirectionalWindTurbulence = 100f;
    public float maxDirectionalWindTrailLifetime = 1f;
    public Vector3 minDirectionalWindVolumeSize = new Vector3(50, 20, 50);
    public Vector3 maxDirectionalWindVolumeSize = new Vector3(100, 40, 100);
    // Circular Wind
    [Tooltip("The circular wind's loop size at full tornado power.")]
    public float maxCircularWindAmplitude = 30f;
    public float maxCircularWindRate = 5f;
    public float maxCircularWindTurbulence = 100f;
    public float maxCircularWindTrailLifetime = 1f;
    public Vector3 minCircularWindVolumeSize = new Vector3(10, 5, 10);
    public Vector3 maxCircularWindVolumeSize = new Vector3(20, 10, 20);

    void Update()
    {
        HandleTornadoSystem();
        HandleRainSystem();
        HandleLightningSystem();
        HandleWindSystem();
    }

    private void HandleTornadoSystem()
    {
        if (tornadoController == null) return;
        bool systemShouldBeActive = isTornadoActive && stormIntensity > tornadoActivationThreshold;
        if (tornadoController.gameObject.activeSelf != systemShouldBeActive) tornadoController.gameObject.SetActive(systemShouldBeActive);
        if (!systemShouldBeActive) return;

        float tornadoPower = Mathf.Clamp01((stormIntensity - tornadoActivationThreshold) / (1f - tornadoActivationThreshold));

        // Control Tornado GameObject Scale
        float currentScale = Mathf.Lerp(minMaxTornadoScale.x, minMaxTornadoScale.y, tornadoPower);
        tornadoController.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        // Control Tornado Script parameters
        tornadoController.movementSpeed = Mathf.Lerp(1f, maxTornadoSpeed, tornadoPower);
        tornadoController.maxAttractionForce = Mathf.Lerp(0f, maxTornadoForce, tornadoPower);

        // Control Dust Cloud
        if (dustCloudVFX != null) dustCloudVFX.SetFloat("Dust Spawn Rate", tornadoPower * maxDustSpawnRate);
    }

    private void HandleRainSystem()
    {
        if (rainVFX == null) return;
        bool systemShouldBeActive = isRainActive && stormIntensity > 0.01f;
        if (rainVFX.gameObject.activeSelf != systemShouldBeActive) rainVFX.gameObject.SetActive(systemShouldBeActive);
        if (!systemShouldBeActive) return;

        rainVFX.SetFloat("Rain Drop Rate", stormIntensity * maxRainDropRate);
        rainVFX.SetFloat("Turbulence Intensity", stormIntensity * maxRainTurbulence);
    }

    private void HandleLightningSystem()
    {
        if (midSkyLightningVFX != null)
        {
            bool midShouldBeActive = isLightningActive && stormIntensity > midLightningThreshold;
            if (midSkyLightningVFX.gameObject.activeSelf != midShouldBeActive) midSkyLightningVFX.gameObject.SetActive(midShouldBeActive);
            if (midShouldBeActive)
            {
                float power = Mathf.Clamp01((stormIntensity - midLightningThreshold) / (1f - midLightningThreshold));
                midSkyLightningVFX.SetFloat("Lightning Spawn Rate", Mathf.Lerp(0.05f, maxMidLightningRate, power));
                midSkyLightningVFX.SetFloat("Trail Lifetime", Mathf.Lerp(maxMidLightningTrailLifetime, minMidLightningTrailLifetime, power));
            }
        }
        if (highSkyLightningVFX != null)
        {
            bool highShouldBeActive = isLightningActive && stormIntensity > highLightningThreshold;
            if (highSkyLightningVFX.gameObject.activeSelf != highShouldBeActive) highSkyLightningVFX.gameObject.SetActive(highShouldBeActive);
            if (highShouldBeActive)
            {
                float power = Mathf.Clamp01((stormIntensity - highLightningThreshold) / (1f - highLightningThreshold));
                highSkyLightningVFX.SetFloat("Lightning Spawn Rate", Mathf.Lerp(0.05f, maxHighLightningRate, power));
                highSkyLightningVFX.SetFloat("Trail Lifetime", Mathf.Lerp(minHighLightningTrailLifetime, maxHighLightningTrailLifetime, power));
            }
        }
    }

    private void HandleWindSystem()
    {
        // Circular Wind (Child of Tornado)
        if (circularWindVFX != null)
        {
            float tornadoPower = Mathf.Clamp01((stormIntensity - tornadoActivationThreshold) / (1f - tornadoActivationThreshold));
            circularWindVFX.SetFloat("Loop Amplitude", Mathf.Lerp(5f, maxCircularWindAmplitude, tornadoPower));
            circularWindVFX.SetFloat("Wind Spawn Rate", tornadoPower * maxCircularWindRate);
            circularWindVFX.SetFloat("Turbulence Intensity", tornadoPower * maxCircularWindTurbulence);
            circularWindVFX.SetFloat("Trail Lifetime", Mathf.Lerp(0.5f, maxCircularWindTrailLifetime, tornadoPower));
            circularWindVFX.SetVector3("Spawn Volume Size", Vector3.Lerp(minCircularWindVolumeSize, maxCircularWindVolumeSize, tornadoPower));
        }

        // Directional Wind
        if (directionalWindVFX != null)
        {
            bool windShouldBeActive = isWindActive && stormIntensity > 0.1f;
            if (directionalWindVFX.gameObject.activeSelf != windShouldBeActive) directionalWindVFX.gameObject.SetActive(windShouldBeActive);
            if (windShouldBeActive)
            {
                directionalWindVFX.SetFloat("Wind Spawn Rate", stormIntensity * maxDirectionalWindRate);
                directionalWindVFX.SetFloat("Turbulence Intensity", stormIntensity * maxDirectionalWindTurbulence);
                directionalWindVFX.SetFloat("Trail Lifetime", Mathf.Lerp(0.5f, maxDirectionalWindTrailLifetime, stormIntensity));
                directionalWindVFX.SetVector3("Spawn Volume Size", Vector3.Lerp(minDirectionalWindVolumeSize, maxDirectionalWindVolumeSize, stormIntensity));
            }
        }
    }
}