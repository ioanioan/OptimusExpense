using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class Person: IEntityBase
    {
        public string EntityId { get => "PersonId"; set { } }
        public int PersonId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public int? PartnerId { get; set; }
        public int? PositionId { get; set; }
        public String PersonalNumericCode { get; set; }
        public String IdentityCardNumber { get; set; }
        public String IssuedBy { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public String IdentityCardSeries { get; set; }
    }
}
