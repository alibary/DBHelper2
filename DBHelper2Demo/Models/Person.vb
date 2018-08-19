Imports JiangNong.DBHelper2

''' <summary>
''' 定义映射到数据库的类
''' </summary>
<TableMapping(Table:="Person.Person")> Public Class Person
    Inherits BusinessEntity
    <ColumnMapping(PrimaryKey:=True, Identity:=False)> Public Overloads Property BusinessEntityID As Integer

    Public Property PersonType As String
    Public Property NameStyle As Boolean
    Public Property Title As String
    Public Property FirstName As String
    Public Property MiddleName As String
    Public Property LastName As String
    Public Property Suffix As String
    Public Property EmailPromotion As Integer
    Public Property AdditionalContactInfo As String
    Public Property Demographics As String
End Class
