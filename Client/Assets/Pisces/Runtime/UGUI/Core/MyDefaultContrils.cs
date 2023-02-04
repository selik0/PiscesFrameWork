/****************
 *@class name:		MyDefaultContrils
 *@description:		自定义创建UI的静态类
 *@author:			selik0
 *@date:			2023-01-13 11:24:57
 *@version: 		V1.0.0
*************************************************************************/
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace UnityEngine.UI
{
    /// <summary>
    /// Utility class for creating default implementations of builtin UI controls.
    /// </summary>
    /// <remarks>
    /// The recommended workflow for using UI controls with the UI system is to create a prefab for each type of control and instantiate those when needed. This way changes can be made to the prefabs which immediately have effect on all used instances.
    ///
    /// However, in certain cases there can be reasons to create UI controls entirely from code. The DefaultControls class provide methods to create each of the builtin UI controls. The resulting objects are the same as are obtained from using the corresponding UI menu entries in the GameObject menu in the Editor.
    ///
    /// An example use of this is creating menu items for custom new UI controls that mimics the ones that are builtin in Unity. Some such UI controls may contain other UI controls. For example, a scroll view contains scrollbars.By using the DefaultControls methods to create those parts, it is ensured that they are identical in look and setup to the ones provided in the menu items builtin with Unity.
    ///
    /// Note that the details of the setup of the UI controls created by the methods in this class may change with later revisions of the UI system.As such, they are not guaranteed to be 100% backwards compatible. It is recommended not to rely on the specific hierarchies of the GameObjects created by these methods, and limit your code to only interface with the root GameObject created by each method.
    /// </remarks>
    public static class MyDefaultControls
    {
        static IFactoryControls m_CurrentFactory = DefaultRuntimeFactory.Default;
        public static IFactoryControls factory
        {
            get { return m_CurrentFactory; }
#if UNITY_EDITOR
            set { m_CurrentFactory = value; }
#endif
        }

        /// <summary>
        /// Factory interface to create a GameObject in this class.
        /// It is necessary to use this interface in the whole class so MenuOption and Editor can work using ObjectFactory and default Presets.
        /// </summary>
        /// <remarks>
        /// The only available method is CreateGameObject.
        /// It needs to be called with every Components the created Object will need because of a bug with Undo and RectTransform.
        /// Adding a UI component on the created GameObject may crash if done after Undo.SetTransformParent,
        /// So it's better to prevent such behavior in this class by asking for full creation with all the components.
        /// </remarks>
        public interface IFactoryControls
        {
            GameObject CreateGameObject(string name, params Type[] components);
        }

        private class DefaultRuntimeFactory : IFactoryControls
        {
            public static IFactoryControls Default = new DefaultRuntimeFactory();

            public GameObject CreateGameObject(string name, params Type[] components)
            {
                return new GameObject(name, components);
            }
        }

        /// <summary>
        /// Object used to pass resources to use for the default controls.
        /// </summary>
        public struct Resources
        {
            /// <summary>
            /// The primary sprite to be used for graphical UI elements, used by the button, toggle, and dropdown controls, among others.
            /// </summary>
            public Sprite standard;

            /// <summary>
            /// Sprite used for background elements.
            /// </summary>
            public Sprite background;

            /// <summary>
            /// Sprite used as background for input fields.
            /// </summary>
            public Sprite inputField;

            /// <summary>
            /// Sprite used for knobs that can be dragged, such as on a slider.
            /// </summary>
            public Sprite knob;

            /// <summary>
            /// Sprite used for representation of an "on" state when present, such as a checkmark.
            /// </summary>
            public Sprite checkmark;

            /// <summary>
            /// Sprite used to indicate that a button will open a dropdown when clicked.
            /// </summary>
            public Sprite dropdown;

            /// <summary>
            /// Sprite used for masking purposes, for example to be used for the viewport of a scroll view.
            /// </summary>
            public Sprite mask;
        }

        private const float kWidth = 160f;
        private const float kThickHeight = 30f;
        private const float kThinHeight = 20f;
        private static Vector2 s_ThickElementSize = new Vector2(kWidth, kThickHeight);
        private static Vector2 s_ThinElementSize = new Vector2(kWidth, kThinHeight);
        private static Vector2 s_ImageElementSize = new Vector2(100f, 100f);
        private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
        private static Color s_PanelColor = new Color(1f, 1f, 1f, 0.392f);
        private static Color s_TextColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

        // Helper methods at top

        private static GameObject CreateUIElementRoot(string name, Vector2 size, params Type[] components)
        {
            GameObject child = factory.CreateGameObject(name, components);
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }

        private static GameObject CreateUIObject(string name, GameObject parent, params Type[] components)
        {
            GameObject go = factory.CreateGameObject(name, components);
            SetParentAndAlign(go, parent);
            return go;
        }

        private static void SetDefaultTextValues(Text lbl)
        {
            // Set text values we want across UI elements in default controls.
            // Don't set values which are the same as the default values for the Text component,
            // since there's no point in that, and it's good to keep them as consistent as possible.
            lbl.color = s_TextColor;
            lbl.raycastTarget = false;
        }

        private static void SetDefaultColorTransitionValues(MySelectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }

        private static void SetDefaultColorTransitionValues(Selectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

#if UNITY_EDITOR
            Undo.SetTransformParent(child.transform, parent.transform, "");
#else
            child.transform.SetParent(parent.transform, false);
#endif
            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        /// <summary>
        /// Create the basic UI Panel.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Image
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreatePanel(Resources resources)
        {
            GameObject panelRoot = CreateUIElementRoot("Panel", s_ThickElementSize, typeof(MyImage));

            // Set RectTransform to stretch
            RectTransform rectTransform = panelRoot.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            MyImage image = panelRoot.GetComponent<MyImage>();
            image.sprite = resources.background;
            image.type = MyImage.Type.Sliced;
            image.color = s_PanelColor;

            return panelRoot;
        }

        /// <summary>
        /// Create the basic UI button.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Button
        ///         -Text
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateButton(Resources resources)
        {
            GameObject buttonRoot = CreateUIElementRoot("Button", s_ThickElementSize, typeof(MyImage), typeof(MyButton));

            GameObject childText = CreateUIObject("Text", buttonRoot, typeof(Text));

            MyImage image = buttonRoot.GetComponent<MyImage>();
            image.sprite = resources.standard;
            image.type = MyImage.Type.Sliced;
            image.color = s_DefaultSelectableColor;

            MyButton bt = buttonRoot.GetComponent<MyButton>();
            SetDefaultColorTransitionValues(bt);

            Text text = childText.GetComponent<Text>();
            text.text = "Button";
            text.alignment = TextAnchor.MiddleCenter;
            SetDefaultTextValues(text);

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;

            return buttonRoot;
        }

        /// <summary>
        /// Create the basic UI Text.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Text
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateText(Resources resources)
        {
            GameObject go = CreateUIElementRoot("Text", s_ThickElementSize, typeof(Text));

            Text lbl = go.GetComponent<Text>();
            lbl.text = "New Text";
            SetDefaultTextValues(lbl);

            return go;
        }

        /// <summary>
        /// Create the basic UI Image.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Image
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateImage(Resources resources)
        {
            GameObject go = CreateUIElementRoot("Image", s_ImageElementSize, typeof(MyImage));
            MyImage image = go.GetComponent<MyImage>();
            image.raycastTarget = false;
            return go;
        }

        /// <summary>
        /// Create the basic UI RawImage.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     RawImage
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateRawImage(Resources resources)
        {
            GameObject go = CreateUIElementRoot("RawImage", s_ImageElementSize, typeof(RawImage));
            return go;
        }

        /// <summary>
        /// Create the basic UI Slider.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Slider
        ///         - Background
        ///         - Fill Area
        ///             - Fill
        ///         - Handle Slide Area
        ///             - Handle
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateSlider(Resources resources)
        {
            // Create GOs Hierarchy
            GameObject root = CreateUIElementRoot("Slider", s_ThinElementSize, typeof(MySlider));

            GameObject background = CreateUIObject("Background", root, typeof(MyImage));
            GameObject fillArea = CreateUIObject("Fill Area", root, typeof(RectTransform));
            GameObject fill = CreateUIObject("Fill", fillArea, typeof(MyImage));
            GameObject handleArea = CreateUIObject("Handle Slide Area", root, typeof(RectTransform));
            GameObject handle = CreateUIObject("Handle", handleArea, typeof(MyImage));

            // Background
            MyImage backgroundImage = background.GetComponent<MyImage>();
            backgroundImage.sprite = resources.background;
            backgroundImage.type = MyImage.Type.Sliced;
            backgroundImage.color = s_DefaultSelectableColor;
            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0.25f);
            backgroundRect.anchorMax = new Vector2(1, 0.75f);
            backgroundRect.sizeDelta = new Vector2(0, 0);

            // Fill Area
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.anchoredPosition = new Vector2(-5, 0);
            fillAreaRect.sizeDelta = new Vector2(-20, 0);

            // Fill
            MyImage fillImage = fill.GetComponent<MyImage>();
            fillImage.sprite = resources.standard;
            fillImage.type = MyImage.Type.Sliced;
            fillImage.color = s_DefaultSelectableColor;

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(10, 0);

            // Handle Area
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.sizeDelta = new Vector2(-20, 0);
            handleAreaRect.anchorMin = new Vector2(0, 0);
            handleAreaRect.anchorMax = new Vector2(1, 1);

            // Handle
            MyImage handleImage = handle.GetComponent<MyImage>();
            handleImage.sprite = resources.knob;
            handleImage.color = s_DefaultSelectableColor;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 0);

            // Setup slider component
            MySlider slider = root.GetComponent<MySlider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handleImage;
            slider.direction = MySlider.Direction.LeftToRight;
            SetDefaultColorTransitionValues(slider);

            return root;
        }

        /// <summary>
        /// Create the basic UI Scrollbar.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Scrollbar
        ///         - Sliding Area
        ///             - Handle
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateScrollbar(Resources resources)
        {
            // Create GOs Hierarchy
            GameObject scrollbarRoot = CreateUIElementRoot("Scrollbar", s_ThinElementSize, typeof(MyImage), typeof(Scrollbar));

            GameObject sliderArea = CreateUIObject("Sliding Area", scrollbarRoot, typeof(RectTransform));
            GameObject handle = CreateUIObject("Handle", sliderArea, typeof(MyImage));

            MyImage bgImage = scrollbarRoot.GetComponent<MyImage>();
            bgImage.sprite = resources.background;
            bgImage.type = MyImage.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;

            MyImage handleImage = handle.GetComponent<MyImage>();
            handleImage.sprite = resources.standard;
            handleImage.type = MyImage.Type.Sliced;
            handleImage.color = s_DefaultSelectableColor;

            RectTransform sliderAreaRect = sliderArea.GetComponent<RectTransform>();
            sliderAreaRect.sizeDelta = new Vector2(-20, -20);
            sliderAreaRect.anchorMin = Vector2.zero;
            sliderAreaRect.anchorMax = Vector2.one;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);

            Scrollbar scrollbar = scrollbarRoot.GetComponent<Scrollbar>();
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;
            SetDefaultColorTransitionValues(scrollbar);

            return scrollbarRoot;
        }

        /// <summary>
        /// Create the basic UI Toggle.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Toggle
        ///         - Background
        ///             - Checkmark
        ///         - Label
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateToggle(Resources resources)
        {
            // Set up hierarchy
            GameObject toggleRoot = CreateUIElementRoot("Toggle", s_ThinElementSize, typeof(MyToggle));

            GameObject background = CreateUIObject("Background", toggleRoot, typeof(MyImage));
            GameObject checkmark = CreateUIObject("Checkmark", background, typeof(MyImage));
            GameObject childLabel = CreateUIObject("Label", toggleRoot, typeof(Text));

            // Set up components
            MyToggle toggle = toggleRoot.GetComponent<MyToggle>();
            toggle.isOn = true;

            MyImage bgImage = background.GetComponent<MyImage>();
            bgImage.sprite = resources.standard;
            bgImage.type = MyImage.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;

            MyImage checkmarkImage = checkmark.GetComponent<MyImage>();
            checkmarkImage.sprite = resources.checkmark;
            checkmarkImage.raycastTarget = false;

            Text label = childLabel.GetComponent<Text>();
            label.text = "Toggle";
            SetDefaultTextValues(label);

            // toggle.graphic = checkmarkImage;
            toggle.targetGraphic = bgImage;
            SetDefaultColorTransitionValues(toggle);

            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 1f);
            bgRect.anchorMax = new Vector2(0f, 1f);
            bgRect.anchoredPosition = new Vector2(10f, -10f);
            bgRect.sizeDelta = new Vector2(kThinHeight, kThinHeight);

            RectTransform checkmarkRect = checkmark.GetComponent<RectTransform>();
            checkmarkRect.anchorMin = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchorMax = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchoredPosition = Vector2.zero;
            checkmarkRect.sizeDelta = new Vector2(20f, 20f);

            RectTransform labelRect = childLabel.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.offsetMin = new Vector2(23f, 1f);
            labelRect.offsetMax = new Vector2(-5f, -2f);

            return toggleRoot;
        }

        /// <summary>
        /// Create the basic UI input field.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     InputField
        ///         - PlaceHolder
        ///         - Text
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateInputField(Resources resources)
        {
            GameObject root = CreateUIElementRoot("InputField", s_ThickElementSize, typeof(MyImage), typeof(InputField));

            GameObject childPlaceholder = CreateUIObject("Placeholder", root, typeof(Text));
            GameObject childText = CreateUIObject("Text", root, typeof(Text));

            MyImage image = root.GetComponent<MyImage>();
            image.sprite = resources.inputField;
            image.type = MyImage.Type.Sliced;
            image.color = s_DefaultSelectableColor;

            InputField inputField = root.GetComponent<InputField>();
            SetDefaultColorTransitionValues(inputField);

            Text text = childText.GetComponent<Text>();
            text.text = "";
            text.supportRichText = false;
            SetDefaultTextValues(text);

            Text placeholder = childPlaceholder.GetComponent<Text>();
            placeholder.text = "Enter text...";
            placeholder.fontStyle = FontStyle.Italic;
            // Make placeholder color half as opaque as normal text color.
            Color placeholderColor = text.color;
            placeholderColor.a *= 0.5f;
            placeholder.color = placeholderColor;

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;
            textRectTransform.offsetMin = new Vector2(10, 6);
            textRectTransform.offsetMax = new Vector2(-10, -7);

            RectTransform placeholderRectTransform = childPlaceholder.GetComponent<RectTransform>();
            placeholderRectTransform.anchorMin = Vector2.zero;
            placeholderRectTransform.anchorMax = Vector2.one;
            placeholderRectTransform.sizeDelta = Vector2.zero;
            placeholderRectTransform.offsetMin = new Vector2(10, 6);
            placeholderRectTransform.offsetMax = new Vector2(-10, -7);

            inputField.textComponent = text;
            inputField.placeholder = placeholder;

            return root;
        }

        /// <summary>
        /// Create the basic UI dropdown.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Dropdown
        ///         - Label
        ///         - Arrow
        ///         - Template
        ///             - Viewport
        ///                 - Content
        ///                     - Item
        ///                         - Item Background
        ///                         - Item Checkmark
        ///                         - Item Label
        ///             - Scrollbar
        ///                 - Sliding Area
        ///                     - Handle
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateDropdown(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Dropdown", s_ThickElementSize, typeof(MyImage), typeof(Dropdown));

            GameObject label = CreateUIObject("Label", root, typeof(Text));
            GameObject arrow = CreateUIObject("Arrow", root, typeof(MyImage));
            GameObject template = CreateUIObject("Template", root, typeof(MyImage), typeof(ScrollRect));
            GameObject viewport = CreateUIObject("Viewport", template, typeof(MyImage), typeof(Mask));
            GameObject content = CreateUIObject("Content", viewport, typeof(RectTransform));
            GameObject item = CreateUIObject("Item", content, typeof(MyToggle));
            GameObject itemBackground = CreateUIObject("Item Background", item, typeof(MyImage));
            GameObject itemCheckmark = CreateUIObject("Item Checkmark", item, typeof(MyImage));
            GameObject itemLabel = CreateUIObject("Item Label", item, typeof(Text));

            // Sub controls.

            GameObject scrollbar = CreateScrollbar(resources);
            scrollbar.name = "Scrollbar";
            SetParentAndAlign(scrollbar, template);

            Scrollbar scrollbarScrollbar = scrollbar.GetComponent<Scrollbar>();
            scrollbarScrollbar.SetDirection(Scrollbar.Direction.BottomToTop, true);

            RectTransform vScrollbarRT = scrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            // Setup item UI components.

            Text itemLabelText = itemLabel.GetComponent<Text>();
            SetDefaultTextValues(itemLabelText);
            itemLabelText.alignment = TextAnchor.MiddleLeft;

            MyImage itemBackgroundImage = itemBackground.GetComponent<MyImage>();
            itemBackgroundImage.color = new Color32(245, 245, 245, 255);

            MyImage itemCheckmarkImage = itemCheckmark.GetComponent<MyImage>();
            itemCheckmarkImage.sprite = resources.checkmark;

            MyToggle itemToggle = item.GetComponent<MyToggle>();
            itemToggle.targetGraphic = itemBackgroundImage;
            // itemToggle.graphic = itemCheckmarkImage;
            itemToggle.isOn = true;

            // Setup template UI components.

            MyImage templateImage = template.GetComponent<MyImage>();
            templateImage.sprite = resources.standard;
            templateImage.type = MyImage.Type.Sliced;

            ScrollRect templateScrollRect = template.GetComponent<ScrollRect>();
            templateScrollRect.content = content.GetComponent<RectTransform>();
            templateScrollRect.viewport = viewport.GetComponent<RectTransform>();
            templateScrollRect.horizontal = false;
            templateScrollRect.movementType = ScrollRect.MovementType.Clamped;
            templateScrollRect.verticalScrollbar = scrollbarScrollbar;
            templateScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            templateScrollRect.verticalScrollbarSpacing = -3;

            Mask scrollRectMask = viewport.GetComponent<Mask>();
            scrollRectMask.showMaskGraphic = false;

            MyImage viewportImage = viewport.GetComponent<MyImage>();
            viewportImage.sprite = resources.mask;
            viewportImage.type = MyImage.Type.Sliced;

            // Setup dropdown UI components.

            Text labelText = label.GetComponent<Text>();
            SetDefaultTextValues(labelText);
            labelText.alignment = TextAnchor.MiddleLeft;

            MyImage arrowImage = arrow.GetComponent<MyImage>();
            arrowImage.sprite = resources.dropdown;

            MyImage backgroundImage = root.GetComponent<MyImage>();
            backgroundImage.sprite = resources.standard;
            backgroundImage.color = s_DefaultSelectableColor;
            backgroundImage.type = MyImage.Type.Sliced;

            Dropdown dropdown = root.GetComponent<Dropdown>();
            dropdown.targetGraphic = backgroundImage;
            SetDefaultColorTransitionValues(dropdown);
            dropdown.template = template.GetComponent<RectTransform>();
            dropdown.captionText = labelText;
            dropdown.itemText = itemLabelText;

            // Setting default Item list.
            itemLabelText.text = "Option A";
            dropdown.options.Add(new Dropdown.OptionData { text = "Option A" });
            dropdown.options.Add(new Dropdown.OptionData { text = "Option B" });
            dropdown.options.Add(new Dropdown.OptionData { text = "Option C" });
            dropdown.RefreshShownValue();

            // Set up RectTransforms.

            RectTransform labelRT = label.GetComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero;
            labelRT.anchorMax = Vector2.one;
            labelRT.offsetMin = new Vector2(10, 6);
            labelRT.offsetMax = new Vector2(-25, -7);

            RectTransform arrowRT = arrow.GetComponent<RectTransform>();
            arrowRT.anchorMin = new Vector2(1, 0.5f);
            arrowRT.anchorMax = new Vector2(1, 0.5f);
            arrowRT.sizeDelta = new Vector2(20, 20);
            arrowRT.anchoredPosition = new Vector2(-15, 0);

            RectTransform templateRT = template.GetComponent<RectTransform>();
            templateRT.anchorMin = new Vector2(0, 0);
            templateRT.anchorMax = new Vector2(1, 0);
            templateRT.pivot = new Vector2(0.5f, 1);
            templateRT.anchoredPosition = new Vector2(0, 2);
            templateRT.sizeDelta = new Vector2(0, 150);

            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin = new Vector2(0, 0);
            viewportRT.anchorMax = new Vector2(1, 1);
            viewportRT.sizeDelta = new Vector2(-18, 0);
            viewportRT.pivot = new Vector2(0, 1);

            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0f, 1);
            contentRT.anchorMax = new Vector2(1f, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.anchoredPosition = new Vector2(0, 0);
            contentRT.sizeDelta = new Vector2(0, 28);

            RectTransform itemRT = item.GetComponent<RectTransform>();
            itemRT.anchorMin = new Vector2(0, 0.5f);
            itemRT.anchorMax = new Vector2(1, 0.5f);
            itemRT.sizeDelta = new Vector2(0, 20);

            RectTransform itemBackgroundRT = itemBackground.GetComponent<RectTransform>();
            itemBackgroundRT.anchorMin = Vector2.zero;
            itemBackgroundRT.anchorMax = Vector2.one;
            itemBackgroundRT.sizeDelta = Vector2.zero;

            RectTransform itemCheckmarkRT = itemCheckmark.GetComponent<RectTransform>();
            itemCheckmarkRT.anchorMin = new Vector2(0, 0.5f);
            itemCheckmarkRT.anchorMax = new Vector2(0, 0.5f);
            itemCheckmarkRT.sizeDelta = new Vector2(20, 20);
            itemCheckmarkRT.anchoredPosition = new Vector2(10, 0);

            RectTransform itemLabelRT = itemLabel.GetComponent<RectTransform>();
            itemLabelRT.anchorMin = Vector2.zero;
            itemLabelRT.anchorMax = Vector2.one;
            itemLabelRT.offsetMin = new Vector2(20, 1);
            itemLabelRT.offsetMax = new Vector2(-10, -2);

            template.SetActive(false);

            return root;
        }

        /// <summary>
        /// Create the basic UI Scrollview.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Scrollview
        ///         - Viewport
        ///             - Content
        ///         - Scrollbar Horizontal
        ///             - Sliding Area
        ///                 - Handle
        ///         - Scrollbar Vertical
        ///             - Sliding Area
        ///                 - Handle
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateScrollView(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Scroll View", new Vector2(200, 200), typeof(MyImage), typeof(ScrollRect));

            GameObject viewport = CreateUIObject("Viewport", root, typeof(MyImage), typeof(Mask));
            GameObject content = CreateUIObject("Content", viewport, typeof(RectTransform));

            // Sub controls.

            GameObject hScrollbar = CreateScrollbar(resources);
            hScrollbar.name = "Scrollbar Horizontal";
            SetParentAndAlign(hScrollbar, root);
            RectTransform hScrollbarRT = hScrollbar.GetComponent<RectTransform>();
            hScrollbarRT.anchorMin = Vector2.zero;
            hScrollbarRT.anchorMax = Vector2.right;
            hScrollbarRT.pivot = Vector2.zero;
            hScrollbarRT.sizeDelta = new Vector2(0, hScrollbarRT.sizeDelta.y);

            GameObject vScrollbar = CreateScrollbar(resources);
            vScrollbar.name = "Scrollbar Vertical";
            SetParentAndAlign(vScrollbar, root);
            vScrollbar.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, true);
            RectTransform vScrollbarRT = vScrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            // Setup RectTransforms.

            // Make viewport fill entire scroll view.
            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin = Vector2.zero;
            viewportRT.anchorMax = Vector2.one;
            viewportRT.sizeDelta = Vector2.zero;
            viewportRT.pivot = Vector2.up;

            // Make context match viewpoprt width and be somewhat taller.
            // This will show the vertical scrollbar and not the horizontal one.
            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = Vector2.up;
            contentRT.anchorMax = Vector2.one;
            contentRT.sizeDelta = new Vector2(0, 300);
            contentRT.pivot = Vector2.up;

            // Setup UI components.

            ScrollRect scrollRect = root.GetComponent<ScrollRect>();
            scrollRect.content = contentRT;
            scrollRect.viewport = viewportRT;
            scrollRect.horizontalScrollbar = hScrollbar.GetComponent<Scrollbar>();
            scrollRect.verticalScrollbar = vScrollbar.GetComponent<Scrollbar>();
            scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.horizontalScrollbarSpacing = -3;
            scrollRect.verticalScrollbarSpacing = -3;

            MyImage rootImage = root.GetComponent<MyImage>();
            rootImage.sprite = resources.background;
            rootImage.type = MyImage.Type.Sliced;
            rootImage.color = s_PanelColor;

            Mask viewportMask = viewport.GetComponent<Mask>();
            viewportMask.showMaskGraphic = false;

            MyImage viewportImage = viewport.GetComponent<MyImage>();
            viewportImage.sprite = resources.mask;
            viewportImage.type = MyImage.Type.Sliced;

            return root;
        }
    }
}
