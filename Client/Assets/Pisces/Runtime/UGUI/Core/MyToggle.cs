using System.Collections.Generic;
/****************
 *@class name:		MyToggle
 *@description:		
 *@author:			selik0
 *@date:			2023-02-01 15:27:07
 *@version: 		V1.0.0
*************************************************************************/
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/MyToggle", 31)]
    [RequireComponent(typeof(RectTransform))]
    public class MyToggle : MySelectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
    {
        public enum ToggleTransition
        {
            None,
            Fade
        }

        [Serializable]
        public class ToggleEvent : UnityEvent<bool> { }
        public ToggleTransition toggleTransition = ToggleTransition.Fade;
        /// <summary>
        /// isOn为true时是否显示Background
        /// </summary>
        public bool isShowBackground = false;
        public List<Graphic> backgroundGraphics;
        public List<Graphic> checkmarkkGraphics;

        [SerializeField]
        private MyToggleGroup m_Group;
        public MyToggleGroup group
        {
            get { return m_Group; }
            set
            {
                SetToggleGroup(value, true);
                PlayEffect(true);
            }
        }
        public ToggleEvent onValueChanged = new ToggleEvent();

        // Whether the toggle is on
        [Tooltip("Is the toggle currently on or off?")]
        [SerializeField]
        private bool m_IsOn;

        protected MyToggle()
        { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(m_IsOn);
#endif
        }

        public virtual void LayoutComplete()
        { }

        public virtual void GraphicUpdateComplete()
        { }

        protected override void OnDestroy()
        {
            if (m_Group != null)
                m_Group.EnsureValidState();
            base.OnDestroy();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetToggleGroup(m_Group, false);
            PlayEffect(true);
        }

        protected override void OnDisable()
        {
            SetToggleGroup(null, false);
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Check if isOn has been changed by the animation.
            // Unfortunately there is no way to check if we don�t have a graphic.
            if (checkmarkkGraphics != null && checkmarkkGraphics.Count > 0 && checkmarkkGraphics[1] != null)
            {
                bool oldValue = !Mathf.Approximately(checkmarkkGraphics[1].canvasRenderer.GetColor().a, 0);
                if (m_IsOn != oldValue)
                {
                    m_IsOn = oldValue;
                    Set(!oldValue);
                }
            }

            base.OnDidApplyAnimationProperties();
        }

        private void SetToggleGroup(MyToggleGroup newGroup, bool setMemberValue)
        {
            // Sometimes IsActive returns false in OnDisable so don't check for it.
            // Rather remove the toggle too often than too little.
            if (m_Group != null)
                m_Group.UnregisterToggle(this);

            // At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
            // That's why we use the setMemberValue parameter.
            if (setMemberValue)
                m_Group = newGroup;

            // Only register to the new group if this Toggle is active.
            if (newGroup != null && IsActive())
                newGroup.RegisterToggle(this);

            // If we are in a new group, and this toggle is on, notify group.
            // Note: Don't refer to m_Group here as it's not guaranteed to have been set.
            if (newGroup != null && isOn && IsActive())
                newGroup.NotifyToggleOn(this);
        }

        public bool isOn
        {
            get { return m_IsOn; }

            set
            {
                Set(value);
            }
        }
        public void SetIsOnWithoutNotify(bool value)
        {
            Set(value, false);
        }

        void Set(bool value, bool sendCallback = true)
        {
            if (m_IsOn == value)
                return;

            // if we are in a group and set to true, do group logic
            m_IsOn = value;
            if (m_Group != null && m_Group.isActiveAndEnabled && IsActive())
            {
                if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.allowSwitchOff))
                {
                    m_IsOn = true;
                    m_Group.NotifyToggleOn(this, sendCallback);
                }
            }

            // Always send event when toggle is clicked, even if value didn't change
            // due to already active toggle in a toggle group being clicked.
            // Controls like Dropdown rely on this.
            // It's up to the user to ignore a selection being set to the same value it already was, if desired.
            PlayEffect(toggleTransition == ToggleTransition.None);
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Toggle.value", this);
                onValueChanged.Invoke(m_IsOn);
            }
        }
        private void PlayEffect(bool instant)
        {
            if (backgroundGraphics == null && checkmarkkGraphics == null)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                foreach (var item in backgroundGraphics)
                {
                    if (item != null)
                        item.canvasRenderer.SetAlpha(m_IsOn ? 0f : 1f);
                }
                foreach (var item in checkmarkkGraphics)
                {
                    if (item != null)
                        item.canvasRenderer.SetAlpha(m_IsOn ? 1f : 0f);
                }
            }
            else
#endif
            {
                foreach (var item in backgroundGraphics)
                {
                    if (item != null)
                        item.CrossFadeAlpha(m_IsOn ? 0f : 1f, instant ? 0f : 0.1f, true);
                }
                foreach (var item in checkmarkkGraphics)
                {
                    if (item != null)
                        item.CrossFadeAlpha(m_IsOn ? 1f : 0f, instant ? 0f : 0.1f, true);
                }
            }
        }
        protected override void Start()
        {
            PlayEffect(true);
        }

        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable())
                return;

            if (isOn && m_Group != null && !m_Group.allowSwitchOff && m_Group.noChangeDontSend)
                return;
            isOn = !isOn;
        }
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            InternalToggle();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            InternalToggle();
        }

        protected override void StartColorTween(Color targetColor, bool instant)
        {
            if (m_Graphics == null || m_Graphics.Length <= 0)
                return;

            foreach (var graphic in m_Graphics)
            {
                if (graphic != null && ((m_IsOn && checkmarkkGraphics.Contains(graphic)) || (!m_IsOn && backgroundGraphics.Contains(graphic))))
                    graphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
            }
        }
    }
}
