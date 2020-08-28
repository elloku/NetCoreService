using Mediinfo.Infrastructure.Core.DBContext;

namespace Mediinfo.Infrastructure.Core.DbMessage
{
    public interface IDbMessageHandler
    {
        void Handler(DBContextBase haloDbContext, DBContextBase pgDbContext, ServiceInfo serviceInfo, DbMessage dbMessage);
    }
}
