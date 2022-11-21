using UnityEngine;

public class KnifeDamageTrigger : MonoBehaviour
{
    [SerializeField] Knife knife;

    void OnTriggerEnter(Collider other)
    {
        BaseCharacter character = other.GetComponent<BaseCharacter>();
        BaseCharacter thisChar = transform.root.GetComponent<BaseCharacter>();
        if(character != null && character.GetInstanceID() != thisChar.GetInstanceID())
        {
            Debug.Log($"Apply knife damage {knife.data.weaponDamage}");
            character.ApplyDamage(knife.data.weaponDamage);
        }
    }
}
