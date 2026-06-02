using UnityEngine;
using UnityEngine.UIElements;

namespace Generations
{
    [RequireComponent(typeof(UIDocument))]
    class UIManager : MonoBehaviour
    {
        UIDocument _uiDoc;

        VisualElement _selectedEntityContainer;
        SelectedEntityView _selectedEntityView;

        VisualElement _buildMenuContainer;
        BuildMenuView _buildMenuView;

        void OnEnable()
        {
            _uiDoc = GetComponent<UIDocument>();

            _selectedEntityContainer = _uiDoc.rootVisualElement.Q<VisualElement>("selected-entity");
            _selectedEntityView = new SelectedEntityView(_selectedEntityContainer);

            _buildMenuContainer = _uiDoc.rootVisualElement.Q<VisualElement>("build-menu");
            _buildMenuView = new BuildMenuView(_buildMenuContainer);
        }
    }
}