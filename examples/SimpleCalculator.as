val1: float = float(to: input(message: "val1: "));
val2: float = float(to: input(message: "val2: "));
op: int = int(to: input(message: "op (plus: 0, minus: 1, multiply: 2, divide: 3): "));

if op == 0
{
    print(message: val1 + val2);
}
if op == 1
{
    print(message: val1 - val2);
}
if op == 2
{
    print(message: val1 * val2);
}
if op == 3
{
    print(message: val1 / val2);
}