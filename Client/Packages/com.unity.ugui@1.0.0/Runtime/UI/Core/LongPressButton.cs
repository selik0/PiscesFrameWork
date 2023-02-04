/****************
 *@class name:		LongPress
 *@description:		
 *@author:			selik0
 *@date:			2023-02-04 09:29:30
 *@version: 		V1.0.0
*************************************************************************/
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
namespace UnityEngine.UI
{
    public class LongPressButton : Button
    {
        [FormerlySerializedAs("onLongPress")]
        [SerializeField]
        private ButtonClickedEvent m_OnLongPress = new ButtonClickedEvent();

        // 是否开启长按功能
        public bool isOpenLongPress = false;
        // 长按回调激活的最大时间
        public float longPressTime = .2f;

        public ButtonClickedEvent onLongPress
        {
            get { return m_OnLongPress; }
            set { m_OnLongPress = value; }
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

            if (isOpenLongPress)
                StartCoroutine(LongPressedCor());
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (isOpenLongPress)
                StopCoroutine(LongPressedCor());
        }

        IEnumerator LongPressedCor()
        {
            var time = longPressTime;
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