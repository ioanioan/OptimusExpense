using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OptimusExpense.Infrastucture.Extensions;
using System.Data;

namespace OptimusExpense.Data.Repositories
{
    public class ReportRepository : EntityBaseRepository<Model.Models.Report>, IReportRepository
    {

        OptimusExpenseContext _context;
        public ReportRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }


        public IEnumerable<ReportDetailInfo> GetReportDetails(Model.Models.Report report)
        {
            var result = (from r in _context.ReportDetail
                          from rc in _context.ReportDetailCombo.Where(p => p.ReportDetailId == r.ReportDetailId).DefaultIfEmpty()
                          where r.ReportId == report.ReportId

                          select new ReportDetailInfo
                          {
                              ReportDetail = r,
                              ReportDetailCombo = rc
                          }).ToList();
            foreach (var r in result)
            {
                if (r.ReportDetailCombo != null)
                {
                    var dt = _context.Database.ExecuteSqlDataTable(r.ReportDetailCombo.Querry).Tables[0];
                    r.ReportDetailComboResult = dt.AsEnumerable().Select(p => new { ValueMember = p[r.ReportDetailCombo.ValueMember], DisplayMember = p[r.ReportDetailCombo.DisplayMember] }).ToList();
                }
            }
            return result;
        }

        public List<Dictionary<String,Object>> RunReport(String userId, ReportInfo report)
        {
            var list = new List<Microsoft.Data.SqlClient.SqlParameter>();
            list.Add(new Microsoft.Data.SqlClient.SqlParameter("@UserId", userId));
            if (report.Parameters != null)
            {
                foreach (var r in report.Parameters)
                {
                    Object v = "" + r.Value;
                    DateTime d = DateTime.Now;
                    if (DateTime.TryParse("" + v, out d))
                    {
                        v = d;
                    }
                    list.Add(new Microsoft.Data.SqlClient.SqlParameter("@" + r.Key, v));
                }
            }
            var query = report.Report.StoredProcedureName + " " + (String.Join(",", list.Select(p => p.ParameterName)));
            var result = _context.Database.ExecuteSqlDataTable(query, list.ToArray()).Tables[0].AsEnumerable().Select(p => p.Table.Columns.Cast<DataColumn>().ToDictionary(c => c.ColumnName, c => p[c]==DBNull.Value?null:p[c])).ToList();
            return result;
        }
        
    }
}
