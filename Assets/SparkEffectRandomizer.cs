using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkEffectRandomizer : MonoBehaviour
{
    [System.Serializable]
    public class SparkRandomizer
    {
        public ParticleSystem sparkEffect;
        public Light sparkLight;
    }

    [Header("Particle Effect Settings")]
    public List<SparkRandomizer> sparkRandomizers; // List of spark randomizers
    public float minTime = 1.0f; // Minimum time between effects
    public float maxTime = 5.0f; // Maximum time between effects

    [Header("Light Settings")]
    public float maxLightIntensity = 0.5f;
    public float lightChangeRangeRate = 0.25f;

    private void Start()
    {
        foreach(SparkRandomizer spark in sparkRandomizers) {
            spark.sparkLight.intensity = 0f;
        }
        StartCoroutine(PlayRandomSparkEffects());
    }

    private IEnumerator PlayRandomSparkEffects()
    {
        while (true)
        {
            // Choose a random spark randomizer
            SparkRandomizer chosenRandomizer = sparkRandomizers[Random.Range(0, sparkRandomizers.Count)];

            // Check if the effect is already playing, if so, stop it
            if (chosenRandomizer.sparkEffect.isPlaying)
            {
                chosenRandomizer.sparkEffect.Stop();
                // yield return new WaitForSeconds(0.1f); // Optional small delay before restarting
                yield return new WaitForSeconds(1.0f); // Optional small delay before restarting
            }

            // Start the particle effect
            chosenRandomizer.sparkEffect.Play();

            // Adjust the light intensity
            // if (chosenRandomizer.sparkLight != null)
            // {
            //     float randomIntensity = Random.Range(maxLightIntensity - lightChangeRange, maxLightIntensity);
            //     chosenRandomizer.sparkLight.intensity = randomIntensity;
            // }
            if (chosenRandomizer.sparkLight != null)
            {
                chosenRandomizer.sparkLight.intensity += lightChangeRangeRate;
                chosenRandomizer.sparkLight.intensity = Mathf.Min(chosenRandomizer.sparkLight.intensity, maxLightIntensity);
            }

            // Wait for a random amount of time between minTime and maxTime before playing the next one
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);

            // Turn off the light when the effect is done
            if (chosenRandomizer.sparkLight != null)
            {
                chosenRandomizer.sparkLight.intensity = 0f;
            }
        }
    }
}