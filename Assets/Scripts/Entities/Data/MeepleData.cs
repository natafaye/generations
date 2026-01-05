using System;
using Unity.Properties;

public class MeepleData : EntityData
{
    // Convenience property for getting the correctly typed Type
    [CreateProperty]
    public new MeepleType Type { get { return (MeepleType)base.Type; } }

    #region Changeable Properties (InDistress, Asleep, CurrentJob, Food, Sleep)

    public bool InDistress;
    public bool Asleep;
    public Structure Bed;

    private JobWork _currentJob;
    public JobWork CurrentJob
    {
        get { return _currentJob; }
        set
        {
            if(value == _currentJob) return;
            _currentJob = value;
            OnChange?.Invoke();
        }
    }

    private int _food;
    [CreateProperty]
    public int Food
    {
        get { return _food; }
        set
        {
            if(value == _food) return;
            _food = Math.Min(value, Type.MaxFood);
            // If we hit negatives on food, we're in distress
            InDistress = _food < 0;
            OnChange?.Invoke();
        }
    }

    private int _sleep;
    [CreateProperty]
    public int Sleep
    {
        get { return _sleep; }
        set
        {
            if(value == _sleep) return;
            _sleep = value;
            // Fall asleep immediately when you hit zero
            if (_sleep <= 0) Asleep = true;
            OnChange?.Invoke();
        }
    }

    #endregion

    #region Calculated Properties

    public override string Status
    {
        get
        {
            if(Asleep) return "Asleep";
            if(CurrentJob != null) return CurrentJob.TypeData.Name + "ing " + CurrentJob.Target.Data.Name;
            return "Chilling";
        }
    }

    #endregion

    // Constructor
    public MeepleData(MeepleType type) : base(type)
    {
        Food = type.MaxFood;
        Sleep = 100;
        Asleep = false;
        InDistress = false;
    }
}