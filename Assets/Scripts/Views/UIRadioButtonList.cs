using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Generations
{
    /// <summary>
    /// A list of buttons
    /// </summary>
    /// <typeparam name="T">The type of the data for the buttons</typeparam>
    public class UIRadioButtonList<T> : UIView
    {
        readonly List<UIRadioButton<T>> _buttons = new();
        readonly Action<T> _onSelect;
        
        readonly VisualTreeAsset _buttonTemplate;

        /// <summary>
        /// Initializes a Radio Button List
        /// </summary>
        /// <param name="rootElement">The UI element to put the buttons in</param>
        /// <param name="buttonTemplate">The template for each button</param>
        /// <param name="onSelect">The event to invoke when a button is selected</param>
        public UIRadioButtonList(VisualElement rootElement, VisualTreeAsset buttonTemplate, Action<T> onSelect) : base(rootElement)
        {
            _buttonTemplate = buttonTemplate;
            _onSelect = onSelect;
        }

        /// <summary>
        /// Recreates all the buttons for the provided data
        /// </summary>
        /// <param name="data">The data to make buttons for (each button will get their data as a dataSource)</param>
        /// <param name="initialValue">Optional initally selected value</param>
        public void SetData(T[] data, T initialValue = default)
        {
            // Clear out old buttons
            _buttons.ForEach(button => button.Dispose());
            _buttons.Clear();
            Root.Clear();

            // Add in new buttons
            foreach(T value in data)
            {
                TemplateContainer buttonElement = _buttonTemplate.Instantiate();
                var radioButton = new UIRadioButton<T>(buttonElement, value, SetSelection, value.Equals(initialValue));
                _buttons.Add(radioButton);
                Root.Add(buttonElement);
            }
        }

        /// <summary>
        /// Sets the selection of a particular button and notifies
        /// </summary>
        /// <param name="value">The new selected value</param>
        public void SetSelection(T value)
        {
            // Update all the buttons to be selected or unselected
            _buttons.ForEach(button => button.OnSelectedChanged(value));
            // Notify that a new value was selected
            _onSelect?.Invoke(value);
        }
    }
}