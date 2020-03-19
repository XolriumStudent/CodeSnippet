using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MasterPresentatie.BehaviourManager
{
    public class TransitionNode : BaseNode
    {
        public Transition targetTransition;
        public StateNode startState;
        public StateNode endState;

        public void Initialize(StateNode _startState, Transition _transition)
        {
            startState = _startState;
            targetTransition = _transition;
        }

        public override void DrawWindow()
        {
            if (targetTransition == null)
                return;

            EditorGUILayout.LabelField("");
            targetTransition.condition = (Condition)EditorGUILayout.ObjectField(targetTransition.condition, typeof(Condition), false);

            if (targetTransition.condition == null)
                EditorGUILayout.LabelField("Condition is empty.");
            else
                targetTransition.isDisabled = EditorGUILayout.Toggle("", targetTransition.isDisabled);
        }

        public override void DrawCurveOnWindow()
        {
            if (startState)
            {
                Rect rectangle = windowRectangle;
                rectangle.y += windowRectangle.height * .5f;
                rectangle.width = 1;
                rectangle.height = 1;

                BehaviourManager.DrawNodeCurve(startState.windowRectangle, rectangle, true, Color.blue);
            }
        }
    }
}
