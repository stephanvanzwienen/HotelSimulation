using System;
using System.Collections.Generic;
using HotelEvents;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace Hotel
{

    public class PersonFactory : AbstractFactory
    {

        public PersonFactory()
        {

        }
        /// <summary>
        /// For further implementation.
        /// </summary>
        public void GenerateEntity()
        {

        }
        /// <summary>
        /// Makes a person and based on the variables it will be either a maid or a customer.
        /// </summary>
        /// <param name="product">What person it's gonna be.</param>
        /// <param name="tempPerson">The temperory variables for the person that is about to be created.</param>
        /// <returns>Either a maid or a customer.</returns>
        public IPerson GetPerson(string product, TempPerson tempPerson)
        {
            Type type = Type.GetType(product);
            if (type != null)
            {

                return (IPerson)Activator.CreateInstance(type);
            }
            else
            {
                string nspace = "Hotel";

                //qeury for all the types
                var q = from x in Assembly.GetExecutingAssembly().GetTypes()
                        where x.IsClass && x.Namespace == nspace
                        select x;

                List<string> types = new List<string>();
                //put the query in the list
                foreach (Type t in q)
                {
                    types.Add(t.ToString());
                }
                //search the list and if found return instance. 
                foreach (string t in types)
                {
                    if (t.Contains(product))
                    {
                        type = Type.GetType(t);
                        return (IPerson)Activator.CreateInstance(type, tempPerson);
                    }
                }

            }
            return null;
        }

    }
}
