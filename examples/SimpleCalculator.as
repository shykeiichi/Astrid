val1: float = float(to: input(message: "val1: "));
val2: float = float(to: input(message: "val2: "));
op: int = input(message: "op (+, -, *, /): ");

match op
{
    "+": {
        print(message: val1 + val2);
    }
    "-": {
        print(message: val1 - val2);
    } 
    "*": {
        print(message: val1 * val2);
    }
    "/": {
        print(message: val1 / val2);
    }
    _: {
        print(message: "Invalid operator!");
    }
}