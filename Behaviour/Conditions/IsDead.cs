using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterPresentatie.BehaviourManager;

namespace MasterPresentatie
{
    [CreateAssetMenu(menuName ="Conditions/Is Dead")]
    public class IsDead : Condition
    {
        public override bool CheckForCondition(StateManager _state)
        {
            return _state.health <= 0;
        }
    }
}
