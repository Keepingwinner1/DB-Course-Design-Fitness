using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Models
{
    public class TokenValidationResult
    {
        public ClaimsPrincipal Principal { get; set; }
        public string Role { get; set; }
        public int userID { get; set; }
        public bool IsValid { get; set; }
    }

}
