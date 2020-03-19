using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MasterPresentatie.BehaviourManager
{
    public class BehaviourManager : EditorWindow
    {
        #region Variables
        private static List<BaseNode> windows = new List<BaseNode>();
        private Vector3 currentMousePosition;
        private bool createTransition;
        private bool isWindowInteracted;
        private int selectedIndex;
        private BaseNode currentSelectedNode;

        public enum UserInteractions {ADDSTATE, CREATETRANSITION, DELETENODE, COMMENTNODE}
        #endregion

        #region Initialization
        [MenuItem("Behaviour Manager/BH Manager")]
        private static void ShowEditor()
        {
            BehaviourManager manager = EditorWindow.GetWindow<BehaviourManager>();
            manager.minSize = new Vector2(450, 450);
        }
        #endregion

        #region UI Functions
        private void OnGUI()
        {
            Event currentEvent = Event.current;
            currentMousePosition = currentEvent.mousePosition;
            UserInput(currentEvent);
            DrawWindows();
        }

        //private void OnEnable()
        //{
        //    windows.Clear();
        //}

        private void DrawWindows()
        {
            BeginWindows();
            foreach (BaseNode node in windows)
            {
                node.DrawCurveOnWindow();
            }

            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].windowRectangle = GUI.Window(i, windows[i].windowRectangle, DrawNodeWindow, windows[i].windowTitle);
            }

            EndWindows();
        }

        private void DrawNodeWindow(int _id)
        {
            windows[_id].DrawWindow();
            GUI.DragWindow();
        }

        private void UserInput(Event _event)
        {
            if(_event.button == 1 && !createTransition)
            {
                if(_event.type == EventType.MouseDown)
                {
                    RightClick(_event);
                }
            }

            if (_event.button == 0 && !createTransition)
            {
                if (_event.type == EventType.MouseDown)
                {
                    
                }
            }
        }

        private void RightClick(Event _event)
        {
            selectedIndex = -1;
            isWindowInteracted = false;
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRectangle.Contains(_event.mousePosition))
                {
                    currentSelectedNode = windows[i];
                    isWindowInteracted = true;
                    selectedIndex = i;
                    break;
                }
            }

            if (!isWindowInteracted)
            {
                AddNewNode(_event);
            }
            else
            {
                ModifyExistingNode(_event);
            }
        }

        private void AddNewNode(Event _event)
        {
            GenericMenu genericMenu = new GenericMenu();

            genericMenu.AddSeparator("");
            genericMenu.AddItem(new GUIContent("Add New State"), false, ContextCallBack, UserInteractions.ADDSTATE);
            genericMenu.AddItem(new GUIContent("Add New Comment"), false, ContextCallBack, UserInteractions.COMMENTNODE);

            genericMenu.ShowAsContext();
            _event.Use();
        }

        private void ModifyExistingNode(Event _event)
        {
            GenericMenu genericMenu = new GenericMenu();

            if (currentSelectedNode is StateNode)
            {
                StateNode stateNode = (StateNode)currentSelectedNode;
                if(stateNode.currentState != null)
                {
                    genericMenu.AddSeparator("");
                    genericMenu.AddItem(new GUIContent("Create Transition"), false, ContextCallBack, UserInteractions.CREATETRANSITION);
                }
                else
                {
                    genericMenu.AddSeparator("");
                    genericMenu.AddDisabledItem(new GUIContent("Create Transition"));
                }

                genericMenu.AddSeparator("");
                genericMenu.AddItem(new GUIContent("Delete State"), false, ContextCallBack, UserInteractions.DELETENODE);
            }

            if (currentSelectedNode is TransitionNode)
            {
                genericMenu.AddSeparator("");
                genericMenu.AddItem(new GUIContent("Delete Condition"), false, ContextCallBack, UserInteractions.DELETENODE);
            }

            if (currentSelectedNode is CommentNode)
            {
                genericMenu.AddSeparator("");
                genericMenu.AddItem(new GUIContent("Delete Comment"), false, ContextCallBack, UserInteractions.DELETENODE);
            }

            genericMenu.ShowAsContext();
            _event.Use();
        }

        private void ContextCallBack(object _object)
        {
            UserInteractions interactions = (UserInteractions)_object;
            switch (interactions)
            {
                case UserInteractions.ADDSTATE:
                    StateNode stateNode = new StateNode { windowRectangle = new Rect(currentMousePosition.x, currentMousePosition.y, 200, 250),
                        windowTitle = "State" };
                    windows.Add(stateNode);
                    break;
                case UserInteractions.CREATETRANSITION:
                    if(currentSelectedNode is StateNode)
                    {
                        StateNode node = (StateNode)currentSelectedNode;
                        Transition transition = node.CreateTransition();
                        CreateTransitionNode(node.currentState.transitions.Count, transition, node);
                    }
                    break;
                case UserInteractions.DELETENODE:
                    if(currentSelectedNode is StateNode)
                    {
                        StateNode targetNode = (StateNode)currentSelectedNode;
                        targetNode.ClearReferences();
                        windows.Remove(targetNode);
                    }

                    if(currentSelectedNode is TransitionNode)
                    {
                        TransitionNode target = (TransitionNode)currentSelectedNode;
                        windows.Remove(target);

                        if (target.startState.currentState.transitions.Contains(target.targetTransition))
                            target.startState.currentState.transitions.Remove(target.targetTransition);
                    }

                    if(currentSelectedNode is CommentNode)
                    {
                        windows.Remove(currentSelectedNode);
                    }
                    break;
                case UserInteractions.COMMENTNODE:
                    CommentNode commentNode = new CommentNode { windowRectangle = new Rect(currentMousePosition.x, currentMousePosition.y, 150, 75),
                        windowTitle = "Comment" };
                    windows.Add(commentNode);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Extra Functions
        public static TransitionNode CreateTransitionNode(int _index, Transition _transition, StateNode _from)
        {
            Rect fromRectangle = _from.windowRectangle;
            fromRectangle.x += 50;
            float targetY = fromRectangle.y - fromRectangle.height;

            if (_from.currentState != null)
                targetY += (_index * 100);

            fromRectangle.y = targetY;

            TransitionNode transitionNode = CreateInstance<TransitionNode>();
            transitionNode.Initialize(_from, _transition);
            transitionNode.windowRectangle = new Rect(fromRectangle.x + 200 + 100, fromRectangle.y + (fromRectangle.height * .7f), 200, 80);
            transitionNode.windowTitle = "Condition Check";
            windows.Add(transitionNode);
            _from.dependencies.Add(transitionNode);

            return transitionNode;
        }

        public static void DrawNodeCurve(Rect _start, Rect _end, bool _left, Color _curveColor)
        {
            Vector3 startPosition = new Vector3((_left) ? _start.x + _start.width : _start.x, _start.y + (_start.height * .5f), 0);
            Vector3 endPosition = new Vector3(_end.x + (_end.width * .5f), _end.y + (_end.height * .5f), 0);

            Vector3 startTransition = startPosition + Vector3.right * 50;
            Vector3 endTransition = endPosition + Vector3.left * 50;

            Color shadow = new Color(0, 0, 0, 0.06f);
            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPosition, endPosition, startTransition, endTransition, shadow, null, (i + 1) * .5f);
            }

            Handles.DrawBezier(startPosition, endPosition, startTransition, endTransition, _curveColor, null, 1);
        }

        public static void ClearWindowsFromList(List<BaseNode> l)
        {
            for (int i = 0; i < l.Count; i++)
            {
                if (windows.Contains(l[i]))
                    windows.Remove(l[i]);
            }
        }
        #endregion

    }
}