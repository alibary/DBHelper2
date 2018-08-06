Imports System.Data.Common
Imports System.Reflection
Imports System.Globalization

Partial Class DBHelper

    Private Shared Function GetTableMappingAttribute(pojoType As Type) As TableMapping
        For Each a As Attribute In pojoType.GetCustomAttributes(True)
            If TypeOf (a) Is TableMapping Then
                Dim t As TableMapping = CType(a, TableMapping)
                If String.IsNullOrEmpty(t.Table) Then
                    t.Table = pojoType.Name
                End If
                Return t
            End If
        Next
        Return New TableMapping With {.Table = pojoType.Name}
    End Function

    Private Shared Function GetColumnMappingAttribute(pi As PropertyInfo) As ColumnMapping
        For Each a As Attribute In pi.GetCustomAttributes(True)
            If TypeOf (a) Is ColumnMapping Then
                Dim p As ColumnMapping = CType(a, ColumnMapping)
                If String.IsNullOrEmpty(p.Column) Then
                    p.Column = pi.Name
                End If
                Return p
            End If
        Next
        '默认值
        Return New ColumnMapping With {.Column = pi.Name,
                                      .Identity = False,
                                      .Mapping = EnumMapping.Both,
                                      .PrimaryKey = False}
    End Function


    '分析reader和pojo，生成映射
    Private Shared Function GetMappingProperty(reader As DbDataReader, pojo As Object, cols As String()) As Dictionary(Of String, PropertyInfo)
        Dim d As New Dictionary(Of String, PropertyInfo)
        '遍历属性
        Dim fields As List(Of String) = GetMatchesCols(reader, Nothing, cols)
        For Each pi As PropertyInfo In pojo.GetType.GetProperties
            '不需要映射的列
            If Not pi.CanWrite OrElse Not FindCol(cols, pi.Name) Then
                Continue For
            End If

            Dim ca As ColumnMapping = GetColumnMappingAttribute(pi)
            If ca.Mapping = EnumMapping.None OrElse ca.Mapping = EnumMapping.Write Then
                Continue For
            End If

            Dim ColumnName As String = ca.Column.ToLower

            '映射实体中是否存在该字段
            If Not fields.Contains(ColumnName) Then
                Continue For
            End If

            '映射值
            If d.ContainsKey(ColumnName) Then
                Continue For
            End If

            d.Add(ColumnName, pi)

        Next
        Return d
    End Function

    Private Sub SetPojoValue(reader As DbDataReader, pojo As Object, map As Dictionary(Of String, PropertyInfo))
        '开始读取
        For Each k As String In map.Keys
            Dim source As Object = reader(k)
            If map(k).PropertyType Is GetType(String) Then
                map(k).SetValue(pojo, ReadStr(source), Nothing)
            Else
                map(k).SetValue(pojo, ReadObj(source), Nothing)
            End If
        Next
    End Sub

    ''' <summary>
    ''' 从对象中读取映射信息
    ''' </summary>
    ''' <param name="pojo">含有键值的对象</param>
    ''' <param name="cols">指定的需要映射的列名（对象中的属性名）</param>
    ''' <param name="keys">指定的主键（对象中的属性名）</param>
    ''' <remarks></remarks>
    Private Function GetMappingTable(pojo As Object, keys As String(), cols As String()) As TableEntity
        If pojo Is Nothing Then
            Throw New ArgumentException("表映射失败：映射对象不能为空")
        End If
        Dim te As New TableEntity
        '确定表名
        Dim ta As TableMapping = GetTableMappingAttribute(pojo.GetType)
        te.TableName = ta.Table.ToLower
        Try
            '查询表中的字段
            Dim fields As List(Of String) = GetMatchesCols(te.TableName, keys, cols)
            '从属性中获取主键和列
            For Each pi As PropertyInfo In pojo.GetType.GetProperties
                Dim ca As ColumnMapping = GetColumnMappingAttribute(pi)
                If ca.Mapping = EnumMapping.None Then
                    Continue For
                End If

                Dim ColumnName As String = ca.Column.ToLower

                '映射实体中是否存在该字段
                If Not fields.Contains(ColumnName) Then
                    Continue For
                End If

                '加入到映射列表
                If te.Columns.ContainsKey(ColumnName) Then
                    Continue For
                End If

                te.Columns.Add(ColumnName, pi.GetValue(pojo, Nothing))
                te.Properties.Add(ColumnName, pi)


                If pi.CanRead AndAlso ca.Mapping <> EnumMapping.Read Then
                    '确定主键
                    If keys Is Nothing OrElse keys.Length = 0 Then
                        If ca.PrimaryKey Then
                            te.KeyNames.Add(ColumnName)
                        End If
                    Else
                        If FindKey(keys, pi.Name) Then
                            te.KeyNames.Add(ColumnName)
                        End If
                    End If
                    '确定字段
                    If Not ca.Identity Then
                        te.InsertColumnNames.Add(ColumnName)
                    End If

                    If FindCol(cols, pi.Name) AndAlso Not te.KeyNames.Contains(ColumnName) Then
                        te.UpdateColumnNames.Add(ColumnName)
                    End If
                End If

                If pi.CanWrite AndAlso ca.Mapping <> EnumMapping.Write Then
                    te.SelectColumnNames.Add(ColumnName)
                End If

            Next
        Catch ex As Exception
            Throw New ArgumentException(te.TableName & "表映射失败：" & ex.Message)
        End Try

        Return te
    End Function

    Private Function GetMappingTable(table As DataTable, tableName As String, keys As String(), cols As String()) As TableEntity
        If table Is Nothing Then
            Throw New ArgumentException("表映射失败：table对象不能为空")
        End If
        Dim te As New TableEntity

        If String.IsNullOrEmpty(tableName) Then
            te.TableName = table.TableName
        Else
            te.TableName = tableName
        End If

        Try
            '滤掉keys,cols中有，数据库没有的字段
            Dim fields As List(Of String) = GetMatchesCols(te.TableName, keys, cols)
            For Each c As DataColumn In table.Columns
                '滤掉数据库有，table中没有字段
                Dim ColumnName As String = c.ColumnName.ToLower(CultureInfo.CurrentCulture)
                If Not fields.Contains(ColumnName) Then
                    Continue For
                End If
                '滤掉重复的字段
                If te.Columns.ContainsKey(ColumnName) Then
                    Continue For
                End If

                te.Columns.Add(ColumnName, Nothing)

                te.InsertColumnNames.Add(ColumnName)
                te.SelectColumnNames.Add(ColumnName)

                If FindKey(keys, ColumnName) Then
                    te.KeyNames.Add(ColumnName)
                Else
                    'update字段是除了主键以外的其它字段
                    te.UpdateColumnNames.Add(ColumnName)
                End If
            Next

        Catch ex As Exception
            Throw New ArgumentException(table.TableName & "表映射失败：" & ex.Message)
        End Try
        Return te
    End Function

    Private Function GetMappingTable(reader As DbDataReader, tableName As String, keys As String(), cols As String()) As TableEntity
       
        If reader Is Nothing Then
            Throw New ArgumentException("保存Reader失败：Reader对象不能为空")
        End If

        Dim te As New TableEntity
        te.TableName = tableName

        Try
            '确定字段是否在目标数据库中存在
            Dim fields As List(Of String) = GetMatchesCols(te.TableName, keys, cols)
            For i As Integer = 0 To reader.FieldCount - 1
                Dim ColumnName As String = reader.GetName(i).ToLower(CultureInfo.CurrentCulture)
                If Not fields.Contains(ColumnName) Then
                    Continue For
                End If
                If te.Columns.ContainsKey(ColumnName) Then
                    Continue For
                End If
                te.Columns.Add(ColumnName, Nothing)
                te.InsertColumnNames.Add(ColumnName)
                te.SelectColumnNames.Add(ColumnName)
                If FindKey(keys, ColumnName) Then
                    te.KeyNames.Add(ColumnName)
                Else
                    te.UpdateColumnNames.Add(ColumnName)
                End If
            Next

        Catch ex As Exception
            Throw New ArgumentException(te.TableName & "表映射失败：" & ex.Message)
        End Try
        Return te
    End Function


    ''' <summary>
    ''' 读取指定表中匹配的字段
    ''' </summary>
    ''' <param name="tableName"></param>
    ''' <returns></returns>
    ''' <remarks>结果改成小写</remarks>
    Private Function GetMatchesCols(tableName As String, keys As String(), cols As String()) As List(Of String)
        tableName = tableName.Replace(" ", "").Replace(vbTab, "").Replace("--", "").Replace(";", "")
        If String.IsNullOrEmpty(tableName) Then
            Throw New ArgumentException("表映射失败：TableName不能为空")
        End If
        Using cmd As DbCommand = createCommand(CommandType.Text, "select * from " & tableName & " where 1=2")
            Dim dr As DbDataReader = cmd.ExecuteReader(CommandBehavior.SchemaOnly)
            Dim Result As List(Of String) = GetMatchesCols(dr, keys, cols)
            dr.Close()
            dr = Nothing
            Return Result
        End Using
    End Function

    ''' <summary>
    ''' 判断映射列是否在数据库中存在
    ''' </summary>
    ''' <param name="reader">数据源</param>
    ''' <remarks>结果改成小写</remarks>
    Private Shared Function GetMatchesCols(reader As DbDataReader, keys As String(), cols As String()) As List(Of String)
        Dim l As New List(Of String)
        For i As Integer = 0 To reader.FieldCount - 1
            Dim c As String = reader.GetName(i).ToLower(CultureInfo.CurrentCulture)
            If FindKey(keys, c) OrElse FindCol(cols, c) Then
                l.Add(c)
            End If
        Next
        Return l
    End Function

    ''' <summary>
    ''' 在数组中找指定的值，忽略大小写。
    ''' 找到指定的值或数组为空或长度为零返回真。
    ''' </summary>
    ''' <param name="cols">被查找的数组</param>
    ''' <param name="col">指定的值</param>
    ''' <returns>数组为空或长度为0返回-1；没找到返回-2；找到返回位置</returns>
    ''' <remarks></remarks>
    Private Shared Function FindCol(cols As String(), col As String) As Boolean
        If cols Is Nothing OrElse cols.Length = 0 Then
            Return True
        End If
        Dim c As String = col.ToLower(CultureInfo.CurrentCulture)
        For i As Integer = 0 To cols.Length - 1
            If cols(i).ToLower(CultureInfo.CurrentCulture) = c Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Shared Function FindKey(keys As String(), col As String) As Boolean
        If keys Is Nothing OrElse keys.Length = 0 Then
            Return False
        End If
        Dim c As String = col.ToLower(CultureInfo.CurrentCulture)
        For i As Integer = 0 To keys.Length - 1
            If keys(i).ToLower(CultureInfo.CurrentCulture) = c Then
                Return True
            End If
        Next
        Return False
    End Function
End Class
