<System.AttributeUsage(AttributeTargets.Class)>
Public NotInheritable Class TableMapping
    Inherits Attribute

    ''' <summary>
    ''' 映射的表名
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Table() As String = String.Empty

End Class


