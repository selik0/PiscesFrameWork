/****************
 *@class name:		MyStateButton
 *@description:		自定义button， unity默认button + 状态，用于一个按钮可能有多种状态的情况
 *@author:			selik0
 *@date:			2023-02-02 12:08:32
 *@version: 		V1.0.0
*************************************************************************/
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
namespace UnityEngine.UI
{
    [AddComponentMenu("UI/MyStateButton", 30)]
    public class MyStateButton : MySelectable, IPointerClickHandler, ISubmitHandler
    {
        [Serializable] public class ButtonClickedEvent : UnityEvent<int> { }

        // Event delegates triggered on click.
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        [FormerlySerializedAs("state")]
        [SerializeField]
        private int m_State = 0;

        protected MyStateButton() { }

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        protected virtual void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClick.Invoke(m_State);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}
