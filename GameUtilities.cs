using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Classe statica per metodi utility
public static class GameUtilities
{
    public static Vector3 GetRandomSpawnPosition(float screenLimitX, float spawnHeight)
    {
        float randomX = UnityEngine.Random.Range(-screenLimitX, screenLimitX);
        return new Vector3(randomX, spawnHeight, 0f);
    }

    public static void PlaySound(AudioSource audioSource)
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
