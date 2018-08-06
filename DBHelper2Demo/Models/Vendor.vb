Imports JiangNong.DBHelper2

<TableMapping(Table:="Purchasing.Vendor")>
Public Class Vendor
    Inherits BusinessEntity
    <ColumnMapping(PrimaryKey:=True, Identity:=False)> Public Overloads Property BusinessEntityID As Integer

    Public Property AccountNumber As String
    Public Property Name As String
    Public Property CreditRating As Byte
    Public Property PreferredVendorStatus As Boolean
    Public Property ActiveFlag As Boolean
    Public Property PurchasingWebServiceURL As String

End Class
