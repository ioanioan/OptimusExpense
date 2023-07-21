using OptimusExpense.Infrastucture.Exception;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class AspNetUsersInfo
    {
        public AspnetUsers AspNetUsers { get; set; }

        public String RoleId { get; set; }

        public String RoleName { get; set; }

        public String Employee { get; set; }

        public String OldPassword { get; set; }

        public String Password { get; set; }
        public String ConfirmPassword { get; set; }

        public bool ValidatePassword()
        {
            if (this.AspNetUsers.Id == null&& (this.Password==""|| this.Password==null))
            {
                throw new HttpResponseException { Value="Introduceti o parola!" };
            }
            if (this.Password != this.ConfirmPassword)
            {
                throw new HttpResponseException { Value="Confirma parola este diferita de parola!" };
            }

            return true;
        }
    }
}
