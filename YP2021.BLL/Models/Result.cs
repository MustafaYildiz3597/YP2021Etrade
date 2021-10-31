using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nero2021.BLL.Models
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Text { get; set; }
        public int? Code { get; set; }
        public int NewID { get; set; }
        public int? ParentCount { get; set; }
        public string ResponseID { get; set; }
        public string ResponseKey { get; set; }
        public int? ReturnedInt { get; set; }
        public List<string> StrList { get; set; }
        public List<ResultRow> StrObj { get; set; }
        public string Message { get; set; }
        public DateTime? TRNDate { get; set; }
        public int TotalCount { get; set; }
        public int SuccededCount { get; set; }
        public int UnProccessedCount { get; set; }
        public int AddedCount { get; set; }
        public int UpdatedCount { get; set; }
    }

    public class ResultRow
    {
        public int? ID { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }


}
