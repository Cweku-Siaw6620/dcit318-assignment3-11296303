using System;
using System.Collections.Generic;

public record Transaction(
    int Id,
    DateTime Date,
    decimal Amount,
    string Category
    );

//interface
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

//classes
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Bank Transfer] Processing {transaction.Amount} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Mobile Money] Processing {transaction.Amount} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Crypto Wallet] Processing {transaction.Amount} for {transaction.Category}");
    }
}

//Base class
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"[Account] Balance after transaction: {Balance}");
    }
}

//sealed class
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance)
    {
    }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"[SavingsAccount] Transaction applied. New Balance: {Balance}");
        }
    }
}

//Finance APP
public class FinanceApp
{
    private List<Transaction> _transactions = new();

    public void Run()
    {
        // Create account with GHC1000
        SavingsAccount account = new SavingsAccount("ACCT12345", 1000m);

        // Sample transactions
        Transaction t1 = new Transaction(1, DateTime.Now, 200m, "Groceries");
        Transaction t2 = new Transaction(2, DateTime.Now, 350m, "Utilities");
        Transaction t3 = new Transaction(3, DateTime.Now, 600m, "Entertainment");

        // Processors
        ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
        ITransactionProcessor bankTransfer = new BankTransferProcessor();
        ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();

        // Process each transaction
        mobileMoney.Process(t1);
        account.ApplyTransaction(t1);

        bankTransfer.Process(t2);
        account.ApplyTransaction(t2);

        cryptoWallet.Process(t3);
        account.ApplyTransaction(t3);

        // Store transactions
        _transactions.Add(t1);
        _transactions.Add(t2);
        _transactions.Add(t3);
    }
}

class Program
{
    static void Main(string[] args)
    {
        FinanceApp app = new FinanceApp();
        app.Run();
    }
}
