namespace FastCache;

public enum CacheStrategy : byte
{
    Expirable = 0x0,
    LRU = 0x1,
    Unbound = 0x3
}
