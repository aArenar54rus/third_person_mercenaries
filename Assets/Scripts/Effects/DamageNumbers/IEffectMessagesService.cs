using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arenar.Services.UI
{
    public interface IEffectMessagesService
    {
        void SpawnDamageNumber(Transform target, int damage);

        void SpawnCritical(Transform target);

        void SpawnStun(Transform target);
        
        void SpawnHealing(Transform target, int damage);
    }
}