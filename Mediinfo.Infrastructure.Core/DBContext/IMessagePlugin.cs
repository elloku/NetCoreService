using System.Web.Http;

namespace Mediinfo.Infrastructure.Core.DBContext
{
    public interface IMessagePlugin
    {
        ApiController apiController { get; set; }

        int ApplyChange();

        void Handler(DBContextBase dbContext);
    }
}
