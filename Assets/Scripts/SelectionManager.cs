using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SelectionManager: MonoBehaviour
{
    public Entity Selected { get; private set; }

    public InputManager InputManager;
    public Camera MainCamera;
    
    public UIDocument UIDoc;
    public VisualTreeAsset SelectedMeepleTemplate;
    private VisualElement _selectedDisplay;

    void Start()
    {
        InputManager.OnClickInput += OnClick;
        _selectedDisplay = UIDoc.rootVisualElement.Q<VisualElement>("SelectedDisplay");
    }

    private void OnClick(InputValue value)
    {
        // Use ray collision to find what was clicked
        if (value.Get<float>() == 0) return;
        var rayHit = Physics2D.GetRayIntersection(MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;

        // Check if a different selectable has been clicked
        GameObject clickedObject = rayHit.collider.gameObject;
        var newSelected = clickedObject.GetComponent<Entity>();
        if (newSelected == Selected) return;

        _selectedDisplay.Clear();

        if (newSelected == null)
        {
            Selected = null;
            return;
        }

        // Update the selected
        if (Selected != null) Selected.IsSelected = false;
        newSelected.IsSelected = true;
        Selected = newSelected;

        // Update the selection display
        if (Selected is Meeple meeple)
        {
            VisualElement newElement = new();
            SelectedMeepleTemplate.CloneTree(newElement);
            newElement.dataSource = meeple;
            _selectedDisplay.Add(newElement);
        }
        else if (Selected is Structure structure)
        {
            Label label = new();
            label.text = structure.Name;
            if(structure is Plant plant)
            {
                label.text += " " + plant.Age;
            }
            _selectedDisplay.Add(label);
            foreach (JobType job in structure.GetAvailableJobs())
            {
                Button jobButton = new();
                jobButton.text = job.ToString();
                jobButton.clicked += () => JobManager.Instance.AddJob(job, structure);
                _selectedDisplay.Add(jobButton);
            }
        }
        else if (Selected is Item item)
        {
            Label label = new();
            label.text = item.Name;
            _selectedDisplay.Add(label);
        }
    }
}