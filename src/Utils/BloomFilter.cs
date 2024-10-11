using BloomFilter; // Import the BloomFilter.Net namespace

namespace BlogApp.Utils;

public class BloomFilterUtil(int size, int numberOfHashFunctions)
{
    private readonly BloomFilter<string> _bloomFilter = new(size, numberOfHashFunctions);

    public void AddEmail(string email)
    {
        _bloomFilter.Add(email);
    }

    public bool CheckEmail(string email)
    {
        return _bloomFilter.Contains(email);
    }
}
