using EmailClient.Common.DataContext.SqlServer;
using EmailClient.Common.EntityModels.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace EmailClient.WebApi.DAL;

/// <summary>
/// Repository class used to access database.
/// </summary>
public class EmailRepo : IDisposable
{
    private readonly EmailClientContext Db;
    private DbSet<Email> Table;

    public EmailClientContext Context => Db;

    public EmailRepo()
    {
        Db = new EmailClientContext();
        Table = Db.Emails;
    }

    public EmailRepo(DbContextOptions<EmailClientContext> options)
    {
        Db = new EmailClientContext(options);
        Table = Db.Emails;
    }

    public bool HasChanges => Db.ChangeTracker.HasChanges();

    public int Count => Table.Count();

    public virtual Email Find(int id) => Table.Find(id);

    public virtual IQueryable<Email> Search(
        Expression<Func<Email, bool>> filter = null,
        Func<IQueryable<Email>, IOrderedQueryable<Email>> orderBy = null,
        string includeProperties = "")
    {
        IQueryable<Email> query = Table.Where(e => e.Id > 0);

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var includeProperty in includeProperties.Split
            (new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        if (orderBy != null)
        {
            return orderBy(query);
        }
        else
        {
            return query;
        }
    }

    internal IEnumerable<Email> GetRange(IQueryable<Email> query, int skip, int take)
        => query.Skip(skip).Take(take);

    public virtual IEnumerable<Email> GetRange(int skip, int take)
        => GetRange(Table, skip, take);

    public virtual int Add(Email email, bool persist = true)
    {
        Table.Add(email);
        return persist ? SaveChanges() : 0;
    }

    public virtual int AddRange(IEnumerable<Email> emails, bool persist = true)
    {
        Table.AddRange(emails);
        return persist ? SaveChanges() : 0;
    }

    //normally, I'd add Update() and UpdateRange(), but that isn't functionality associated with an email storage db, I wouldn't think?

    public virtual int Delete(Email email, bool persist = true)
    {
        Table.Remove(email);
        return persist ? SaveChanges() : 0;
    }

    public virtual int DeleteRange(IEnumerable<Email> emails, bool persist = true)
    {
        Table.RemoveRange(emails);
        return persist ? SaveChanges() : 0;
    }

    internal Email GetEntryFromChangeTracker(int? id)
    {
        return Db.ChangeTracker.Entries<Email>()
            .Select((EntityEntry e) => (Email)e.Entity)
            .FirstOrDefault(e => e.Id == id);
    }
    

    public virtual int Delete(int id, byte[] rowVersion, bool persist = true)
    {
        var entry = GetEntryFromChangeTracker(id);
        if (entry != null)
        {
            if (entry.RowVersion == rowVersion)
            {
                return Delete(entry, persist);
            }
            throw new Exception("Unable to delete due to concurrency violation.");
        }
        Db.Entry(new Email { Id = id, RowVersion = rowVersion }).State = EntityState.Deleted;
        return persist ? SaveChanges() : 0;
    }


    public int SaveChanges()
    {
        try
        {
            return Db.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            //A concurrency error occurred
            //Should handle intelligently
            Console.WriteLine(ex);
            throw;
        }
        catch (RetryLimitExceededException ex)
        {
            //DbResiliency retry limit exceeded
            //Should handle intelligently
            Console.WriteLine(ex);
            throw;
        }
        catch (Exception ex)
        {
            //Should handle intelligently
            Console.WriteLine(ex);
            throw;
            //-2146232060
            //throw new Exception($"{ex.HResult}");
        }
    }

    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // Free any other managed objects here. 
            //
        }
        Db.Dispose();
        _disposed = true;
    }
}
