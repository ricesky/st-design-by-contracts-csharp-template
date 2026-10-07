using System.Globalization;

namespace Contracts.Domain;

public class LoggedEquipmentLoan : IEquipmentLoan
{
    public int TotalStock { get; }
    public int AvailableStock { get; private set; }
    public int BorrowedStock { get; private set; }
    public IReadOnlyList<string> History { get; }

    public LoggedEquipmentLoan(int totalStock)
    {
        // TODO: tambahkan implementasi disini
        throw new NotImplementedException();
    }

    public void Borrow(int quantity)
    {
        // TODO: tambahkan implementasi disini
        throw new NotImplementedException();
    }

    public void Return(int quantity)
    {
        // TODO: tambahkan implementasi disini
        throw new NotImplementedException();
    }
}
