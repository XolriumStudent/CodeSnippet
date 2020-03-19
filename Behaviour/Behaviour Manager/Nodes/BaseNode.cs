using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterPresentatie.BehaviourManager
{
    public abstract class BaseNode : ScriptableObject
    {
        public Rect windowRectangle;
        public string windowTitle;

        public virtual void DrawWindow()
        {

        }

        public virtual void DrawCurveOnWindow()
        {

        }
    }
}

