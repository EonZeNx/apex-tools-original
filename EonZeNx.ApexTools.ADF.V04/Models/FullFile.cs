using System.Collections.Generic;
using EonZeNx.ApexTools.Core.Refresh.Interfaces;

namespace EonZeNx.ApexTools.ADF.V04.Models
{
    public class FullFile : IAvaFileSimple
    {
        #region Variables

        public Header Header { get; set; }
        public List<Instance> Instances { get; set; }

        #endregion

        
        #region Public Functions

        public void Deserialize(byte[] contents)
        {
            throw new System.NotImplementedException();
        }

        public byte[] Export()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}