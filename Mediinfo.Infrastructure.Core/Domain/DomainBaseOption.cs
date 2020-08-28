using Mediinfo.Enterprise;
using Mediinfo.Enterprise.Exceptions;
using Mediinfo.Infrastructure.Core.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediinfo.Infrastructure.Core.Domain
{
    class GY_XuHao
    {
        public string XULIEMC { get; set; }
        public int DANGQIANZHI { get; set; }
        public int? CHANGDU { get; set; }
        public string ZUHEFS { get; set; }
    }
    public class DomainBaseOption : IDomainBaseOption
    {
        public DBContextBase DBContext { get; set; }
        public ServiceContext ServiceContext { get; set; }
        public DomainBaseOption(DBContextBase dbContext = null, ServiceContext serviceContext = null)
        {
            DBContext = dbContext;
            ServiceContext = serviceContext;
        }

        public DateTime? GetSYSDate()
        {
            return DBContext.Database.SqlQuery<DateTime>("select sysdate from dual").FirstOrDefault().Date;
        }
        public DateTime? GetSYSTime()
        {
            return DBContext.Database.SqlQuery<DateTime>("select sysdate from dual").FirstOrDefault();
        }
        public List<string> GetOrder(string XuHaoMC, string QianZhui = null, int Count = 1)
        {
            if (QianZhui == null)
            {
                QianZhui = string.Empty;
            }
            if (String.IsNullOrWhiteSpace(XuHaoMC))
            {
                throw new DomainException("序号名称不能为空");
            }
            bool isYaoFang = false;
            int val = 0;
            if (XuHaoMC.Length > 3 && int.TryParse(XuHaoMC.Substring(1, 2), out val))
            {
                isYaoFang = true;
            }
            List<string> ids = new List<string>();
            GY_XuHao XuHao = null;

            XuHao = DBContext.Database.SqlQuery<GY_XuHao>("select XULIEMC, DANGQIANZHI,ZUHEFS,CHANGDU from GY_XUHAO where XUHAOMC = :Name for update wait 1", XuHaoMC).FirstOrDefault();
            if (XuHao == null)
            {
                if (isYaoFang)
                {
                    var success = DBContext.Database.ExecuteSqlCommand("insert into GY_XUHAO (XUHAOMC, DANGQIANZHI, ZUIXIAOZHI, ZUIDAZHI, CHANGDU, ZUHEFS) values (:Name ,:ZUIXIAOZHI ,1 ,999999 ,12 ,'1')", XuHaoMC, Count);
                    if (success < 0)
                    {
                        throw new DBException("序号表新增失败");
                    }
                    else
                    {
                        XuHao = new GY_XuHao()
                        {
                            DANGQIANZHI = Count,
                            CHANGDU = 12,
                            ZUHEFS = "1"
                        };
                        for (int i = 1; i <= Count; i++)
                        {
                            ids.Add(i.ToString());
                        }
                    }
                }
                else
                {
                    throw new DomainException("序号名称在序号表中不存在");
                }
            }
            else
            {
                if (isYaoFang)
                {
                    var success = DBContext.Database.ExecuteSqlCommand("update GY_XUHAO set DANGQIANZHI = :DANGQIANZHI where XUHAOMC = :Name", XuHao.DANGQIANZHI + Count, XuHaoMC);
                    if (success < 0)
                    {
                        throw new DBException("序号表更新失败");
                    }
                    else
                    {
                        for (int i = XuHao.DANGQIANZHI + 1; i <= XuHao.DANGQIANZHI + Count; i++)
                        {
                            ids.Add(i.ToString());
                        }
                    }
                }
                else
                {
                    //   ids.Add(.FirstOrDefault().ToString());
                    var xuhao =  DBContext.Database.SqlQuery<Decimal>(string.Format(sqlSEQ, XuHao.XULIEMC, Count.ToString())).ToList();
                    ids.AddRange(xuhao.Select(o => o.ToString()));
                    //for (int i = 0; i < Count; i++)
                    //{
                    //  ids.Add(DBContext.Database.SqlQuery<Decimal>(string.Format(sqlSEQ, XuHao.XULIEMC)).FirstOrDefault().ToString());
                    //}
                }
            }
            List<string> IDS = new List<string>();
            if ((XuHao.ZUHEFS == null || XuHao.ZUHEFS == "0") && !XuHao.CHANGDU.HasValue)
            {
                return ids;
            }
            else if ((XuHao.ZUHEFS == null || XuHao.ZUHEFS == "0") && XuHao.CHANGDU.HasValue)
            {
                ids.ForEach(o =>
                {
                    if (o.Length <= XuHao.CHANGDU)
                    {
                        IDS.Add(o.PadLeft(XuHao.CHANGDU.Value, '0'));
                    }
                });
                return IDS;
            }
            else if (XuHao.ZUHEFS != null && XuHao.ZUHEFS != "0" && (!XuHao.CHANGDU.HasValue || XuHao.CHANGDU.Value == 0))
            {
                ids.ForEach(o =>
                {
                    IDS.Add(QianZhui + o);
                });
                return IDS;
            }
            else if (XuHao.ZUHEFS != null && XuHao.ZUHEFS != "0" && XuHao.CHANGDU.HasValue)
            {
                ids.ForEach(o =>
                {
                    if ((o.Length + QianZhui.Length) > XuHao.CHANGDU)
                    {
                        throw new DomainException("序号超出定义的长度");
                    }
                    else if ((o.Length + QianZhui.Length) == XuHao.CHANGDU)
                    {
                        IDS.Add(QianZhui + o);
                    }
                    else
                    {
                        IDS.Add(QianZhui + o.PadLeft(XuHao.CHANGDU.Value - QianZhui.Length, '0'));
                    }
                });
                return IDS;
            }
            return ids;
        }
        const string sqlSEQ = "Select {0}.nextval from dual  connect by rownum< {1}";
    }
}
