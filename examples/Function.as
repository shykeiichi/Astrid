<<<<<<< HEAD
add :: (a: int, b: int) int {
    print(message: "Adding" + "test");
    print(message: "a");
    print(message: a);
    print(message: "b");
    print(message: b);
    return a + b;
}

val1: int = 2 + 2;
val: int = add(b: 1, a: val1 + 1) + 1;
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
=======
add :: (a: int, b: int) int 
{
    return a + b;
}

print(message: add(a: 1, b: 2));
>>>>>>> fcebca8a7c3a6a29ba0baad502b7f76957a360bd
