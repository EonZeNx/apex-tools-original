using System;
using System.Collections.Generic;
using EonZeNx.ApexTools.IRTPC.V01.Models.Variants;

namespace EonZeNx.ApexTools.IRTPC.V01.Models
{
    public class PropertyComparer : Comparer<PropertyVariants>
    {
        public override int Compare(PropertyVariants x, PropertyVariants y)
        {
            // Null compare
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;
            
            // Name null compare
            if (x.Name == null && y.Name == null) return 0;
            if (x.Name == null) return 1;
            if (y.Name == null) return -1;
            
            // No name compare
            if (x.Name.Length == 0 && y.Name.Length == 0)
            {
                return string.Compare(x.HexNameHash, y.HexNameHash, StringComparison.Ordinal);
            }
            if (x.Name.Length == 0) return 1;
            if (y.Name.Length == 0) return -1;

            // Name compare
            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }
    }
    
    public class ContainerComparer : Comparer<Container>
    {
        public override int Compare(Container x, Container y)
        {
            // Null compare
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;
            
            // Name null compare
            if (x.Name == null && y.Name == null) return 0;
            if (x.Name == null) return 1;
            if (y.Name == null) return -1;
            
            // No name compare
            if (x.Name.Length == 0 && y.Name.Length == 0)
            {
                return string.Compare(x.HexNameHash, y.HexNameHash, StringComparison.Ordinal);
            }
            if (x.Name.Length == 0) return 1;
            if (y.Name.Length == 0) return -1;

            // Name compare
            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }
    }
}