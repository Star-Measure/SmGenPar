namespace SmGenPar.Logic.Models;

public class LengthAttribute : Attribute
{
    public readonly long Lenght;

    public LengthAttribute(long lenght)
    {
        Lenght = lenght;
    }
}