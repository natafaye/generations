using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Generations
{
    class BuildMenuView : UIView
    {
        bool _expanded = false;
        readonly UIRadioButtonList<StructureType> _typeList;

        Button _toggleButton;
        VisualElement _typeMenu;
        readonly VisualTreeAsset _typeButtonTemplate;

        public BuildMenuView(VisualElement rootElement) : base(rootElement)
        {
            BuildEvents.RecipesUpdated += OnRecipesUpdated;
            InputEvents.KeyDown += OnKeyDown;

            _typeButtonTemplate = Resources.Load<VisualTreeAsset>("RecipeButton");
            _typeList = new UIRadioButtonList<StructureType>(_typeMenu, _typeButtonTemplate, OnTypeSelected);
        }

        protected override void SetVisualElements()
        {
            _toggleButton = Root.Q<Button>("build-button");
            _typeMenu = Root.Q<VisualElement>("types-menu");
        }

        protected override void RegisterButtonCallbacks()
        {
            _toggleButton.clicked += ToggleMenu;
        }

        private void OnRecipesUpdated(StructureType[] recipes)
        {
            _typeList.SetData(recipes);
        }

        private void OnTypeSelected(StructureType type)
        {
            BuildEvents.SelectedTypeChanged?.Invoke(type);
        }

        private void OnKeyDown(Key key)
        {
            if (key == Key.Escape)
            {
                _expanded = false;
                _typeList.SetSelection(null);
                UpdateView();
            }
        }

        private void ToggleMenu()
        {
            _expanded = !_expanded;
            if(!_expanded) _typeList.SetSelection(null);
            UpdateView();
        }

        private void UpdateView()
        {
            _typeMenu.style.display = _expanded ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}