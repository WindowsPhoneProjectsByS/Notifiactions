using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Powiadomienia
{
    static class IdService
    {
        private static List<int> IdConteiner = new List<int>();
        private static Random random = new Random();

        public static String GetNewId()
        {
            int newId = 0;
            do
            {
                newId = random.Next(0, 200);
                Debug.WriteLine(newId);
            } while (!IdConteiner.Contains(newId) && IdConteiner.Count > 0);

            IdConteiner.Add(newId);

            return newId.ToString();
        }

        public static void Remove(string id)
        {
            int intId = Int32.Parse(id);
            IdConteiner.Remove(intId);
        }
    }
}
