using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterPresentatie
{
    [CreateAssetMenu]
    public class State : ScriptableObject
    {
        public StateActions[] stateActionsOnUpdate;
        public StateActions[] stateActionsOnEnter;
        public StateActions[] stateActionsOnExit;

        public List<Transition> transitions = new List<Transition>();

        public void CheckForTransitions(StateManager _states)
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                if (transitions[i].isDisabled)
                    continue;
                if (transitions[i].condition.CheckForCondition(_states))
                {
                    if(transitions[i].targetState != null)
                    {
                        _states.currentState = transitions[i].targetState;
                        StateActionsOnExit(_states);
                        _states.currentState.StateActionsOnEnter(_states);
                    }
                    return;
                }
            }
        }

        public void Tick(StateManager _states)
        {
            ExecuteActions(_states, stateActionsOnUpdate);
            CheckForTransitions(_states);
        }

        public void ExecuteActions(StateManager _states, StateActions[] l)
        {
            for (int i = 0; i < l.Length; i++)
            {
                if (l[i] != null)
                    l[i].Execute(_states);
            }
        }

        public void StateActionsOnEnter(StateManager _states)
        {
            ExecuteActions(_states, stateActionsOnExit);
        }

        public void StateActionsOnExit(StateManager _states)
        {
            ExecuteActions(_states, stateActionsOnEnter);
        }

        public Transition CreateTransition()
        {
            Transition returnValue = new Transition();
            transitions.Add(returnValue);

            return returnValue;
        }
    }
}