a: int = 1;
b: int = 2;

c: int = a + {
    a += 1;
    return a + b; // use return to return value
};

// c = 5