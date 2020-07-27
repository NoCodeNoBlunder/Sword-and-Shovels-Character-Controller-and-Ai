
using UnityEngine;

public class AttackedMobPlaySound : MonoBehaviour, IAttackable
{
    public void OnAttack(GameObject attacker, Attack attack)
    {
        SoundManager.Instance.PlaySoundEffect(SoundEffect.MOBDAMAGE);
    }
}
