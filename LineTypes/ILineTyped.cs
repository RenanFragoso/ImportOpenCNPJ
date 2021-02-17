using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportOpenCNPJ
{
    public interface ILineTyped
    {
        void UploadLine(NpgsqlConnection conn);
    }
}
