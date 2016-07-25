using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainShell.Tests.Domain.Models;
using DomainShell.Tests.Domain.Repositories.Read;
using System.Data;
using System.IO;
using System.Text;

namespace DomainShell.Tests.Domain.Service
{
    public class PersonReader
    {
        private PersonReadRepository _repository = new PersonReadRepository();

        public PersonModel Get(string id)
        {
            return _repository.Get(id);
        }

        public PersonModel[] GetAll()
        {
            return _repository.GetAll();
        }

        public void OutputTsv(MemoryStream stream)
        {
            DataTable table = new DataTable();

            _repository.LoadAll(table);

            using (StreamWriter writer = new StreamWriter(stream))
            {
                StringBuilder line = new StringBuilder();

                foreach (DataColumn column in table.Columns)
                {
                    if (!string.IsNullOrEmpty(line.ToString()))
                    {
                        line.Append("\t");
                    }                    

                    line.Append(column.ColumnName);
                }

                writer.WriteLine(line);

                foreach (DataRow row in table.Rows)
                {
                    line = new StringBuilder();                    

                    foreach (object item in row.ItemArray)
                    {
                        if (!string.IsNullOrEmpty(line.ToString()))
                        {
                            line.Append("\t");
                        }

                        line.Append(item.ToString());
                    }

                    writer.WriteLine(line);
                }
            }
        }
    }
}