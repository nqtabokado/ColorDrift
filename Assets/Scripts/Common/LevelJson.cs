[System.Serializable]
public class RootLevel
{
    public int width;
    public int height;
    public ColorPosition[] regions;
}
[System.Serializable]
public class ColorPosition
{
    public int x;
    public int y;
    public string region;
}
