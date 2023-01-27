/****************
 *@class name:		BaseButton
 *@description:		自定义button 添加点击间隔，长按功能
 *@author:			selik0
 *@date:			2023-01-14 15:46:18
 *@version: 		V1.0.0
*************************************************************************/
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    public class BaseButton : Button
    {
        /// <summary>
        /// 是否开启长按功能
        /// </summary>
        [SerializeField]
        public bool isOpenLongPress = false;
        [FormerlySerializedAs("LongPressed")]
        [SerializeField]
        private ButtonClickedEvent m_OnLongPress = new ButtonClickedEvent();
        public ButtonClickedEvent onLongPressed
        {
            get { return m_OnLongPress; }
            set { m_OnLongPress = value; }
        }
        [SerializeField]
        private float m_LongPressedTime = .2f;
        private WaitForSecondsRealtime m_LongPressedWaitTime;

        [SerializeField]
        private float m_ClickIntervalTime = .2f;
        /// <summary>
        /// 点击间隔时间，防止快速点击
        /// </summary>
        /// <value></value>
        public float clickIntervalTime
        {
            get { return m_ClickIntervalTime; }
            set { m_ClickIntervalTime = value; m_ClickIntervalWaitTime = new WaitForSecondsRealtime(value); }
        }
        private bool m_IsInClickInterval = false;
        private WaitForSecondsRealtime m_ClickIntervalWaitTime;

        protected override void Awake()
        {
            base.Awake();
            m_ClickIntervalWaitTime = new WaitForSecondsRealtime(m_ClickIntervalTime);
            m_LongPressedWaitTime = new WaitForSecondsRealtime(m_LongPressedTime);
        }

        protected override void OnDisable()
        {
            StopAllCoroutines();
            m_IsInClickInterval = false;
            base.OnDisable();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            StartCoroutine(LongPressedCor());
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            StopCoroutine(LongPressedCor());
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (m_IsInClickInterval) return;
            base.OnPointerClick(eventData);
            StartCoroutine(ClickIntervalCor());
        }

        IEnumerator ClickIntervalCor()
        {
            m_IsInClickInterval = true;
            yield return m_ClickIntervalWaitTime;
            m_IsInClickInterval = false;
        }

        IEnumerator LongPressedCor()
        {
            yield return m_ClickIntervalWaitTime;
            LongPress();
        }

        private void LongPress()
        {
            if (!IsActive() || !IsInteractable() || !IsPressed())
                return;

            UISystemProfilerApi.AddMarker("Button.onLongPress", this);
            m_OnLongPress.Invoke();
        }
    }
}
