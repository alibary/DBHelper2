Imports JiangNong.DBHelper2

<TableMapping(Table:="Person.BusinessEntity")>
Public Class BusinessEntity
    <ColumnMapping(PrimaryKey:=True, Identity:=True)> Public Property BusinessEntityID As Integer
    Public Property rowguid As Guid = Guid.NewGuid
    Public Property ModifiedDate As DateTime = Now
End Class
