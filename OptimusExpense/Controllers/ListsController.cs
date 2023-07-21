using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OptimusExpense.Data.Abstract;
using OptimusExpense.Infrastucture;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusExpense.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ListsController: BaseController
    {
        private readonly ILogger<ListsController> _logger;
        private IExpenseProjectRepository _expenseProjectRepository;
        private IExpenseNatureRepository _expenseNatureRepository;
        private IDictionaryDetailRepository _dictionaryDetailRepository;
        private IPartnerRepository _partnerRepository;
        private IPersonRepository _personRepository;
        private IEmployeeRepository _employeeRepository;
        private IPartnerPointRepository _partnerPointRepository;
        private ICurrencyRepository _currencyRepository;
        private IExpenseCostCenterRepository _expenseCostCenterRepository;

        public ListsController(ILogger<ListsController> logger,
            IExpenseProjectRepository expenseProjectRepository, IExpenseNatureRepository expenseNatureRepository, IDictionaryDetailRepository dictionaryDetailRepository,
            IPartnerRepository partnerRepository, IPersonRepository personRepository, IEmployeeRepository employeeRepository, IPartnerPointRepository partnerPointRepository,
            ICurrencyRepository currencyRepository, IExpenseCostCenterRepository expenseCostCenterRepository,
            UserManager<AspnetUsers> _userManager, IWebHostEnvironment webHostEnvirnoment ) : base(_userManager, webHostEnvirnoment)
        {
            _expenseProjectRepository = expenseProjectRepository;
            _expenseNatureRepository = expenseNatureRepository;
            _dictionaryDetailRepository = dictionaryDetailRepository;
            _partnerRepository = partnerRepository;
            _personRepository = personRepository;
            _employeeRepository = employeeRepository;
            _partnerPointRepository = partnerPointRepository;
            _currencyRepository = currencyRepository;
            _expenseCostCenterRepository = expenseCostCenterRepository;
            _logger = logger;       
        }

        [HttpGet("GetExpenseProjects")]
        public IEnumerable<OptimusExpense.Model.Models.ExpenseProject> GetExpenseProjects()
        {
            var result = _expenseProjectRepository.Get();
            return result;
        }

        [HttpPost("SaveProject")]
        public  OptimusExpense.Model.Models.ExpenseProject SaveProject(OptimusExpense.Model.Models.ExpenseProject entity)
        {
            return _expenseProjectRepository.Save(entity);
        }

        [HttpPost("DeleteProject")]
        public OptimusExpense.Model.Models.ExpenseProject DeleteProject(OptimusExpense.Model.Models.ExpenseProject entity)
        {
            _expenseProjectRepository.Remove(entity);
            return entity;
        }

        [HttpGet("GetExpenseNature")]
        public IEnumerable<OptimusExpense.Model.DTOs.ExpenseNatureInfo> GetExpenseNature()
        {
            var result = _expenseNatureRepository.GetAllExpenseNature();
            return result;
        }

        [HttpGet("GetExpenseNatureActive")]
        public IEnumerable<OptimusExpense.Model.DTOs.ExpenseNatureInfo> GetExpenseNatureActive()
        {
            var result = _expenseNatureRepository.GetAllExpenseNature().Where(p=>p.ExpenseNature.Active);
            return result;
        }

        [HttpPost("SaveExpenseNature")]
        public OptimusExpense.Model.DTOs.ExpenseNatureInfo SaveExpenseNature(OptimusExpense.Model.DTOs.ExpenseNatureInfo entity)
        {
            _expenseNatureRepository.Save(entity.ExpenseNature);
            var result = _expenseNatureRepository.GetExpenseNatureById(entity.ExpenseNature.ExpenseNatureId);
            return result;
        }

        [HttpPost("DeleteExpenseNature")]
        public OptimusExpense.Model.DTOs.ExpenseNatureInfo DeleteExpenseNature(OptimusExpense.Model.DTOs.ExpenseNatureInfo entity)
        {
            _expenseNatureRepository.Remove(entity.ExpenseNature);
            return entity;
        }

        [HttpGet("GetAllExpenseCostCenter")]
        public IEnumerable<OptimusExpense.Model.DTOs.ExpenseCostCenterInfo> GetAllExpenseCostCenter()
        {
            var result = _expenseCostCenterRepository.GetAllExpenseCostCenter();
            return result;
        }

        [HttpPost("SaveExpenseCostCenter")]
        public OptimusExpense.Model.DTOs.ExpenseCostCenterInfo SaveExpenseCostCenter(OptimusExpense.Model.DTOs.ExpenseCostCenterInfo entity)
        {
            _expenseCostCenterRepository.Save(entity.ExpenseCostCenter);
            var result = _expenseCostCenterRepository.GetExpenseCostCenterById(entity.ExpenseCostCenter.CostCenterId);
            return result;
        }

        [HttpPost("DeleteExpenseCostCenter")]
        public OptimusExpense.Model.DTOs.ExpenseCostCenterInfo DeleteExpenseCostCenter(OptimusExpense.Model.DTOs.ExpenseCostCenterInfo entity)
        {
            _expenseCostCenterRepository.Remove(entity.ExpenseCostCenter);
            return entity;
        }

        [HttpGet("GetDictionaryDetail")]
        public IEnumerable<OptimusExpense.Model.DTOs.DictionaryDetailInfo> GetDictionaryDetail()
        {
            var result = _dictionaryDetailRepository.GetDictionaryDetail();
            return result;
        }

        [HttpGet("GetDictionary")]
        public IEnumerable<OptimusExpense.Model.Models.Dictionary> GetDictionary()
        {
            var result = _dictionaryDetailRepository.GetDictionary();
            return result;
        }

        [HttpGet("GetDictionaryDetailByDictionaryId/{dictionaryId}")]
        public IEnumerable<OptimusExpense.Model.Models.DictionaryDetail> GetDictionaryDetailByDictionaryId(int dictionaryId)
        {
            var result = _dictionaryDetailRepository.GetDictionaryDetailByDictionaryId(dictionaryId);
            return result;
        }

        [HttpGet("GetPartnerPointsByPersonId/{personId}")]
        public IEnumerable<OptimusExpense.Model.Models.PartnerPoint> GetPartnerPointsByPersonId(int personId)
        {
            var result = _partnerPointRepository.GetPartnerPointsByPersonId(personId);
            return result;
        }


        [HttpGet("GetPartnerPointsByPartnerId/{partnerId}")]
        public IEnumerable<OptimusExpense.Model.DTOs.PartnerPointInfo> GetPartnerPointsByPartnerId(int partnerId)
        {
            var result = _partnerPointRepository.GetPartnerPointsByPartnerId(partnerId);
            return result;
        }

        [HttpPost("SavePartnerPoint")]
        public OptimusExpense.Model.DTOs.PartnerPointInfo SavePartnerPoint(OptimusExpense.Model.DTOs.PartnerPointInfo entity)
        {
            _partnerPointRepository.Save(entity.PartnerPoint);
            return entity;
        }


        [HttpPost("DeletePartnerPoint")]
        public OptimusExpense.Model.DTOs.PartnerPointInfo DeletePartnerPoint(OptimusExpense.Model.DTOs.PartnerPointInfo entity)
        {
            _partnerPointRepository.Remove(entity.PartnerPoint);
            return entity;
        }


        [HttpPost("SaveDictionaryDetail")]
        public OptimusExpense.Model.DTOs.DictionaryDetailInfo SaveDictionaryDetail(OptimusExpense.Model.DTOs.DictionaryDetailInfo entity)
        {

            _dictionaryDetailRepository.Save(entity.DictionaryDetail);
            return entity;
        }

        [HttpPost("DeleteDictionaryDetail")]
        public OptimusExpense.Model.Models.DictionaryDetail DeleteDictionaryDetail(OptimusExpense.Model.DTOs.DictionaryDetailInfo entity)
        {
            _dictionaryDetailRepository.Remove(entity.DictionaryDetail);
            return entity.DictionaryDetail;
        }

        [HttpGet("GetAllPartners")]
        public IEnumerable<OptimusExpense.Model.DTOs.PartnerInfo> GetAllPartners()
        {
            var result = _partnerRepository.GetAllPartners();
            return result;
        }

        [HttpGet("GetAllCompanies")]
        public IEnumerable<OptimusExpense.Model.DTOs.PartnerInfo> GetAllCompanies()
        {
            var tip = PartnerType.OwnCompany.GetHashCode();
            var result = _partnerRepository.GetAllPartners().Where(p=>p.Partner.PartnerTypeId == tip);
            return result;
        }

        [HttpGet("GetAllActivePartners")]
        public IEnumerable<OptimusExpense.Model.DTOs.PartnerInfo> GetAllActivePartners()
        {
            var companyId = _partnerRepository.GetCompanyByUserId(GetUserId()).Partner.PartnerId;
            var result = _partnerRepository.GetAllPartners().Where(p=>p.Partner.StatusId == -10 && p.Partner.CompanyId == companyId); //doar cei activi
            return result;
        }

        [HttpPost("SavePartner")]
        public OptimusExpense.Model.DTOs.PartnerInfo SavePartner(OptimusExpense.Model.DTOs.PartnerInfo entity)
        {
            _partnerRepository.Save(entity.Partner);
            return entity;
        }

        [HttpPost("DeletePartner")]
        public OptimusExpense.Model.DTOs.PartnerInfo DeletePartner(OptimusExpense.Model.DTOs.PartnerInfo entity)
        {
            _partnerPointRepository.DeleteBulk("PartnerPoint",new String[] { "PartnerId" }, new Object[]{ entity.Partner.PartnerId });
            _partnerRepository.Remove(entity.Partner);
            return entity;
        }

        [HttpGet("GetAllPersons")]
        public IEnumerable<OptimusExpense.Model.DTOs.PersonInfo> GetAllPersons()
        {
            var result = _personRepository.GetAllPersons();
            return result;
        }

        [HttpPost("SavePerson")]
        public OptimusExpense.Model.DTOs.PersonInfo SavePerson(OptimusExpense.Model.DTOs.PersonInfo entity)
        {

             _personRepository.Save(entity.Person);
            return entity;
        }

        [HttpPost("DeletePerson")]
        public OptimusExpense.Model.DTOs.PersonInfo DeletePerson(OptimusExpense.Model.DTOs.PersonInfo entity)
        {
            _personRepository.Remove(entity.Person);
            return entity;
        }

        [HttpGet("GetAllEmployees")]
        public IEnumerable<OptimusExpense.Model.DTOs.EmployeeInfo> GetAllEmployees()
        {
            var result = _employeeRepository.GetAllEmployees();
            return result;
        }

        [HttpPost("SaveEmployee")]
        public OptimusExpense.Model.DTOs.EmployeeInfo SaveEmployee(OptimusExpense.Model.DTOs.EmployeeInfo entity)
        {
            _employeeRepository.Save(entity.Employee);
            return entity;
        }

        [HttpPost("DeleteEmployee")]
        public OptimusExpense.Model.DTOs.EmployeeInfo DeleteEmployee(OptimusExpense.Model.DTOs.EmployeeInfo entity)
        {
            _employeeRepository.Remove(entity.Employee);
            return entity;
        }

        [HttpGet("GetAllCurrencies")]
        public IEnumerable<OptimusExpense.Model.Models.Currency> GetAllCurrencies()
        {
            var result = _currencyRepository.Get();
            return result;
        }

        [HttpGet("GetCompanyByUserId")]
        public OptimusExpense.Model.DTOs.PartnerInfo GetCompanyByUserId()
        {
            var userId = GetUserId();
            var result = _partnerRepository.GetCompanyByUserId(userId);
            return result;
        }
    }
}
