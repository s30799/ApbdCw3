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
        MaxCapacity = 1000;
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

    public GasContainer(double mass, double hight, double netMass, double depth) : 
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

    public RefrigratedContainer(double mass, double hight, double netMass, double depth, string ProductType) :
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

public class Ship
{
    public string Name { get; set; }
    public int MacContainers { get; private set; }
    public double MaxSpeed { get; private set; }
    public double MaXWeight { get; private set; }
    public List<Container>  Containers { get; private set; }

    public Ship(string name, int macContainers, double maxSpeed, double maXWeight)
    {
        Name = name;
        MacContainers = macContainers;
        MaxSpeed = maxSpeed;
        MaXWeight = maXWeight;
        Containers = new List<Container>();
        
    }

    public void LoadContainer(Container container)
    {
        if (Containers.Count >= MacContainers)
        {
            throw new InvalidOperationException("Cannot load container more than a mac container!");
        }
        double totalWeight = Containers.Sum(c => c.Mass) + container.Mass;
        if (totalWeight > MaXWeight)
        {
            throw new InvalidOperationException("Cannot load container more than a mac container!");
        }
        Containers.Add(container);
        Console.WriteLine($"Loaded container: {container.SerialNum} for {Name}");
    }

    public void UnloadContainer(string serialNum)
    {
        Container container = Containers.FirstOrDefault(c => c.SerialNum == serialNum);
        if (container == null)
        {
            throw new ArgumentException($"No container found for {serialNum}");
        }
        Containers.Remove(container);
        Console.WriteLine($"Unloaded container: {container.SerialNum} for {Name}");
    }

    public void ReplaceContainer(string serialNum, Container newContainer)
    {
        UnloadContainer(serialNum);
        LoadContainer(newContainer);
    }

    public void TransferContainer(string serialNum, Ship destShip)
    {
        Container container = Containers.FirstOrDefault(c => c.SerialNum == serialNum);
        if (container == null)
        {
            throw new ArgumentException($"No container found for {serialNum}");
        }
        destShip.LoadContainer(container);
        Containers.Remove(container);
        Console.WriteLine($"Transfer container: {container.SerialNum} for {Name}");
    }

    public override string ToString()
    {
        return $"Ship {Name} - Containers: {Containers.Count}/{MacContainers}, Total Weight: {Containers.Sum(c => c.Mass)}kg/{MaXWeight}kg";
    }
}

class program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Container Management system");
        Console.WriteLine("=============================");
        try
        {
        Ship s1 = new Ship("Hehe", 10, 30, 5000);
        Ship s2 = new Ship("Yllo", 15, 25, 8000);
        
        Container standardContainer = new Container(100, 250, 100, 600);
        LiquidContainer liquidContainer = new LiquidContainer(200, 200, 150, 500, false);
        LiquidContainer hazardousLiquidContainer = new LiquidContainer(150, 200, 150, 500, true);
        GasContainer gasContainer = new GasContainer(120, 180, 120, 450);
        RefrigratedContainer refrigeratedContainer = new RefrigratedContainer(180, 220, 180, 550, "Fish");
        
        standardContainer.LoadCargo(200);
        liquidContainer.LoadCargo(300);
        gasContainer.LoadCargo(20);
        refrigeratedContainer.LoadCargo(300);
        
        try
        {
            hazardousLiquidContainer.LoadCargo(200);
        }
        catch (OverfillException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        s1.LoadContainer(standardContainer);
        s1.LoadContainer(liquidContainer);
        s1.LoadContainer(gasContainer);
        
        s2.LoadContainer(refrigeratedContainer);
        s2.LoadContainer(hazardousLiquidContainer);
        
        Console.WriteLine("\nShip status:");
        Console.WriteLine(s1);
        Console.WriteLine(s2);
        
        Console.WriteLine("\nTransferring container:");
        s1.TransferContainer(gasContainer.SerialNum, s2);
        
        Console.WriteLine("\nUpdated ship status:");
        Console.WriteLine(s1);
        Console.WriteLine(s2);
        
        Console.WriteLine("\nUnloading container cargo:");
        liquidContainer.UnloadCargo();
        
        Console.WriteLine("\nChanging refrigerated container product type:");
        refrigeratedContainer.UnloadCargo();
        refrigeratedContainer.ChangeProductType("Chocolate");
        refrigeratedContainer.LoadCargo(250);
        }
        
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
