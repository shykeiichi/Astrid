n1: int = 0;
n2: int = 1;

i: int = 0;

n: int = int(from: input(message: "n: "));

while i < n
{
    i = i + 1;
    print(message: n1);

    n3: int = n1 + n2;
    n1 = n2;
    n2 = n3;
}