
<System.AttributeUsage(AttributeTargets.Property)>
Public NotInheritable Class ColumnMapping
    Inherits Attribute

    ''' <summary>
    ''' 映射的列名
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Column() As String = String.Empty

    ''' <summary>
    ''' 数据库中列的类型
    ''' </summary>
    ''' <returns></returns>
    Public Property ColumnType As EnumColumnType = EnumColumnType.Object

    ''' <summary>
    ''' 是否要映射
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Mapping() As EnumMapping = EnumMapping.Both

    ''' <summary>
    ''' 是否主键
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>至少指定一个字段为主键</remarks>
    Public Property PrimaryKey() As Boolean = False


    ''' <summary>
    ''' 是否标识字段(自增)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>标识字段在Insert操作时忽略</remarks>
    Public Property Identity() As Boolean = False

End Class

Public Enum EnumMapping
    None = 0
    Read = 1
    Write = 2
    Both = 3
End Enum

Public Enum EnumColumnType
    [Object]
    [Boolean]
    [String]
    [Date]
    [Long]
    [Integer]
    [Short]
    [Single]
    [Double]
    [Decimal]
    BoolToStr
    BoolToInt
End Enum
