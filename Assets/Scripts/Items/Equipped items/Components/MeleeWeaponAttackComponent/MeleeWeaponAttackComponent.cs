using Arenar;
using Arenar.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponAttackComponent : IMeleeWeaponAttackComponent
{
    private ICharacterEntity entityOwner;
    
    
    public void SetOwner(ICharacterEntity entityOwner)
    {
        this.entityOwner = entityOwner;
    }

    public void MakeMeleeAttack(DamageData damageData)
    {
        Collider[] hitBox = Physics.OverlapSphere(entityOwner.CharacterTransform.position
            + entityOwner.CharacterTransform.forward, 1);

        if (hitBox.Length == 0)
        {
            return;
        }

        bool isAttackSuccess = false;
        List<ICharacterLiveComponent> attacked = new List<ICharacterLiveComponent>();
        foreach (var hit in hitBox)
        {
            if (!hit.TryGetComponent<ICharacterEntity>(out ICharacterEntity attackedComponentsOwner)
                || attackedComponentsOwner == entityOwner
                || !attackedComponentsOwner.TryGetCharacterComponent(out ICharacterLiveComponent liveComponent)
                || attacked.Contains(liveComponent))
            {
                continue;
            }

            liveComponent.SetDamage(damageData);
            attacked.Add(liveComponent);
        }
            
        return;
    }
}
