using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportOpenCNPJ
{
    public class PartnerLine : ILineTyped
    {
        private string TableName { get; set; } = "";
        public string CNPJEmpresa { get; set; } = "";
        public string PFPJ { get; set; } = "";
        public string NomeRazaoSocio { get; set; } = "";
        public string CNPJCPF { get; set; } = "";
        public string CodQualificacao { get; set; } = "";
        public double PercentCapital { get; set; } = 0;
        public string DataEntradaSociedade { get; set; } = "";
        public string CodPais { get; set; } = "";
        public string NomePais { get; set; } = "";
        public string CPFRepLegal { get; set; } = "";
        public string NomeRespresentante { get; set; } = "";
        public string CodQualificacaoRep { get; set; } = "";


        public PartnerLine()
        {
        }

        public PartnerLine(string tableName, string line)
        {
            TableName = tableName;

            CNPJEmpresa = line.Substring(3, 14);
            PFPJ = line.Substring(17, 1);
            NomeRazaoSocio = line.Substring(18, 150);
            CNPJCPF = line.Substring(168, 14);
            CodQualificacao = line.Substring(182, 2);
            PercentCapital = Convert.ToDouble(line.Substring(184, 5));
            DataEntradaSociedade = line.Substring(189, 8);
            CodPais = line.Substring(197, 3);
            NomePais = line.Substring(200, 70);
            CPFRepLegal = line.Substring(270, 11);
            NomeRespresentante = line.Substring(281, 60);
            CodQualificacaoRep = line.Substring(341, 2);
        }

        public void UploadLine(NpgsqlConnection conn)
        {
            string insertQry = @"INSERT INTO " + TableName + @" 
                                    (CNPJEmpresa,PFPJ,NomeRazaoSocio,CNPJCPF,CodQualificacao,PercentCapital,DataEntradaSociedade,CodPais,NomePais,
                                        CPFRepLegal,NomeRespresentante,CodQualificacaoRep)
                                    VALUES (@CNPJEmpresa,@PFPJ,@NomeRazaoSocio,@CNPJCPF,@CodQualificacao,@PercentCapital,@DataEntradaSociedade,@CodPais,@NomePais,
                                            @CPFRepLegal,@NomeRespresentante,@CodQualificacaoRep)";
            try
            {
                using (var cmd = new NpgsqlCommand(insertQry, conn))
                {
                    //cmd.Parameters.AddWithValue("TableName", TableName);
                    cmd.Parameters.AddWithValue("CNPJEmpresa", CNPJEmpresa);
                    cmd.Parameters.AddWithValue("PFPJ", PFPJ);
                    cmd.Parameters.AddWithValue("NomeRazaoSocio", NomeRazaoSocio);
                    cmd.Parameters.AddWithValue("CNPJCPF", CNPJCPF);
                    cmd.Parameters.AddWithValue("CodQualificacao", CodQualificacao);
                    cmd.Parameters.AddWithValue("PercentCapital", PercentCapital);
                    cmd.Parameters.AddWithValue("DataEntradaSociedade", DataEntradaSociedade);
                    cmd.Parameters.AddWithValue("CodPais", CodPais);
                    cmd.Parameters.AddWithValue("NomePais", NomePais);
                    cmd.Parameters.AddWithValue("CPFRepLegal", CPFRepLegal);
                    cmd.Parameters.AddWithValue("NomeRespresentante", NomeRespresentante);
                    cmd.Parameters.AddWithValue("CodQualificacaoRep", CodQualificacaoRep);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception except)
            {
                throw except;
            }
        }

    }
}
