add :: (a: int, b: int) int {
    print(message: "Adding" + "test");
    return a + b;
}

val1: int = 2 + 2;
val: int = add(a: 1, b: val1 + 1);
val += 1;

if val1 == 4 {
    print(message: "val1 = 4");
}

while val1 > 0 {
    val1 -= 1;
    if val1 == 2 {
        print(message: "val1 = 2");
    }
    print(message: "test");
}

print(message: val + 1);
