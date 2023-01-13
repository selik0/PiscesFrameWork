/****************
 *@class name:		Slider2D
 *@description:		坐标系的slider
 *@author:			selik0
 *@date:			2022-07-04 08:25:31
 *@version: 		V1.0.0
*************************************************************************/
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pisces
{
    public class Slider2D : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        [Serializable]
        public class SliderEvent : UnityEvent<Vector2, Vector2> { }

        [SerializeField]
        private RectTransform m_HandleRect;

        public RectTransform handleRect { get { return m_HandleRect; } set { if (SetPropertyUtility.SetClass(ref m_HandleRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [Space]

        [SerializeField]
        private Vector2 m_MinValue = Vector2.zero;

        public Vector2 minValue { get { return m_MinValue; } set { if (SetPropertyUtility.SetStruct(ref m_MinValue, value)) { Set(m_Value); UpdateVisuals(); } } }

        [SerializeField]
        private Vector2 m_MaxValue = Vector2.one;

        public Vector2 maxValue { get { return m_MaxValue; } set { if (SetPropertyUtility.SetStruct(ref m_MaxValue, value)) { Set(m_Value); UpdateVisuals(); } } }

        [SerializeField]
        private bool m_WholeNumbers = false;

        public bool wholeNumbers { get { return m_WholeNumbers; } set { if (SetPropertyUtility.SetStruct(ref m_WholeNumbers, value)) { Set(m_Value); UpdateVisuals(); } } }

        [SerializeField]
        protected Vector2 m_Value;

        public virtual Vector2 value
        {
            get
            {
                if (wholeNumbers)
                {
                    m_Value.x = Mathf.Round(m_Value.x);
                    m_Value.y = Mathf.Round(m_Value.y);
                }
                return m_Value;
            }
            set
            {
                Set(value);
            }
        }

        public virtual void SetValueWithoutNotify(Vector2 input)
        {
            Set(input, false);
        }

        public Vector2 normalizedValue
        {
            get
            {
                if (Mathf.Approximately(minValue.x, maxValue.x) || Mathf.Approximately(minValue.y, maxValue.y))
                    return Vector2.zero;
                return InverseLerp(minValue, maxValue, value);
            }
            set
            {
                this.value = Lerp(minValue, maxValue, value);
            }
        }

        [Space]

        [SerializeField]
        private SliderEvent m_OnValueChanged = new SliderEvent();

        public SliderEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }

        [SerializeField]
        private SliderEvent m_OnPointerDown = new SliderEvent();
        public SliderEvent onPointerDown { get { return m_OnPointerDown; } set { m_OnPointerDown = value; } }

        [SerializeField]
        private SliderEvent m_OnPointerUp = new SliderEvent();
        public SliderEvent onPointerUp { get { return m_OnPointerUp; } set { m_OnPointerUp = value; } }


        // Private fields
        private Transform m_HandleTransform;
        private RectTransform m_HandleContainerRect;

        // The offset from handle position to mouse down position
        private Vector2 m_Offset = Vector2.zero;

        private DrivenRectTransformTracker m_Tracker;

        // This "delayed" mechanism is required for case 1037681.
        private bool m_DelayedUpdateVisuals = false;

        // Size of each step.
        Vector2 stepSize { get { return wholeNumbers ? Vector2.one : (maxValue - minValue) * 0.1f; } }

        protected Slider2D() { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (wholeNumbers)
            {
                m_MinValue.x = Mathf.Round(m_MinValue.x);
                m_MinValue.y = Mathf.Round(m_MinValue.y);
                m_MaxValue.x = Mathf.Round(m_MaxValue.x);
                m_MaxValue.y = Mathf.Round(m_MaxValue.y);
            }

            //Onvalidate is called before OnEnabled. We need to make sure not to touch any other objects before OnEnable is run.
            if (IsActive())
            {
                UpdateCachedReferences();
                // Update rects in next update since other things might affect them even if value didn't change.
                m_DelayedUpdateVisuals = true;
            }

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(value, normalizedValue);
#endif
        }

        public virtual void LayoutComplete()
        { }

        public virtual void GraphicUpdateComplete()
        { }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCachedReferences();
            Set(m_Value, false);
            // Update rects since they need to be initialized correctly.
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        protected virtual void Update()
        {
            if (m_DelayedUpdateVisuals)
            {
                m_DelayedUpdateVisuals = false;
                Set(m_Value, false);
                UpdateVisuals();
            }
        }

        Vector2 InverseLerp(Vector2 a, Vector2 b, Vector2 value)
        {
            Vector2 reVec = Vector2.zero;
            reVec.x = Mathf.InverseLerp(a.x, b.x, value.x);
            reVec.y = Mathf.InverseLerp(a.y, b.y, value.y);
            return reVec;
        }

        Vector2 Lerp(Vector2 a, Vector2 b, Vector2 value)
        {
            Vector2 reVec = Vector2.zero;
            reVec.x = Mathf.Lerp(a.x, b.x, value.x);
            reVec.y = Mathf.Lerp(a.y, b.y, value.y);
            return reVec;
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Has value changed? Various elements of the slider have the old normalisedValue assigned, we can use this to perform a comparison.
            // We also need to ensure the value stays within min/max.
            m_Value = ClampValue(m_Value);
            Vector2 oldNormalizedValue = normalizedValue;
            if (m_HandleContainerRect != null)
                oldNormalizedValue = (m_HandleRect.anchorMin);

            UpdateVisuals();

            if (oldNormalizedValue != normalizedValue)
            {
                UISystemProfilerApi.AddMarker("Slider.value", this);
                onValueChanged.Invoke(m_Value, normalizedValue);
            }
        }

        void UpdateCachedReferences()
        {
            if (m_HandleRect && m_HandleRect != (RectTransform)transform)
            {
                m_HandleTransform = m_HandleRect.transform;
                if (m_HandleTransform.parent != null)
                    m_HandleContainerRect = m_HandleTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_HandleRect = null;
                m_HandleContainerRect = null;
            }
        }

        Vector2 ClampValue(Vector2 input)
        {
            Vector2 newValue = Vector2.zero;
            newValue.x = Mathf.Clamp(input.x, minValue.x, maxValue.x);
            newValue.y = Mathf.Clamp(input.y, minValue.y, maxValue.y);
            if (wholeNumbers)
            {
                newValue.x = Mathf.Round(newValue.x);
                newValue.y = Mathf.Round(newValue.y);
            }
            return newValue;
        }

        protected virtual void Set(Vector2 input, bool sendCallback = true)
        {
            // Clamp the input
            Vector2 newValue = ClampValue(input);

            // If the stepped value doesn't match the last one, it's time to update
            if (m_Value == newValue)
                return;

            m_Value = newValue;
            UpdateVisuals();
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Slider.value", this);
                m_OnValueChanged.Invoke(newValue, normalizedValue);
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
            if (!IsActive())
                return;

            UpdateVisuals();
        }

        enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }

        // Force-update the slider. Useful if you've changed the properties and want it to update visually.
        private void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif

            m_Tracker.Clear();

            if (m_HandleContainerRect != null)
            {
                m_Tracker.Add(this, m_HandleRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;
                anchorMin = anchorMax = (normalizedValue);
                m_HandleRect.anchorMin = anchorMin;
                m_HandleRect.anchorMax = anchorMax;
            }
        }

        // Update the slider's position based on the mouse.
        void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = m_HandleContainerRect;
            if (clickRect != null && clickRect.rect.size.x > 0 && clickRect.rect.size.y > 0)
            {
                Vector2 position = Vector2.zero;
                if (!MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
                    return;

                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, position, cam, out localCursor))
                    return;
                localCursor -= clickRect.rect.position;

                Vector2 val = Vector2.zero;
                Vector2 temp = (localCursor - m_Offset);
                val.x = Mathf.Clamp01(temp.x / clickRect.rect.size.x);
                val.y = Mathf.Clamp01(temp.y / clickRect.rect.size.y);
                normalizedValue = (val);
            }
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);
            onPointerDown.Invoke(value, normalizedValue);

            m_Offset = Vector2.zero;
            if (m_HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
            {
                Vector2 localMousePos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out localMousePos))
                    m_Offset = localMousePos;
            }
            else
            {
                // Outside the slider handle - jump to this point instead
                UpdateDrag(eventData, eventData.pressEventCamera);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;
            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerUp(eventData);
            onPointerUp.Invoke(value, normalizedValue);
        }

        public override void OnMove(AxisEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }
            float angle;
            if (eventData.moveVector.x >= 0)
                angle = Vector2.Angle(eventData.moveVector, Vector2.right);
            else
                angle = Vector2.Angle(eventData.moveVector, Vector2.left);
            Vector2 scale = Vector2.zero;
            scale.x = Mathf.Cos(angle * Mathf.Deg2Rad);
            scale.y = Mathf.Sin(angle * Mathf.Deg2Rad);
            Set(value - stepSize * scale);
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }
    }
}
