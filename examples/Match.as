in: string = input(message: "in: ");

match in 
{
    "hello": 
    {
        print(message: "Hello to you too!");
    }
    "goodbye":
    {
        print(message: "See you later!");
    }
    _:
    {
        print(message: "You said " + in);
    }
}