//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MissionskyOA.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class ExpenseMember
    {
        public int Id { get; set; }
        public int DId { get; set; }
        public int MemberId { get; set; }
    
        public virtual ExpenseDetail ExpenseDetail { get; set; }
        public virtual User User { get; set; }
    }
}
