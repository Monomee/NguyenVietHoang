using System.Collections.Generic;

public class LevelData
{
    public List<TubeData> Tubes;
    public int NumberOfTubes;

    public LevelData(int numberOfTubes = 8)
    {
        this.NumberOfTubes = numberOfTubes;
        Tubes = new List<TubeData>(numberOfTubes);
    }
}
