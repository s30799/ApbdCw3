

public class Container
{
    public float Mass { get; set; }
    public float Hight { get; set; }
    public float NetMass { get; set; }
    public float Depth { get; set; }
    public string SerialNum { get; private set; }
    public float MaxCapacity { get; set; }
    
    public static void Main(string[] args)
    {
        
    }

    public void EmptyContainer(bool isEmpty)
    {
        
    }

    public void Fillcontainer(float LoadMass)
    {
        
    }
}

public interface IHazadNotifer
{
    public void Notify();
}
public class LiquidContainer : Container, IHazadNotifer
{
    public void Notify()
    {
        Console.WriteLine("Uwaga,niebezpieczna substancja w kontenerze o numerze: " + SerialNum);
    }

    public void Fillcontainer(float LoadMass)
    {
        
    }
}