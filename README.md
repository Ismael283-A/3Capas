# 3 Capas
# 1. ContactsApp 

├── ContactsApp.Data       // Capa de Acceso a Datos 

├── ContactsApp.Business   // Capa de Lógica de Negocio 

└── ContactsApp.UI         // Capa de Presentación (WinForms o WPF) 

Cada proyecto compila un ensamblado independiente y sólo expone las interfaces públicas necesarias. 
 # 2. Capa de Entidades (puede vivir en Data o en un proyecto aparte)  

    namespace ContactsApp.Data.Entities 

    { 

    public class Contact 

    { 

        public int    Id        { get; set; } 

        public string Name      { get; set; } 

        public string Email     { get; set; } 

        public string Phone     { get; set; } 

    } 

    }  
# 3. Capa de Acceso a Datos 

Define una interfaz y una implementación (aquí con lista en memoria; podrías reemplazarla por EF Core, Dapper, etc.).   
# 4. Capa de Lógica de Negocio 

Encapsula reglas, validaciones y orquesta llamadas a repositorios.  

    using System; 

    using System.Collections.Generic; 

        using ContactsApp.Data; 

      using ContactsApp.Data.Entities; 

  

      namespace ContactsApp.Business 

    { 

    public interface  

    { 

        IEnumerable<Contact> ListarContactos(); 

        void AddContact(Contact c); 

        void UpdateContact(Contact c); 

        void DeleteContact(int id); 

    } 

  

    public class ContactService : IContactService 

    { 

        private readonly IContactRepository _repo; 

  

        public ContactService(IContactRepository repo) 

        { 

            _repo = repo; 

        } 

  

        public IEnumerable<Contact> ListarContactos() 

        { 

            // podrías aplicar paginación, filtros, ordenamientos... 

            return _repo.GetAll(); 

        } 

  

        public void AddContact(Contact c) 

        { 

            if (string.IsNullOrWhiteSpace(c.Name)) 

                throw new ArgumentException("El nombre es obligatorio"); 

            _repo.Add(c); 

        } 

  

        public void UpdateContact(Contact c) 

        { 

            if (c.Id <= 0) 

                throw new ArgumentException("Id no válido"); 

            _repo.Update(c); 

        } 

  

        public void DeleteContact(int id) 

        { 

            _repo.Delete(id); 

        } 

    } 

     } 
# 5. Capa de Presentación (WinForms)   
Un form simple con un DataGridView, y botones Agregar, Editar, Eliminar.   
          
      using System; 

      using System.Linq; 

      using System.Windows.Forms; 

      using ContactsApp.Business; 

      using ContactsApp.Data; 

      using ContactsApp.Data.Entities; 

  

    namespace ContactsApp.UI 

      { 

    public partial class MainForm : Form 

    { 

        private readonly IContactService _service; 

  

        public MainForm() 

        { 

            InitializeComponent(); 

  

            // Inyección manual de dependencias 

            var repo    = new InMemoryContactRepository(); 

            _service    = new ContactService(repo); 

  

            CargarGrilla(); 

        } 

  

        private void CargarGrilla() 

        { 

            var datos = _service.ListarContactos() 

                                .Select(c => new { 

                                    c.Id, 

                                    c.Name, 

                                    c.Email, 

                                    c.Phone 

                                }) 

                                .ToList(); 

            dataGridView1.DataSource = datos; 

        } 

  

        private void btnAdd_Click(object sender, EventArgs e) 

        { 

            var nuevo = new Contact { 

                Name  = txtName.Text, 

                Email = txtEmail.Text, 

                Phone = txtPhone.Text 

            }; 

            try 

            { 

                _service.AddContact(nuevo); 

                CargarGrilla(); 

            } 

            catch (Exception ex) 

            { 

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); 

            } 

        } 

  

        private void btnDelete_Click(object sender, EventArgs e) 

        { 

            if (dataGridView1.CurrentRow == null) return; 

            int id = (int)dataGridView1.CurrentRow.Cells["Id"].Value; 

            _service.DeleteContact(id); 

            CargarGrilla(); 

        } 

  

        private void btnEdit_Click(object sender, EventArgs e) 

        { 

            if (dataGridView1.CurrentRow == null) return; 

            var c = new Contact { 

                Id    = (int)dataGridView1.CurrentRow.Cells["Id"].Value, 

                Name  = txtName.Text, 

                Email = txtEmail.Text, 

                Phone = txtPhone.Text 

            }; 

            _service.UpdateContact(c); 

            CargarGrilla(); 

        } 

    } 

      } 

 Este patrón de separación te ayuda a: 

* Mantener y probar cada capa de forma aislada. 

* Reemplazar fácilmente la implementación de acceso a datos (por ejemplo, con EF Core y SQL Server). 

* Aplicar validaciones y reglas de negocio en un solo lugar. 

Así obtienes un código más limpio, modular y escalable 
 
   
