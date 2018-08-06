Imports System.Data.Common
Imports System.Reflection
Imports System.Text
Imports System.Globalization

Partial Class DBHelper
#Region "------------ExecuteNonQuery---------------"
    ''' <summary>
    ''' 执行非查询SQL
    ''' </summary>
    ''' <param name="cmdType">命令类型</param>
    ''' <param name="cmdText">SQL文本</param>
    ''' <param name="parmNames">SQL参数名</param>
    ''' <param name="parmValues">SQL参数值</param>
    ''' <returns>返回SQL语句影响到的记录行数</returns>
    ''' <remarks></remarks>
    Public Function ExecuteNonQuery(cmdType As CommandType, cmdText As String, parmNames As String(), parmValues As Object()) As Integer
        Try
            Dim cmd As DbCommand = createCommand(cmdType, cmdText)
            prepareCommand(cmd, parmNames, parmValues)
            Dim Result As Integer = cmd.ExecuteNonQuery()
            'cmd.Parameters.Clear()
            cmd = Nothing
            Return Result
        Catch
            Throw
        End Try
    End Function
    ''' <summary>
    ''' 执行非查询SQL
    ''' </summary>
    ''' <param name="cmdText">SQL文本</param>
    ''' <param name="parmNames">SQL参数名</param>
    ''' <param name="parmValues">SQL参数值</param>
    ''' <returns>返回SQL语句影响到的记录行数</returns>
    ''' <remarks></remarks>
    Public Function ExecuteNonQuery(cmdText As String, parmNames As String(), parmValues As Object()) As Integer
        Return ExecuteNonQuery(CommandType.Text, cmdText, parmNames, parmValues)
    End Function
    ''' <summary>
    ''' 执行非查询SQL
    ''' </summary>
    ''' <param name="cmdText">SQL文本</param>
    ''' <returns>返回SQL语句影响到的记录行数</returns>
    ''' <remarks></remarks>
    Public Function ExecuteNonQuery(cmdText As String) As Integer
        Return ExecuteNonQuery(cmdText, Nothing, Nothing)
    End Function

#End Region

#Region "------------ExecuteReader---------------"
    ''' <summary>
    ''' 执行DataReader
    ''' </summary>
    ''' <param name="cmdType">命令类型</param>
    ''' <param name="cmdText">命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <returns>返回DataReader</returns>
    ''' <remarks></remarks>
    Public Function ExecuteReader(cmdType As CommandType, cmdText As String, parmNames As String(), parmValues As Object()) As DbDataReader
        '返回DataReader时不能使用using自动回收Connection
        '返回ODBCDatareader后，仍不能清除Parmaters，也不能关闭连接
        '考虑到DataReader在一个连接中可能会执行多次，取消强行关闭连接功能。2015.12.2
        Try
            Dim cmd As DbCommand = createCommand(cmdType, cmdText)
            prepareCommand(cmd, parmNames, parmValues)
            'Dim dr As DbDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
            Dim dr As DbDataReader = cmd.ExecuteReader()
            'cmd.Parameters.Clear()
            cmd = Nothing
            Return dr
        Catch
            Throw
        End Try
    End Function
    ''' <summary>
    ''' 执行DataReader
    ''' </summary>
    ''' <param name="cmdText">命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <returns>返回DataReader</returns>
    ''' <remarks></remarks>
    Public Function ExecuteReader(cmdText As String, parmNames As String(), parmValues As Object()) As DbDataReader
        Return ExecuteReader(CommandType.Text, cmdText, parmNames, parmValues)
    End Function

    ''' <summary>
    ''' 执行DataReader
    ''' </summary>
    ''' <param name="cmdText">命令文本</param>
    ''' <returns>返回DataReader</returns>
    ''' <remarks></remarks>
    Public Function ExecuteReader(cmdText As String) As DbDataReader
        Return ExecuteReader(cmdText, Nothing, Nothing)
    End Function

#End Region

#Region "------------ExecuteScalar---------------"

    ''' <summary>
    ''' 执行Scalar
    ''' </summary>
    ''' <param name="cmdType">命令类型</param>
    ''' <param name="cmdText">命令文本</param>
    ''' <param name="parmNames">命令参数名</param>
    ''' <param name="parmValues">命令参数值</param>
    ''' <returns>返回Scalar</returns>
    ''' <remarks></remarks>
    Public Function ExecuteScalar(cmdType As CommandType, cmdText As String, parmNames As String(), parmValues As Object()) As Object
        Try
            Dim cmd As DbCommand = createCommand(cmdType, cmdText)
            prepareCommand(cmd, parmNames, parmValues)
            Dim Result As Object = cmd.ExecuteScalar()
            'cmd.Parameters.Clear()
            cmd = Nothing
            Return Result
        Catch
            Throw
        End Try
    End Function

    ''' <summary>
    ''' 执行Scalar
    ''' </summary>
    ''' <param name="cmdText">命令文本</param>
    ''' <param name="parmNames">命令参数名</param>
    ''' <param name="parmValues">命令参数值</param>
    ''' <returns>返回Scalar</returns>
    ''' <remarks></remarks>
    Public Function ExecuteScalar(cmdText As String, parmNames As String(), parmValues As Object()) As Object
        Return ExecuteScalar(CommandType.Text, cmdText, parmNames, parmValues)
    End Function

    ''' <summary>
    ''' 执行Scalar
    ''' </summary>
    ''' <param name="cmdText">命令文本</param>
    ''' <returns>返回Scalar</returns>
    ''' <remarks></remarks>
    Public Function ExecuteScalar(cmdText As String) As Object
        Return ExecuteScalar(cmdText, Nothing, Nothing)
    End Function
#End Region

#Region "------------ReadTable------------"
    ''' <summary>
    ''' 读数据集
    ''' </summary>
    ''' <param name="cmdType">查询命令类型</param>
    ''' <param name="cmdText">查询命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <param name="startRecord">起始记录位置</param>
    ''' <param name="maxRecords">最大记录数</param>
    ''' <remarks></remarks>
    Public Function ReadTable(cmdType As CommandType, cmdText As String, parmNames As String(), parmValues As Object(), startRecord As Integer, maxRecords As Integer) As DataTable
        Try
            Using dt As New DataTable     '新建一个表
                dt.Locale = CultureInfo.CurrentCulture
                Dim da As DbDataAdapter = DBProvider.CreateDataAdapter
                da.SelectCommand = createCommand(cmdType, cmdText)
                prepareCommand(da.SelectCommand, parmNames, parmValues)
                If maxRecords >= 0 Then
                    da.Fill(startRecord, maxRecords, dt)
                Else
                    da.Fill(dt)
                End If
                da = Nothing

                If _ConvertEncoding Then
                    With dt '转码
                        For i As Integer = 0 To .Columns.Count - 1
                            If .Columns(i).DataType Is GetType(String) Then
                                For j As Integer = 0 To .Rows.Count - 1
                                    If .Rows(j).Item(i) IsNot Nothing AndAlso .Rows(j).Item(i) IsNot DBNull.Value Then
                                        .Rows(j).Item(i) = E2C(CStr(.Rows(j).Item(i)))
                                    End If
                                Next
                            End If
                        Next
                        .AcceptChanges()
                    End With
                End If
                Return dt
            End Using
        Catch
            Throw
        End Try
    End Function

    ''' <summary>
    ''' 读数据集
    ''' </summary>
    ''' <param name="cmdText">查询命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <param name="startRecord">起始记录位置</param>
    ''' <param name="maxRecords">最大记录数</param>
    ''' <remarks></remarks>
    Public Function ReadTable(cmdText As String, parmNames As String(), parmValues As Object(), startRecord As Integer, maxRecords As Integer) As DataTable
        Return ReadTable(CommandType.Text, cmdText, parmNames, parmValues, startRecord, maxRecords)
    End Function

    ''' <summary>
    ''' 读数据集
    ''' </summary>
    ''' <param name="cmdText">查询命令文本</param>
    ''' <param name="startRecord">起始记录位置</param>
    ''' <param name="maxRecords">最大记录数</param>
    ''' <remarks></remarks>
    Public Function ReadTable(cmdText As String, startRecord As Integer, maxRecords As Integer) As DataTable
        Return ReadTable(CommandType.Text, cmdText, Nothing, Nothing, startRecord, maxRecords)
    End Function

    ''' <summary>
    ''' 读数据集
    ''' </summary>
    ''' <param name="cmdType">查询命令类型</param>
    ''' <param name="cmdText">查询命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <remarks></remarks>
    Public Function ReadTable(cmdType As CommandType, cmdText As String, parmNames As String(), parmValues As Object()) As DataTable
        Return ReadTable(cmdType, cmdText, parmNames, parmValues, 0, -1)
    End Function

    ''' <summary>
    ''' 读数据集
    ''' </summary>
    ''' <param name="cmdText">查询命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <remarks></remarks>
    Public Function ReadTable(cmdText As String, parmNames As String(), parmValues As Object()) As DataTable
        Return ReadTable(CommandType.Text, cmdText, parmNames, parmValues, 0, -1)
    End Function

    ''' <summary>
    ''' 读数据集
    ''' </summary>
    ''' <param name="cmdText">查询命令文本</param>
    ''' <remarks></remarks>
    Public Function ReadTable(cmdText As String) As DataTable
        Return ReadTable(CommandType.Text, cmdText, Nothing, Nothing)
    End Function

#End Region

#Region "---------------SaveTable---------------"
    Public Function SaveTable(table As DataTable, tableName As String, keys As String()) As Integer
        Return SaveTable(table, tableName, keys, Nothing)
    End Function
    ''' <summary>
    ''' 将数据集保存到数据库
    ''' </summary>
    ''' <param name="table">数据集</param>
    ''' <param name="tableName">数据库中的表名，如果省略，由table中的TableName确定</param>
    ''' <param name="keys">指定的主键，如果省略，由table中的PrimaryKey确定</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SaveTable(table As DataTable, tableName As String, keys As String(), cols As String()) As Integer
        If table Is Nothing OrElse table.Rows.Count = 0 Then
            Return 0
        End If
        'If keys Is Nothing OrElse keys.Length = 0 Then
        '    Throw New ArgumentException("保存数据表失败：未指定主键")
        'End If

        Dim te As TableEntity = GetMappingTable(table, tableName, keys, cols)

        'Dim cmdExists As CommandEntity = BuildExistsCommandText(te)
        Dim cmdInsert As CommandEntity = BuildInsertCommandText(te)
        Dim cmdUpdate As CommandEntity = BuildUpdateCommandText(te)

        Dim Result As Integer = 0 '受影响的记录数
        Dim v As New Dictionary(Of String, Object)
        For Each r As DataRow In table.Rows
            'table中的数据
            v.Clear()
            For Each k As String In te.Columns.Keys
                v.Add(k, r.Item(k))
            Next
            Dim u As Integer
            If cmdUpdate.commandIgnore Then '未指定主键，忽略update操作
                u = 0
            Else
                u = prepareCommand(cmdUpdate, v).ExecuteNonQuery() '先尝试update
            End If
            If u = 0 Then
                u = prepareCommand(cmdInsert, v).ExecuteNonQuery  '然后执行insert
            End If

            Result += u
        Next
        cmdInsert = Nothing
        cmdUpdate = Nothing
        Return Result
    End Function

#End Region

#Region "----------WriteToDb----------"
    ''' <summary>
    ''' 将DataReader结果保存到数据库
    ''' </summary>
    ''' <param name="reader">要读取数据的DataReader组件</param>
    ''' <param name="dbSave">要保存数据的DbHelper组件</param>
    ''' <param name="tableName">指定保存数据的表</param>
    ''' <param name="keys">指定主键</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteToDb(reader As DbDataReader, dbSave As DBHelper, tableName As String, keys As String()) As Integer
        Return WriteToDb(reader, dbSave, tableName, keys, Nothing)
    End Function

    ''' <summary>
    ''' 将DataReader结果保存到数据库
    ''' </summary>
    ''' <param name="reader">要读取数据的DataReader组件</param>
    ''' <param name="dbSave">要保存数据的DbHelper组件</param>
    ''' <param name="tableName">指定保存数据的表</param>
    ''' <param name="keys">指定主键</param>
    ''' <param name="cols">要保存的字段列表</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteToDb(reader As DbDataReader, dbSave As DBHelper, tableName As String, keys As String(), cols As String()) As Integer
        If reader Is Nothing OrElse reader.HasRows = False Then
            Return 0
        End If

        Dim table As TableEntity = dbSave.GetMappingTable(reader, tableName, keys, cols)

        'Dim cmdExists As CommandEntity = dbSave.BuildExistsCommandText(table)
        Dim cmdInsert As CommandEntity = dbSave.BuildInsertCommandText(table)
        Dim cmdUpdate As CommandEntity = dbSave.BuildUpdateCommandText(table)

        Dim Result As Integer = 0 '受影响的记录数
        Dim v As New Dictionary(Of String, Object)
        Do While reader.Read()
            '源数据中的参数值
            v.Clear()
            For Each k As String In table.Columns.Keys
                If reader.GetFieldType(reader.GetOrdinal(k)) Is GetType(String) Then
                    v.Add(k, ReadStr(reader(k)))
                Else
                    v.Add(k, ReadObj(reader(k)))
                End If
            Next
            'If dbSave.prepareCommand(cmdExists, v).ExecuteScalar Is Nothing Then
            '    Result += dbSave.prepareCommand(cmdInsert, v).ExecuteNonQuery
            'Else
            '    Result += dbSave.prepareCommand(cmdUpdate, v).ExecuteNonQuery
            'End If
            Dim u As Integer
            If cmdUpdate.commandIgnore Then
                u = 0
            Else
                u = dbSave.prepareCommand(cmdUpdate, v).ExecuteNonQuery
            End If
            If u = 0 Then
                u = dbSave.prepareCommand(cmdInsert, v).ExecuteNonQuery
            End If
            Result += u
        Loop
        reader.Close()
        cmdInsert = Nothing
        cmdUpdate = Nothing
        Return Result
    End Function

#End Region

#Region "------------ReadList------------"

    ''' <summary>
    ''' 读取指定类型的数据列表
    ''' </summary>
    ''' <typeparam name="T">指定数据类型</typeparam>
    ''' <param name="cmdText">命令文本</param>
    ''' <returns>数据列表</returns>
    ''' <remarks></remarks>
    Public Function ReadList(Of T)(cmdText As String) As List(Of T)
        Return ReadList(Of T)(cmdText, Nothing, Nothing, 0, -1)
    End Function

    ''' <summary>
    ''' 读取指定类型的数据列表
    ''' </summary>
    ''' <typeparam name="T">指定数据类型</typeparam>
    ''' <param name="cmdText">命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <returns>数据列表</returns>
    ''' <remarks></remarks>
    Public Function ReadList(Of T)(cmdText As String, parmNames As String(), parmValues As Object()) As List(Of T)
        Return ReadList(Of T)(CommandType.Text, cmdText, parmNames, parmValues, 0, -1)
    End Function

    ''' <summary>
    ''' 读取指定类型的数据列表
    ''' </summary>
    ''' <typeparam name="T">指定数据类型</typeparam>
    ''' <param name="cmdType">命令类型</param>
    ''' <param name="cmdText">命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <returns>数据列表</returns>
    ''' <remarks></remarks>
    Public Function ReadList(Of T)(cmdType As CommandType, cmdText As String, parmNames As String(), parmValues As Object()) As List(Of T)
        Return ReadList(Of T)(cmdType, cmdText, parmNames, parmValues, 0, -1)
    End Function


    ''' <summary>
    ''' 读取指定类型的数据列表
    ''' </summary>
    ''' <typeparam name="T">指定数据类型</typeparam>
    ''' <param name="cmdText">命令文本</param>
    ''' <param name="startRecord">起始记录位置</param>
    ''' <param name="maxRecords">最大填充记录数</param>
    ''' <returns>数据列表</returns>
    ''' <remarks></remarks>
    Public Function ReadList(Of T)(cmdText As String, startRecord As Integer, maxRecords As Integer) As List(Of T)
        Return ReadList(Of T)(CommandType.Text, cmdText, Nothing, Nothing, startRecord, maxRecords)
    End Function


    ''' <summary>
    ''' 读取指定类型的数据列表
    ''' </summary>
    ''' <typeparam name="T">指定数据类型</typeparam>
    ''' <param name="cmdText">命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <param name="startRecord">起始记录位置</param>
    ''' <param name="maxRecords">最大填充记录数</param>
    ''' <returns>数据列表</returns>
    ''' <remarks></remarks>
    Public Function ReadList(Of T)(cmdText As String, parmNames As String(), parmValues As Object(), startRecord As Integer, maxRecords As Integer) As List(Of T)
        Return ReadList(Of T)(CommandType.Text, cmdText, parmNames, parmValues, startRecord, maxRecords)
    End Function

    ''' <summary>
    ''' 读取指定类型的数据列表
    ''' </summary>
    ''' <typeparam name="T">指定数据类型</typeparam>
    ''' <param name="cmdType">命令类型</param>
    ''' <param name="cmdText">命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <param name="startRecord">起始记录位置</param>
    ''' <param name="maxRecords">最大填充记录数</param>
    ''' <returns>数据列表</returns>
    ''' <remarks></remarks>
    Public Function ReadList(Of T)(cmdType As CommandType, cmdText As String, parmNames As String(), parmValues As Object(), startRecord As Integer, maxRecords As Integer) As List(Of T)
        Dim l As New List(Of T)
        Dim i As Integer = 0
        Dim reader As DbDataReader = ExecuteReader(cmdType, cmdText, parmNames, parmValues)
        Dim pojo As T = Activator.CreateInstance(Of T)()
        Dim map As Dictionary(Of String, PropertyInfo) = GetMappingProperty(reader, pojo, Nothing)
        Do While reader.Read
            i += 1
            If i <= startRecord Then
                Continue Do
            End If
            If maxRecords > 0 AndAlso i > startRecord + maxRecords Then
                Exit Do
            End If

            Dim p As T = Activator.CreateInstance(Of T)()
            SetPojoValue(reader, p, map)
            l.Add(p)
        Loop
        reader.Close()
        reader = Nothing
        Return l
    End Function
#End Region

#Region "----------SaveList----------"

    ''' <summary>
    ''' 保存数据列表
    ''' </summary>
    ''' <typeparam name="T">列表类型</typeparam>
    ''' <param name="data">列表数据</param>
    ''' <returns>受影响的记录数</returns>
    ''' <remarks></remarks>
    Public Function SaveList(Of T)(data As List(Of T)) As Integer
        Return SaveList(Of T)(data, Nothing, Nothing)
    End Function

    ''' <summary>
    ''' 保存数据列表
    ''' </summary>
    ''' <typeparam name="T">列表类型</typeparam>
    ''' <param name="data">列表数据</param>
    ''' <param name="keys">指定主键</param>
    ''' <returns>受影响的记录数</returns>
    ''' <remarks></remarks>
    Public Function SaveList(Of T)(data As List(Of T), keys As String()) As Integer
        Return SaveList(Of T)(data, keys, Nothing)
    End Function
    ''' <summary>
    ''' 保存列表
    ''' </summary>
    ''' <typeparam name="T">列表类型</typeparam>
    ''' <param name="data">列表数据</param>
    ''' <param name="keys">指定主键</param>
    ''' <returns>受影响的记录数</returns>
    ''' <remarks></remarks>
    Public Function SaveList(Of T)(data As List(Of T), keys As String(), cols As String()) As Integer
        If data Is Nothing OrElse data.Count = 0 Then
            Return 0
        End If
        Dim pojo As T

        For Each p As T In data
            If p IsNot Nothing Then
                pojo = p
                Exit For
            End If
        Next
        If pojo Is Nothing Then
            Return 0
        End If

        Dim tp As Type = pojo.GetType
        If tp.IsPrimitive OrElse tp.IsValueType OrElse tp.IsEnum OrElse tp.IsInterface Then
            Return 0
        End If

        Dim table As TableEntity = GetMappingTable(pojo, keys, cols)

        ' Dim cmdExists As CommandEntity = BuildExistsCommandText(table)
        Dim cmdInsert As CommandEntity = BuildInsertCommandText(table)
        Dim cmdUpdate As CommandEntity = BuildUpdateCommandText(table)

        Dim Result As Integer = 0
        For Each p As T In data
            If p Is Nothing Then
                Continue For
            End If
            '对象中的数据转为参数
            Dim v As New Dictionary(Of String, Object)
            For Each k As String In table.Columns.Keys
                v.Add(k, table.Properties(k).GetValue(p, Nothing))
            Next
            Dim u As Integer
            If cmdUpdate.commandIgnore Then
                u = 0
            Else
                u = prepareCommand(cmdUpdate, v).ExecuteNonQuery()
            End If
            If u = 0 Then
                u = prepareCommand(cmdInsert, v).ExecuteNonQuery
            End If
            Result += u
        Next
        cmdInsert = Nothing
        cmdUpdate = Nothing
        Return Result

    End Function

#End Region


#Region "------------ReadEntity------------"
    ''' <summary>
    ''' 读取指定类型的数据对象
    ''' </summary>
    ''' <typeparam name="T">指定数据类型</typeparam>
    ''' <param name="cmdType">命令类型</param>
    ''' <param name="cmdText">命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <returns>读取的对象</returns>
    ''' <remarks></remarks>
    Public Function ReadEntity(Of T)(cmdType As CommandType, cmdText As String, parmNames As String(), parmValues As Object()) As T
        Dim p As T = Nothing
        Dim reader As DbDataReader = ExecuteReader(cmdType, cmdText, parmNames, parmValues)
        If reader.Read Then
            p = Activator.CreateInstance(Of T)()
            Load(reader, p)
        End If
        reader.Close()
        reader = Nothing
        Return p
    End Function

    ''' <summary>
    ''' 读取指定类型的数据对象
    ''' </summary>
    ''' <typeparam name="T">指定数据类型</typeparam>
    ''' <param name="cmdText">命令文本</param>
    ''' <param name="parmNames">参数名</param>
    ''' <param name="parmValues">参数值</param>
    ''' <returns>读取的对象</returns>
    ''' <remarks></remarks>
    Public Function ReadEntity(Of T)(cmdText As String, parmNames As String(), parmValues As Object()) As T
        Return ReadEntity(Of T)(CommandType.Text, cmdText, parmNames, parmValues)
    End Function

    ''' <summary>
    ''' 读取指定类型的数据对象
    ''' </summary>
    ''' <typeparam name="T">指定数据类型</typeparam>
    ''' <param name="cmdText">命令文本</param>
    ''' <returns>读取的对象</returns>
    ''' <remarks></remarks>
    Public Function ReadEntity(Of T)(cmdText As String) As T
        Return ReadEntity(Of T)(cmdText, Nothing, Nothing)
    End Function
#End Region

#Region "------------Load------------"

    ''' <summary>
    ''' 从DataReader中读取对象的值
    ''' </summary>
    ''' <param name="reader">数据源</param>
    ''' <param name="pojo">要读取数据的对象</param>
    ''' <returns>读取是否成功</returns>
    ''' <remarks>忽略数据源中不存在的列</remarks>
    Public Function Load(reader As DbDataReader, pojo As Object) As Boolean
        Return Load(reader, pojo, Nothing)
    End Function


    ''' <summary>
    ''' 从DataReader中读取对象的值
    ''' </summary>
    ''' <param name="reader">数据源</param>
    ''' <param name="pojo">要读取数据的对象</param>
    ''' <param name="cols">指定读取的列,可以为空</param>
    ''' <returns>读取是否成功</returns>
    ''' <remarks>忽略数据源中不存在的列</remarks>
    Public Function Load(reader As DbDataReader, pojo As Object, cols As String()) As Boolean
        If pojo Is Nothing Then
            Throw New ArgumentException("装入对象失败：POJO对象不能为空")
        End If
        If reader Is Nothing Then
            Throw New ArgumentException("装入对象失败：DbDataReader对象不能为空")
        End If
        If reader.IsClosed OrElse reader.HasRows = False Then
            Return False
        End If
        Dim map As Dictionary(Of String, PropertyInfo) = GetMappingProperty(reader, pojo, cols)
        SetPojoValue(reader, pojo, map)
        Return True
    End Function



    ''' <summary>
    ''' 根据对象的结构读取数据
    ''' </summary>
    ''' <param name="pojo">要读取数据的对象</param>
    ''' <returns>读取是否成功</returns>
    ''' <remarks></remarks>
    Public Function Load(pojo As Object) As Boolean
        Return Load(pojo, Nothing, Nothing)
    End Function

    ''' <summary>
    ''' 根据对象的结构读取数据
    ''' </summary>
    ''' <param name="pojo">要读取数据的对象</param>
    ''' <param name="keys">指定主键，可以为空</param>
    ''' <returns>读取是否成功</returns>
    ''' <remarks></remarks>
    Public Function Load(pojo As Object, keys As String()) As Boolean
        Return Load(pojo, keys, Nothing)
    End Function

    ''' <summary>
    ''' 根据对象的结构读取数据
    ''' </summary>
    ''' <param name="pojo">要读取数据的对象</param>
    ''' <param name="keys">指定主键，可以为空</param>
    ''' <param name="cols">指定读取的列，可以为空</param>
    ''' <returns>读取是否成功</returns>
    ''' <remarks></remarks>
    Public Function Load(pojo As Object, keys As String(), cols As String()) As Boolean
        '映射表和字段
        Dim table As TableEntity = GetMappingTable(pojo, keys, cols)
        Dim cmd As CommandEntity = BuildSelectCommandText(table)
        Dim dr As DbDataReader = PrepareCommand(cmd, table.Columns).ExecuteReader(CommandBehavior.SingleRow)
        Dim Result As Boolean = False
        If dr.Read Then
            Result = Load(dr, pojo, cols)
        End If
        dr.Close()
        dr = Nothing
        cmd = Nothing
        Return Result
    End Function
#End Region

#Region "---------Save----------"
    ''' <summary>
    ''' 根据对象的结构保存数据
    ''' </summary>
    ''' <param name="pojo">要保存数据的对象</param>
    ''' <returns>受影响的记录数</returns>
    ''' <remarks></remarks>
    Public Function Save(pojo As Object) As Integer
        Return Save(pojo, Nothing, Nothing)
    End Function

    ''' <summary>
    ''' 根据对象的结构保存数据
    ''' </summary>
    ''' <param name="pojo">要保存数据的对象</param>
    ''' <param name="keys">指定的主键，可以为空</param>
    ''' <returns>受影响的记录数</returns>
    Public Function Save(pojo As Object, keys As String()) As Integer
        Return Save(pojo, keys, Nothing)
    End Function

    ''' <summary>
    ''' 根据对象的结构保存数据
    ''' </summary>
    ''' <param name="pojo">要保存数据的对象</param>
    ''' <param name="cols">指定要保存的列，可以为空</param>
    ''' <param name="keys">指定的主键，可以为空</param>
    ''' <returns>受影响的记录数</returns>
    Public Function Save(pojo As Object, keys As String(), cols As String()) As Integer
        If pojo Is Nothing Then
            Return 0
        End If
        Dim t As Type = pojo.GetType
        If t.IsPrimitive OrElse t.IsValueType OrElse t.IsEnum OrElse t.IsInterface Then
            Return 0
        End If

        '按读的方式映射表和字段
        Dim table As TableEntity = GetMappingTable(pojo, keys, cols)
        Dim cmdUpdate As CommandEntity = BuildUpdateCommandText(table)
        Dim cmdInsert As CommandEntity = BuildInsertCommandText(table)
        Dim u As Integer
        If cmdUpdate.commandIgnore Then
            u = 0
        Else
            u = prepareCommand(cmdUpdate, table.Columns).ExecuteNonQuery
        End If
        If u = 0 Then
            u = prepareCommand(cmdInsert, table.Columns).ExecuteNonQuery()
        End If
        Return u
    End Function

#End Region

#Region "----------Remove----------"
    ''' <summary>
    ''' 从数据库中删除对象
    ''' </summary>
    ''' <param name="pojo">要删除的对象</param>
    ''' <returns>受影响的记录数</returns>
    Public Function Remove(pojo As Object) As Integer
        Return Remove(pojo, Nothing)
    End Function

    ''' <summary>
    ''' 从数据库中删除对象
    ''' </summary>
    ''' <param name="pojo">要删除的对象</param>
    ''' <param name="keys">指定主键，可以为空</param>
    ''' <returns>受影响的记录数</returns>
    Public Function Remove(pojo As Object, keys As String()) As Integer
        Dim table As TableEntity = GetMappingTable(pojo, keys, Nothing)
        Dim cmd As CommandEntity = BuildDeleteCommandText(table)
        Dim Result As Integer = prepareCommand(cmd, table.Columns).ExecuteNonQuery() '受影响的记录数
        cmd = Nothing
        Return Result
    End Function

#End Region

End Class
