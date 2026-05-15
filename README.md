# Guia Completo - Migrações, Models, Controllers e Views

## Índice
1. [Migrações](#migrações)
2. [Models](#models)
3. [Controllers](#controllers)
4. [Views](#views)
5. [Exemplo Prático Completo](#exemplo-prático-completo)

---

## Migrações

As migrações permitem versionar e controlar as alterações da base de dados de forma segura.

### O que é uma Migração?
Uma migração é um ficheiro que descreve as alterações na estrutura da base de dados (criar tabelas, adicionar colunas, etc.).

### Comandos Principais

#### 1. **Criar uma nova migração**
```bash
dotnet ef migrations add NomeMigracao
```
**Exemplo:**
```bash
dotnet ef migrations add AddDepartmentTable
```
Este comando cria dois ficheiros na pasta `Data/Migrations/`:
- `[timestamp]_AddDepartmentTable.cs` - O ficheiro da migração
- `[timestamp]_AddDepartmentTable.Designer.cs` - Ficheiro gerado automaticamente

#### 2. **Atualizar a base de dados com a migração**
```bash
dotnet ef database update
```
Este comando executa todas as migrações pendentes na base de dados.

#### 3. **Desfazer a última migração (Remover da BD)**
```bash
dotnet ef database update NomeMigracaoAnterior
```
**Exemplo:**
```bash
dotnet ef database update InitialCreate
```

#### 4. **Ver estatuto das migrações**
```bash
dotnet ef migrations list
```

#### 5. **Remover a última migração criada** (antes de aplicar à BD)
```bash
dotnet ef migrations remove
```
⚠️ **Nota:** Só funciona se ainda não foi aplicada à base de dados.

### Ficheiros de Migração - Estrutura

```csharp
public partial class AddDepartmentTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Código para CRIAR/ADICIONAR alterações
        migrationBuilder.CreateTable(
            name: "Departments",
            columns: table => new
            {
                DepartmentId = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Departments", x => x.DepartmentId);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Código para DESFAZER a migração
        migrationBuilder.DropTable(name: "Departments");
    }
}
```

### Boas Práticas para Migrações
- ✅ Criar uma migração para cada alteração lógica
- ✅ Dar nomes descritivos (Add, Remove, Update + o que mudou)
- ✅ Testar antes de fazer `update`
- ✅ Fazer commit das migrações junto com o código
- ❌ Não editar migrações já aplicadas
- ❌ Não partilhar checkpoints de rebase com migrações não aplicadas

---

## Models

Os Models representam as tabelas e estrutura da base de dados.

### Localização
```
14548_employes_managment/
└── Models/
    ├── Employee.cs
    ├── LeaveApplication.cs
    └── [seus novos models vão aqui]
```

### Estrutura Básica de um Model

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace seu_namespace.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Foreign Key
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
```

### Anotações Importantes

| Anotação | Função |
|----------|--------|
| `[Key]` | Define a chave primária |
| `[Required]` | Campo obrigatório |
| `[StringLength(100)]` | Limita o comprimento |
| `[ForeignKey("DepartmentId")]` | Define relação com outra tabela |
| `[Table("TableName")]` | Especifica o nome da tabela |
| `[NotMapped]` | Campo não é mapeado para a BD |

### Registar Model no DbContext

Arquivo: `14548_employes_managment/Data/ApplicationDbContext.cs`

```csharp
public DbSet<Department> Departments { get; set; }
```

---

## Controllers

Os Controllers contêm a lógica de negócio e processam pedidos HTTP.

### Localização
```
14548_employes_managment/
└── Controllers/
    ├── EmployeesController.cs
    ├── LeaveApplicationsController.cs
    └── [seus novos controllers vão aqui]
```

### Estrutura Básica

```csharp
using Microsoft.AspNetCore.Mvc;
using seu_namespace.Data;
using seu_namespace.Models;
using Microsoft.EntityFrameworkCore;

namespace seu_namespace.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Dependência injetada
        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments.ToListAsync();
            return View(departments);
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentId,Name,Description")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentId,Name,Description")] Department department)
        {
            if (id != department.DepartmentId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.DepartmentId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentId == id);
        }
    }
}
```

### Métodos CRUD Explicados

| Método | HTTP | Função |
|--------|------|--------|
| `Index()` | GET | Listar todos |
| `Details(id)` | GET | Ver detalhes |
| `Create()` | GET | Mostrar formulário |
| `Create(obj)` | POST | Guardar novo |
| `Edit(id)` | GET | Mostrar formulário edição |
| `Edit(id, obj)` | POST | Guardar alteração |
| `Delete(id)` | GET | Confirmação delete |
| `DeleteConfirmed(id)` | POST | Executar delete |

---

## Views

As Views são as páginas HTML que o utilizador vê.

### Localização
```
14548_employes_managment/
└── Views/
    ├── Employees/
    │   ├── Index.cshtml
    │   ├── Create.cshtml
    │   ├── Edit.cshtml
    │   ├── Delete.cshtml
    │   └── Details.cshtml
    ├── LeaveApplications/
    └── [suas novas views vão aqui]
```

### View Index - Listar (Index.cshtml)

```html
@model IEnumerable<seu_namespace.Models.Department>

@{
    ViewData["Title"] = "Departamentos";
}

<div class="container mt-4">
    <h1>@ViewData["Title"]</h1>
    
    <a asp-action="Create" class="btn btn-primary mb-3">Novo Departamento</a>

    <table class="table table-striped">
        <thead>
            <tr>
                <th>Nome</th>
                <th>Descrição</th>
                <th>Ações</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var item in Model)
            {
                <tr>
                    <td>@item.Name</td>
                    <td>@item.Description</td>
                    <td>
                        <a asp-action="Details" asp-route-id="@item.DepartmentId" class="btn btn-sm btn-info">Ver</a>
                        <a asp-action="Edit" asp-route-id="@item.DepartmentId" class="btn btn-sm btn-warning">Editar</a>
                        <a asp-action="Delete" asp-route-id="@item.DepartmentId" class="btn btn-sm btn-danger">Apagar</a>
                    </td>
                </tr>
            }
        </tbody>
    </table>
</div>
```

### View Create/Edit - Formulário (Create.cshtml)

```html
@model seu_namespace.Models.Department

@{
    ViewData["Title"] = "Novo Departamento";
}

<div class="container mt-4">
    <h1>@ViewData["Title"]</h1>

    <form asp-action="Create" method="post" class="mt-4">
        <div asp-validation-summary="ModelOnly" class="text-danger"></div>

        <div class="form-group">
            <label asp-for="Name" class="form-label"></label>
            <input asp-for="Name" class="form-control" />
            <span asp-validation-for="Name" class="text-danger"></span>
        </div>

        <div class="form-group">
            <label asp-for="Description" class="form-label"></label>
            <textarea asp-for="Description" class="form-control" rows="4"></textarea>
            <span asp-validation-for="Description" class="text-danger"></span>
        </div>

        <div class="form-group mt-3">
            <button type="submit" class="btn btn-primary">Guardar</button>
            <a asp-action="Index" class="btn btn-secondary">Cancelar</a>
        </div>
    </form>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

### View Details - Detalhes (Details.cshtml)

```html
@model seu_namespace.Models.Department

@{
    ViewData["Title"] = "Detalhes do Departamento";
}

<div class="container mt-4">
    <h1>@ViewData["Title"]</h1>

    <div class="card mt-4">
        <div class="card-body">
            <p>
                <strong>Nome:</strong> @Model.Name
            </p>
            <p>
                <strong>Descrição:</strong> @Model.Description
            </p>
        </div>
    </div>

    <div class="mt-3">
        <a asp-action="Edit" asp-route-id="@Model.DepartmentId" class="btn btn-warning">Editar</a>
        <a asp-action="Delete" asp-route-id="@Model.DepartmentId" class="btn btn-danger">Apagar</a>
        <a asp-action="Index" class="btn btn-secondary">Voltar</a>
    </div>
</div>
```

### View Delete - Confirmar Apagar (Delete.cshtml)

```html
@model seu_namespace.Models.Department

@{
    ViewData["Title"] = "Apagar Departamento";
}

<div class="container mt-4">
    <h1 class="text-danger">@ViewData["Title"]</h1>

    <div class="alert alert-danger mt-4" role="alert">
        Tem certeza que deseja apagar este departamento?
    </div>

    <div class="card">
        <div class="card-body">
            <p>
                <strong>Nome:</strong> @Model.Name
            </p>
            <p>
                <strong>Descrição:</strong> @Model.Description
            </p>
        </div>
    </div>

    <form asp-action="DeleteConfirmed" method="post" class="mt-3">
        <input type="hidden" asp-for="DepartmentId" />
        <button type="submit" class="btn btn-danger">Apagar Definitivamente</button>
        <a asp-action="Index" class="btn btn-secondary">Cancelar</a>
    </form>
</div>
```

---

## Exemplo Prático Completo

Vamos implementar uma nova funcionalidade: **Sistema de Departamentos**

### Passo 1: Criar o Model

**Ficheiro:** `14548_employes_managment/Models/Department.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace _14548_employes_managment.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "O nome do departamento é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais que 100 caracteres")]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Relação com Employee (um departamento tem muitos funcionários)
        public ICollection<Employee>? Employees { get; set; }
    }
}
```

### Passo 2: Adicionar Model ao DbContext

**Ficheiro:** `14548_employes_managment/Data/ApplicationDbContext.cs`

Adicionar esta linha no ficheiro (dentro da classe ApplicationDbContext):

```csharp
public DbSet<Department> Departments { get; set; }
```

### Passo 3: Atualizar o Model Employee

**Ficheiro:** `14548_employes_managment/Models/Employee.cs`

Adicionar a propriedade de Foreign Key:

```csharp
// Foreign Key para Department
public int? DepartmentId { get; set; }
public Department? Department { get; set; }
```

### Passo 4: Criar a Migração

Execute no terminal:

```bash
dotnet ef migrations add AddDepartmentTable
```

Isto cria os ficheiros na pasta `Data/Migrations/`

### Passo 5: Atualizar a Base de Dados

Execute no terminal:

```bash
dotnet ef database update
```

### Passo 6: Criar o Controller

**Ficheiro:** `14548_employes_managment/Controllers/DepartmentsController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using _14548_employes_managment.Data;
using _14548_employes_managment.Models;
using Microsoft.EntityFrameworkCore;

namespace _14548_employes_managment.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments.ToListAsync();
            return View(departments);
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentId,Name,Description")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentId,Name,Description")] Department department)
        {
            if (id != department.DepartmentId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.DepartmentId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentId == id);
        }
    }
}
```

### Passo 7: Criar a Pasta de Views

Crie a pasta: `14548_employes_managment/Views/Departments/`

### Passo 8: Criar as Views

**Ficheiro:** `14548_employes_managment/Views/Departments/Index.cshtml`

```html
@model IEnumerable<_14548_employes_managment.Models.Department>

@{
    ViewData["Title"] = "Departamentos";
}

<div class="container mt-4">
    <h1>@ViewData["Title"]</h1>
    
    <a asp-action="Create" class="btn btn-primary mb-3">
        <i class="fas fa-plus"></i> Novo Departamento
    </a>

    @if (Model.Any())
    {
        <div class="table-responsive">
            <table class="table table-striped table-hover">
                <thead class="table-dark">
                    <tr>
                        <th>Nome</th>
                        <th>Descrição</th>
                        <th>Data de Criação</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach (var item in Model)
                    {
                        <tr>
                            <td>@item.Name</td>
                            <td>@item.Description</td>
                            <td>@item.CreatedDate.ToString("dd/MM/yyyy")</td>
                            <td>
                                <a asp-action="Details" asp-route-id="@item.DepartmentId" 
                                   class="btn btn-sm btn-info">
                                    <i class="fas fa-eye"></i> Ver
                                </a>
                                <a asp-action="Edit" asp-route-id="@item.DepartmentId" 
                                   class="btn btn-sm btn-warning">
                                    <i class="fas fa-edit"></i> Editar
                                </a>
                                <a asp-action="Delete" asp-route-id="@item.DepartmentId" 
                                   class="btn btn-sm btn-danger">
                                    <i class="fas fa-trash"></i> Apagar
                                </a>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    }
    else
    {
        <div class="alert alert-info" role="alert">
            Nenhum departamento encontrado. <a asp-action="Create">Criar novo</a>
        </div>
    }
</div>
```

**Ficheiro:** `14548_employes_managment/Views/Departments/Create.cshtml`

```html
@model _14548_employes_managment.Models.Department

@{
    ViewData["Title"] = "Novo Departamento";
}

<div class="container mt-4">
    <h1>@ViewData["Title"]</h1>

    <form asp-action="Create" method="post" class="mt-4">
        <div asp-validation-summary="ModelOnly" class="alert alert-danger" role="alert"></div>

        <div class="form-group mb-3">
            <label asp-for="Name" class="form-label">Nome do Departamento</label>
            <input asp-for="Name" class="form-control" placeholder="Ex: Recursos Humanos" />
            <span asp-validation-for="Name" class="text-danger"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="Description" class="form-label">Descrição</label>
            <textarea asp-for="Description" class="form-control" rows="4" 
                      placeholder="Descrição do departamento"></textarea>
            <span asp-validation-for="Description" class="text-danger"></span>
        </div>

        <div class="form-group mt-4">
            <button type="submit" class="btn btn-primary">
                <i class="fas fa-save"></i> Guardar
            </button>
            <a asp-action="Index" class="btn btn-secondary">
                <i class="fas fa-times"></i> Cancelar
            </a>
        </div>
    </form>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

**Ficheiro:** `14548_employes_managment/Views/Departments/Edit.cshtml`

```html
@model _14548_employes_managment.Models.Department

@{
    ViewData["Title"] = "Editar Departamento";
}

<div class="container mt-4">
    <h1>@ViewData["Title"]</h1>

    <form asp-action="Edit" method="post" class="mt-4">
        <div asp-validation-summary="ModelOnly" class="alert alert-danger" role="alert"></div>

        <input type="hidden" asp-for="DepartmentId" />

        <div class="form-group mb-3">
            <label asp-for="Name" class="form-label">Nome do Departamento</label>
            <input asp-for="Name" class="form-control" />
            <span asp-validation-for="Name" class="text-danger"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="Description" class="form-label">Descrição</label>
            <textarea asp-for="Description" class="form-control" rows="4"></textarea>
            <span asp-validation-for="Description" class="text-danger"></span>
        </div>

        <div class="form-group mt-4">
            <button type="submit" class="btn btn-primary">
                <i class="fas fa-save"></i> Guardar Alterações
            </button>
            <a asp-action="Index" class="btn btn-secondary">
                <i class="fas fa-times"></i> Cancelar
            </a>
        </div>
    </form>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

**Ficheiro:** `14548_employes_managment/Views/Departments/Details.cshtml`

```html
@model _14548_employes_managment.Models.Department

@{
    ViewData["Title"] = "Detalhes do Departamento";
}

<div class="container mt-4">
    <h1>@ViewData["Title"]</h1>

    <div class="card mt-4">
        <div class="card-header">
            <h5>@Model.Name</h5>
        </div>
        <div class="card-body">
            <p>
                <strong>Descrição:</strong> @Model.Description
            </p>
            <p>
                <strong>Data de Criação:</strong> @Model.CreatedDate.ToString("dd/MM/yyyy HH:mm")
            </p>
        </div>
    </div>

    @if (Model.Employees != null && Model.Employees.Any())
    {
        <div class="card mt-4">
            <div class="card-header">
                <h5>Funcionários do Departamento</h5>
            </div>
            <div class="card-body">
                <ul class="list-group">
                    @foreach (var employee in Model.Employees)
                    {
                        <li class="list-group-item">
                            @employee.FullName
                        </li>
                    }
                </ul>
            </div>
        </div>
    }

    <div class="mt-4">
        <a asp-action="Edit" asp-route-id="@Model.DepartmentId" class="btn btn-warning">
            <i class="fas fa-edit"></i> Editar
        </a>
        <a asp-action="Delete" asp-route-id="@Model.DepartmentId" class="btn btn-danger">
            <i class="fas fa-trash"></i> Apagar
        </a>
        <a asp-action="Index" class="btn btn-secondary">
            <i class="fas fa-arrow-left"></i> Voltar
        </a>
    </div>
</div>
```

**Ficheiro:** `14548_employes_managment/Views/Departments/Delete.cshtml`

```html
@model _14548_employes_managment.Models.Department

@{
    ViewData["Title"] = "Apagar Departamento";
}

<div class="container mt-4">
    <h1 class="text-danger">@ViewData["Title"]</h1>

    <div class="alert alert-danger mt-4" role="alert">
        <i class="fas fa-exclamation-triangle"></i> 
        <strong>Aviso:</strong> Tem certeza que deseja apagar este departamento?
    </div>

    <div class="card">
        <div class="card-body">
            <p>
                <strong>Nome:</strong> @Model.Name
            </p>
            <p>
                <strong>Descrição:</strong> @Model.Description
            </p>
        </div>
    </div>

    <form asp-action="DeleteConfirmed" method="post" class="mt-3">
        <input type="hidden" asp-for="DepartmentId" />
        <button type="submit" class="btn btn-danger">
            <i class="fas fa-trash"></i> Apagar Definitivamente
        </button>
        <a asp-action="Index" class="btn btn-secondary">
            <i class="fas fa-times"></i> Cancelar
        </a>
    </form>
</div>
```

---

## Resumo do Processo Completo

```
1. Criar Model em: Models/Department.cs
2. Adicionar DbSet em: Data/ApplicationDbContext.cs
3. Criar Migração: dotnet ef migrations add AddDepartmentTable
4. Atualizar BD: dotnet ef database update
5. Criar Controller em: Controllers/DepartmentsController.cs
6. Criar pasta Views: Views/Departments/
7. Criar 5 Views: Index.cshtml, Create.cshtml, Edit.cshtml, Details.cshtml, Delete.cshtml
8. Testar a aplicação!
```

## Comandos Úteis

```bash
# Ver lista de migrações
dotnet ef migrations list

# Remover última migração (antes de aplicar)
dotnet ef migrations remove

# Atualizar para versão anterior
dotnet ef database update NomeMigracaoAnterior

# Criar script SQL da migração
dotnet ef migrations script AddDepartmentTable

# Limpar a BD completamente
dotnet ef database drop
```

---

**Criado em:** Maio de 2026
 x
