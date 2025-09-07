public enum StructureType
{
    Plant,
    Wall,
    Bed
}

public class Structure : Selectable
{
    public StructureType Type;

    private int _maxHealth = 10;
    private int _health;
    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;
        }
    }

    void Start()
    {
        Health = _maxHealth;
    }
}