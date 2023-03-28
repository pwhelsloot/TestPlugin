using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{ 
  [Serializable]
  [DataContract(Namespace = "http://www.solutionworks.co.uk/wims")]
  public class EntityBlobData
  {
    [DataMember(Name = "BlobMemberName")]
    public string BlobMemberName { get; set; }

    [DataMember(Name = "Data")]
    public byte[] Data { get; set; }
  }
}
