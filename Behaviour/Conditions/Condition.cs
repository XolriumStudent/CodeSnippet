using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterPresentatie.BehaviourManager;

namespace MasterPresentatie
{
    public abstract class Condition : ScriptableObject
    {
        public abstract bool CheckForCondition(StateManager _state);
    }
}