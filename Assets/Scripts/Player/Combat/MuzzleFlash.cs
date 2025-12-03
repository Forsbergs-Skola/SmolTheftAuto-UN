using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] private ParticleSystem muzzleFlash;
    public void PlayFlash() => muzzleFlash.Play();
    
}
