/****************
 *@class name:		StatusButton
 *@description:		状态按钮
 *@author:			selik0
 *@date:			2023-02-04 10:31:10
 *@version: 		V1.0.0
*************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
namespace UnityEngine.UI
{
    public class StatusButton : Selectable, IPointerClickHandler, ISubmitHandler
    {
        private class StatusData
        {
            public int status;
            public string statusDesc;
            public List<Graphic> graphics = new List<Graphic>();

            public StatusData(int _status, string _statusDesc)
            {
                status = _status;
                statusDesc = _statusDesc;
            }
        }

        [Serializable]
        /// <summary>
        /// Function definition for a button click event.
        /// </summary>
        public class ButtonClickedEvent : UnityEvent<int> { }

        // Event delegates triggered on click.
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        [FormerlySerializedAs("statusDataList")]
        [SerializeField]
        private List<StatusData> m_StatusDataList = new List<StatusData>();

        [FormerlySerializedAs("status")]
        [SerializeField]
        private int m_CurrentStatus;

        // 是否开启点击间隔
        public bool isOpenClickInterval = false;
        // 点击间隔的时间
        public float clickIntervalTime = .2f;
        // 是否可以点击
        private bool m_IsInClickInterval = false;

        private Dictionary<int, StatusData> m_StatusDataDict = new Dictionary<int, StatusData>();

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        protected StatusButton() { }


        protected override void Awake()
        {
            base.Awake();
            if (m_StatusDataList.Count <= 0)
                m_StatusDataList.Add(new StatusData(1, "status1"));

            foreach (var statusData in m_StatusDataList)
            {
                if (m_StatusDataDict.ContainsKey(statusData.status))
                    Debug.LogWarning($"状态列表里有重复的状态id={statusData.status}");
                else
                    m_StatusDataDict.Add(statusData.status, statusData);
            }

            SetStatus(m_StatusDataList[0].status);
        }

        protected override void OnDisable()
        {
            // 禁用时停止协程
            StopAllCoroutines();
            m_IsInClickInterval = false;
            base.OnDisable();
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable() || (isOpenClickInterval && m_IsInClickInterval))
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClick.Invoke(m_CurrentStatus);
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

            var time = clickIntervalTime;
            var elapsedTime = 0f;

            while (elapsedTime < time)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            m_IsInClickInterval = false;
        }

        public void SetStatus(int status)
        {
            if (!m_StatusDataDict.ContainsKey(status))
                return;
            m_CurrentStatus = status;
            PlayEffect();
        }

        void PlayEffect()
        {
            StatusData currentStatusData = null;
            if (m_StatusDataDict.ContainsKey(m_CurrentStatus))
                currentStatusData = m_StatusDataDict[m_CurrentStatus];

            if (currentStatusData == null)
                return;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                foreach (var statusData in m_StatusDataList)
                {
                    foreach (var item in statusData.graphics)
                    {
                        if (item != null)
                            item.canvasRenderer.SetAlpha(currentStatusData.status == statusData.status ? 0f : 1f);
                    }
                }
            }
            else
#endif
            {
                foreach (var statusData in m_StatusDataList)
                {
                    foreach (var item in statusData.graphics)
                    {
                        if (item != null)
                            item.CrossFadeAlpha(currentStatusData.status == statusData.status ? 0f : 1f, 0, true);
                    }
                }
            }
        }

#if UNITY_EDITOR
        protected override void GetGraphics(Transform go, List<Graphic> arr)
        {
            if (go.GetComponent<Selectable>() != null)
                return;
            Graphic graphic = go.GetComponent<Graphic>();

            bool isContains = false;
            foreach (var statusData in m_StatusDataList)
            {
                if (statusData.graphics != null && statusData.graphics.Contains(graphic))
                {
                    isContains = true;
                    break;
                }
            }

            if (graphic != null && !isContains)
                arr.Add(graphic);

            for (int i = 0; i < go.childCount; i++)
            {
                GetGraphics(go.GetChild(i), arr);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            // 至少要有一个状态
            if (m_StatusDataList.Count <= 0)
                m_StatusDataList.Add(new StatusData(1, "status"));

            // 当前的状态id必须在状态列表里
            if (!m_StatusDataDict.ContainsKey(m_CurrentStatus))
                m_CurrentStatus = m_StatusDataList[0].status;
        }
#endif // UNITY_EDITOR

        protected override void StartColorTween(Color targetColor, bool instant)
        {
            StatusData currentStatusData = null;
            if (m_StatusDataDict.ContainsKey(m_CurrentStatus))
                currentStatusData = m_StatusDataDict[m_CurrentStatus];

            if (currentStatusData != null && currentStatusData.graphics != null || currentStatusData.graphics.Count > 0)
            {
                foreach (var graphic in currentStatusData.graphics)
                {
                    if (graphic != null)
                        graphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
                }
            }

            if (m_HighlightGraphics != null || m_HighlightGraphics.Length > 0)
            {
                foreach (var graphic in m_HighlightGraphics)
                {
                    if (graphic != null)
                        graphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
                }
            }
        }
    }
}