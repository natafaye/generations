using UnityEngine;

public class MeepleData : EntityData
{
    // Convenience property for getting the correctly typed Type
    public new MeepleType Type { get { return (MeepleType)base.Type; } }

    #region Changeable Properties (InDistress, Asleep, CurrentJob, Food, Sleep)

    public bool InDistress;
    public bool Asleep;

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
    public int Food
    {
        get { return _food; }
        set
        {
            if(value == _food) return;
            _food = value;
            InDistress = _food < 0;
            OnChange?.Invoke();
        }
    }

    private int _sleep;
    public int Sleep
    {
        get { return _sleep; }
        set
        {
            if(value == _sleep) return;
            _sleep = value;
            // Wake up when you're filled up on sleep
            if (Asleep && _sleep >= 20) Asleep = false;
            // Fall asleep when you hit zero
            if (_sleep <= 0) Asleep = true;
            OnChange?.Invoke();
        }
    }

    #endregion

    // Constructor
    public MeepleData(EntityType type) : base(type)
    {
        Food = 100;
        Sleep = 100;
        Asleep = false;
        InDistress = false;
    }
}