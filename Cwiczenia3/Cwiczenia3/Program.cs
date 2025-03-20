public enum ContainerType
{
    Liquid,
    Gas,
    Refrigrated
}
public class Container
{
    public double Mass { get; set; }
    public double Hight { get; set; }
    public double NetMass { get; set; }
    public double Depth { get; set; }
    public string SerialNum { get;  set; }
    public double MaxCapacity { get; set; }
    
    public ContainerType Type { get;  set; }

    public Container(double mass, double hight, double netMass, double depth)
    {
        Mass = mass;
        Hight = hight;
        NetMass = netMass;
        Depth = depth;
        MaxCapacity = 0;
        GenerateSerialNumber();
    }

    public void GenerateSerialNumber()
    {
        char TypeChar = GetType();
        Random rnd = new Random();
        SerialNum = rnd.Next(1, 9999).ToString();
        SerialNum = $"KON--{GetType()}--{SerialNum}";
    }

    public virtual char GetType()
    {
        return 'C';
    }

    public virtual void LoadCargo(double cargoMass)
    {
        if (cargoMass > MaxCapacity)
        {
            throw new OverfillException($"Cannot load {cargoMass}kg. Maximum capacity is {MaxCapacity}kg.");
        }
        Mass += cargoMass;
        Console.WriteLine($"Loaded {cargoMass}kg to container {SerialNum}. Current mass: {Mass}kg");
    }

    public virtual void UnloadCargo()
    {
        Mass = NetMass;
        Console.WriteLine($"Container {SerialNum} unloaded. Current mass: {Mass}kg");
    }

    public override string ToString()
    {
        return $"Container {SerialNum} - Type: {Type}, Mass: {Mass}kg, Capacity: {MaxCapacity}kg";
    }
}
public class OverfillException : Exception
{
    public OverfillException(string message) : base(message) { }
}
public interface IHazadNotifer
{
    void Notify(string serialNum);
}
public class LiquidContainer : Container, IHazadNotifer
{
public bool IsHazardous { get; set; }


public LiquidContainer(double mass, double hight, double netMass, double depth, bool isHazardous) : base(mass, hight, netMass, depth)
{
    Type = ContainerType.Liquid;
    IsHazardous = isHazardous;
    MaxCapacity = isHazardous ? 0.5 * depth * hight : 0.9 * depth * hight;
}

public override char GetType()
{
    return 'L';
}
public override void LoadCargo(double cargoMass)
{
    if (cargoMass > MaxCapacity)
    {
        Notify(SerialNum);
        throw new OverfillException($"Cannot load {cargoMass}kg. Maximum capacity for hazardous liquid is {MaxCapacity}kg.");
    }
            
    base.LoadCargo(cargoMass);
}

public void Notify (string serialNumber)
{
    Console.WriteLine($"HAZARD ALERT: Dangerous operation detected on liquid container {serialNumber}!");
}
}

public class GasContainer : Container, IHazadNotifer
{
    public double Preasure { get; set; }

    public GasContainer(double mass, double hight, double netMass, double depth, double preasure) : 
        base(mass, hight, netMass, depth)
    {
        Type = ContainerType.Gas;
        Preasure = 1.0;
        MaxCapacity = depth * hight * 0.05;
    }

    public override char GetType()
    {
        return 'G';
    }

    public override void LoadCargo(double cargoMass)
    {
        if (cargoMass > MaxCapacity)
        {
            Notify(SerialNum);
            throw new OverfillException($"Cannot load {cargoMass}kg. Maximum capacity for gas is {MaxCapacity}kg.");
        }

        Preasure += (cargoMass / MaxCapacity) * 2.0;
        base.LoadCargo(cargoMass);
    }

    public void Notify(string serialNum)
    {
        Console.WriteLine($"HAZARD ALERT: Dangerous pressure levels in gas container {serialNum}!");
    }
}

public class RefrigratedContainer : Container
{
    public double Temperature { get; set; }
    public string ProductType { get; set; }
    
    private Dictionary<string, double> ProductTemp = new Dictionary<string, double>()
    {
        { "Bananas", 13.3 },
        { "Chocolate", 18 },
        { "Fish", -15 },
        { "Meat", -15 },
        { "Ice cream", -18 },
        { "Frozen pizza", -30 },
        { "Cheese", 7.2 },
        { "Sausages", 5 },
        { "Butter", 20.5 },
        { "Eggs", 19 }
    };

    public RefrigratedContainer(double mass, double hight, double netMass, double depth, double temperature, string ProductType) :
        base(mass, hight, netMass, depth)
    {
        Type = ContainerType.Refrigrated;
        MaxCapacity = depth * hight * 0.8;
        if (!ProductTemp.ContainsKey(ProductType))
        {
            throw new ArgumentException($"Invalid product type: {ProductType}");
        }
        ProductType = ProductType;
        Temperature = ProductTemp[ProductType];
    }

    public override char GetType()
    {
        return 'C';
    }

    public override void LoadCargo(double cargoMass)
    {
        if (cargoMass > MaxCapacity)
        {
            throw new OverfillException($"Cannot load {cargoMass}kg. Maximum capacity for refrigerated container is {MaxCapacity}kg.");
        }
        base.LoadCargo(cargoMass);
    }

    public void ChangeProductType(string NewProductType)
    {
        if (!ProductTemp.ContainsKey(NewProductType))
        {
            throw new ArgumentException($"Invalid product type: {NewProductType}");
        }

        if (ProductType != NewProductType)
        {
            if (Mass > NetMass)
            {
                throw new ArgumentException("Cannot change product type when product type is more than the net mass.");
            }
            ProductType = NewProductType;
            Temperature = ProductTemp[NewProductType];
            Console.WriteLine($"Changed product type to {ProductType}. Temperature: {Temperature}.");
        }
    }
}

