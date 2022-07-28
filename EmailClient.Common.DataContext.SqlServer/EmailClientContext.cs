using EmailClient.Common.EntityModels.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace EmailClient.Common.DataContext.SqlServer;

public partial class EmailClientContext : DbContext
{
    public EmailClientContext() { }

    public EmailClientContext(DbContextOptions<EmailClientContext> options) : base(options) { }

    public virtual DbSet<Email> Emails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);//do I need this?
        if (!optionsBuilder.IsConfigured)
        {
            //if no connection string is passed from client application, create a default DB to store the email data
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=EmailClient;Integrated Security=true;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Email>(entity =>
        {
            entity.ToTable("Email");
        });
    }
}
