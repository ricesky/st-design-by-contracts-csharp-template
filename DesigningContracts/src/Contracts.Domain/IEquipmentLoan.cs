namespace Contracts.Domain;

public interface IEquipmentLoan
{
    int TotalStock { get; }
    int AvailableStock { get; }
    int BorrowedStock { get; }
    void Borrow(int quantity);
    void Return(int quantity);
}
