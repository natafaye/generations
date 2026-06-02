using UnityEngine;
using UnityEngine.UIElements;

namespace Generations
{
    public class SelectedEntityView : UIView
    {
        EntityData _selectedEntity;

        VisualElement _propertiesContainer;
        VisualElement _buttonsContainer;

        readonly VisualTreeAsset _meepleProperties;
        readonly VisualTreeAsset _plantProperties;
        readonly VisualTreeAsset _structureProperties;
        readonly VisualTreeAsset _jobButtonTemplate;

        UIRadioButtonList<JobTypeData> _jobButtonList;

        public SelectedEntityView(VisualElement rootElement): base(rootElement)
        {
            EntityEvents.EntitySelected += SetEntityDataSource;
            
            _meepleProperties = Resources.Load<VisualTreeAsset>("MeepleProperties");
            _plantProperties = Resources.Load<VisualTreeAsset>("PlantProperties");
            _structureProperties = Resources.Load<VisualTreeAsset>("StructureProperties");
            _jobButtonTemplate = Resources.Load<VisualTreeAsset>("JobButton");

            _jobButtonList = new UIRadioButtonList<JobTypeData>(_buttonsContainer, _jobButtonTemplate, OnJobSelected);
            
            Hide();
        }

        protected override void SetVisualElements()
        {
            _propertiesContainer = Root.Q<VisualElement>("selected-entity__type-specific-properties");
            _buttonsContainer = Root.Q<VisualElement>("selected-entity__job-buttons");
        }

        void SetEntityDataSource(EntityData entity)
        {
            if (entity == null)
            {
                _selectedEntity.DataChanged -= OnDataChanged;
                Hide();
            }
            else
            {
                Root.dataSource = entity;
                entity.DataChanged += OnDataChanged;
                UpdateView(entity);
                Show();
            }
            _selectedEntity = entity;
        }

        void OnDataChanged()
        {
            UpdateView(_selectedEntity);
        }

        void OnJobSelected(JobTypeData jobType)
        {
            if(jobType == null) JobManager.Instance.RemoveJob(_selectedEntity.QueuedJob);
            else JobManager.Instance.AddJob(jobType, _selectedEntity);
        }

        void UpdateView(EntityData entity)
        {
            if(entity == null) return;

            // Show type-specific properties
            _propertiesContainer.Clear();
            TemplateContainer body;
            if (entity is MeepleData)
                body = _meepleProperties.Instantiate();
            else if (entity is PlantData)
                body = _plantProperties.Instantiate();
            else
                body = _structureProperties.Instantiate();
            _propertiesContainer.Add(body);

            // Show job buttons
            _jobButtonList.SetData(entity.AvailableJobs, entity.QueuedJob?.TypeData);
        }
    }
}