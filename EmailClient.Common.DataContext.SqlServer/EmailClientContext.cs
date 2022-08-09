using EmailClient.Common.EntityModels.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace EmailClient.Common.DataContext.SqlServer;

/// <summary>
/// Database context for EmailClient. Specific to Microsoft SQL Server.
/// </summary>
public partial class EmailClientContext : DbContext
{
    public EmailClientContext() { }

    public EmailClientContext(DbContextOptions<EmailClientContext> options) : base(options) { }

    public virtual DbSet<Email> Emails { get; set; }

    /// <summary>
    /// Overide to establish a new default database is a connection string is not provided.
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (!optionsBuilder.IsConfigured)
        {
            //if no connection string is passed from client application, create a default DB to store the email data
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=EmailClient;Integrated Security=true;");
        }
    }

    /// <summary>
    /// Override to create the Email table using EF Core migrations.
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Email>(entity =>
        {
            entity.ToTable("Email");
        });
    }
}
