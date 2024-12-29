using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using FAE;

public class PostChangingEffect : MonoBehaviour
{
    public float targetSaturation = 0f;
    public float windStrengthIncrease = 0.5f; 
    public float windWeightIncrease = 1.0f;
    public float windSwingIncrease = 0.3f; 
    public float effectDuration = 2f;

    private ColorAdjustments colorAdjustments;
    private float originalSaturation;
    private Coroutine effectCoroutine;
  
    private WindController windController;
    private float originalWindStrength;
    private float originalWindWeight;
    private float originalWindSwing;

    private void Start()
    {
        Volume postProcessVolume = FindObjectOfType<Volume>();
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet(out colorAdjustments);
            if (colorAdjustments != null)
                originalSaturation = colorAdjustments.saturation.value;
        }

        windController = FindObjectOfType<WindController>();
        if (windController != null)
        {
            originalWindStrength = windController.windStrength;
            originalWindWeight = windController.trunkWindWeight;
            originalWindSwing = windController.trunkWindSwinging;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (effectCoroutine != null)
            {
                StopCoroutine(effectCoroutine);
            }
            effectCoroutine = StartCoroutine(ApplyEffects());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (effectCoroutine != null)
            {
                StopCoroutine(effectCoroutine);
            }
            effectCoroutine = StartCoroutine(ResetEffects());
        }
    }

    private IEnumerator ApplyEffects()
    {
        float elapsedTime = 0f;
        float currentSaturation = colorAdjustments.saturation.value;
        float currentWindStrength = windController.windStrength;
        float currentWindWeight = windController.trunkWindWeight;
        float currentWindSwing = windController.trunkWindSwinging;

        while (elapsedTime < effectDuration)
        {
            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = Mathf.Lerp(currentSaturation, targetSaturation, elapsedTime / effectDuration);
            }

            if (windController != null)
            {
                float targetStrength = originalWindStrength + windStrengthIncrease;
                float targetWeight = originalWindWeight + windWeightIncrease;
                float targetSwing = originalWindSwing + windSwingIncrease;

                windController.SetStrength(Mathf.Lerp(currentWindStrength, targetStrength, elapsedTime / effectDuration));
                windController.SetTrunkWeight(Mathf.Lerp(currentWindWeight, targetWeight, elapsedTime / effectDuration));
                windController.trunkWindSwinging = Mathf.Lerp(currentWindSwing, targetSwing, elapsedTime / effectDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = targetSaturation;
        }

        if (windController != null)
        {
            windController.SetStrength(originalWindStrength + windStrengthIncrease);
            windController.SetTrunkWeight(originalWindWeight + windWeightIncrease);
            windController.trunkWindSwinging = originalWindSwing + windSwingIncrease;
        }
    }

    private IEnumerator ResetEffects()
    {
        float elapsedTime = 0f;
        float currentSaturation = colorAdjustments.saturation.value;
        float currentWindStrength = windController.windStrength;
        float currentWindWeight = windController.trunkWindWeight;
        float currentWindSwing = windController.trunkWindSwinging;

        while (elapsedTime < effectDuration)
        {
            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = Mathf.Lerp(currentSaturation, originalSaturation, elapsedTime / effectDuration);
            }

            if (windController != null)
            {
                windController.SetStrength(Mathf.Lerp(currentWindStrength, originalWindStrength, elapsedTime / effectDuration));
                windController.SetTrunkWeight(Mathf.Lerp(currentWindWeight, originalWindWeight, elapsedTime / effectDuration));
                windController.trunkWindSwinging = Mathf.Lerp(currentWindSwing, originalWindSwing, elapsedTime / effectDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = originalSaturation;
        }

        if (windController != null)
        {
            windController.SetStrength(originalWindStrength);
            windController.SetTrunkWeight(originalWindWeight);
            windController.trunkWindSwinging = originalWindSwing;
        }
    }
}
