using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Models.DTOs.RequestDTOs
{
    public class UserLogin
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
