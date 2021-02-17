using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportOpenCNPJ
{
    public class CompanyLine : ILineTyped
    {
        private string TableName { get; set; } = "";

        public string CNPJ { get; set; } = "";
        public string MatrizFilial { get; set; } = "";
        public string RazaoSocial { get; set; } = "";
        public string NomeFantasia { get; set; } = "";
        public string SituacaoCadastral { get; set; } = "";
        public string DataSituacao { get; set; } = "";
        public string MotivoSituacao { get; set; } = "";
        public string CidadeExterior { get; set; } = "";
        public string CodPais { get; set; } = "";
        public string NomePais { get; set; } = "";
        public string CodNatureza { get; set; } = "";
        public string DataInicial { get; set; } = "";
        public string CNAEFiscal { get; set; } = "";
        public string DescLogradouro { get; set; } = "";
        public string Logradouro { get; set; } = "";
        public string NumeroLogradouro { get; set; } = "";
        public string Complemento { get; set; } = "";
        public string Bairro { get; set; } = "";
        public string CEP { get; set; } = "";
        public string UF { get; set; } = "";
        public string CodMunicipio { get; set; } = "";
        public string Municipio { get; set; } = "";
        public string DDD1 { get; set; } = "";
        public string Telefone1 { get; set; } = "";
        public string DDD2 { get; set; } = "";
        public string Telefone2 { get; set; } = "";
        public string DDDFax { get; set; } = "";
        public string Fax { get; set; } = "";
        public string Email { get; set; } = "";
        public string QualificacaoResp { get; set; } = "";
        public double CapitalSocial { get; set; } = 0;
        public string Porte { get; set; } = "";
        public string Simples { get; set; } = "";
        public string DataOpcSimples { get; set; } = "";
        public string DataExclusaoSimples { get; set; } = "";
        public string OpcaoMEI { get; set; } = "";
        public string SituacaoEspecial { get; set; } = "";
        public string DataSituacaoEspecial { get; set; } = "";
        public CompanyLine()
        {
        }

        public CompanyLine(string tableName, string line)
        {
            TableName = tableName;

            CNPJ = line.Substring(3,14);
            MatrizFilial = line.Substring(17, 1);
            RazaoSocial = line.Substring(18, 150);
            NomeFantasia = line.Substring(168, 55);
            SituacaoCadastral = line.Substring(223, 2);
            DataSituacao = line.Substring(225, 8);
            MotivoSituacao = line.Substring(233, 2);
            CidadeExterior = line.Substring(235, 55);
            CodPais = line.Substring(290, 3);
            NomePais = line.Substring(293, 70);
            CodNatureza = line.Substring(363, 4);
            DataInicial = line.Substring(367, 8);
            CNAEFiscal = line.Substring(375, 7);
            DescLogradouro = line.Substring(382, 20);
            Logradouro = line.Substring(402, 60);
            NumeroLogradouro = line.Substring(462, 6);
            Complemento = line.Substring(468, 156);
            Bairro = line.Substring(624, 50);
            CEP = line.Substring(674, 8);
            UF = line.Substring(682, 2);
            CodMunicipio = line.Substring(684, 4);
            Municipio = line.Substring(688, 50);
            DDD1 = line.Substring(738, 4); 
            Telefone1 = line.Substring(742, 8);
            DDD2 = line.Substring(750, 4);
            Telefone2 = line.Substring(754, 8);
            DDDFax = line.Substring(762, 4);
            Fax = line.Substring(766, 8);
            Email = line.Substring(774, 115);
            QualificacaoResp = line.Substring(889, 2);
            CapitalSocial = Convert.ToDouble(line.Substring(891, 14));
            Porte = line.Substring(905, 2);
            Simples = line.Substring(907, 1);
            DataOpcSimples = line.Substring(908, 8);
            DataExclusaoSimples = line.Substring(916, 8);
            OpcaoMEI = line.Substring(924, 1);
            SituacaoEspecial = line.Substring(925, 23);
            DataSituacaoEspecial = line.Substring(948, 8);
        }

        public void UploadLine(NpgsqlConnection conn)
        {
            string insertQry = "INSERT INTO " + TableName + @" 
                                    (   CNPJ,MatrizFilial,RazaoSocial,NomeFantasia,SituacaoCadastral,DataSituacao,MotivoSituacao,CidadeExterior,
                                        CodPais,NomePais,CodNatureza,DataInicial,CNAEFiscal,DescLogradouro,Logradouro,NumeroLogradouro,Complemento,
                                        Bairro,CEP,UF,CodMunicipio,Municipio,DDD1,Telefone1,DDD2,Telefone2,DDDFax,Fax,Email,QualificacaoResp,CapitalSocial,
                                        Porte,Simples,DataOpcSimples,DataExclusaoSimples,OpcaoMEI,SituacaoEspecial,DataSituacaoEspecial)
                                    VALUES (@CNPJ,@MatrizFilial,@RazaoSocial,@NomeFantasia,@SituacaoCadastral,@DataSituacao,@MotivoSituacao,@CidadeExterior,
                                        @CodPais,@NomePais,@CodNatureza,@DataInicial,@CNAEFiscal,@DescLogradouro,@Logradouro,@NumeroLogradouro,@Complemento,
                                        @Bairro,@CEP,@UF,@CodMunicipio,@Municipio,@DDD1,@Telefone1,@DDD2,@Telefone2,@DDDFax,@Fax,@Email,@QualificacaoResp,@CapitalSocial,
                                        @Porte,@Simples,@DataOpcSimples,@DataExclusaoSimples,@OpcaoMEI,@SituacaoEspecial,@DataSituacaoEspecial)";
            try
            {
                using (var cmd = new NpgsqlCommand(insertQry, conn))
                {
                    //cmd.Parameters.AddWithValue("TableName", TableName);
                    cmd.Parameters.AddWithValue("CNPJ", CNPJ);
                    cmd.Parameters.AddWithValue("MatrizFilial", MatrizFilial);
                    cmd.Parameters.AddWithValue("RazaoSocial", RazaoSocial);
                    cmd.Parameters.AddWithValue("NomeFantasia", NomeFantasia);
                    cmd.Parameters.AddWithValue("SituacaoCadastral", SituacaoCadastral);
                    cmd.Parameters.AddWithValue("DataSituacao", DataSituacao);
                    cmd.Parameters.AddWithValue("MotivoSituacao", MotivoSituacao);
                    cmd.Parameters.AddWithValue("CidadeExterior", CidadeExterior);
                    cmd.Parameters.AddWithValue("CodPais", CodPais);
                    cmd.Parameters.AddWithValue("NomePais", NomePais);
                    cmd.Parameters.AddWithValue("CodNatureza", CodNatureza);
                    cmd.Parameters.AddWithValue("DataInicial", DataInicial);
                    cmd.Parameters.AddWithValue("CNAEFiscal", CNAEFiscal);
                    cmd.Parameters.AddWithValue("DescLogradouro", DescLogradouro);
                    cmd.Parameters.AddWithValue("Logradouro", Logradouro);
                    cmd.Parameters.AddWithValue("NumeroLogradouro", NumeroLogradouro);
                    cmd.Parameters.AddWithValue("Complemento", Complemento);
                    cmd.Parameters.AddWithValue("Bairro", Bairro);
                    cmd.Parameters.AddWithValue("CEP", CEP);
                    cmd.Parameters.AddWithValue("UF", UF);
                    cmd.Parameters.AddWithValue("CodMunicipio", CodMunicipio);
                    cmd.Parameters.AddWithValue("Municipio", Municipio);
                    cmd.Parameters.AddWithValue("DDD1", DDD1);
                    cmd.Parameters.AddWithValue("Telefone1", Telefone1);
                    cmd.Parameters.AddWithValue("DDD2", DDD2);
                    cmd.Parameters.AddWithValue("Telefone2", Telefone2);
                    cmd.Parameters.AddWithValue("DDDFax", DDDFax);
                    cmd.Parameters.AddWithValue("Fax", Fax);
                    cmd.Parameters.AddWithValue("Email", Email);
                    cmd.Parameters.AddWithValue("QualificacaoResp", QualificacaoResp);
                    cmd.Parameters.AddWithValue("CapitalSocial", CapitalSocial);
                    cmd.Parameters.AddWithValue("Porte", Porte);
                    cmd.Parameters.AddWithValue("Simples", Simples);
                    cmd.Parameters.AddWithValue("DataOpcSimples", DataOpcSimples);
                    cmd.Parameters.AddWithValue("DataExclusaoSimples", DataExclusaoSimples);
                    cmd.Parameters.AddWithValue("OpcaoMEI", OpcaoMEI);
                    cmd.Parameters.AddWithValue("SituacaoEspecial", SituacaoEspecial);
                    cmd.Parameters.AddWithValue("DataSituacaoEspecial", DataSituacaoEspecial);
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
