using System.Web;
using System.Web.Mvc;

namespace WebApp_B2C_DotNet
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
