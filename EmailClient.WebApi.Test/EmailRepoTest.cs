using EmailClient.Common.DataContext.SqlServer;
using EmailClient.Common.EntityModels.SqlServer;
using EmailClient.WebApi.DAL;
using Microsoft.EntityFrameworkCore;

namespace EmailClient.WebApi.Test;

public class EmailRepoTest
{
    private EmailRepo emailRepo;

    [SetUp]
    public void SetUp()
    {
        DbContextOptionsBuilder<EmailClientContext> options = new();
        emailRepo = new();
    }

    [Test]
    public void FindTest()
    {
        //arrange
        Email expected = new()
        {
            Recipients = "timcrum06,tcrum1@wvup.edu",
            Bccs = "timcrum88@gmail.com,tcrum06@gmail.com",
            Sender = "ectester90@gmail.com",
            Subject = "This is test email",
            Message = "Here is the content of my repo test email.",
            SendDate = DateTime.Now,
            SentSuccessfully = true
        };

        //act
        emailRepo.Add(expected);
        Email actual = emailRepo.Find(expected.Id);

        //assert
        Assert.IsNotNull(actual); //assert that an email found with that Id
        Assert.AreEqual(expected, actual); //assert the properties are the same
    }

    [Test]
    public void SearchTest()
    {
        //arrange
        Email expected = emailRepo.Find(1);

        //act
        Email actual = emailRepo.Search(e => e.Id == 1).First();

        //assert
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void GetRangeTest()
    {
        //arrange
        List<Email> expected = new()
        {
            emailRepo.Find(5),
            emailRepo.Find(6)
        };

        //act
        List<Email> actual = emailRepo.GetRange(3, 2).ToList();

        //assert
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void AddTest()
    {
        //arrange
        Email expected = new()
        {
            Recipients = "timcrum06,tcrum1@wvup.edu",
            Bccs = "timcrum88@gmail.com,tcrum06@gmail.com",
            Sender = "ectester90@gmail.com",
            Subject = "This is test email",
            Message = "Here is the content of my repo test email.",
            SendDate = DateTime.Now,
            SentSuccessfully = true
        };

        //act
        emailRepo.Add(expected);
        Email actual = emailRepo.Find(expected.Id);

        //assert
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void AddRangeTest()
    {
        //arrange

        Email email1 = new()
        {
            Recipients = "timcrum06,tcrum1@wvup.edu",
            Bccs = "timcrum88@gmail.com,tcrum06@gmail.com",
            Sender = "ectester90@gmail.com",
            Subject = "This is test email",
            Message = "Here is the content of my repo test email.",
            SendDate = DateTime.Now,
            SentSuccessfully = true
        };

        Email email2 = new()
        {
            Recipients = "test@email.com,test2@email.com",
            Bccs = "testCC@email.com",
            Sender = "ectester90@gmail.com",
            Subject = "Another test email",
            Message = "Content of another test email",
            SendDate = DateTime.Now,
            SentSuccessfully = false
        };

        Email email3 = new()
        {
            Recipients = "test@email.com,test2@email.com",
            Bccs = "testCC@email.com",
            Sender = "ectester90@gmail.com",
            Subject = "Another test email",
            Message = "Content of another test email",
            SendDate = DateTime.Now,
            SentSuccessfully = false
        };
        List<Email> expected = new();
        expected.Add(email1);
        expected.Add(email2);   
        expected.Add(email3);

        //act
        emailRepo.AddRange(expected);

        //assert
        Assert.AreEqual(expected[0], emailRepo.Find(email1.Id));
        Assert.AreEqual(expected[1], emailRepo.Find(email2.Id));
        Assert.AreEqual(expected[2], emailRepo.Find(email3.Id));
    }

    [Test]
    public void DeleteTest()
    {
        //arrange
        Email expected = emailRepo.Find(4);

        //act
        emailRepo.Delete(expected);

        //assert
        Assert.IsNull(emailRepo.Find(4));
    }

    [Test]
    public void DeleteRangeTest()
    {
        //arrange
        List<Email> expected = new()
        {
            emailRepo.Find(2),
            emailRepo.Find(3)
        };

        //act
        emailRepo.DeleteRange(expected);

        //assert
        Assert.IsNull(emailRepo.Find(2));
        Assert.IsNull(emailRepo.Find(3));
    }

}
