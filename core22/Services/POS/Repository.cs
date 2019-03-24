using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core22.Services.POS
{
    public class Repository : IRepository
    {
        public string GeneratePONumber()
        {
            string result = "";

            try
            {
                result = this.GenereateNumber("PO");
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public string GenerateSONumber()
        {
            string result = "";

            try
            {
                result = this.GenereateNumber("SO");
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public string GenerateGRNumber()
        {
            string result = "";

            try
            {
                result = this.GenereateNumber("GR");
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public string GenerateInvenTranNumber()
        {
            string result = "";

            try
            {
                result = this.GenereateNumber("TRN");
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        private string GenereateNumber(string module)
        {
            string result = "";

            try
            {
                result = Guid.NewGuid().ToString().Substring(0, 7).ToUpper() + "-" + DateTime.Now.ToString("yyyyMMdd") + "#" + module;
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }
    }
}
