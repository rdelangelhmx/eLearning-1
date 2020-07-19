using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Models
{
    [Serializable]
    public class BlockchainTransaction
    {
        [JsonProperty]
        public string HoldersName { get; set; }

        [JsonProperty]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMMM/yyyy}")]
        public DateTime DateOfIssue { get; set; }

        [JsonProperty]
        public string CourseName { get; set; }
    }
}
