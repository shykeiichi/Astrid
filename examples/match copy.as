a: int = 2;

// Matches can be used in variables to return a value using blocks 
b: str = match a 
{
    2: {
        return "a = 2";
    }
    3: {
        return "a = 3";
    }
    _:
    {
        return "a is not 2 or 3";
    }
}

print(b);

// They can also be used like a normal match
match a 
{
    2: {
        print("a = 2");
    }
    3: {
        print("a = 3");
    }
    _:
    {
        print("a is not 2 or 3");
    }
}