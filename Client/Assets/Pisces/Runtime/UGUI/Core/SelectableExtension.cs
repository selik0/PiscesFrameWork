/****************
 *@class name:		SelectableExtension
 *@description:		
 *@author:			selik0
 *@date:			2023-01-15 09:35:13
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.UI
{
    public class SelectableExtension : Selectable
    {
        private Graphic[] graphics;

        protected override void InstantClearState()
        {
            base.InstantClearState();

            if (transition == Transition.ColorTint)
                StartColorTween(Color.white, true);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy || transition != Transition.ColorTint)
                return;

            Color tintColor;

            switch (state)
            {
                case SelectionState.Normal:
                    tintColor = colors.normalColor;
                    break;
                case SelectionState.Highlighted:
                    tintColor = colors.highlightedColor;
                    break;
                case SelectionState.Pressed:
                    tintColor = colors.pressedColor;
                    break;
                case SelectionState.Selected:
                    tintColor = colors.selectedColor;
                    break;
                case SelectionState.Disabled:
                    tintColor = colors.disabledColor;
                    break;
                default:
                    tintColor = Color.black;
                    break;
            }

            StartColorTween(tintColor * colors.colorMultiplier, instant);
        }

        void StartColorTween(Color targetColor, bool instant)
        {
            if (graphics == null || graphics.Length <= 0)
                return;

            foreach (var graphic in graphics)
            {
                if (graphic != null)
                    graphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (isActiveAndEnabled && transition == Transition.ColorTint)
                StartColorTween(Color.white, true);
        }
#endif

        private void OnTransformChildrenChanged()
        {
            Debug.Log("123");
            if (transition == Transition.ColorTint)
                graphics = GetComponentsInChildren<Graphic>();
        }
    }
}