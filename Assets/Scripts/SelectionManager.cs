using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SelectionManager: MonoBehaviour
{
    public Entity Selected { get; private set; }

    public InputManager InputManager;
    public Camera MainCamera;
    
    public UIDocument UIDoc;
    private VisualElement _selectedDisplay;
    public VisualTreeAsset SelectedTemplate;
    public VisualTreeAsset MeepleTemplate;
    public VisualTreeAsset StructureTemplate;
    public VisualTreeAsset PlantTemplate;
    public VisualTreeAsset ItemTemplate;
    public VisualTreeAsset JobButtonTemplate;

    void Start()
    {
        InputManager.OnClickInput += OnClick;
        _selectedDisplay = UIDoc.rootVisualElement.Q<VisualElement>("SelectedDisplay");
    }

    private void OnClick(InputValue value)
    {
        // Use ray collision to find out what entity was clicked
        if (value.Get<float>() == 0) return;
        var rayHit = Physics2D.GetRayIntersection(MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;
        GameObject clickedObject = rayHit.collider.gameObject;
        Entity newSelected = clickedObject.GetComponent<Entity>();

        // If it's the same entity as before, ignore it
        if (newSelected == Selected) return;

        // Update what's selected
        if (Selected != null) Selected.Data.IsSelected = false;
        if (newSelected != null) newSelected.Data.IsSelected = true;
        Selected = newSelected;

        // Keep the selected display up to date, even with info that isn't data bound
        UpdateSelectedDisplay();
        Selected.Data.OnChange += UpdateSelectedDisplay;
    }

    private void UpdateSelectedDisplay()
    {
        _selectedDisplay.Clear();

        // If there's nothing selected, nothing to display
        if(Selected == null) return;

        // Set up the general layout and entity data
        VisualElement bottomUI = new();
        SelectedTemplate.CloneTree(bottomUI);
        bottomUI.dataSource = Selected.Data;

        // Fill in the type-specific properties
        VisualElement properties = bottomUI.Q<VisualElement>("properties");
        if (Selected is Meeple)
            MeepleTemplate.CloneTree(properties);
        else if (Selected is Plant)
            PlantTemplate.CloneTree(properties);
        else if (Selected is Structure)
            StructureTemplate.CloneTree(properties);
        else if (Selected is Item)
            StructureTemplate.CloneTree(properties);

        // Add it to the UI
        _selectedDisplay.Add(bottomUI);

        // Show buttons for all available jobs
        UpdateButtonsDisplay();
    }

    private void UpdateButtonsDisplay()
    {
        if(Selected == null) return;

        VisualElement buttonContainer = _selectedDisplay.Q<VisualElement>("job-buttons");
        buttonContainer.Clear();

        // Add a button for each available job
        Debug.Log(Selected.Data.AvailableJobs);
        foreach(JobTypeData jobType in Selected.Data.AvailableJobs)
        {
            VisualElement buttonBox = new();
            JobButtonTemplate.CloneTree(buttonBox);
            buttonBox.dataSource = jobType;
            Button button = buttonBox.Q<Button>("job-button");

            // If this job is already queued, the button unqueues it
            if(Selected.Data.QueuedJob?.TypeData == jobType) {
                button.AddToClassList("selected-button");
                button.clicked += () => JobManager.Instance.RemoveJob(Selected.Data.QueuedJob);
            // If this job isn't already queued, the button queues it
            } else {
                button.clicked += () => JobManager.Instance.AddJob(jobType, Selected);
            }
            buttonContainer.Add(buttonBox);
        }
    }
}