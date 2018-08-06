Imports System
Imports System.Data.Odbc
Imports System.Configuration

Public Class OdbcHelper
    Inherits DBHelper

    Public Sub New(connectionString As String)
        MyBase.New(connectionString)
    End Sub

    Protected Overrides Function ParameterPrefix(parameterName As String) As String
        Return "?"
    End Function

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
    Public Overloads Function ExecuteReader( cmdType As CommandType,  cmdText As String,  parmNames As String(),  parmValues As Object()) As OdbcDataReader
        Return CType(MyBase.ExecuteReader(cmdType, cmdText, parmNames, parmValues), OdbcDataReader)
    End Function
    ''' <summary>
    ''' 执行DataReader
    ''' </summary>
    ''' <param name="cmdText">命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <returns>返回DataRead</returns>
    ''' <remarks></remarks>
    Public Overloads Function ExecuteReader( cmdText As String,  parmNames As String(),  parmValues As Object()) As OdbcDataReader
        Return CType(MyBase.ExecuteReader(cmdText, parmNames, parmValues), OdbcDataReader)
    End Function

    ''' <summary>
    ''' 执行DataReader
    ''' </summary>
    ''' <param name="cmdText">命令文本</param>
    ''' <returns>返回DataRead</returns>
    ''' <remarks></remarks>
    Public Overloads Function ExecuteReader( cmdText As String) As OdbcDataReader
        Return CType(MyBase.ExecuteReader(cmdText), OdbcDataReader)
    End Function

#End Region

    Protected Overrides ReadOnly Property DBProvider As Common.DbProviderFactory
        Get
            Return OdbcFactory.Instance
        End Get
    End Property
End Class

