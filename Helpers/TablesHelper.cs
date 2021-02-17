using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportOpenCNPJ.Helpers
{
    public static class TablesHelper
    {
        public static void CreateCompaniesTable(NpgsqlConnection conn, string tableName)
        {
            string createSQL = "CREATE TABLE " + tableName + @" (
                Id serial PRIMARY KEY,
                CNPJ VARCHAR(14) NOT NULL,
                MatrizFilial VARCHAR(14),
                RazaoSocial VARCHAR(150),
                NomeFantasia VARCHAR(55),
                SituacaoCadastral VARCHAR(2),
                DataSituacao VARCHAR(8),
                MotivoSituacao VARCHAR(2),
                CidadeExterior VARCHAR(55),
                CodPais VARCHAR(3),
                NomePais VARCHAR(70),
                CodNatureza VARCHAR(4),
                DataInicial VARCHAR(8),
                CNAEFiscal VARCHAR(7),
                DescLogradouro VARCHAR(20),
                Logradouro VARCHAR(60),
                NumeroLogradouro VARCHAR(6),
                Complemento VARCHAR(156),
                Bairro VARCHAR(50),
                CEP VARCHAR(8),
                UF VARCHAR(2),
                CodMunicipio  VARCHAR(4),
                Municipio VARCHAR(50),
                DDD1 VARCHAR(4),
                Telefone1 VARCHAR(8),
                DDD2 VARCHAR(4),
                Telefone2 VARCHAR(8),
                DDDFax VARCHAR(4),
                Fax VARCHAR(8),
                Email VARCHAR(115),
                QualificacaoResp  VARCHAR(2),
                CapitalSocial NUMERIC,
                Porte VARCHAR(2),
                Simples VARCHAR(1),
                DataOpcSimples VARCHAR(8),
                DataExclusaoSimples VARCHAR(8),
                OpcaoMEI VARCHAR(1),
                SituacaoEspecial VARCHAR(23),
                DataSituacaoEspecial VARCHAR(8))";

            try
            {
                using (var cmd = new NpgsqlCommand(createSQL, conn))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    //cmd.Parameters.AddWithValue("@TableName", tableName);
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception except)
            {
                throw except;
            }
        }

        public static void CreatePartnersTable(NpgsqlConnection conn, string tableName)
        {
            string createSQL = @"CREATE TABLE " + tableName + @" (
                Id serial PRIMARY KEY,
                CNPJEmpresa VARCHAR (14) NOT NULL,
                PFPJ VARCHAR (1),
                NomeRazaoSocio VARCHAR (150),
                CNPJCPF VARCHAR (14),
                CodQualificacao VARCHAR (2),
                PercentCapital NUMERIC,
                DataEntradaSociedade VARCHAR (8),
                CodPais VARCHAR (3),
                NomePais VARCHAR (70),
                CPFRepLegal VARCHAR (11),
                NomeRespresentante VARCHAR (60),
                CodQualificacaoRep VARCHAR (2))";

            try
            {
                using (var cmd = new NpgsqlCommand(createSQL, conn))
                {
                    //cmd.Parameters.AddWithValue("TableName", tableName);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception except)
            {
                throw except;
            }

        }

        public static void CreateCNAEsTable(NpgsqlConnection conn, string tableName)
        {
            string createSQL = @"CREATE TABLE " + tableName + @" (
                Id serial PRIMARY KEY,
                CNPJ VARCHAR (14) NOT NULL,
                CNAESecundaria VARCHAR (7))";

            try
            {
                using (var cmd = new NpgsqlCommand(createSQL, conn))
                {
                    //cmd.Parameters.AddWithValue("TableName", tableName);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception except)
            {
                throw except;
            }
        }

        public static void ClearTable(NpgsqlConnection conn, string tableName)
        {
            string createSQL = @"TRUNCATE TABLE " + tableName;

            try
            {
                using (var cmd = new NpgsqlCommand(createSQL, conn))
                {
                    //cmd.Parameters.AddWithValue("TableName", tableName);
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
