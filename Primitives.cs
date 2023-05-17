namespace Astrid;

public class AsType {}

public class AsInt : AsType {
    int value;

    public AsInt(int value)
    {
        this.value = value;
    }
}

public class AsFloat : AsType {
    float value;

    public AsFloat(float value)
    {
        this.value = value;
    }
}

public class AsString : AsType {
    string value;

    public AsString(string value)
    {
        this.value = value;
    }
}


