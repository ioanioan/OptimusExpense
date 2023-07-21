using OptimusExpense.Model;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimusExpense.Data.Abstract
{
    public interface IUserActionRepository : IEntityBaseRepository<UserAction>
    {
        List<MenuInfo> GetMenuByUser(String userId);
    }


    public interface IDocumentStateRepository: IEntityBaseRepository<DocumentState>
    {
    }

    public interface IPropertyEntityValueRepository : IEntityBaseRepository<PropertyEntityValue>
    {
    }
     

    public interface IExpenseProjectRepository : IEntityBaseRepository<ExpenseProject>
    {  
    }

    public interface ISerialNumberRepository : IEntityBaseRepository<SerialNumber>
    {
        void SetNumberDocument(Document doc);
    }

    public interface IDocumentRepository : IEntityBaseRepository<Document>
    {
    }

    public interface IDocumentDetailRepository : IEntityBaseRepository<DocumentDetail>
    {
    }

    public interface Ipck_OrderViewRepository
    {
        IQueryable<OptimusExpense.Model.DTOs.pck_OrderViewInfo> GetActiveOrders(String userId);
        IQueryable<pck_TaskViewInfo> GetTasksByOrder(String orderNumber);
        IQueryable<pck_CartViewInfo> GetCartsByOrder(String orderNumber);
    }

    public interface IAspnetUsersRepository : IEntityBaseRepository<AspnetUsers>
    {
        IQueryable<AspNetUsersInfo> GetUsers();
        IQueryable<AspNetRoles> GetAspNetRoles();
         Task<AspNetUsersInfo> Save(AspNetUsersInfo entity);
        IQueryable<String> GetSubordination(String userId);
        Task<AspNetUsersInfo> ChangePassword(AspNetUsersInfo entity);
    }

    public interface IExpenseReportRepository : IEntityBaseRepository<ExpenseReport>
    {
        ExpenseReportInfo Save(ExpenseReportInfo entity);
        IQueryable<ExpenseReportInfo> GetListExpenseReport(FilterInfo param);
        List<DashboardExpenseReportInfo> GetDashboardExpenseReport(FilterInfo param);
        DataSet GetRaportExpenseReport(FilterInfo filter);
        ExpenseReportInfo Remove(ExpenseReportInfo entity);
    }

    public interface IExpenseAdvanceRepository : IEntityBaseRepository<ExpenseAdvance>
    {
        ExpenseAdvanceInfo Save(ExpenseAdvanceInfo entity);
        IQueryable<ExpenseAdvanceInfo> GetListExpenseAdvance(FilterInfo param);
        ExpenseAdvanceInfo Remove(ExpenseAdvanceInfo entity);
    }

    public interface IExpenseNatureRepository : IEntityBaseRepository<ExpenseNature>
    {
        IQueryable<ExpenseNatureInfo> GetAllExpenseNature();
        ExpenseNatureInfo GetExpenseNatureById(int idExp);
    }

    public interface IExpenseCostCenterRepository : IEntityBaseRepository<ExpenseCostCenter>
    {
        IQueryable<ExpenseCostCenterInfo> GetAllExpenseCostCenter();
        ExpenseCostCenterInfo GetExpenseCostCenterById(int id);
    }

    public interface IReportRepository : IEntityBaseRepository<Report>
    {

        IEnumerable<ReportDetailInfo> GetReportDetails(Model.Models.Report report);
        List<Dictionary<String, Object>> RunReport(String userId, ReportInfo report);
    }

    public interface Ipck_OrderLogRepository: IEntityBaseRepository<pck_OrderLog>
    {
        List<Model.Models.pck_OrderLog> SaveOrder(pck_OrderViewInfo entity, String userId);
    }

    public interface ICurrencyRepository : IEntityBaseRepository<Currency>
    {
    }

    public interface IExpenseRepository : IEntityBaseRepository<Expense>
    {
        IQueryable<ExpenseInfo> GetListExpenseDraft(FilterInfo param);
        IQueryable<ExpenseInfo> GetListExpense(String userId = null);
        List<ExpenseInfo> GetListLastExpenses(FilterInfo param);
        dynamic GetGraphicExpenses(String userId);
    }

    public interface IDictionaryDetailRepository : IEntityBaseRepository<DictionaryDetail>
    {
        List<DictionaryDetailInfo> GetDictionaryDetail();
        IQueryable<Dictionary> GetDictionary();
        IQueryable<DictionaryDetail> GetDictionaryDetailByDictionaryId(int dictionaryId);
    }

    public interface IPartnerRepository : IEntityBaseRepository<Partner>
    {
        List<PartnerInfo> GetAllPartners();
        PartnerInfo GetCompanyByUserId(String userId);
    }

    public interface IPartnerPointRepository : IEntityBaseRepository<PartnerPoint>
    {
        List<PartnerPoint> GetPartnerPointsByPersonId(int personId);
        IQueryable<PartnerPointInfo> GetPartnerPointsByPartnerId(int partnerId);
    }

    public interface ILogRepository : IEntityBaseRepository<Log>
    {
        
    }

    public interface IPersonRepository : IEntityBaseRepository<Person>
    {
        List<PersonInfo> GetAllPersons();
    }

    public interface IEmployeeRepository : IEntityBaseRepository<Employee>
    {
        List<EmployeeInfo> GetAllEmployees();
        EmployeeInfo GetEmployeeByDocumentId(int docId);
        EmployeeInfo GetEmployeeByUserId(String userId);
    }

    public interface IEF_RaportareRepository : IEntityBaseRepository<EF_Raportare>
    {
         EF_RaportareInfo SendEFactura(String server, String port,  int id);
        EF_RaportareInfo SendEFactura(String server, String port, EF_RaportareXML factura);

        EF_Raportare UploadEFactura(String userId,String facturi, String xml);

        IQueryable<EF_RaportareInfo> GetRaportari(FilterInfo oaram);

        EF_RaportareInfo GetStareEFactura(String server, String port, EF_RaportareInfo factura);

        void UplodNewFacturi(FilterInfo param);

        EF_RaportareInfo Remove(EF_RaportareInfo entity);

        byte[] DownLoadEFactura(String server, String port, int id);

        void UpdateDataFactura();

        EF_RaportareInfo GetStariEFacturi(String server, String port, FilterInfo param);

        EF_RaportareInfo SendEFacturi(String server, String port, FilterInfo param);
    }

     
}
