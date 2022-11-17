using Andrius.Core.Utils;
using Fusion;
using UnityEngine;

public abstract class BaseCharacter : NetworkBehaviour
{
    public string characterName;
    public DamageScreen damageScreen;
    public float damageScreenvalue = 0.1f;
    public float damageScreenDuration = .5f;

    [Networked] public byte maxHealth { get; set; }
    [Networked(OnChanged = nameof(OnHealthChanged))] public float initHealth { get; set; }
    [Networked] public Teams playerTeam { get; set; }

    public HitboxRoot hitboxRoot;
    public PlayerStatsScreen statsScreen;
    public WeaponsHolder weaponsHolder;


    static void OnHealthChanged(Changed<BaseCharacter> changed)
    {
        changed.LoadNew();
        var newHp = changed.Behaviour.initHealth;

        if (changed.Behaviour.Object.HasInputAuthority && changed.Behaviour.initHealth != changed.Behaviour.maxHealth)
        {
            changed.Behaviour.damageScreen.FadeIn(changed.Behaviour.damageScreenvalue, changed.Behaviour.damageScreenDuration);
            changed.Behaviour.statsScreen.SetHealthStats(changed.Behaviour.initHealth, changed.Behaviour.maxHealth);

        }
        if (newHp <= 0)
        {
            changed.Behaviour.Die();
        }
    }

    public virtual void ApplyDamage(float damage)
    {
        if (initHealth <= 0) return;
        initHealth -= damage;
        //if (initHealth <= 0)
        //{
        //    Die();
        //}
    }

    protected virtual void Die()
    {
        foreach (var weapon in weaponsHolder.weaponsList)
        {
            if (weapon.isDropable)
            {
                weapon.DropWeapon();
            }
        }
        GameManager.instance.RespawnPlayer(this);
        gameObject.SetActive(false);
    }
}
