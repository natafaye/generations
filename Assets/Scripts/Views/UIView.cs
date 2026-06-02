using UnityEngine.UIElements;
using System;

namespace Generations
{
    /// <summary>
    /// This is a base class for a functional unit of the UI. This can make up a full-screen interface or just
    /// part of one.
    /// </summary>
    public class UIView : IDisposable
    {
        protected bool m_HideOnAwake = false;

        // UI reveals other underlaying UIs, partially see-through
        protected bool m_IsOverlay;

        protected VisualElement m_RootElement;

        // Properties
        public VisualElement Root => m_RootElement;
        public bool IsTransparent => m_IsOverlay;
        public bool IsHidden => m_RootElement.style.display == DisplayStyle.None;

        // Constructor
        /// <summary>
        /// Initializes a new instance of the UIView class.
        /// </summary>
        /// <param name="rootElement">The topmost VisualElement in the UXML hierarchy.</param>
        public UIView(VisualElement rootElement)
        {
            m_RootElement = rootElement ?? throw new ArgumentNullException(nameof(rootElement));
            Initialize();
        }

        public virtual void Initialize()
        {
            if (m_HideOnAwake) Hide();
            SetVisualElements();
            RegisterButtonCallbacks();
        }

        // Sets up the VisualElements for the UI. Override to customize.
        protected virtual void SetVisualElements()
        {
  
        }

        // Registers callbacks for buttons in the UI. Override to customize.
        protected virtual void RegisterButtonCallbacks()
        {

        }

        // Displays the UI.
        public virtual void Show()
        {
            m_RootElement.style.display = DisplayStyle.Flex;
        }

        // Hides the UI.
        public virtual void Hide()
        {
            m_RootElement.style.display = DisplayStyle.None;
        }

        // Unregisters any callbacks or event handlers. Override to customize.
        public virtual void Dispose()
        {

        }
    }
}

