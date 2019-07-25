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

    ''' <summary>
    ''' 将对象的属性转为列映射
    ''' </summary>
    ''' <param name="pi"></param>
    ''' <returns></returns>
    Private Shared Function getColumnMappingAttribute(pi As PropertyInfo) As ColumnMapping
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


    ''' <summary>
    ''' 分析reader和pojo，生成映射
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="tp">对象的类型</param>
    ''' <param name="cols"></param>
    ''' <returns>reader与pojo的映射，当pojo是简单类型时，返回空</returns>
    Private Shared Function getMappingProperty(reader As DbDataReader, tp As Type, cols As String()) As Dictionary(Of String, PropertyInfo)
        'Dim tp As Type = pojo.GetType
        If tp.IsPrimitive OrElse tp.IsValueType OrElse tp.IsEnum OrElse tp.IsInterface OrElse tp Is GetType(String) Then
            Return Nothing
        End If
        Dim d As New Dictionary(Of String, PropertyInfo)
        '有效的字段：reader,cols都存在的字段
        Dim fields As List(Of String) = getMatchesCols(reader, Nothing, cols)
        '遍历pojo的属性
        For Each pi As PropertyInfo In tp.GetProperties
            '不需要映射的列
            If Not pi.CanWrite OrElse Not findCol(cols, pi.Name) Then
                Continue For
            End If

            Dim ca As ColumnMapping = getColumnMappingAttribute(pi)
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

    ''' <summary>
    ''' 从DataReader中读取数据
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="pojo"></param>
    ''' <param name="map">映射</param>
    ''' <remarks>允许读取简单类型 2019.7.24</remarks>
    Private Sub setPojoValue(reader As DbDataReader, ByRef pojo As Object, map As Dictionary(Of String, PropertyInfo))
        '当pojo为简单类型时，map应该为空。此时读取reader的第1个字段
        If map Is Nothing Then
            If TypeOf pojo Is String Then
                pojo = ReadStr(reader(0))
            Else
                pojo = ReadObj(reader(0))
            End If
        Else
            '开始读取
            For Each k As String In map.Keys
                If map(k).PropertyType Is GetType(String) Then
                    map(k).SetValue(pojo, ReadStr(reader(k)), Nothing)
                Else
                    map(k).SetValue(pojo, ReadObj(reader(k)), Nothing)
                End If
            Next
        End If

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

                '加入到映射列表，如果已经有同名字段，不加入
                If te.Columns.ContainsKey(ColumnName) Then
                    Continue For
                End If

                Dim v As Object = pi.GetValue(pojo, Nothing)
                Select Case ca.ColumnType
                    Case EnumColumnType.Boolean
                        te.Columns.Add(ColumnName, SafeReader.ReadBool(v))
                    Case EnumColumnType.Date
                        te.Columns.Add(ColumnName, SafeReader.ReadDate(v))
                    Case EnumColumnType.Decimal
                        te.Columns.Add(ColumnName, SafeReader.ReadDec(v))
                    Case EnumColumnType.Double
                        te.Columns.Add(ColumnName, SafeReader.ReadDbl(v))
                    Case EnumColumnType.Integer
                        te.Columns.Add(ColumnName, SafeReader.ReadInt(v))
                    Case EnumColumnType.Long
                        te.Columns.Add(ColumnName, SafeReader.ReadLng(v))
                    Case EnumColumnType.Short
                        te.Columns.Add(ColumnName, SafeReader.ReadShort(v))
                    Case EnumColumnType.Single
                        te.Columns.Add(ColumnName, SafeReader.ReadSng(v))
                    Case EnumColumnType.String
                        te.Columns.Add(ColumnName, SafeReader.ReadStr(v))
                    Case EnumColumnType.BoolToInt
                        te.Columns.Add(ColumnName, SafeReader.BoolToInt(v))
                    Case EnumColumnType.BoolToStr
                        te.Columns.Add(ColumnName, SafeReader.BoolToStr(v))
                    Case Else
                        te.Columns.Add(ColumnName, v)
                End Select

                te.Properties.Add(ColumnName, pi)


                If pi.CanRead AndAlso ca.Mapping <> EnumMapping.Read Then
                    '确定主键
                    If keys Is Nothing OrElse keys.Length = 0 Then
                        '类中指定的主键，是按照数据库主键指定，因此这里的FindKey要用ColumnName
                        If ca.PrimaryKey OrElse findKey(ta.Keys, ColumnName) Then
                            te.KeyNames.Add(ColumnName)
                        End If
                    Else
                        '程序指定的主键，是按照类的属性名称，因此这里的FindKey要用pi.Name
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

    ''' <summary>
    ''' 找出与目标数据匹配的源数据映射
    ''' </summary>
    ''' <param name="table">源数据</param>
    ''' <param name="tableName">目标数据表名，允许为空</param>
    ''' <param name="keys"></param>
    ''' <param name="cols"></param>
    ''' <returns></returns>
    Private Function getMappingTable(table As DataTable, tableName As String, keys As String(), cols As String()) As TableEntity
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
            Dim fields As List(Of String) = getMatchesCols(te.TableName, keys, cols)
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

                If findKey(keys, ColumnName) Then
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

    ''' <summary>
    ''' 找出与目标数据匹配的源数据映射
    ''' </summary>
    ''' <param name="reader">源数据</param>
    ''' <param name="tableName">目标数据表名</param>
    ''' <param name="keys"></param>
    ''' <param name="cols"></param>
    ''' <returns></returns>
    Private Function getMappingTable(reader As DbDataReader, tableName As String, keys As String(), cols As String()) As TableEntity

        If reader Is Nothing Then
            Throw New ArgumentException("保存Reader失败：Reader对象不能为空")
        End If

        Dim te As New TableEntity
        te.TableName = tableName

        Try
            '确定字段是否在目标数据库中存在
            Dim fields As List(Of String) = getMatchesCols(te.TableName, keys, cols)
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
                If findKey(keys, ColumnName) Then
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
    Private Function getMatchesCols(tableName As String, keys As String(), cols As String()) As List(Of String)
        tableName = tableName.Replace(" ", "").Replace(vbTab, "").Replace("--", "").Replace(";", "")
        If String.IsNullOrEmpty(tableName) Then
            Throw New ArgumentException("表映射失败：TableName不能为空")
        End If
        Using cmd As DbCommand = createCommand(CommandType.Text, "select * from " & tableName & " where 1=2")
            Dim dr As DbDataReader = cmd.ExecuteReader(CommandBehavior.SchemaOnly)
            Dim Result As List(Of String) = getMatchesCols(dr, keys, cols)
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
    Private Shared Function getMatchesCols(reader As DbDataReader, keys As String(), cols As String()) As List(Of String)
        Dim l As New List(Of String)
        For i As Integer = 0 To reader.FieldCount - 1
            Dim c As String = reader.GetName(i).ToLower(CultureInfo.CurrentCulture)
            If findKey(keys, c) OrElse findCol(cols, c) Then
                l.Add(c)
            End If
        Next
        Return l
    End Function

    ''' <summary>
    ''' 在字段数组中找指定的值，忽略大小写。
    ''' 找到指定的值或数组为空或长度为零返回真（允许不明确指定字段）
    ''' </summary>
    ''' <param name="cols">被查找的数组</param>
    ''' <param name="col">指定的值</param>
    ''' <returns>数组为空或长度为0返回-1；没找到返回-2；找到返回位置</returns>
    ''' <remarks></remarks>
    Private Shared Function findCol(cols As String(), col As String) As Boolean
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
    ''' <summary>
    ''' 在主键数组中找指定的值，忽略大小写
    ''' 主键数据组为空时返回假（必须有明确的主键）
    ''' </summary>
    ''' <param name="keys"></param>
    ''' <param name="col"></param>
    ''' <returns></returns>
    Private Shared Function findKey(keys As String(), col As String) As Boolean
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
