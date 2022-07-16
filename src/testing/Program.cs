using shared._2d;
using System.Numerics;

try
{
    Console.WriteLine("Start try");
    return;
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
finally
{
    Console.WriteLine("Finally executed");
}

