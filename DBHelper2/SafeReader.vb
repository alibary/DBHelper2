Imports System.Text
Imports System.Globalization

''' <summary>
''' 安全读取带有空值的对象
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class SafeReader
    Private Sub New()

    End Sub
    ''' <summary>
    ''' 布尔值转字符: True=Y, False=N
    ''' </summary>
    ''' <param name="booleanValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function BoolToStr(booleanValue As Object, Optional defaultValue As String = "N") As String
        If ReadBool(booleanValue) Then
            Return "Y"
        Else
            Return "N"
        End If
    End Function

    ''' <summary>
    ''' 布尔值转整型:True=1, False=0
    ''' </summary>
    ''' <param name="booleanValue"></param>
    ''' <returns></returns>
    ''' <remarks>布尔值转整型:True=1, False=0</remarks>
    Public Shared Function BoolToInt(booleanValue As Object, Optional defaultValue As Integer = 0) As Integer
            If ReadBool(booleanValue) Then
                Return 1
            Else
                Return 0
            End If
    End Function


    Private Shared trueString As String() = {"TRUE", "YES", "SUCCESS", "OK", "ON", "Y", "T", "1", "是", "对", "真"}
    Private Shared falseString As String() = {"FALSE", "NO", "FAIL", "ERROR", "OFF", "N", "F", "0", "否", "错", "假"}
    ''' <summary>                             
    ''' 字符型转布尔型                        
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ReadBool(value As Object, Optional defaultValue As Boolean = False) As Boolean
        If value Is Nothing OrElse value Is DBNull.Value Then
            Return defaultValue
        End If
        Dim stringValue As String = CStr(value).Trim.ToUpper(CultureInfo.CurrentCulture)
        If Array.IndexOf(trueString, StringValue) >= 0 Then
            Return True
        ElseIf Array.IndexOf(falseString, stringValue) >= 0 Then
            Return False
        Else
            Return defaultValue
        End If
    End Function

    ''' <summary>
    ''' 转为字符串，清除尾部空格
    ''' 日期类型小于1/1/1900为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ReadStr(source As Object, Optional defaultValue As String = "") As String
        If source Is Nothing OrElse source Is DBNull.Value OrElse (TypeOf (source) Is DateTime AndAlso CDate(source) < #1/1/1900#) Then
            Return defaultValue
        Else
            Return CStr(source).TrimEnd
        End If
    End Function

    ''' <summary>
    ''' 转为日期，DBNULL转为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ReadDate(source As Object, Optional defaultValue As Date = Nothing) As Date
        If source Is Nothing OrElse source Is DBNull.Value OrElse Not IsDate(source) Then
            Return defaultValue
        Else
            Return CDate(source)
        End If
    End Function

    ''' <summary>
    ''' 转为长整型，无效时为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ReadLng(source As Object, Optional defaultValue As Long = Nothing) As Long
        If source Is Nothing OrElse source Is DBNull.Value OrElse Not IsNumeric(source) Then
            Return defaultValue
        Else
            Return CLng(source)
        End If
    End Function

    ''' <summary>
    ''' 转为整型，无效时为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ReadInt(source As Object, Optional defaultValue As Integer = Nothing) As Integer
        If source Is Nothing OrElse source Is DBNull.Value OrElse Not IsNumeric(source) Then
            Return defaultValue
        Else
            Return CInt(source)
        End If
    End Function

    ''' <summary>
    ''' 转为短整型，无效时为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ReadShort(source As Object, Optional defaultValue As Short = Nothing) As Short
        If source Is Nothing OrElse source Is DBNull.Value OrElse Not IsNumeric(source) Then
            Return defaultValue
        Else
            Return CShort(source)
        End If
    End Function

    ''' <summary>
    ''' 转为单精度浮点数，无效时为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ReadSng(source As Object, Optional defaultValue As Single = Nothing) As Single
        If source Is Nothing OrElse source Is DBNull.Value OrElse Not IsNumeric(source) Then
            Return defaultValue
        Else
            Return CSng(source)
        End If
    End Function

    ''' <summary>
    ''' 转为双精度浮点数，无效时为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ReadDbl(source As Object, Optional defaultValue As Double = Nothing) As Double
        If source Is Nothing OrElse source Is DBNull.Value OrElse Not IsNumeric(source) Then
            Return defaultValue
        Else
            Return CDbl(source)
        End If
    End Function

    ''' <summary>
    ''' 转为十进制数，无效时为空值
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ReadDec(source As Object, Optional defaultValue As Decimal = Nothing) As Decimal
        If source Is Nothing OrElse source Is DBNull.Value OrElse Not IsNumeric(source) Then
            Return defaultValue
        Else
            Return CDec(source)
        End If
    End Function

    ''' <summary>
    ''' 把DBNULL转换为NULL
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ReadObj(source As Object, Optional defaultValue As Object = Nothing) As Object
        If source Is Nothing OrElse source Is DBNull.Value Then
            Return defaultValue
        Else
            Return source
        End If
    End Function

    ''' <summary>
    ''' 空串转为DBNULL
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function WriteStr(source As String) As Object
        If source Is Nothing Then
            Return DBNull.Value
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
    Public Shared Function WriteDate(source As String) As Object
        Dim d As Date
        If Date.TryParse(source, d) AndAlso d >= #1/1/1900# Then
            Return d
        Else
            Return DBNull.Value
        End If
    End Function
    Public Shared Function WriteBool(source As String) As Object
        Dim b As Boolean = False
        If Boolean.TryParse(source, b) Then
            Return b
        Else
            Return DBNull.Value
        End If
    End Function
    Public Shared Function WriteDbl(source As String) As Object
        Dim n As Double
        If Double.TryParse(source, n) Then
            Return n
        Else
            Return DBNull.Value
        End If
    End Function
    Public Shared Function WriteDec(source As String) As Object
        Dim n As Decimal
        If Decimal.TryParse(source, n) Then
            Return n
        Else
            Return DBNull.Value
        End If
    End Function
    Public Shared Function WriteInt(source As String) As Object
        Dim n As Integer
        If Integer.TryParse(source, n) Then
            Return n
        Else
            Return DBNull.Value
        End If
    End Function
    Public Shared Function WriteLng(source As String) As Object
        Dim n As Long
        If Long.TryParse(source, n) Then
            Return n
        Else
            Return DBNull.Value
        End If
    End Function
    Public Shared Function WriteShort(source As String) As Object
        Dim n As Short
        If Short.TryParse(source, n) Then
            Return n
        Else
            Return DBNull.Value
        End If
    End Function
    Public Shared Function WriteSng(source As String) As Object
        Dim n As Single
        If Single.TryParse(source, n) Then
            Return n
        Else
            Return DBNull.Value
        End If
    End Function

    ''' <summary>
    ''' 空值转为DBNULL
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function WriteObj(source As Object) As Object
        If Source Is Nothing Then
            Return DBNull.Value
        Else
            Return Source
        End If
    End Function

    ''' <summary>
    ''' 日期型+字符型时间转换为Date型
    ''' </summary>
    ''' <param name="DatePart">日期</param>
    ''' <param name="TimePart">时间字符串(HH:MM:SS)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function DateTimeStrToDate(datePart As Date, timePart As String) As Date
        Dim span As TimeSpan
        If TimeSpan.TryParse(TimePart, span) Then
            Return DatePart.Date.Add(span)
        Else
            Return DatePart.Date
        End If
    End Function

    ''' <summary>
    ''' 8位数字转为日期
    ''' </summary>
    ''' <param name="D8">YYYYMMDD格式的日期值</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function D8ToDate(d8 As String) As Date
        If d8 IsNot Nothing AndAlso d8.Length = 8 AndAlso IsNumeric(d8) Then
            Dim s As String = String.Format(CultureInfo.CurrentCulture, "{0}-{1}-{2}", _
                                            d8.Substring(0, 4), _
                                            d8.Substring(4, 2), _
                                            d8.Substring(6, 2))
            If IsDate(s) Then
                Return CDate(s)
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' 14位数字转为日期
    ''' </summary>
    ''' <param name="D14">YYYYMMDDHHmmSS格式的日期值</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function D14ToDate(d14 As String) As Date
        If D14 IsNot Nothing AndAlso D14.Length = 14 AndAlso IsNumeric(D14) Then
            Dim s As String = String.Format(CultureInfo.CurrentCulture, "{0}-{1}-{2} {3}:{4}:{5}", _
                                            D14.Substring(0, 4), _
                                            D14.Substring(4, 2), _
                                            D14.Substring(6, 2), _
                                            D14.Substring(8, 2), _
                                            D14.Substring(10, 2), _
                                            D14.Substring(12, 2))
            If IsDate(s) Then
                Return CDate(s)
            Else
                Return Nothing
            End If
        Else
            Return D8ToDate(D14)
        End If
    End Function


End Class

