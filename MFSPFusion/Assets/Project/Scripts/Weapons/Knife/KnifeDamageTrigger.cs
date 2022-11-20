using UnityEngine;

public class KnifeDamageTrigger : MonoBehaviour
{
    [SerializeField] Knife knife;

    void OnTriggerEnter(Collider other)
    {
        BaseCharacter character = other.GetComponent<BaseCharacter>();
        if(character != null)
        {
            Debug.Log($"Apply knife damage {knife.data.weaponDamage}");
            character.ApplyDamage(knife.data.weaponDamage);
        }
    }
}
