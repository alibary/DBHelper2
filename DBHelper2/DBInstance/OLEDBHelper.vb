Imports System.Data.OleDb

Public Class OledbHelper
    Inherits DBHelper


    Public Sub New( connectionString As String)
        MyBase.New(connectionString)
    End Sub


    'Protected Overrides ReadOnly Property DbProvider As System.Data.Common.DbProviderFactory
    '    Get
    '        Return OleDbFactory.Instance
    '    End Get
    'End Property


    Protected Overrides Function ParameterPrefix( parameterName As String) As String
        Return "?"
    End Function

    'Public Overloads Overrides Function TestConnection( ConnectionString As String) As String
    '    Try
    '        Dim cn As New OleDbConnection(ConnectionString)
    '        cn.Open()
    '        cn.Close()
    '        Return "OK"
    '    Catch ex As Exception
    '        Return ex.ToString
    '    End Try
    'End Function

#Region "------------ExecuteReader---------------"
    ''' <summary>
    ''' 执行DataReader
    ''' </summary>
    ''' <param name="cmdType">命令类型</param>
    ''' <param name="cmdText">命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <returns>返回DataRead</returns>
    ''' <remarks></remarks>
    Public Overloads Function ExecuteReader( cmdType As CommandType,  cmdText As String,  parmNames As String(),  parmValues As Object()) As OleDbDataReader
        Return CType(MyBase.ExecuteReader(cmdType, cmdText, parmNames, parmValues), OleDbDataReader)
    End Function
    ''' <summary>
    ''' 执行DataReader
    ''' </summary>
    ''' <param name="cmdText">命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <returns>返回DataRead</returns>
    ''' <remarks></remarks>
    Public Overloads Function ExecuteReader( cmdText As String,  parmNames As String(),  parmValues As Object()) As OleDbDataReader
        Return CType(MyBase.ExecuteReader(cmdText, parmNames, parmValues), OleDbDataReader)
    End Function

    ''' <summary>
    ''' 执行DataReader
    ''' </summary>
    ''' <param name="cmdText">命令文本</param>
    ''' <returns>返回DataRead</returns>
    ''' <remarks></remarks>
    Public Overloads Function ExecuteReader( cmdText As String) As OleDbDataReader
        Return CType(MyBase.ExecuteReader(cmdText), OleDbDataReader)
    End Function

#End Region

    Protected Overrides ReadOnly Property DBProvider As Common.DbProviderFactory
        Get
            Return OleDbFactory.Instance
        End Get
    End Property
End Class
