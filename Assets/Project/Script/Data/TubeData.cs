using System.Collections.Generic;

public class TubeData 
{
    public Stack<ColorType> Colors;
    public readonly int Depth;

    public TubeData(int depth = 4)
    {
        this.Depth = depth;
        Colors = new Stack<ColorType>(depth);
    }
}

public enum ColorType
{
    Red,
    Blue,
    Green,
    Yellow,
    Pink,
    Purple,
    Orange,
    Cyan
}
