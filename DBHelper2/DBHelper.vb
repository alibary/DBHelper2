Imports System.Configuration
Imports System.Data.Common
Imports System.Text
Imports System
Imports System.Globalization

''' <summary>
''' Database Access Helper
''' </summary>
''' <remarks>
''' 2013.2.19 Save方法中，如果映射的列在数据库中不存在，则忽略。
''' </remarks>
Public MustInherit Class DBHelper
    Implements IDisposable


#Region "私有变量"
    ' ''' <summary>
    ' ''' 组件工厂
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private _ProviderFactory As DbProviderFactory = Nothing
    ''' <summary>
    ''' 数据库连接
    ''' </summary>
    ''' <remarks></remarks>
    Private _Connection As DbConnection = Nothing
    ''' <summary>
    ''' 连接上的事务
    ''' </summary>
    ''' <remarks></remarks>
    Private _Transaction As DbTransaction = Nothing

    ''' <summary>
    ''' 是否需要转码
    ''' </summary>
    Private _ConvertEncoding As Boolean = False
    ''' <summary>
    ''' 读取数据库字符字段时是否将空值转为空串
    ''' </summary>
    Private _ConvertNullToEmpty As Boolean = True
    ''' <summary>
    ''' 命令执行超时
    ''' </summary>
    ''' <remarks></remarks>
    Private _CommandTimeout As Integer = 0
#End Region

    Protected Sub New(connectionString As String)
        'If providerFactory Is Nothing Then
        '    Throw New ArgumentException("providerFactory参数不能为空")
        'End If
        If String.IsNullOrEmpty(connectionString) Then
            Throw New ArgumentException("connectionString参数不能为空")
        End If

        '_ProviderFactory = providerFactory

        _Connection = DBProvider.CreateConnection

        GetConnectionFlag(connectionString,
                          _Connection.ConnectionString,
                          _ConvertEncoding,
                          _ConvertNullToEmpty,
                          _CommandTimeout)

    End Sub

    ''' <summary>
    ''' 分析连接串，获取配置
    ''' </summary>
    ''' <param name="ConfigString">配置文件提供的连接串</param>
    ''' <param name="ConnectionString">用于数据连接的串</param>
    ''' <param name="ConvertEncoding">是否转码标志</param>
    ''' <param name="ConvertNullToEmpty">是否转空值标志</param>
    ''' <remarks></remarks>
    Private Shared Sub GetConnectionFlag(ConfigString As String,
                                  ByRef ConnectionString As String,
                                  ByRef ConvertEncoding As Boolean,
                                  ByRef ConvertNullToEmpty As Boolean,
                                  ByRef CommandTimeout As Integer)
        Dim strConnectionString As New List(Of String)
        Dim strConvertEncoding As String = Nothing
        Dim strConvertNullToEmpaty As String = Nothing
        Dim strCommandTimeout As String = Nothing

        For Each s As String In ConfigString.Split(";"c) '以;分隔的配置串
            Dim t As String = s.Trim.ToLower(CultureInfo.CurrentCulture)
            If t.StartsWith("needconvertencoding", StringComparison.CurrentCulture) OrElse t.StartsWith("convertencoding", StringComparison.CurrentCulture) Then    '连接串中有NeedConvertEncoding属性
                strConvertEncoding = s
                Continue For
            ElseIf t.StartsWith("needconvertnulltoempty", StringComparison.CurrentCulture) OrElse t.StartsWith("convertnulltoempty", StringComparison.CurrentCulture) Then
                strConvertNullToEmpaty = s
                Continue For
            ElseIf t.StartsWith("commandtimeout", StringComparison.CurrentCulture) Then
                strCommandTimeout = s
                Continue For
            End If
            strConnectionString.Add(s)
        Next

        '转码配置
        If Not String.IsNullOrEmpty(strConvertEncoding) Then
            ConvertEncoding = SafeReader.ReadBool(GetValueSplitWithEqual(strConvertEncoding))
        End If
        '空值配置
        If Not String.IsNullOrEmpty(strConvertNullToEmpaty) Then
            ConvertNullToEmpty = SafeReader.ReadBool(GetValueSplitWithEqual(strConvertNullToEmpaty))
        End If
        '命令超时
        If Not String.IsNullOrEmpty(strCommandTimeout) Then
            CommandTimeout = SafeReader.ReadInt(GetValueSplitWithEqual(strCommandTimeout))
        End If
        '连接串
        ConnectionString = Join(strConnectionString.ToArray, ";")
    End Sub

    ''' <summary>
    ''' 获取等号后的配置值
    ''' </summary>
    ''' <param name="configString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetValueSplitWithEqual(configString As String) As String
        Dim a2 As String() = configString.Trim.Split("="c)
        If a2.Length > 1 Then
            Return a2(1).Trim
        Else
            Return String.Empty
        End If
    End Function

    ''' <summary>
    ''' 打开数据库连接
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Open()
        If _Connection.State = ConnectionState.Closed Then
            _Connection.Open()
        ElseIf _Connection.State <> ConnectionState.Open Then
            _Connection.Close()
            _Connection.Open()
        End If
    End Sub

    ''' <summary>
    ''' 关闭数据库连接
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Close()
        If _Connection.State <> ConnectionState.Closed Then
            CommitTransaction()
            _Connection.Close()
        End If
    End Sub

    ''' <summary>
    ''' 开启事务
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub BeginTransaction()
        If _Transaction Is Nothing Then
            Open()
            _Transaction = _Connection.BeginTransaction()
        End If
    End Sub

    ''' <summary>
    ''' 开启事务
    ''' </summary>
    ''' <param name="IsoLevel">事务的隔离级别</param>
    ''' <remarks></remarks>
    Public Sub BeginTransaction(isoLevel As IsolationLevel)
        If _Transaction Is Nothing Then
            Open()
            _Transaction = _Connection.BeginTransaction(IsoLevel)
        End If
    End Sub

    ''' <summary>
    ''' 提交事务（如果有）
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CommitTransaction()
        If _Transaction IsNot Nothing Then
            _Transaction.Commit()
            _Transaction = Nothing
        End If
    End Sub

    ''' <summary>
    ''' 回滚事务（如果有）
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RollbackTransaction()
        If _Transaction IsNot Nothing Then
            _Transaction.Rollback()
            _Transaction = Nothing
        End If
    End Sub


    ''' <summary>
    ''' ado.net对象工厂
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected MustOverride ReadOnly Property DBProvider As DbProviderFactory

    ''' <summary>
    ''' SQL参数前缀
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected MustOverride Function ParameterPrefix(parameterName As String) As String


    ''' <summary>
    ''' 取数据库连接
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Connection() As DbConnection
        Get
            Return _Connection
        End Get
    End Property

    ''' <summary>
    ''' 事务
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Transaction As DbTransaction
        Get
            Return _Transaction
        End Get
    End Property

    ''' <summary>
    ''' 是否需要转码标志
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ConvertEncoding As Boolean
        Get
            Return _ConvertEncoding
        End Get
        Set(value As Boolean)
            _ConvertEncoding = value
        End Set
    End Property

    ''' <summary>
    ''' 数据库空值转为空串否则转为空对象
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ConvertNullToEmpty As Boolean
        Get
            Return _ConvertNullToEmpty
        End Get
        Set(value As Boolean)
            _ConvertNullToEmpty = value
        End Set
    End Property
    ''' <summary>
    ''' 命令执行越时时限（秒）
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CommandTimeout As Integer
        Get
            Return _CommandTimeout
        End Get
        Set(value As Integer)
            _CommandTimeout = value
        End Set
    End Property

#Region "------------PrepareCommand------------"

    'Command对象预处理
    Private Sub prepareCommand(cmd As DbCommand, parmNames As String(), parmValues As Object())
        If parmNames Is Nothing OrElse parmValues Is Nothing Then
            Return
        End If
        If parmValues.Length <> parmNames.Length Then
            Throw New ArgumentException("查询参数名与值的数量不等。")
        End If
        cmd.Parameters.Clear()
        For i As Integer = 0 To parmValues.Length - 1
            Dim p1 As DbParameter = DBProvider.CreateParameter
            p1.ParameterName = parmNames(i)

            If parmValues(i) Is Nothing Then
                p1.Value = DBNull.Value
            ElseIf _ConvertEncoding AndAlso TypeOf (parmValues(i)) Is String Then
                p1.Value = C2E(CStr(parmValues(i)))
            ElseIf TypeOf (parmValues(i)) Is Date AndAlso CDate(parmValues(i)) < #1/1/1900# Then
                p1.Value = DBNull.Value
            Else
                p1.Value = parmValues(i)
            End If
            cmd.Parameters.Add(p1)
        Next
    End Sub

    Private Function prepareCommand(cmd As CommandEntity, paramValues As Dictionary(Of String, Object)) As DbCommand
        Dim v As New List(Of Object)
        For Each c As String In cmd.ColumnNames
            v.Add(paramValues(c))
        Next
        prepareCommand(cmd.command, cmd.ParmNames.ToArray, v.ToArray)
        Return cmd.command
    End Function

    Private Function createCommand(cmdType As CommandType, cmdText As String) As DbCommand
        Open()
        Dim cmd As DbCommand = DBProvider.CreateCommand
        cmd.CommandType = cmdType
        If _ConvertEncoding Then
            cmd.CommandText = C2E(cmdText)
        Else
            cmd.CommandText = cmdText
        End If
        cmd.Connection = _Connection
        If _Transaction IsNot Nothing Then
            cmd.Transaction = _Transaction
        End If
        If _CommandTimeout > 0 Then
            cmd.CommandTimeout = _CommandTimeout
        End If
        Return cmd
    End Function





#End Region



#Region "------------数据库转码------------"
    ''' <summary>
    ''' Informix字符编码修正
    ''' 从ISO-8859-1转为GB18030
    ''' 蒋农
    ''' 2009年11月
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 从Informix CSDK 2.8版本开始，对于用Informix默认DBLOCLE创建的数据库都无法读取中文
    ''' 分析原因，是因为字符在数据库中用ISO-8859-1编码，而在程序中要把它们理解为GB18030（GB2312），才能正常显示。
    ''' 所以从数据读取时需要人为地把ISO08859-1转为Gb2312或GB18030编码。
    ''' </remarks>
    Private Shared Function E2C(s As String) As String
        Return Encoder_GB18030.GetString(Encoder_ISO8859.GetBytes(s))
    End Function

    ''' <summary>
    ''' Informix字符编码修正
    ''' 从GB18030转为ISO-8859-1
    ''' 蒋农
    ''' 2009年11月
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' 从Informix CSDK 2.8版本开始，对于用Informix默认DBLOCLE创建的数据库都无法读取中文
    ''' 分析原因，是因为字符在数据库中用ISO-8859-1编码，而在程序中要把它们理解为GB18030（GB2312），才能正常显示。
    ''' 对于写回数据库的，需要人为地把GB18030（GB2312）转为ISO-8859-1
    ''' <remarks>
    ''' </remarks>
    Private Shared Function C2E(s As String) As String
        Return Encoder_ISO8859.GetString(Encoder_GB18030.GetBytes(s))
    End Function

    ''' <summary>
    ''' 编码器
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared Encoder_ISO8859 As Encoding = Encoding.GetEncoding("ISO-8859-1")
    Private Shared Encoder_GB18030 As Encoding = Encoding.GetEncoding("GB18030")

#End Region


#Region "IDisposable Support"
    Private disposedValue As Boolean ' 检测冗余的调用

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)。
                If _Transaction IsNot Nothing Then
                    _Transaction.Dispose()
                    _Transaction = Nothing
                End If

                If _Connection IsNot Nothing Then
                    _Connection.Dispose()
                    _Connection = Nothing
                End If
            End If

            ' TODO: 释放非托管资源(非托管对象)并重写下面的 Finalize()。
            ' TODO: 将大型字段设置为 null。
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: 仅当上面的 Dispose( disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
    'Protected Overrides Sub Finalize()
    '    ' 不要更改此代码。请将清理代码放入上面的 Dispose( disposing As Boolean)中。
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Visual Basic 添加此代码是为了正确实现可处置模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入上面的 Dispose (disposing As Boolean)中。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
