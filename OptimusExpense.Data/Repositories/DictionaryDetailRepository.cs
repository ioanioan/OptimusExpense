using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace OptimusExpense.Data.Repositories
{
    public class DictionaryDetailRepository :  EntityBaseRepository<DictionaryDetail>, IDictionaryDetailRepository
    {

        OptimusExpenseContext _context;
        public DictionaryDetailRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }

        public List<DictionaryDetailInfo> GetDictionaryDetail()
        {
            var result = (from dd in _context.DictionaryDetail
                          join d in _context.Dictionary on dd.DictionaryId equals d.DictionaryId
                          select new Model.DTOs.DictionaryDetailInfo
                          {
                              DictionaryDetail = dd,
                              NumeDictionar = d.Name
                          }).OrderBy(p=>p.DictionaryDetail.Name).ToList();
            return result;
        }
        public IQueryable<Dictionary> GetDictionary()
        {
            var result = (from d in _context.Dictionary
                          select d).OrderBy(p => p.Name);
            return result;
        }

        public IQueryable<DictionaryDetail> GetDictionaryDetailByDictionaryId(int dictionaryId)
        {
            var result = (from dd in _context.DictionaryDetail
                          where dd.DictionaryId == dictionaryId && dd.Active
                          select dd).OrderBy(p => p.DisplayOrder).ThenBy(p=>p.Name);
            return result;
        }


        public override DictionaryDetail Save(DictionaryDetail entity)
        {

            if (entity.DictionaryDetailId == 0)
            {
                var nextId = 1;
                try
                {
                    nextId = _context.DictionaryDetail.Max(p => p.DictionaryDetailId)+1;
                }
                catch
                {

                }
                if (nextId <= 0)
                {
                    nextId = 1;
                }
                entity.DictionaryDetailId = nextId;
            }
            return base.Save(entity);
        }

    }
}
