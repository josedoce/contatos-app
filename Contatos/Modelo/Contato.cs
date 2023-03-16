namespace Contatos.Modelo
{
    public class Contato
    {
        private string _id;
        private string _name;
        private string _phone;

        public Contato()
        {
        }

        public Contato(string id, string name, string phone) 
        {
            _id = id;
            _name = name;
            _phone = phone;
        }

        public string Id { get { return _id;} set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public string Phone { get { return _phone; } set { _phone = value; } }
    }
}
