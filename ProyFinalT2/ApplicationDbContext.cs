using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ProyFinalT2.Entidades;

namespace ProyFinalT2
{
        // esta clase es la que se encarga de interactuar con la base de datos 
        public class ApplicationDbContext : IdentityDbContext
        {
            public ApplicationDbContext(DbContextOptions options) : base(options)
            {
                // en el constructor  se recibe un objeto DbContextOptions que se utiliza para configurar las opciones de conexión a la base de datos
            }

            // este método se llama cuando se crea el modelo de la base de datos , sirve para configurar las entidades
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {   // se llama al método OnModelCreating de la clase base para que configure las entidades de Identity
                base.OnModelCreating(modelBuilder);

                //modelBuilder.Entity<Tarea>().Property(t => t.Titulo).HasMaxLength(250).IsRequired();
            }

        ////representa la tabla Tareas de la base de datos 
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Paso> Pasos { get; set; }


    }
    
}
