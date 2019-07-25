<System.AttributeUsage(AttributeTargets.Class)>
Public NotInheritable Class TableMapping
    Inherits Attribute

    ''' <summary>
    ''' 映射的数据库表名
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Table() As String = String.Empty

    ''' <summary>
    ''' 数据库主键字段名
    ''' </summary>
    ''' <returns></returns>
    Public Property Keys As String()
End Class


