using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportOpenCNPJ
{
    public class CNAELine : ILineTyped
    {
        private string TableName { get; set; } = "";
        private List<string> _allCNAEs { get; set; } = new List<string>();
        public string CNPJ { get; set; } = "";
        public string CNAESecundaria {
            get { return String.Join("", _allCNAEs.ToArray()); }
            set {
                for(var i=1; i<100; i++)
                {
                    var cnae = value.Substring((i - 1) * 7, 7);
                    if (cnae != "0000000")
                        this._allCNAEs.Add(cnae);
                }
            }
        }

        public CNAELine()
        {
        }

        public CNAELine(string tableName, string line)
        {
            TableName = tableName;

            CNPJ = line.Substring(3,14);
            CNAESecundaria = line.Substring(17, 693);
        }

        public void UploadLine(NpgsqlConnection conn)
        {
            try
            {
                string insertQry = @"INSERT INTO " + TableName + @"
                                    (CNPJ, CNAESecundaria)
                                    VALUES (@CNPJ, @CNAE)";

                using (var cmd = new NpgsqlCommand(insertQry, conn))
                {
                    //cmd.Parameters.Add(new NpgsqlParameter("TableName", NpgsqlTypes.NpgsqlDbType.Text));
                    cmd.Parameters.Add(new NpgsqlParameter("CNPJ", NpgsqlTypes.NpgsqlDbType.Text));
                    cmd.Parameters.Add(new NpgsqlParameter("CNAE", NpgsqlTypes.NpgsqlDbType.Text));
                    //cmd.Parameters[0].Value = TableName;
                    cmd.Prepare();

                    foreach (var cnae in this._allCNAEs)
                    {
                        cmd.Parameters[0].Value = CNPJ;
                        cmd.Parameters[1].Value = cnae;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception except)
            {
                throw except;
            }
        }

    }
}
