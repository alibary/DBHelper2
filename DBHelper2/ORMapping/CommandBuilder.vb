Imports System.Text
Imports System.Globalization

Partial Class DBHelper
    ' ''' <summary>
    ' ''' 为DataTable生成EXISTS语句
    ' ''' </summary>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Private Function BuildExistsCommandText(table As TableEntity) As CommandEntity
    '    Dim cmd As New CommandEntity
    '    Dim sb2 As New StringBuilder '主键列表
    '    For Each k As String In table.KeyNames
    '        If sb2.Length = 0 Then
    '            sb2.Append(k & " = " & ParameterPrefix(k))
    '        Else
    '            sb2.Append(" and " & k & " = " & ParameterPrefix(k))
    '        End If
    '        cmd.ParmNames.Add(ParameterPrefix(k))
    '        cmd.ColumnNames.Add(k)
    '    Next
    '    If sb2.Length = 0 Then
    '        Throw New ArgumentException(table.TableName & "构造SELECT语句失败：对象没有映射的键。")
    '    End If

    '    cmd.command = createCommand(CommandType.Text, String.Format(CultureInfo.CurrentCulture, "select 1 from {0} where {1}", table.TableName, sb2.ToString))
    '    Return cmd
    'End Function

    Private Function BuildSelectCommandText(table As TableEntity) As CommandEntity
        Dim cmd As New CommandEntity
        Dim sb1 As New StringBuilder
        For Each c As String In table.SelectColumnNames
            If sb1.Length = 0 Then
                sb1.Append(c)
            Else
                sb1.Append(", " & c)
            End If
        Next
        If sb1.Length = 0 Then
            Throw New ArgumentException(table.TableName & "构造SELECT语句失败：对象没有映射的列。")
        End If

        Dim sb2 As New StringBuilder
        For Each k As String In table.KeyNames
            If sb2.Length = 0 Then
                sb2.Append(k & " = " & ParameterPrefix(k))
            Else
                sb2.Append(" and " & k & " = " & ParameterPrefix(k))
            End If
            cmd.ParmNames.Add(ParameterPrefix(k))
            cmd.ColumnNames.Add(k)
        Next
        If sb2.Length = 0 Then
            'Throw New ArgumentException(table.TableName & "构造SELECT语句失败：对象没有映射的键。")
            sb2.Append("1=1")
        End If

        cmd.command = createCommand(CommandType.Text, String.Format(CultureInfo.CurrentCulture, "select {1} from {0} where {2}", table.TableName, sb1.ToString, sb2.ToString))
        Return cmd
    End Function

    ''' <summary>
    ''' 为DataTable生成插入SQL语句
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function BuildInsertCommandText(table As TableEntity) As CommandEntity
        Dim cmd As New CommandEntity
        Dim sb1 As New StringBuilder  '字段列表
        Dim sb2 As New StringBuilder  '参数列表

        For Each c As String In table.InsertColumnNames
            If sb1.Length = 0 Then
                sb1.Append(c)
                sb2.Append(ParameterPrefix(c))
            Else
                sb1.Append(", " & c)
                sb2.Append(", " & ParameterPrefix(c))
            End If
            cmd.ParmNames.Add(ParameterPrefix(c))
            cmd.ColumnNames.Add(c)
        Next
        If sb1.Length = 0 OrElse sb2.Length = 0 Then
            Throw New ArgumentException(table.TableName & "构造INSERT语句失败：对象没有映射的列。")
        End If
        cmd.command = createCommand(CommandType.Text, String.Format(CultureInfo.CurrentCulture, "insert into {0} ({1}) values ({2}) ", table.TableName, sb1.ToString, sb2.ToString))
        Return cmd
    End Function

    ''' <summary>
    ''' 为DataTable构建Update语句
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function BuildUpdateCommandText(table As TableEntity) As CommandEntity
        Dim cmd As New CommandEntity
        '保证UPDATE语句的字段列表不为空2017.3.7
        Dim UpdateColumnNames As List(Of String) = table.UpdateColumnNames
        If UpdateColumnNames Is Nothing OrElse UpdateColumnNames.Count = 0 Then
            UpdateColumnNames = table.KeyNames
        End If
        Dim sb1 As New StringBuilder '字段和参数列表
        For Each c As String In UpdateColumnNames
            If sb1.Length = 0 Then
                sb1.Append(c & " = " & ParameterPrefix(c))
            Else
                sb1.Append(" , " & c & " = " & ParameterPrefix(c))
            End If
            cmd.parmNames.Add(ParameterPrefix(c))
            cmd.columnNames.Add(c)
        Next
        If sb1.Length = 0 Then
            Throw New ArgumentException(table.TableName & "构造UPDATE语句失败：对象没有映射的列。")
        End If
        Dim sb2 As New StringBuilder '主键和参数列表
        For Each k As String In table.KeyNames
            If sb2.Length = 0 Then
                sb2.Append(k & " = " & ParameterPrefix(k))
            Else
                sb2.Append(" and " & k & " = " & ParameterPrefix(k))
            End If
            cmd.ParmNames.Add(ParameterPrefix(k))
            cmd.ColumnNames.Add(k)
        Next
        If sb2.Length = 0 Then
            'Throw New ArgumentException(table.TableName & "构造UPDATE语句失败：对象没有映射的键。")
            sb2.Append("1=2")
            cmd.commandIgnore = True
        End If
        cmd.command = createCommand(CommandType.Text, String.Format(CultureInfo.CurrentCulture, "update {0} set {1} where {2} ", table.TableName, sb1.ToString, sb2.ToString))
        Return cmd
    End Function

    Private Function BuildDeleteCommandText(table As TableEntity) As CommandEntity
        Dim cmd As New CommandEntity
        Dim sb1 As New StringBuilder
        For Each k As String In table.KeyNames
            If sb1.Length = 0 Then
                sb1.Append(k & " = " & ParameterPrefix(k))
            Else
                sb1.Append(" and " & k & " = " & ParameterPrefix(k))
            End If
            cmd.ParmNames.Add(ParameterPrefix(k))
            cmd.ColumnNames.Add(k)
        Next
        If sb1.Length = 0 Then
            'Throw New ArgumentException(table.TableName & "构造DELETE语句失败：对象没有映射的键。")
            sb1.Append("1=2")
            cmd.commandIgnore = True
        End If
        cmd.command = createCommand(CommandType.Text, String.Format(CultureInfo.CurrentCulture, "delete from {0} where {1}", table.TableName, sb1.ToString))
        Return cmd
    End Function
End Class
