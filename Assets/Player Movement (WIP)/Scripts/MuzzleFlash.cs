using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public ParticleSystem muzzleFlash;
    public void PlayFlash() => muzzleFlash.Play();
}
