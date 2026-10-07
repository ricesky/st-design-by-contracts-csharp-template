![.NET](https://img.shields.io/badge/.NET-10.0-blue)
![C%23](https://img.shields.io/badge/C%23-14-purple)
![NUnit](https://img.shields.io/badge/NUnit-4.x-green)
![Contracts](https://img.shields.io/badge/Testing-Designing%20Contracts-orange)

# Tutorial Designing Contracts dengan C# dan NUnit

Tutorial dan tugas ini terdiri dari dua studi kasus:

1. **Reservasi Kursi Workshop** sebagai tutorial.
2. **Peminjaman Peralatan Laboratorium** sebagai tugas mandiri.

Mahasiswa diberikan kontrak, kerangka kode produksi, dan unit test NUnit. Tugas mahasiswa adalah melengkapi kode agar memenuhi kontrak. Unit test sudah disediakan dan digunakan untuk memeriksa implementasi.

Pada setiap bagian, ikuti urutan berikut:

1. baca kontrak;
2. perhatikan keadaan awal dan hasil yang dijanjikan;
3. implementasikan kode produksi;
4. jalankan test yang disediakan; dan
5. gunakan hasil pengujian untuk memperbaiki implementasi.

Kontrak adalah acuan utama. Implementasi harus berlaku untuk seluruh input yang memenuhi kontrak, bukan hanya contoh angka dalam test.

---

## Capaian Pembelajaran

Setelah menyelesaikan tutorial ini, mahasiswa mampu:

1. menerjemahkan pre-condition menjadi validasi parameter dan keadaan objek;
2. mengimplementasikan post-condition secara tepat;
3. menjaga invariant setelah konstruksi dan operasi publik;
4. memastikan operasi yang ditolak tidak mengubah keadaan objek;
5. membaca unit test sebagai pemeriksaan terhadap kontrak;
6. menerapkan Liskov Substitution Principle pada implementasi interface; dan
7. menjelaskan dampak perubahan kontrak terhadap kode pemanggil.

---

## Lingkungan Pengembangan

- **IDE:** Visual Studio 2026, atau editor lain yang mendukung C#.
- **SDK:** .NET 10.
- **Bahasa:** C# 14.
- **Test framework:** NUnit 4.x.
- **Test runner:** Microsoft.NET.Test.Sdk dan NUnit3TestAdapter, menggunakan VSTest.

Perintah utama, dijalankan dari direktori utama repository:

```bash
dotnet restore
dotnet build
dotnet test
```

---

## Struktur Solution

Struktur template yang digunakan:

```text
DesigningContracts/
├── DesigningContracts.slnx
├── README.md
├── REFLECTION.md
├── src/
│   └── Contracts.Domain/
│       ├── Contracts.Domain.csproj
│       ├── WorkshopReservation.cs
│       ├── IEquipmentLoan.cs
│       ├── EquipmentLoan.cs
│       └── LoggedEquipmentLoan.cs
└── tests/
    └── Contracts.Domain.Tests/
        ├── Contracts.Domain.Tests.csproj
        ├── WorkshopReservationTests.cs
        ├── EquipmentLoanTests.cs
        ├── InvariantsTests.cs
        └── SubstitutionTests.cs
```

Ketentuan umum:

- Mahasiswa melengkapi `WorkshopReservation.cs`, `EquipmentLoan.cs`, dan `LoggedEquipmentLoan.cs` di folder `src/`.
- Interface `IEquipmentLoan` dan seluruh file di folder `tests/` tidak boleh diubah.
- Jangan mengubah nama solution, project, namespace, class, constructor, property, atau method publik.
- Namespace kode produksi adalah `Contracts.Domain`; namespace test adalah `Contracts.Domain.Tests`.
- Boleh menambah field dan helper privat untuk mendukung implementasi.
- Property keadaan objek tidak boleh memiliki setter publik. Seluruh perubahan keadaan dilakukan melalui operasi yang ditentukan.
- Jangan menonaktifkan test, mengganti assertion, atau menambahkan aturan khusus untuk mengenali input test.
- Tidak diperlukan UI, database, file, jaringan, atau pengaturan waktu.
- Operasi dilakukan secara berurutan; penanganan akses bersamaan tidak termasuk tugas.
- Kerangka awal harus dapat dikompilasi. Method yang belum selesai boleh melempar `NotImplementedException`; test yang gagal pada tahap awal merupakan hal yang diharapkan.

---

## Memulai Pengerjaan

1. Buka tautan assignment Classroom50 dan accept assignment.
2. Clone repository assignment Anda. Ganti `<URL_REPOSITORY_ANDA>` dengan URL repository yang diperoleh.

```bash
git clone <URL_REPOSITORY_ANDA>
```

3. Masuk ke direktori hasil clone dan buka `DesigningContracts.slnx`.
4. Jalankan `dotnet restore`, `dotnet build`, dan `dotnet test`.
5. Baca pesan kegagalan test sebelum mulai mengimplementasikan kode.

---

# Bagian 1. Tutorial: Reservasi Kursi Workshop

## 1.1 Tujuan

Mengimplementasikan pre-condition dan post-condition pada sistem reservasi sederhana.

Sebuah workshop memiliki kapasitas tetap. Sistem mencatat jumlah kursi terpesan dan jumlah kursi tersedia. Satu objek merepresentasikan satu workshop; identitas peserta tidak dicatat.

## 1.2 API yang Disediakan

```csharp
public class WorkshopReservation
{
    public int Capacity { get; }
    public int ReservedSeats { get; private set; }
    public int AvailableSeats => Capacity - ReservedSeats;

    public WorkshopReservation(int capacity)
    {
        throw new NotImplementedException();
    }

    public void Reserve(int quantity)
    {
        throw new NotImplementedException();
    }

    public void Cancel(int quantity)
    {
        throw new NotImplementedException();
    }
}
```

## 1.3 Kontrak Constructor

| Unsur | Kontrak |
|---|---|
| Pre-condition | `capacity > 0` |
| Post-condition | `Capacity == capacity`, `ReservedSeats == 0`, dan `AvailableSeats == capacity` |
| Jika ditolak | Lempar `ArgumentOutOfRangeException` dengan `ParamName` bernilai `capacity`; objek tidak berhasil dibuat |

Lengkapi constructor pada `WorkshopReservation.cs`. Periksa parameter sebelum menetapkan keadaan awal objek.

Jalankan test constructor:

```bash
dotnet test --filter "FullyQualifiedName~WorkshopReservationTests.Constructor_"
```

## 1.4 Kontrak Reserve

Notasi `oldReservedSeats` berarti jumlah kursi terpesan sebelum operasi.

| Unsur | Kontrak |
|---|---|
| Pre-condition | `quantity > 0` dan `quantity <= AvailableSeats` |
| Post-condition | `ReservedSeats == oldReservedSeats + quantity`; `Capacity` tetap |
| Nominal tidak positif | Lempar `ArgumentOutOfRangeException` dengan `ParamName` bernilai `quantity` |
| Kursi tidak mencukupi | Lempar `InvalidOperationException` |
| Semua kegagalan | `Capacity`, `ReservedSeats`, dan `AvailableSeats` tidak berubah |

Periksa `quantity > 0` terlebih dahulu, lalu ketersediaan kursi. Jangan mengubah jumlah kursi sebelum seluruh pemeriksaan berhasil.

| Test case | Keadaan awal | Tindakan | Expected result |
|---|---|---|---|
| WR-01 | Kapasitas 30; terpesan 0 | `Reserve(5)` | Terpesan 5; tersedia 25 |
| WR-02 | Kapasitas 30; terpesan 0 | `Reserve(30)` | Terpesan 30; tersedia 0 |
| WR-03 | Kapasitas 30; terpesan 0 | `Reserve(31)` | `InvalidOperationException`; keadaan tetap |
| WR-04 | Kapasitas 30; terpesan 0 | `Reserve(0)` | `ArgumentOutOfRangeException`; keadaan tetap |
| WR-05 | Kapasitas 30; terpesan 0 | `Reserve(-1)` | `ArgumentOutOfRangeException`; keadaan tetap |

## 1.5 Membaca Test yang Disediakan

Pahami unit test berikut dan implementasikan method yang diuji.

```csharp
[Test]
public void Reserve_WithAvailableSeats_UpdatesState()
{
    // Arrange
    var workshop = new WorkshopReservation(30);

    // Act
    workshop.Reserve(5);

    // Assert
    Assert.Multiple(() =>
    {
        Assert.That(workshop.ReservedSeats, Is.EqualTo(5));
        Assert.That(workshop.AvailableSeats, Is.EqualTo(25));
        Assert.That(workshop.Capacity, Is.EqualTo(30));
    });
}
```

## 1.6 Kontrak Cancel

| Unsur | Kontrak |
|---|---|
| Pre-condition | `quantity > 0` dan `quantity <= ReservedSeats` |
| Post-condition | `ReservedSeats == oldReservedSeats - quantity`; `Capacity` tetap |
| Nominal tidak positif | Lempar `ArgumentOutOfRangeException` dengan `ParamName` bernilai `quantity` |
| Melebihi reservasi | Lempar `InvalidOperationException` |
| Semua kegagalan | Seluruh property keadaan tidak berubah |

Periksa nominal tidak positif terlebih dahulu. Membatalkan tepat seluruh kursi terpesan adalah operasi valid.

| Test case | Keadaan awal | Tindakan | Expected result |
|---|---|---|---|
| WC-01 | Kapasitas 30; terpesan 10 | `Cancel(4)` | Terpesan 6; tersedia 24 |
| WC-02 | Kapasitas 30; terpesan 10 | `Cancel(10)` | Terpesan 0; tersedia 30 |
| WC-03 | Kapasitas 30; terpesan 10 | `Cancel(11)` | `InvalidOperationException`; keadaan tetap |
| WC-04 | Kapasitas 30; terpesan 0 | `Cancel(1)` | `InvalidOperationException`; keadaan tetap |
| WC-05 | Kapasitas 30; terpesan 10 | `Cancel(0)` | `ArgumentOutOfRangeException`; keadaan tetap |

## 1.7 Menjalankan Test

```bash
dotnet test --filter "FullyQualifiedName~WorkshopReservationTests"
```

Pastikan test terdeteksi dan seluruh test dalam fixture ini lulus.

## 1.8 Tugas Tutorial

1. Lengkapi constructor, `Reserve`, dan `Cancel` berdasarkan kontrak.
2. Pastikan operasi yang ditolak tidak mengubah keadaan objek.
3. Pastikan tidak ada setter publik yang dapat digunakan untuk melanggar kontrak.
4. Jalankan kembali `WorkshopReservationTests`.

---

# Bagian 2. Tugas Mandiri: Peminjaman Peralatan Laboratorium

## 2.1 Deskripsi

Laboratorium meminjamkan peralatan kepada mahasiswa. Satu objek merepresentasikan satu jenis peralatan, misalnya kamera. Sistem hanya mencatat jumlah stok; identitas peminjam dan tenggat pengembalian tidak termasuk tugas.

## 2.2 Interface yang Disediakan

```csharp
public interface IEquipmentLoan
{
    int TotalStock { get; }
    int AvailableStock { get; }
    int BorrowedStock { get; }

    void Borrow(int quantity);
    void Return(int quantity);
}
```

Class `EquipmentLoan` mengimplementasikan interface tersebut dan memiliki constructor:

```csharp
public EquipmentLoan(int totalStock)
```

Seluruh property stok hanya dapat dibaca dari luar objek.

## 2.3 Kontrak Constructor

| Unsur | Kontrak |
|---|---|
| Pre-condition | `totalStock > 0` |
| Post-condition | `TotalStock == totalStock`, `AvailableStock == totalStock`, dan `BorrowedStock == 0` |
| Jika ditolak | Lempar `ArgumentOutOfRangeException` dengan `ParamName` bernilai `totalStock`; objek tidak berhasil dibuat |

## 2.4 Kontrak Borrow

| Unsur | Kontrak |
|---|---|
| Pre-condition | `quantity > 0` dan `quantity <= AvailableStock` |
| Post-condition | `AvailableStock == oldAvailableStock - quantity`; `BorrowedStock == oldBorrowedStock + quantity`; `TotalStock` tetap |
| Nominal tidak positif | Lempar `ArgumentOutOfRangeException` dengan `ParamName` bernilai `quantity` |
| Stok tidak cukup | Lempar `InvalidOperationException` |
| Semua kegagalan | Seluruh stok tidak berubah |

## 2.5 Kontrak Return

| Unsur | Kontrak |
|---|---|
| Pre-condition | `quantity > 0` dan `quantity <= BorrowedStock` |
| Post-condition | `AvailableStock == oldAvailableStock + quantity`; `BorrowedStock == oldBorrowedStock - quantity`; `TotalStock` tetap |
| Nominal tidak positif | Lempar `ArgumentOutOfRangeException` dengan `ParamName` bernilai `quantity` |
| Pengembalian berlebih | Lempar `InvalidOperationException` |
| Semua kegagalan | Seluruh stok tidak berubah |

Pada `Borrow` dan `Return`, periksa nominal tidak positif sebelum memeriksa stok. Pesan exception bebas; jenis exception dan `ParamName` mengikuti kontrak.

## 2.6 Contoh Perilaku

Setiap baris berikut menggunakan objek baru dengan total stok 10. Keadaan awal disiapkan melalui constructor dan method publik.

| Test case | Persiapan | Tindakan | Expected result |
|---|---|---|---|
| EL-01 | Tidak ada | `Borrow(3)` | Tersedia 7; dipinjam 3 |
| EL-02 | Tidak ada | `Borrow(10)` | Tersedia 0; dipinjam 10 |
| EL-03 | Tidak ada | `Borrow(11)` | `InvalidOperationException`; stok tetap |
| EL-04 | `Borrow(6)` | `Return(2)` | Tersedia 6; dipinjam 4 |
| EL-05 | `Borrow(6)` | `Return(6)` | Tersedia 10; dipinjam 0 |
| EL-06 | `Borrow(6)` | `Return(7)` | `InvalidOperationException`; stok tetap |
| EL-07 | Tidak ada | `Borrow(0)` | `ArgumentOutOfRangeException`; stok tetap |
| EL-08 | Tidak ada | `Return(-1)` | `ArgumentOutOfRangeException`; stok tetap |

## 2.7 Menjalankan Test

```bash
dotnet test --filter "FullyQualifiedName~EquipmentLoanTests"
```

## 2.8 Tugas

1. Lengkapi `EquipmentLoan.cs` tanpa mengubah interface.
2. Implementasikan constructor, `Borrow`, dan `Return`.
3. Pastikan hasil operasi tepat dan seluruh stok tetap ketika operasi ditolak.
4. Jalankan `EquipmentLoanTests` hingga seluruh test lulus.

---

# Bagian 3. Menjaga Invariant dan Kontrak Kegagalan

## 3.1 Tujuan

Menjaga keadaan objek setelah rangkaian operasi berhasil maupun ditolak.

Invariant harus berlaku setelah konstruksi dan pada batas operasi publik, termasuk setelah exception yang ditentukan kontrak.

## 3.2 Invariant Workshop

```csharp
Capacity > 0
ReservedSeats >= 0
ReservedSeats <= Capacity
AvailableSeats >= 0
AvailableSeats + ReservedSeats == Capacity
```

`Capacity` tetap sama dengan nilai saat konstruksi.

## 3.3 Invariant Peralatan

```csharp
TotalStock > 0
AvailableStock >= 0
BorrowedStock >= 0
AvailableStock + BorrowedStock == TotalStock
```

`TotalStock` tetap sama dengan nilai saat konstruksi. Invariant ini berlaku pada kedua implementasi `IEquipmentLoan`, termasuk implementasi pada Bagian 4.

## 3.4 Rangkaian Operasi Workshop

Gunakan kapasitas 20.

| Langkah | Tindakan | Terpesan | Tersedia | Hasil |
|---|---|---:|---:|---|
| 1 | Constructor | 0 | 20 | Berhasil |
| 2 | `Reserve(8)` | 8 | 12 | Berhasil |
| 3 | `Reserve(12)` | 20 | 0 | Berhasil |
| 4 | `Reserve(1)` | 20 | 0 | `InvalidOperationException` |
| 5 | `Cancel(5)` | 15 | 5 | Berhasil |
| 6 | `Cancel(16)` | 15 | 5 | `InvalidOperationException` |
| 7 | `Cancel(15)` | 0 | 20 | Berhasil |

## 3.5 Rangkaian Operasi Peralatan

Gunakan total stok 10.

| Langkah | Tindakan | Tersedia | Dipinjam | Hasil |
|---|---|---:|---:|---|
| 1 | Constructor | 10 | 0 | Berhasil |
| 2 | `Borrow(4)` | 6 | 4 | Berhasil |
| 3 | `Borrow(6)` | 0 | 10 | Berhasil |
| 4 | `Borrow(1)` | 0 | 10 | `InvalidOperationException` |
| 5 | `Return(3)` | 3 | 7 | Berhasil |
| 6 | `Return(8)` | 3 | 7 | `InvalidOperationException` |
| 7 | `Return(7)` | 10 | 0 | Berhasil |

Test memeriksa keadaan setelah setiap langkah, bukan hanya keadaan akhir.

## 3.6 Menjalankan Test

```bash
dotnet test --filter "FullyQualifiedName~InvariantsTests"
```

Test invariant untuk `LoggedEquipmentLoan` baru akan lulus setelah Bagian 4 selesai.

## 3.7 Tugas

1. Periksa implementasi agar setiap perubahan keadaan mengikuti post-condition.
2. Pastikan validasi selesai sebelum keadaan diubah.
3. Periksa kasus tepat pada kapasitas atau stok, serta pemesanan dan peminjaman satu unit.

---

# Bagian 4. LSP dan Perubahan Kontrak

## 4.1 Tujuan

Membuat implementasi lain yang dapat digunakan melalui interface yang sama tanpa melanggar kontrak pemanggil.

## 4.2 API LoggedEquipmentLoan

Lengkapi class berikut dalam namespace `Contracts.Domain`:

```csharp
public class LoggedEquipmentLoan : IEquipmentLoan
{
    public int TotalStock { get; }
    public int AvailableStock { get; private set; }
    public int BorrowedStock { get; private set; }
    public IReadOnlyList<string> History { get; }

    public LoggedEquipmentLoan(int totalStock)
    {
        throw new NotImplementedException();
    }

    public void Borrow(int quantity)
    {
        throw new NotImplementedException();
    }

    public void Return(int quantity)
    {
        throw new NotImplementedException();
    }
}
```

Anda boleh menggunakan komposisi untuk menggunakan kembali `EquipmentLoan`. Sesuaikan implementasi internal property tanpa mengubah nama, tipe, dan akses baca publiknya.

## 4.3 Kontrak Dasar

Constructor, `Borrow`, dan `Return` harus memenuhi seluruh kontrak Bagian 2 serta invariant Bagian 3.

Ketentuan berikut tetap berlaku:

1. Meminjam satu unit diterima jika stok tersedia.
2. Meminjam tepat seluruh stok tersedia diterima.
3. Mengembalikan tepat seluruh stok dipinjam diterima.
4. Tidak ada pengurangan stok tambahan untuk biaya atau pencatatan.
5. Jenis exception, `ParamName`, dan keadaan setelah kegagalan sama dengan kontrak dasar.

## 4.4 Kontrak Tambahan Riwayat

| Kondisi | Jaminan |
|---|---|
| Setelah constructor | `History` tidak null dan kosong |
| `Borrow(quantity)` berhasil | Tambahkan tepat satu string `BORROW:{quantity}` |
| `Return(quantity)` berhasil | Tambahkan tepat satu string `RETURN:{quantity}` |
| Operasi ditolak | Isi dan jumlah riwayat tidak berubah |
| Beberapa operasi berhasil | Catatan mengikuti urutan operasi; catatan lama tetap |

Gunakan angka desimal tanpa pemisah ribuan dengan kultur invariant. Contoh: `BORROW:3`, `RETURN:2`, dan `BORROW:1000`.

Pemanggil tidak boleh dapat mengubah riwayat internal melalui `History`. Gunakan tampilan koleksi baca saja atau salinan yang aman; jangan mengekspos `List<string>` internal secara langsung. Kontrak diperiksa dengan membaca ulang `History` setelah operasi, sehingga tampilan langsung maupun snapshot diperbolehkan.

Contoh rangkaian dengan total stok 10:

| Tindakan | Riwayat setelah tindakan |
|---|---|
| Constructor | Kosong |
| `Borrow(3)` | `BORROW:3` |
| `Return(2)` | `BORROW:3`, `RETURN:2` |
| `Borrow(10)` ditolak | Tetap `BORROW:3`, `RETURN:2` |

## 4.5 Penggunaan Melalui Interface

```csharp
IEquipmentLoan loan = new LoggedEquipmentLoan(10);
loan.Borrow(1);
loan.Return(1);
```

Pemanggil hanya mengetahui kontrak `IEquipmentLoan`. Karena itu, kode tersebut harus menghasilkan stok yang sama jika implementasi diganti menjadi `EquipmentLoan`.

Test substitusi menjalankan skenario kontrak dasar terhadap kedua implementasi melalui `IEquipmentLoan`. Test tambahan memeriksa riwayat `LoggedEquipmentLoan`.

## 4.6 Menjalankan Test

```bash
dotnet test --filter "FullyQualifiedName~SubstitutionTests"
```

## 4.7 Tugas Implementasi

1. Lengkapi `LoggedEquipmentLoan.cs`.
2. Pertahankan seluruh kontrak dasar.
3. Tambahkan riwayat hanya untuk operasi berhasil.
4. Lindungi riwayat dari perubahan oleh pemanggil.
5. Jalankan `SubstitutionTests` dan ulangi `InvariantsTests`.

# Menjalankan Seluruh Test

```bash
dotnet restore
dotnet build
dotnet test
```

Sebelum mengirim:

1. Pastikan solution berhasil dikompilasi.
2. Pastikan keempat fixture test terdeteksi dan seluruh test lulus.
3. Pastikan interface, project, dan test bawaan tidak berubah.

Di Visual Studio, gunakan **Test > Test Explorer**, lalu **Run All Tests**.

---
