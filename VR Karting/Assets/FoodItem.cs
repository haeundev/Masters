using UnityEngine;

public class FoodItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            PlayFoodSFX();
    }

    private void PlayFoodSFX()
    {
        GlobalInfo.Score++;
        
        var sfx = new GameObject("SFX").AddComponent<AudioSource>();
        sfx.clip = GetComponentInChildren<FoodItemCommon>().eatSound;
        sfx.volume = .5f;
        sfx.Play();
        var duration = sfx.clip.length;
        Destroy(sfx.gameObject, duration);
    }
}