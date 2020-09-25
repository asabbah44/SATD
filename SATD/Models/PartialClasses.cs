using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SATD.Models
{
  
    [MetadataType(typeof(participantMetaData))]
    public partial class participant { }

    [MetadataType(typeof(ClassificationMetaData))]
    public partial class Classification { }
}