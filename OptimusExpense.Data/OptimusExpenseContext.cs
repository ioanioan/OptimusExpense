using Microsoft.EntityFrameworkCore;
using OptimusExpense.Model;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptimusExpense.Data
{
    public class OptimusExpenseContext: DbContext
    {

        public DbSet<UserAction> UserAction { get; set; }
        public DbSet<UserActionXRoles> UserActionXRoles { get; set; }
        public DbSet<UserXForbiddenUserAction> UserXForbiddenUserAction { get; set; }
        public DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public DbSet<ExpenseProject> ExpenseProject { get; set; }
        public DbSet<ExpenseNature> ExpenseNature { get; set; }
        public DbSet<DictionaryDetail> DictionaryDetail { get; set; }
        public DbSet<Dictionary> Dictionary { get; set; }
        public DbSet<Partner> Partner { get; set; }
        public DbSet<PartnerPoint> PartnerPoint { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Expense> Expense { get; set; }
        public DbSet<ExpenseReport> ExpenseReport { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<DocumentDetail> DocumentDetail { get; set; }
        public DbSet<DocumentType> DocumentType { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<AspnetUsers> AspnetUsers { get; set; }
        public DbSet<AspNetRoles> AspNetRoles { get; set; }
        public DbSet<DocumentState> DocumentState { get; set; }
        public DbSet<PropertyEntityValue> PropertyEntityValue { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<ReportDetail> ReportDetail { get; set; }
        public DbSet<ReportDetailCombo> ReportDetailCombo { get; set; }
        public DbSet<ExpenseAdvance> ExpenseAdvance { get; set; }
        public DbSet<SerialNumber> SerialNumber { get; set; }
        public DbSet<ExpenseCostCenter> ExpenseCostCenter { get; set; }

        public DbSet<EF_Raportare> EF_Raportare { get; set; }

        public DbSet<EF_RaportareXML> EF_RaportareXML { get; set; }

        public DbSet<Log> Log { get; set; }

        public DbSet<pck_CartView> pck_CartView { get; set; }
        public DbSet<pck_OrderView> pck_OrderView { get; set; }
        public DbSet<pck_TaskView> pck_TaskView { get; set; }
        public DbSet<pck_OrderLog> pck_OrderLog { get; set; }

        public OptimusExpenseContext(DbContextOptions<OptimusExpenseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            ConfigureModelBuilderForUser(modelBuilder);
        }

        void ConfigureModelBuilderForUser(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserAction>().HasKey(p => p.UserActionId);
            modelBuilder.Entity<UserAction>().Ignore(p => p.EntityId);
            modelBuilder.Entity<UserActionXRoles>().HasKey(p => new { p.UserActionId, p.RoleId });
            modelBuilder.Entity<UserXForbiddenUserAction>().HasKey(p => new { p.UserActionId, p.UserId });
            modelBuilder.Entity<AspNetUserRoles>().HasKey(p => new { p.RoleId, p.UserId });
            modelBuilder.Entity<ExpenseProject>().HasKey(p => p.ExpenseProjectId);
            modelBuilder.Entity<ExpenseProject>().Ignore(p => p.EntityId);
            modelBuilder.Entity<DictionaryDetail>().HasKey(p => p.DictionaryDetailId);
            modelBuilder.Entity<DictionaryDetail>().Ignore(p => p.EntityId);
            modelBuilder.Entity<Dictionary>().HasKey(p => p.DictionaryId);
            modelBuilder.Entity<ExpenseNature>().HasKey(p => p.ExpenseNatureId);
            modelBuilder.Entity<ExpenseNature>().Ignore(p => p.EntityId);
            modelBuilder.Entity<Partner>().HasKey(p => p.PartnerId);
            modelBuilder.Entity<Partner>().Ignore(p => p.EntityId);
            modelBuilder.Entity<PartnerPoint>().HasKey(p => p.PartnerPointId);
            modelBuilder.Entity<PartnerPoint>().Ignore(p => p.EntityId);
            modelBuilder.Entity<Person>().HasKey(p => p.PersonId);
            modelBuilder.Entity<Person>().Ignore(p => p.EntityId);
            modelBuilder.Entity<Employee>().HasKey(p => p.EmployeeId);
            modelBuilder.Entity<Employee>().Ignore(p => p.EntityId);
            modelBuilder.Entity<Document>().HasKey(p => p.DocumentId);
            modelBuilder.Entity<Document>().Ignore(p => p.EntityId);
            modelBuilder.Entity<DocumentDetail>().HasKey(p => p.DocumentDetailId);
            modelBuilder.Entity<DocumentDetail>().Ignore(p => p.EntityId);
            modelBuilder.Entity<Expense>().HasKey(p => p.ExpenseId);
            modelBuilder.Entity<Expense>().Ignore(p => p.EntityId).Property(x => x.CurrencyRate) .HasPrecision(18, 6); 
            modelBuilder.Entity<ExpenseReport>().HasKey(p => p.ExpenseReportId);
            modelBuilder.Entity<ExpenseReport>().Ignore(p => p.EntityId);
            modelBuilder.Entity<DocumentType>().HasKey(p => p.DocumentTypeId);
            modelBuilder.Entity<Currency>().HasKey(p => p.CurrencyId);
            modelBuilder.Entity<Currency>().Ignore(p => p.EntityId);
            modelBuilder.Entity<AspnetUsers>().HasKey(p => p.Id);
            modelBuilder.Entity<AspnetUsers>().Ignore(p => p.EntityId);
            modelBuilder.Entity<AspNetRoles>().HasKey(p => p.Id);
            modelBuilder.Entity<DocumentState>().HasKey(p => new { p.Id });
            modelBuilder.Entity<DocumentState>().Ignore(p => p.EntityId);
            modelBuilder.Entity<PropertyEntityValue>().HasKey(p => new { p.PropertyEntityValueId  });
            modelBuilder.Entity<PropertyEntityValue>().Ignore(p => p.EntityId);
            modelBuilder.Entity<Report>().HasKey(p => new { p.ReportId });
            modelBuilder.Entity<Report>().Ignore(p => p.EntityId);
            modelBuilder.Entity<ReportDetail>().HasKey(p => new { p.ReportDetailId });
            modelBuilder.Entity<ReportDetail>().Ignore(p => p.EntityId);
            modelBuilder.Entity<ReportDetailCombo>().HasKey(p => new { p.ReportDetailComboId });
            modelBuilder.Entity<ReportDetailCombo>().Ignore(p => p.EntityId);
            modelBuilder.Entity<ExpenseAdvance>().HasKey(p => new { p.ExpenseAdvanceId });
            modelBuilder.Entity<ExpenseAdvance>().Ignore(p => p.EntityId);
            modelBuilder.Entity<SerialNumber>().HasKey(p => new { p.SerialNumberId });
            modelBuilder.Entity<SerialNumber>().Ignore(p => p.EntityId);
            modelBuilder.Entity<ExpenseCostCenter>().HasKey(p => p.CostCenterId);
            modelBuilder.Entity<ExpenseCostCenter>().Ignore(p => p.EntityId);

            modelBuilder.Entity<EF_Raportare>().HasKey(p => p.IdEFRaportare);
            modelBuilder.Entity<EF_Raportare>().Ignore(p => p.EntityId);

            modelBuilder.Entity<EF_RaportareXML >().HasKey(p => p.IdEFRaportare);
            modelBuilder.Entity<EF_RaportareXML>().Ignore(p => p.EntityId);

            modelBuilder.Entity<Log>().HasKey(p => p.LogId);
            modelBuilder.Entity<Log>().Ignore(p => p.EntityId);
            modelBuilder.Entity<pck_OrderView>().HasKey(p => new { p.OrderNumber, p.TaskCode });
            modelBuilder.Entity<pck_TaskView>().HasKey(p => new { p.TaskCode, p.TaskName });
            modelBuilder.Entity<pck_CartView>().HasKey(p => new { p.OrderNumber, p.CarriageNumber });
            modelBuilder.Entity<pck_OrderLog>().HasKey(p => new { p.LogId });
            modelBuilder.Entity<pck_OrderLog>().Ignore(p => p.EntityId);

            //modelBuilder.Entity<User>()
            //    .Property(user => user.Username)
            //    .HasMaxLength(60)
            //    .IsRequired();

            //modelBuilder.Entity<User>()
            //    .Property(user => user.Email)
            //    .HasMaxLength(60)
            //    .IsRequired();
        }
    }

}
