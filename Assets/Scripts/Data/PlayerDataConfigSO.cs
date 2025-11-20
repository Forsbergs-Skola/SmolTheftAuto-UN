using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataConfigSO", menuName = "Player Data/Player Data")]
public class PlayerDataConfigSO : ScriptableObject
{
    [SerializeField] private int playerMaxHealth;
    [SerializeField] private int playerMaxAmmo;
    [SerializeField] private int playerMaxGrenades;
}
