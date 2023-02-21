/****************
 *@class name:		StateButton
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
    [AddComponentMenu("UI/StateButton", 30)]
    [ExecuteAlways]
    [SelectionBase]
    public class StateButton : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [Serializable]
        public class StateData
        {
            public int state;
            public string stateDesc;
            public List<Graphic> graphics = new List<Graphic>();

            public StateData(int _state, string _stateDesc)
            {
                state = _state;
                stateDesc = _stateDesc;
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

        [FormerlySerializedAs("stateDataList")]
        [SerializeField]
        private List<StateData> m_StateDataList = new List<StateData>();

        [FormerlySerializedAs("state")]
        [SerializeField]
        private int m_CurrentState;

        public int currentState
        {
            get => m_CurrentState;
            set => SetState(value);
        }

        // 是否开启点击间隔
        public bool isOpenClickInterval = false;
        // 点击间隔的时间
        public float clickIntervalTime = .2f;
        // 是否可以点击
        private bool m_IsInClickInterval = false;

        private Dictionary<int, StateData> m_StateDataDict = new Dictionary<int, StateData>();
        private Dictionary<int, StateData> _stateDataDict
        {
            get
            {
                if (m_StateDataDict.Count <= 0)
                {
                    foreach (var stateData in m_StateDataList)
                    {
                        if (!m_StateDataDict.ContainsKey(stateData.state))
                            m_StateDataDict.Add(stateData.state, stateData);
                    }
                }
                return m_StateDataDict;
            }
        }

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        protected StateButton() { }


        protected override void Awake()
        {
            base.Awake();
            // 至少要有一个状态
            if (m_StateDataList.Count <= 0)
            {
                m_StateDataList.Add(new StateData(1, "state"));
                m_StateDataList.Add(new StateData(2, "state"));
            }

            RefreshStateDataDic();

            SetState(m_StateDataList[0].state);
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
            m_OnClick.Invoke(m_CurrentState);
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

        public void SetState(int state)
        {
            if (!_stateDataDict.ContainsKey(state))
                return;
            m_CurrentState = state;
            PlayEffect();
        }

        void PlayEffect()
        {
            StateData currentStateData = null;
            if (!_stateDataDict.TryGetValue(m_CurrentState, out currentStateData))
                return;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                foreach (var stateData in m_StateDataList)
                {
                    foreach (var item in stateData.graphics)
                    {
                        if (item != null)
                            item.canvasRenderer.SetAlpha(currentStateData.state == stateData.state ? 0f : 1f);
                    }
                }
            }
            else
#endif
            {
                foreach (var stateData in m_StateDataList)
                {
                    foreach (var item in stateData.graphics)
                    {
                        if (item != null)
                            item.CrossFadeAlpha(currentStateData.state == stateData.state ? 0f : 1f, 0, true);
                    }
                }
            }
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            if (m_StateDataList.Count <= 0)
                m_StateDataList.Add(new StateData(1, "state1"));
        }

        protected override void GetGraphics(Transform go, List<Graphic> arr)
        {
            Selectable selectable = go.GetComponent<Selectable>();
            if (selectable != null && selectable != this)
                return;

            Graphic graphic = go.GetComponent<Graphic>();

            bool isContains = false;
            foreach (var stateData in m_StateDataList)
            {
                if (stateData.graphics != null && stateData.graphics.Contains(graphic))
                {
                    isContains = true;
                    break;
                }
            }

            if (graphic != null && !isContains)
                arr.Add(graphic);

            for (int i = 0; i < go.childCount; i++)
                GetGraphics(go.GetChild(i), arr);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            // 当前的状态id必须在状态列表里
            m_StateDataDict.Clear();
            if (!_stateDataDict.ContainsKey(m_CurrentState))
                m_CurrentState = m_StateDataList[0].state;
        }

        public void ResetStateDataDic()
        {
            RefreshStateDataDic();
        }

#endif // UNITY_EDITOR

        private void RefreshStateDataDic()
        {
            foreach (var stateData in m_StateDataList)
            {
                if (_stateDataDict.ContainsKey(stateData.state))
                    Debug.LogWarning($"状态列表里有重复的状态id={stateData.state}");
                else
                    _stateDataDict.Add(stateData.state, stateData);
            }
        }

        protected override void StartColorTween(Color targetColor, bool instant)
        {
            StateData currentStateData = null;
            if (!_stateDataDict.TryGetValue(m_CurrentState, out currentStateData))
                return;

            if (currentStateData != null && currentStateData.graphics != null && currentStateData.graphics.Count > 0)
            {
                foreach (var graphic in currentStateData.graphics)
                {
                    if (graphic != null)
                        graphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
                }
            }

            if (m_ColorTintGraphics != null && m_ColorTintGraphics.Length > 0)
            {
                foreach (var graphic in m_ColorTintGraphics)
                {
                    if (graphic != null)
                        graphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
                }
            }
        }
    }
}