
using System.Collections.Generic;



namespace _3Capas

{

    public interface IContactService

    {

        IEnumerable<Contact> ListarContactos();

        void AddContact(Contact c);

        void UpdateContact(Contact c);

        void DeleteContact(int id);

    }

} 