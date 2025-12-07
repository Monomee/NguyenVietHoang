[System.Serializable]
public class DifficultyConfig
{
    public int numberOfTubes;
    public int emptyTubes;
    public int depth;
    public int colorCount;

    public int maxCompletedAtStart;
    public int minValidMoves;

    public int maxShuffleAttempts;
}
public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Expert
}
public static class DifficultyTable
{
    public static DifficultyConfig Get(Difficulty diff)
    {
        switch (diff)
        {
            case Difficulty.Easy:
                return new DifficultyConfig
                {
                    numberOfTubes = 6,
                    emptyTubes = 2,
                    depth = 4,
                    colorCount = 4,
                    maxCompletedAtStart = 1,
                    minValidMoves = 2,
                    maxShuffleAttempts = 20
                };

            case Difficulty.Normal:
                return new DifficultyConfig
                {
                    numberOfTubes = 8,
                    emptyTubes = 2,
                    depth = 4,
                    colorCount = 6,
                    maxCompletedAtStart = 1,
                    minValidMoves = 1,
                    maxShuffleAttempts = 40
                };

            case Difficulty.Hard:
                return new DifficultyConfig
                {
                    numberOfTubes = 9,
                    emptyTubes = 1,
                    depth = 4,
                    colorCount = 7,
                    maxCompletedAtStart = 0,
                    minValidMoves = 1,
                    maxShuffleAttempts = 80
                };

            case Difficulty.Expert:
                return new DifficultyConfig
                {
                    numberOfTubes = 10,
                    emptyTubes = 1,
                    depth = 5,
                    colorCount = 8,
                    maxCompletedAtStart = 0,
                    minValidMoves = 1,
                    maxShuffleAttempts = 150
                };

            default:
                return Get(Difficulty.Normal);
        }
    }
}

