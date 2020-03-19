using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterPresentatie.BehaviourManager
{
    public class CommentNode : BaseNode
    {
        private string comment = "Enter comment";

        public override void DrawWindow()
        {
            comment = GUILayout.TextArea(comment, 200);
        }
    }

}