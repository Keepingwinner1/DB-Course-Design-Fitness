using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Models
{
    public class CourseSearchParams
    {
            public string CourseName { get; set; }
            public int? TypeID { get; set; }
            public int? MinPrice { get; set; }
            public int? MaxPrice { get; set; }
/*            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }*/
    }
}
