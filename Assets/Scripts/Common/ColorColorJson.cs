[System.Serializable]
public class RootConfig
{
    public IgnoreConfig ignore;
    public RegionConfig[] regions;
}

[System.Serializable]
public class IgnoreConfig
{
    public float black_v;
    public float white_s;
    public float white_v;
}

[System.Serializable]
public class RegionConfig
{
    public string name;
    public float h_min;
    public float h_max;
    public float s_min;
}