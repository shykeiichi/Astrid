val1: float = float(to: input(message: "val1: "));
val2: float = float(to: input(message: "val2: "));
op: int = int(to: input(message: "op (+: 0, -: 1, *: 2, /: 3): "));

match op
{
    0: {
        print(message: val1 + val2);
    }
    1: {
        print(message: val1 - val2);
    } 
    2: {
        print(message: val1 * val2);
    }
    3: {
        print(message: val1 / val2);
    }
    _: {
        print(message: "Invalid operator!");
    }
}