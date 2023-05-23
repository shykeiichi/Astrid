nums: [int] = [2, 7, 11, 15];
target: int = 9;

ind1: int = 0;
ind2: int = 0;

arr = arrpush(arr: nums, value: 12);

i: int = 0;
while i < len(of: nums)
{
    j: int = 0;
    while j < len(of: nums)
    {
        if nums[i] + nums[j] == target
        {
            ind1 = i;
            ind2 = j;
        }
    }
}

print(message: str(from: ind1) + " and " + str(from: ind2));