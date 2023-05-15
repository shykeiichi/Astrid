Operator :: enum {
    Plus 
    Minus
    Multiply
    Divide
    Power
}

a: float = 10;
b: float = 20;
op: Operator = .Plus;

result: float = match op {
    .Plus: a + b,
    .Minus: a - b,
    .Multiply: a * b,
    .Divide: a / b,
    .Power: a ** b
};

print(message: result);