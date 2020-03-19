using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System;
using MasterPresentatie;

namespace MasterPresentatie.BehaviourManager
{
    public class StateNode : BaseNode
    {

        public State currentState;
        private bool isCollapsed;
        private State previousState;

        private SerializedObject serializedState;
        private ReorderableList stateActionOnUpdateList;
        private ReorderableList stateActionOnEnterList;
        private ReorderableList stateActionOnExitList;

        public List<BaseNode> dependencies = new List<BaseNode>();

        public override void DrawWindow()
        {
            if(currentState == null)
            {
                EditorGUILayout.LabelField("State: ");
            }
            else
            {
                if (isCollapsed)
                    windowRectangle.height = 100;

                isCollapsed = EditorGUILayout.Toggle("", isCollapsed);
            }
            
                

            currentState = (State)EditorGUILayout.ObjectField(currentState, typeof(State), false);

            if(previousState != currentState)
            {
                serializedState = null;

                previousState = currentState;
                ClearReferences();

                for (int i = 0; i < currentState.transitions.Count; i++)
                {
                    dependencies.Add(BehaviourManager.CreateTransitionNode(i, currentState.transitions[i], this));
                }
            }

            if(currentState != null)
            {
                if(serializedState == null)
                {
                    serializedState = new SerializedObject(currentState);
                    stateActionOnUpdateList = new ReorderableList(serializedState, serializedState.FindProperty("stateActionsOnUpdate"), true, true, true, true);
                    stateActionOnEnterList = new ReorderableList(serializedState, serializedState.FindProperty("stateActionsOnEnter"), true, true, true, true);
                    stateActionOnExitList = new ReorderableList(serializedState, serializedState.FindProperty("stateActionsOnExit"), true, true, true, true);
                }

                if (!isCollapsed)
                {
                    serializedState.Update();

                    HandleReorderableList(stateActionOnEnterList, "State Action On Enter");
                    EditorGUILayout.LabelField("");
                    stateActionOnEnterList.DoLayoutList();

                    HandleReorderableList(stateActionOnUpdateList, "State Action On Update");
                    EditorGUILayout.LabelField("");
                    stateActionOnUpdateList.DoLayoutList();

                    HandleReorderableList(stateActionOnExitList, "State Action On Exit");
                    EditorGUILayout.LabelField("");
                    stateActionOnExitList.DoLayoutList();

                    serializedState.ApplyModifiedProperties();

                    float standard = 350;
                    standard += (stateActionOnUpdateList.count) * 15;
                    windowRectangle.height = standard;
                }
            }
        }

        public void HandleReorderableList(ReorderableList _list, string _name)
        {
            _list.drawHeaderCallback = (Rect rectangle) =>
            {
                EditorGUI.LabelField(rectangle, _name);
            };

            _list.drawElementCallback = (Rect rectangle, int index, bool isActive, bool isFocused) =>
            {
                var element = _list.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rectangle.x, rectangle.y, rectangle.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };
        }

        public override void DrawCurveOnWindow()
        {
            
        }

        public Transition CreateTransition()
        {
            return currentState.CreateTransition();
        }

        public void ClearReferences()
        {
            BehaviourManager.ClearWindowsFromList(dependencies);
            dependencies.Clear();
        }
    }
}