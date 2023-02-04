/****************
 *@class name:		MyButton
 *@description:		自定义button 添加点击间隔，长按功能
 *@author:			selik0
 *@date:			2023-01-14 15:46:18
 *@version: 		V1.0.0
*************************************************************************/
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/MyButton", 30)]
    public class MyButton : MySelectable, IPointerClickHandler, ISubmitHandler
    {
        [Serializable] public class ButtonClickedEvent : UnityEvent { }

        // Event delegates triggered on click.
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        // Event delegates triggered on long press.
        [FormerlySerializedAs("onLongPress")]
        [SerializeField]
        private ButtonClickedEvent m_OnLongPress = new ButtonClickedEvent();

        // 是否开启点击间隔
        [FormerlySerializedAs("isOpenClickInterval")]
        [SerializeField]
        private bool m_IsOpenClickInterval = false;
        // 点击间隔的时间
        [FormerlySerializedAs("clickIntervalTime")]
        [SerializeField]
        private float m_ClickIntervalTime = .2f;

        // 是否开启长按功能
        [FormerlySerializedAs("isOpenClickInterval")]
        [SerializeField]
        private bool m_IsOpenLongPress = false;
        // 长按回调激活的最大时间
        [FormerlySerializedAs("longPressTime")]
        [SerializeField]
        private float m_LongPressTime = .2f;

        protected MyButton() { }

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        public ButtonClickedEvent onLongPress
        {
            get { return m_OnLongPress; }
            set { m_OnLongPress = value; }
        }

        public float clickIntervalTime
        {
            get { return m_ClickIntervalTime; }
            set { m_ClickIntervalTime = value; }
        }

        public float longPressTime
        {
            get { return m_LongPressTime; }
            set { m_LongPressTime = value; }
        }

        // 是否可以点击
        private bool m_IsInClickInterval = false;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnDisable()
        {
            // 禁用时停止协程
            StopAllCoroutines();
            m_IsInClickInterval = false;
            base.OnDisable();
        }

        protected virtual void Press()
        {
            if (!IsActive() || !IsInteractable() || (m_IsOpenClickInterval && m_IsInClickInterval))
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClick.Invoke();

            if (m_IsOpenClickInterval)
                StartCoroutine(ClickIntervalCor());
        }

        protected virtual void LongPress()
        {
            if (!IsActive() || !IsInteractable() || !IsPressed())
                return;

            UISystemProfilerApi.AddMarker("Button.onLongPress", this);
            m_OnLongPress.Invoke();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (m_IsOpenLongPress)
                StartCoroutine(LongPressedCor());
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (m_IsOpenLongPress)
                StopCoroutine(LongPressedCor());
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

        IEnumerator ClickIntervalCor()
        {
            m_IsInClickInterval = true;

            var time = m_ClickIntervalTime;
            var elapsedTime = 0f;

            while (elapsedTime < time)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            m_IsInClickInterval = false;
        }

        IEnumerator LongPressedCor()
        {
            var time = m_LongPressTime;
            var elapsedTime = 0f;

            while (elapsedTime < time)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            LongPress();
        }
    }
}
