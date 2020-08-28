using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediinfo.Infrastructure.Core.DBEntity
{
    public class DBEntityPropertys
    {
        static Dictionary<Type, List<DBEntityKey>> Items = new Dictionary<Type, List<DBEntityKey>>();
        public static List<DBEntityKey> GetKeys(Type DBEntityType)
        {
            if (!Items.ContainsKey(DBEntityType))
            {
                Items.Add(DBEntityType, new List<DBEntityKey>());
                DBEntityType.GetProperties().ToList().ForEach(o =>
                {
                    var attrs = o.GetCustomAttributes(true).ToList();
                    foreach (var attr in attrs)
                    {
                        if (attr is KeyAttribute)
                        {
                            Items[DBEntityType].Add(new DBEntityKey() { Name = o.Name, GetValue = o.GetValue });
                        }
                    }
                });
            }
            return Items[DBEntityType];
        }
    }

    public class DBEntityKey
    {
        public string Name { get; set; }
        public Func<object, object> GetValue { get; set; }
    }
}
