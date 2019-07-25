Imports System.Configuration
Imports System.Data.Odbc
Imports System.Data.OleDb
Imports System.Data.SqlClient
'Imports Oracle.ManagedDataAccess.Client
Imports System.Data.Common
Imports System.Globalization
Imports System.Reflection

''' <summary>
''' DBHelper工厂
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class DBFactory
    Private Sub New()

    End Sub
#Region "数据库连接串"
    ''' <summary>
    ''' 允许用户初始化时修改此连接串
    ''' </summary>
    Private Shared _defaultConnectionStringName As String = "ConnectionString"
    ''' <summary>
    ''' 默认数据库连接串名称
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property DefaultConnectionStringName As String
        Get
            Return _defaultConnectionStringName
        End Get
        Set(value As String)
            _defaultConnectionStringName = value
        End Set
    End Property

    'Private Shared Function GetConnectionSettings(ConnectionStringName As String) As ConnectionStringSettings
    '    Dim cnSettings As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(ConnectionStringName)
    '    If cnSettings Is Nothing Then
    '        cnSettings = New ConnectionStringSettings(ConnectionStringName, ConnectionStringName, "")
    '    End If
    '    Return cnSettings
    'End Function

    ''' <summary>
    ''' 从配置文件中找到指定的数据库连接串。如果没有找到，直接返回字串。
    ''' </summary>
    ''' <param name="ConnectionStringName">数据库连接串名称</param>
    ''' <returns></returns>
    ''' <remarks></remarks> 
    Public Shared ReadOnly Property ConnectionString(connectionStringName As String) As String
        Get
            Dim cnSettings As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(connectionStringName)
            If cnSettings Is Nothing Then
                'Throw New ArgumentException("数据库连接串名无效：" & connectionStringName)
                Return connectionStringName
            End If
            Return cnSettings.ConnectionString
        End Get
    End Property

    ''' <summary>
    ''' 从配置文件中找到默认的数据库连接串，默认值由appSetting中的DefaultConnection指定，如不指定，连接串名称为ConnectionString。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property ConnectionString() As String
        Get
            Return ConnectionString(_defaultConnectionStringName)
        End Get
    End Property
#End Region

#Region "Create DBHelper"
    ''' <summary>
    ''' 根据指定的连接串名创建DBHelper，数据库类型由连接串的ProviderName决定
    ''' </summary>
    ''' <param name="ConnectionStringName">数据库连接串名称</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateDBHelper(connectionStringName As String) As DBHelper
        Dim cnSettings As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(connectionStringName)
        Dim _providerName As String
        Dim _connectionString As String
        If cnSettings Is Nothing Then
            'Throw New ArgumentException("数据库连接串名无效：" & connectionStringName)
            _connectionString = connectionStringName
            If _connectionString.ToLower.Contains("dsn=") Then
                _providerName = "odbc"
            Else
                _providerName = ""
            End If
        Else
            _providerName = cnSettings.ProviderName.ToLower
            _connectionString = cnSettings.ConnectionString
        End If
        Try
            If String.IsNullOrEmpty(_providerName) OrElse _providerName = "sqlhelper" OrElse _providerName = "sqlserver" Then
                Return New SqlHelper(_connectionString)
            ElseIf _providerName = "odbchelper" OrElse _providerName = "odbc" Then
                Return New OdbcHelper(_connectionString)
            ElseIf _providerName = "oledbhelper" OrElse _providerName = "oledb" Then
                Return New OledbHelper(_connectionString)
            ElseIf _providerName.Contains(",") Then  '指定的程序集
                Dim f As String() = _providerName.Split(","c)
                Dim AssemblyName As String = f(0).Trim
                Dim TypeName As String = f(1).Trim
                Dim helper As DBHelper = CType(Assembly.Load(AssemblyName).CreateInstance(TypeName, True, BindingFlags.Default, Nothing, {_connectionString}, Nothing, Nothing), DBHelper)
                Return helper
            Else
                Throw New ArgumentException("不能识别ProviderName：" & _providerName)
            End If
        Catch ex As Exception
            Throw New InvalidOperationException("创建DBHelper失败：" & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' 使用默认连接串创建一个DBHelper，数据库类型由连接串的ProviderName决定
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateDBHelper() As DBHelper
        Return CreateDBHelper(_defaultConnectionStringName)
    End Function
#End Region

#Region "Create ODBC Helper"
    ''' <summary>
    ''' 使用默认连接串名称创建一个ODBCHelper
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateOdbcHelper() As OdbcHelper
        Return New OdbcHelper(ConnectionString)
    End Function

    ''' <summary>
    ''' 使用指定的连接串创建一个ODBCHelper
    ''' </summary>
    ''' <param name="ConnectionStringName">数据库连接串名称</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateOdbcHelper(connectionStringName As String) As OdbcHelper
        Return New OdbcHelper(ConnectionString(connectionStringName))
    End Function

#End Region

#Region "Create OLEDB Helper"
    ''' <summary>
    ''' 使用默认连接串名称创建一个OLEDBHelper
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateOledbHelper() As OledbHelper
        Return New OledbHelper(ConnectionString)
    End Function
    ''' <summary>
    ''' 使用默认连接串创建一个OLEDBHelper
    ''' </summary>
    ''' <param name="ConnectionStringName">数据库连接串名称</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateOledbHelper(connectionStringName As String) As OledbHelper
        Return New OledbHelper(ConnectionString(connectionStringName))
    End Function

#End Region

#Region "Create SQL Helper"
    ''' <summary>
    ''' 使用默认连接串创建一个SQLSERVERHelper
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateSqlHelper() As SqlHelper
        Return New SqlHelper(ConnectionString)
    End Function

    ''' <summary>
    ''' 使用指定的连接串创建一个SQLSERVERHelper
    ''' </summary>
    ''' <param name="ConnectionStringName">数据库连接串名称</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateSqlHelper(connectionStringName As String) As SqlHelper
        Return New SqlHelper(ConnectionString(connectionStringName))
    End Function

#End Region

    '#Region "Create Oracle Helper"
    '    Public Shared Function CreateOracleHelper() As OracleHelper
    '        Return New OracleHelper(New OracleConnection(ConnectionString()))
    '    End Function

    '    Public Shared Function CreateOracleHelper(ConnectionStringName As String) As OracleHelper
    '        Return New OracleHelper(New OracleConnection(ConnectionString(ConnectionStringName)))
    '    End Function
    '#End Region


End Class
