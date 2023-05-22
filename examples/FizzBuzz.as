val: int = 1;
while val < 100
{
    fb: string = "";
    if val % 3 == 0
    {
        fb += "Fizz";
    }
    if val % 5 == 0
    {
        fb += "Buzz";
    }
    if fb == ""
    {
        print(message: val);
    } 
    if fb != ""
    {
        print(message: fb);
    }
    val += 1;
}