Imports JiangNong.DBHelper2

<TableMapping(Table:="dbo.ErrorLog")> Public Class ErrorLog
    <ColumnMapping(PrimaryKey:=True, Identity:=True)> Public Property ErrorLogID As Integer
    Public Property ErrorTime As DateTime = Now
    Public Property UserName As String
    Public Property ErrorNumber As Integer
    Public Property ErrorMessage As String

End Class
