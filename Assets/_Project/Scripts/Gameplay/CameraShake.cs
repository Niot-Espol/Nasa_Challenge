// Assets/_Project/Scripts/Gameplay/CameraShake.cs
// Sacudida de cámara para Cinemachine (extensión de la CinemachineCamera).
// Uso: CameraShake.Shake(0.3f, 1f);  CameraShake.SetContinuous(0.05f);  (0 = apagar)
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : CinemachineExtension
{
    static CameraShake instance;

    float impulse, impulseDuration, impulseTime;
    float continuous;
    float seed;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
        seed = Random.value * 100f;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (instance == this) instance = null;
    }

    public static void Shake(float amplitude, float duration)
    {
        if (instance == null) return;
        instance.impulse = Mathf.Max(instance.impulse * Mathf.Clamp01(1 - instance.impulseTime / Mathf.Max(0.01f, instance.impulseDuration)), amplitude);
        instance.impulseDuration = duration;
        instance.impulseTime = 0f;
    }

    public static void SetContinuous(float amplitude)
    {
        if (instance != null) instance.continuous = amplitude;
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Noise || Time.timeScale <= 0f) return;
        if (deltaTime > 0) impulseTime += deltaTime;
        float k = impulseDuration > 0 ? Mathf.Clamp01(1 - impulseTime / impulseDuration) : 0;
        float amp = impulse * k * k + continuous;
        if (amp <= 0.0001f) return;
        float t = Time.time * 25f;
        var offset = new Vector3(Mathf.PerlinNoise(seed, t) - 0.5f, Mathf.PerlinNoise(seed + 10, t) - 0.5f, 0) * 2f * amp;
        state.PositionCorrection += offset;
    }
}
