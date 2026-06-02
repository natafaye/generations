using System;
using UnityEngine.UIElements;

namespace Generations
{
    /// <summary>
    /// A "radio" style button that allows the user to select one of a group of buttons
    /// The selected button will have a "selected-button" class applied to it
    /// </summary>
    /// <typeparam name="T">The type of the value of the buttons</typeparam>
    public class UIRadioButton<T> : UIView
    {
        bool _isSelected;
        readonly T _value;
        readonly Action<T> _onSelect;
        
        Button _button;

        /// <summary>
        /// Creates a new UIRadioButton
        /// </summary>
        /// <param name="rootElement">The UI element with one button inside</param>
        /// <param name="value">The value for this button (radio button style)</param>
        /// <param name="onSelect">The event that will be invoked when this button is selected or unselected</param>
        /// <param name="startSelected">If this button should be selected right now</param>
        public UIRadioButton(VisualElement rootElement, T value, Action<T> onSelect, bool startSelected = false) : base(rootElement)
        {
            _value = value;
            _isSelected = startSelected;
            _onSelect = onSelect;

            Root.dataSource = _value;
            UpdateView();
        }

        protected override void SetVisualElements()
        {
            _button = Root.Q<Button>();
        }

        protected override void RegisterButtonCallbacks()
        {
            _button.clicked += OnButtonClick;
        }

        private void OnButtonClick()
        {
            // Notify the parent that this button was selected
            // The parent should call OnSelectedChanged on all the radio buttons
            if (!_isSelected) _onSelect?.Invoke(_value);
            else _onSelect?.Invoke(default(T));
        }

        /// <summary>
        /// Called by parent to set one button as selected and the rest as deselected
        /// </summary>
        /// <param name="selectedValue">The new selected value amongst the buttons</param>
        public void OnSelectedChanged(T selectedValue)
        {
            _isSelected = _value.Equals(selectedValue);
            UpdateView();
        }

        protected void UpdateView()
        {
            if (_isSelected) _button.AddToClassList("selected-button");
            else _button.RemoveFromClassList("selected-button");
        }
    }
}