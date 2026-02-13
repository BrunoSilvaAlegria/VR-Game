using UnityEngine;
using System;

public static class SoundSystem
{
    public static event Action<Vector3, float> OnSound;
    // position, intensity

    public static void Emit(Vector3 position, float intensity)
    {
        Debug.Log($"Enemy heard event at {position} intensity {intensity}");
        OnSound?.Invoke(position, intensity);
    }
}
