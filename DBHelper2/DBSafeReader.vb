Partial Class DBHelper
    ''' <summary>
    ''' 转为字符串并转码，清除尾部空格。
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadStr(source As Object, Optional defaultValue As String = Nothing) As String
        If source Is Nothing OrElse source Is DBNull.Value OrElse (TypeOf (source) Is DateTime AndAlso CDate(source) < #1/1/1900#) Then
            If _ConvertNullToEmpty AndAlso defaultValue Is Nothing Then
                Return String.Empty
            Else
                Return defaultValue
            End If
        Else
            If _ConvertEncoding Then
                Return E2C(CStr(source)).TrimEnd
            Else
                Return CStr(source).TrimEnd
            End If
        End If
    End Function

    ''' <summary>
    ''' 布尔值转字符: True=Y, False=N
    ''' </summary>
    ''' <param name="booleanValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function BoolToStr(booleanValue As Object, Optional defaultValue As String = "N") As String
        Return SafeReader.BoolToStr(booleanValue, defaultValue)
    End Function

    ''' <summary>
    ''' 布尔值转整型:True=1, False=0
    ''' </summary>
    ''' <param name="booleanValue"></param>
    ''' <returns></returns>
    ''' <remarks>布尔值转整型:True=1, False=0</remarks>
    Public Function BoolToInt(booleanValue As Object, Optional defaultValue As Integer = 0) As Integer
        Return SafeReader.BoolToInt(booleanValue, defaultValue)
    End Function


    ''' <summary>
    ''' 字符型转布尔型
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadBool(value As Object, Optional defaultValue As Boolean = False) As Boolean
        Return SafeReader.ReadBool(value, defaultValue)
    End Function



    ''' <summary>
    ''' 转为日期，DBNULL转为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadDate(source As Object, Optional defaultValue As Date = Nothing) As Date
        Return SafeReader.ReadDate(source, defaultValue)
    End Function

    ''' <summary>
    ''' 转为长整型，无效时为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadLng(source As Object, Optional defaultValue As Long = Nothing) As Long
        Return SafeReader.ReadLng(source, defaultValue)
    End Function

    ''' <summary>
    ''' 转为整型，无效时为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadInt(source As Object, Optional defaultValue As Integer = Nothing) As Integer
        Return SafeReader.ReadInt(source, defaultValue)
    End Function

    ''' <summary>
    ''' 转为短整型，无效时为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadShort(source As Object, Optional defaultValue As Short = Nothing) As Short
        Return SafeReader.ReadShort(source, defaultValue)
    End Function

    ''' <summary>
    ''' 转为单精度浮点数，无效时为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadSng(source As Object, Optional defaultValue As Single = Nothing) As Single
        Return SafeReader.ReadSng(source, defaultValue)
    End Function

    ''' <summary>
    ''' 转为双精度浮点数，无效时为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadDbl(source As Object, Optional defaultValue As Double = Nothing) As Double
        Return SafeReader.ReadDbl(source, defaultValue)
    End Function

    ''' <summary>
    ''' 转为十进制数，无效时为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadDec(source As Object, Optional defaultValue As Decimal = Nothing) As Decimal
        Return SafeReader.ReadDec(source, defaultValue)
    End Function

    ''' <summary>
    ''' 把DBNULL转换为NULL
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadObj(source As Object, Optional defaultValue As Object = Nothing) As Object
        Return SafeReader.ReadObj(source, defaultValue)
    End Function

    ''' <summary>
    ''' 字符串写入数据库，空串转为DBNULL，并转码
    ''' </summary>
    ''' <param name="source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteStr(source As String) As Object
        If source Is Nothing Then
            Return DBNull.Value
        ElseIf _ConvertNullToEmpty AndAlso source.Trim = String.Empty Then
            Return DBNull.Value
        ElseIf _ConvertEncoding Then
            Return C2E(source)
        Else
            Return source
        End If
    End Function

    ''' <summary>
    ''' 日期型转为数据库类型
    ''' 空值和20世纪以前转为数据库空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteDate(source As String) As Object
        Return SafeReader.WriteDate(source)
    End Function

    Public Function WriteBool(source As String) As Object
        Return SafeReader.WriteBool(source)
    End Function
    Public Function WriteDbl(source As String) As Object
        Return SafeReader.WriteDbl(source)
    End Function
    Public Function WriteDec(source As String) As Object
        Return SafeReader.WriteDec(source)
    End Function
    Public Function WriteInt(source As String) As Object
        Return SafeReader.WriteInt(source)
    End Function
    Public Function WriteLng(source As String) As Object
        Return SafeReader.WriteLng(source)
    End Function
    Public Function WriteShort(source As String) As Object
        Return SafeReader.WriteShort(source)
    End Function
    Public Function WriteSng(source As String) As Object
        Return SafeReader.WriteSng(source)
    End Function
    ''' <summary>
    ''' 空值转为DBNULL
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteObj( source As Object) As Object
        Return SafeReader.WriteObj(source)
    End Function

    ''' <summary>
    ''' 日期型+字符型时间转换为Date型
    ''' </summary>
    ''' <param name="DatePart">日期</param>
    ''' <param name="TimePart">时间字符串(HH:MM:SS)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DateTimeStrToDate( datePart As Date,  timePart As String) As Date
        Return SafeReader.DateTimeStrToDate(datePart, timePart)
    End Function

    ''' <summary>
    ''' 8位数字转为日期
    ''' </summary>
    ''' <param name="D8">YYYYMMDD格式的日期值</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function D8ToDate( d8 As String) As Date
        Return SafeReader.D8ToDate(d8)
    End Function

    ''' <summary>
    ''' 14位数字转为日期
    ''' </summary>
    ''' <param name="D14">YYYYMMDDHHmmSS格式的日期值</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function D14ToDate( d14 As String) As Date
        Return SafeReader.D14ToDate(d14)
    End Function


End Class

