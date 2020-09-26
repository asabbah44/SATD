using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SATD.Models
{

    public class participantMetaData
    {
        [Required]
        [EmailAddress]
        public string Email;
        [Required]
        public Nullable<int> JobTitle;
        [Required]
        public Nullable<int> Experience;
    }

    public class ClassificationMetaData
    {
        public int ID;
        public int ParticipantID;
        public int CommentsID;
        [Required]
        public int ClassificationID;
    }
}