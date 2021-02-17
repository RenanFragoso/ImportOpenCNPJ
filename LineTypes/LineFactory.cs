using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportOpenCNPJ
{
    public static class LineFactory
    {
        public static ILineTyped ExtractLine(string line, Dictionary<string,string> tableNames)
        {
            try
            {
                int typeNumber = 0;
                string tableName = "";
                if (int.TryParse(line.Substring(0, 1), out typeNumber))
                {
                    switch(typeNumber)
                    {
                        case 1:
                            tableNames.TryGetValue("Empresa", out tableName);
                            return new CompanyLine(tableName, line);

                        case 2:
                            tableNames.TryGetValue("Socio", out tableName);
                            return new PartnerLine(tableName, line);

                        case 6:
                            tableNames.TryGetValue("CNAE", out tableName);
                            return new CNAELine(tableName, line);
                    }
                }

                return null;
            }
            catch (Exception except)
            {
                return null;
            }
        }
    }
}
