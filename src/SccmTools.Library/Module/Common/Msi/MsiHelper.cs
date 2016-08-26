using Microsoft.Deployment.WindowsInstaller;

namespace SccmTools.Library.Common.Msi
{
    public class MsiHelper : IMsiHelper
    {
        public string GetProperty(Database database, string propertyName)
        {
            using (var view = database.OpenView(database.Tables["Property"].SqlSelectString + string.Format(" WHERE Property.Property='{0}'", propertyName)))
            {
                view.Execute();
                using(var record = view.Fetch())
                {
                    return record["Value"].ToString();
                }
            }
        }
    }
}