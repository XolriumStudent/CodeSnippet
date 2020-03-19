using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterPresentatie
{
    [CreateAssetMenu(menuName = "Actions/Debug/Add Health")]
    public class ChangeHealth : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.health += 10;
        }
    }
}
