Imports System.Reflection
Imports System.Data.Common

Partial Class DBHelper
    ''' <summary>
    ''' 映射完成后的表实体
    ''' </summary>
    ''' <remarks></remarks>
    Private Class TableEntity

        ''' <summary>
        ''' 表名
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TableName() As String = String.Empty

        ''' <summary>
        ''' 字段列表（所有相关字段）
        ''' </summary>
        ''' <remarks></remarks>
        Public Property Columns() As New Dictionary(Of String, Object)
        ''' <summary>
        ''' 字段与属性对照
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Properties As New Dictionary(Of String, PropertyInfo)
        ''' <summary>
        ''' 主键，生成Where 子句的字段列表
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property KeyNames() As New List(Of String)
        ''' <summary>
        ''' 生成SELECT语句的字段列表：包括cols+keys，不含Mapping:=none|write
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SelectColumnNames As New List(Of String)
        ''' <summary>
        ''' 生成INSERT语句的字段列表：包括cols+keys，不含Mapping:=none|read
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property InsertColumnNames As New List(Of String)
        ''' <summary>
        ''' 生成UPDATE语句的字段列表，包括cols，不含keys，Mapping:=none|read
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property UpdateColumnNames As New List(Of String)
    End Class

    ''' <summary>
    ''' 生成Command的要素
    ''' </summary>
    ''' <remarks></remarks>
    Private Class CommandEntity
        Implements IDisposable

        ''' <summary>
        ''' 可执行的Command
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property command As DbCommand

        ''' <summary>
        ''' SQL中包含的参数列表
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property parmNames As New List(Of String)

        ''' <summary>
        ''' 和参数对应的数据字字段名列表
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property columnNames As New List(Of String)

        ''' <summary>
        ''' 命令是否可以忽略
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property commandIgnore As Boolean = False

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)。
                    If command IsNot Nothing Then
                        command.Dispose()
                        command = Nothing
                    End If
                End If

                ' TODO: 释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO: 将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: 仅当上面的 Dispose(ByVal disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean)中。
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

End Class
