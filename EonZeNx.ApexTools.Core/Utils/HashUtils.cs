using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using EonZeNx.ApexTools.Configuration;

namespace EonZeNx.ApexTools.Core.Utils
{
    public class HashCacheEntry
    {
        public string Value;
        public int Count;

        public HashCacheEntry(string value)
        {
            Value = value;
            Count = 1;
        }
    }
    
    public class HashCache
    {
        private readonly Dictionary<int, HashCacheEntry> Cache = new();
        private int MaxSize { get; set; }

        public HashCache(int size)
        {
            MaxSize = size;
        }

        /// <summary>
        /// Whether or not a given hash exists in the cache
        /// </summary>
        /// <param name="hash">Hash to lookup</param>
        /// <returns></returns>
        public bool Contains(int hash)
        {
            return Cache.ContainsKey(hash);
        }

        /// <summary>
        /// Get a value from the cache.
        /// </summary>
        /// <param name="hash">Hash to search for.</param>
        /// <returns>Non-nullable string</returns>
        public string Get(int hash)
        {
            if (!Contains(hash)) return "";
            
            return Cache[hash].Value;

        }

        /// <summary>
        /// Gets the lowest count hash in the cache and returns it.
        /// </summary>
        /// <returns>Nullable HashCacheEntry</returns>
        public int Lowest()
        {
            if (Cache.Count == 0) return 0;
            
            var low = Cache.Keys.ElementAt(0);
            int lowestValue = 999;
            foreach (var key in Cache.Keys)
            {
                if (Cache[key].Count < lowestValue)
                {
                    low = key;
                    lowestValue = Cache[key].Count;
                }
            }

            return low;
        }

        /// <summary>
        /// Adds a value to the cache
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="value"></param>
        public void Add(int hash, string value)
        {
            // Check if exists in cache already
            if (Contains(hash))
            {
                Cache[hash].Count++;
                return;
            }

            // Trim cache if at limit
            if (Cache.Count >= MaxSize)
            {
                var lowestHash = Lowest();
                Cache.Remove(hash);
            }

            Cache[hash] = new HashCacheEntry(value);
        }
    }
    
    public static class HashUtils
    {
        public static HashCache Cache;
        
        public static string Lookup(SQLiteConnection con, int hash)
        {
            if (!ConfigData.PerformDehash) return "";

            Cache ??= new HashCache(ConfigData.HashCacheSize);

            if (Cache.Contains(hash))
            {
                return Cache.Get(hash);
            }
            
            var command = con.CreateCommand();
            command.CommandText = "SELECT Value " +
                                  "FROM properties " +
                                  $"WHERE Hash = {hash}";
            using (var dbr = command.ExecuteReader())
            {
                if (dbr.Read())
                {
                    var value = dbr.GetString(0);
                    
                    Cache.Add(hash, value);
                    
                    return value;
                }
                return "";
            }
        }

        /// <summary>
        /// Simple string version.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static int HashJenkinsL3(string data, int index = 0, uint seed = 0)
        {
            return HashJenkinsL3(Encoding.UTF8.GetBytes(data), index, seed);
        }

        /// <summary>
        /// This is an implementation of Bob Jenkins Lookup3.
        /// </summary>
        /// <param name="data">Data to hash as a byte array</param>
        /// <param name="index">Index to start at. Default is 0</param>
        /// <param name="seed">Passable seed variable. Default is 0</param>
        /// <returns>Hashed value as a uint</returns>
        public static int HashJenkinsL3(byte[] data, int index = 0, uint seed = 0)
        {
            uint a = 0xDEADBEEF + (uint) data.Length + seed;
            uint b = a;
            uint c = a;

            // Don't get why just throw error or something, but w/e
            var remainderCount = data.Length % 12;
            if (data.Length != 0 && remainderCount == 0) remainderCount = 12;

            // Blocks of 12, make sure to evenly read 1 block past data.Length
            var remainderOffset = index + data.Length - remainderCount;
            
            // Main shift
            var currentOffset = index;
            while (currentOffset < remainderOffset)
            {
                a += BitConverter.ToUInt32(data, currentOffset);
                b += BitConverter.ToUInt32(data, currentOffset + 4);
                c += BitConverter.ToUInt32(data, currentOffset + 8);

                Mix(ref a, ref b, ref c);

                currentOffset += 12;
            }
            
            // Remainder shift
            switch (remainderCount)
            {
                case 12:
                    c += BitConverter.ToUInt32(data, currentOffset + 8);
                    goto case 8;

                case 11: c += (uint) data[currentOffset + 10] << 16; goto case 10;
                case 10: c += (uint) data[currentOffset + 9] << 8; goto case 9;
                case 9:  c += (uint) data[currentOffset + 8]; goto case 8;

                case 8:
                    b += BitConverter.ToUInt32(data, currentOffset + 4);
                    goto case 4;

                case 7: b += (uint) data[currentOffset + 6] << 16; goto case 6;
                case 6: b += (uint) data[currentOffset + 5] << 8; goto case 5;
                case 5: b += (uint) data[currentOffset + 4]; goto case 4;

                case 4:
                    a += BitConverter.ToUInt32(data, currentOffset);

                    Final(ref a, ref b, ref c);
                    break;

                case 3: a += (uint) data[currentOffset + 2] << 16; goto case 2;
                case 2: a += (uint) data[currentOffset + 1] << 8; goto case 1;
                case 1:
                    a += (uint) data[currentOffset];

                    Final(ref a, ref b, ref c);
                    break;
            }
            
            // return BitConverter.GetBytes(c);
            // return c;
            return (int) c;
        }
        
        private static void Mix(ref uint a, ref uint b, ref uint c)
        {
            a -= c; a ^= RotateLeft(c, 4); c += b;
            b -= a; b ^= RotateLeft(a,  6); a += c;
            c -= b; c ^= RotateLeft(b,  8); b += a;

            a -= c; a ^= RotateLeft(c, 16); c += b;
            b -= a; b ^= RotateLeft(a, 19); a += c;
            c -= b; c ^= RotateLeft(b,  4); b += a;
        }

        private static void Final(ref uint a, ref uint b, ref uint c)
        {
            c ^= b; c -= RotateLeft(b, 14);
            a ^= c; a -= RotateLeft(c, 11);
            b ^= a; b -= RotateLeft(a, 25);

            c ^= b; c -= RotateLeft(b, 16);
            a ^= c; a -= RotateLeft(c,  4);
            b ^= a; b -= RotateLeft(a, 14);

            c ^= b; c -= RotateLeft(b, 24);
        }

        private static uint RotateLeft(uint operand, int shiftCount)
        {
            shiftCount &= 0x1f;

            return (operand << shiftCount) | (operand >> (32 - shiftCount));
        }
    }
}