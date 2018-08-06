Imports JiangNong.DBHelper2
Imports System.Data.Common

''' <summary>
''' DBHelper2功能演示
''' by JiangNong 2018.8.1
''' </summary>
Public Class Form1

#Region "Display Message"

    ''' <summary>
    ''' 显示信息
    ''' </summary>
    ''' <param name="message"></param>
    Private Sub appendText(message As String)
        txtResult.AppendText(message & vbCrLf)
    End Sub
    Private Sub appendText(p As Person)
        appendText(String.Format("First Name:{0}, Last Name:{1}, Person Type:{2}, rowguid:{3}, ModifiedDate:{4:yyyy-MM-dd}.", p.FirstName, p.LastName, p.PersonType, p.rowguid, p.ModifiedDate))
    End Sub

    Private Sub appendText(r As DataRow)
        appendText(String.Format("First Name:{0}, Last Name:{1}, Person Type:{2}, rowguid:{3}, ModifiedDate:{4:yyyy-MM-dd}.", r("FirstName"), r("LastName"), r("PersonType"), r("rowguid"), r("ModifiedDate")))
    End Sub

#End Region
    Private Sub btnExecScalar_Click(sender As Object, e As EventArgs) Handles btnExecScalar.Click
        Dim ProductID As Integer = 1
        '使用DBFactory创建DBHelper实例
        '指定数据库连接串，根据连接串的providerName来确定数据库类型，默认为SQLSERVER
        Using db As DBHelper = DBFactory.CreateDBHelper("ConnectionString")
            Dim SQL As String = "select Name from Production.Product where ProductID=@ProductID"
            '执行ExecuteScalar，需要SQL,参数名数组，参数值数组
            Dim Name As Object = db.ExecuteScalar(SQL, {"@ProductID"}, {ProductID})
            appendText(String.Format("Product Name:{0}", Name))
        End Using
    End Sub

    Private Sub btnExecReader_Click(sender As Object, e As EventArgs) Handles btnExecReader.Click
        Dim ProductID As Integer = 1
        '创建DBHelper实例时不指定连接串，默认使用name="ConnectionString"的连接串
        Using db As DBHelper = DBFactory.CreateDBHelper
            Dim SQL As String = "select * from Production.Product where ProductID=@ProductID"
            '执行ExecuteReader，需SQL,参数名数组,参数值数组
            Dim dr As DbDataReader = db.ExecuteReader(SQL, {"@ProductID"}, {ProductID})
            If dr.Read Then
                appendText(String.Format("Product Name:{0}", dr("Name")))
            Else
                appendText("Product not found")
            End If
        End Using
    End Sub

    Private Sub btnExecNonQuery_Click(sender As Object, e As EventArgs) Handles btnExecNonQuery.Click
        '创建DBHelper实例
        Using db As DBHelper = DBFactory.CreateDBHelper
            Dim Result As Integer
            Try
                Dim sqlInsert As String = "Insert into dbo.ErrorLog (UserName,ErrorNumber,ErrorMessage) values (@UserName,@ErrorNumber,@ErrorMessage)"
                Result = 0
                For errorNumber As Integer = 9990 To 9999
                    '执行ExecuteNonQuery，需SQL,参数名数组,参数值数组，返回受影响的记录数
                    Result += db.ExecuteNonQuery(sqlInsert, {"@UserName", "@ErrorNumber", "@ErrorMessage"}, {"TestUser", errorNumber, "测试数据。" & errorNumber})
                Next
                appendText(String.Format("Insert Result: {0}.", Result))
            Catch ex As Exception
                appendText("Insert Error: " & ex.Message)
            End Try

            Try
                Dim sqlUpdate As String = "update dbo.ErrorLog set ErrorMessage=@ErrorMessage where ErrorNumber=@ErrorNumber"
                Result = db.ExecuteNonQuery(sqlUpdate, {"@ErrorMessage", "@ErrorNumber"}, {"更新后的测试数据。", 9999})
                appendText(String.Format("Update Result: {0}.", Result))
            Catch ex As Exception
                appendText("Update Error: " & ex.Message)
            End Try

            Try
                Dim sqlDelete As String = "delete from dbo.ErrorLog where ErrorNumber=@ErrorNumber"
                Result = db.ExecuteNonQuery(sqlDelete, {"@ErrorNumber"}, {9990})
                appendText(String.Format("Delete Result: {0}.", Result))
            Catch ex As Exception
                appendText("Delete Error: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        '创建DBHelper实例
        Using db As DBHelper = DBFactory.CreateDBHelper
            '创建一个数据对象，赋给一个主键值
            Dim p As New Person With {.BusinessEntityID = 10}
            '使用Load方法，默认按照主键值从数据库装载记录
            If db.Load(p) Then
                appendText(p)
            Else
                appendText("Person not found.")
            End If
        End Using

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        '创建DBHelper实例
        Using db As DBHelper = DBFactory.CreateDBHelper
            '由于需要保存两个对象，需要将两个Save方法放在一个事务中
            db.BeginTransaction()
            Try
                '创建主表对象，它含有一个自增字段(Identity)
                Dim b As New BusinessEntity
                '保存主表对象，自动生成的Insert语句中不包含自增字段
                db.Save(b)
                '从数据库获取自增字段的值
                Dim BusinessEntityID As Integer = getBusinessEntityID(db)
                '构建附表对象，它的主键值必须与主表对象一致
                Dim p As New Person With {
                    .BusinessEntityID = BusinessEntityID,
                    .PersonType = "SC",
                    .FirstName = "Cougar",
                    .LastName = "Jiang"}
                '保存附表对象
                db.Save(p)
                '提交事务
                db.CommitTransaction()
                appendText("Save Record OK.")
            Catch ex As Exception
                '如果发生错误回滚事务
                db.RollbackTransaction()
                appendText("Save Record Error: " & ex.Message)
            End Try
        End Using
    End Sub

    ''' <summary>
    ''' SQLSERVER中获取自增字段的方法
    ''' </summary>
    ''' <param name="db">Insert语句关联的DBHelper对象</param>
    ''' <returns></returns>
    Private Function getBusinessEntityID(db As DBHelper) As Integer
        Return db.ReadInt(db.ExecuteScalar("select @@IDENTITY"))
    End Function

    Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        '创建DBHelper实例
        Using db As DBHelper = DBFactory.CreateDBHelper
            '创建一个对象，并赋值给非主键字段
            Dim p As New Person With {.FirstName = "Cougar", .LastName = "Jiang"}
            '指定有值的字段为查询关键字，查询记录
            If db.Load(p, {"FirstName", "LastName"}) Then
                appendText("No record to be removed.")
                Exit Sub
            End If
            '获得了主键值，按照主键来操作
            Dim b As New BusinessEntity With {.BusinessEntityID = p.BusinessEntityID}
            '开启事务
            db.BeginTransaction()
            Try
                '删除附表对象
                db.Remove(p)
                '删除主表对象
                db.Remove(b)
                '提交事务
                db.CommitTransaction()
                appendText("Record are removed.")
            Catch ex As Exception
                '回滚事务
                db.RollbackTransaction()
                appendText("Remove Error: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub btnReadEntity_Click(sender As Object, e As EventArgs) Handles btnReadEntity.Click
        '创建DBHelper实例
        Using db As DBHelper = DBFactory.CreateDBHelper
            '将SQL的查询结果直接转换为对象
            Dim p As Person = db.ReadEntity(Of Person)("select * from person.person where lastname=@lastname and firstname=@firstname",
                                                       {"@lastname", "@firstname"},
                                                       {"Jiang", "Cougar"})
            If p IsNot Nothing Then
                appendText(p)
            Else
                appendText("Entity is not found.")
            End If
        End Using
    End Sub

    Private Sub btnReadList_Click(sender As Object, e As EventArgs) Handles btnReadList.Click
        '创建DBHelper实例
        Using db As DBHelper = DBFactory.CreateDBHelper
            '将SQL的查询结果直接转换为列表
            Dim l As List(Of Person) = db.ReadList(Of Person)("select top 10 * from person.person")
            If l IsNot Nothing AndAlso l.Count > 0 Then
                For Each p As Person In l
                    appendText(p)
                Next
            Else
                appendText("List is empty.")
            End If
        End Using
    End Sub

    Private Sub btnReadTable_Click(sender As Object, e As EventArgs) Handles btnReadTable.Click
        '创建DBHelper实例
        Using db As DBHelper = DBFactory.CreateDBHelper
            '将SQL的查询结果直接转换为数据表
            Dim t As DataTable = db.ReadTable("select top 10 * from person.person")
            If t IsNot Nothing AndAlso t.Rows.Count > 0 Then
                For Each r As DataRow In t.Rows
                    appendText(r)
                Next
            Else
                appendText("Table is empty.")
            End If
        End Using
    End Sub


    Private Sub btnSaveList_Click(sender As Object, e As EventArgs) Handles btnSaveList.Click
        '创建DBHelper实例
        Using db As DBHelper = DBFactory.CreateDBHelper
            '生成一个列表对象，并加入10个ErrorLog元素
            Dim l As New List(Of ErrorLog)
            For i As Integer = 8990 To 8999
                Dim r As New ErrorLog With {
                .ErrorMessage = "Test Error Message for Save List, " & i,
                .ErrorNumber = i,
                .UserName = "SaveList"}
                l.Add(r)
            Next
            '开启一个事务
            db.BeginTransaction()
            Try
                '保存10个元素的ErrorLog列表
                Dim Result As Integer = db.SaveList(Of ErrorLog)(l)
                '提交一个事务
                db.CommitTransaction()
                appendText(String.Format("List has been saved, Record count: {0}", Result))
            Catch ex As Exception
                '如果出错，回滚事务
                db.RollbackTransaction()
                appendText("Save List Error: " & ex.Message)
            End Try
        End Using
    End Sub


    Private Sub btnSaveTable_Click(sender As Object, e As EventArgs) Handles btnSaveTable.Click
        '创建DBHelper实例
        Using db As DBHelper = DBFactory.CreateDBHelper
            '创建一个数据表
            Dim t As DataTable = db.ReadTable("select top 10 ErrorLogID, 'SaveTable' as UserName,ErrorNumber,ErrorMessage from dbo.errorlog where username='SaveList'")
            '开启一个事务
            db.BeginTransaction()
            Try
                '将数据表中的数据保存到指定的表中
                Dim Result As Integer = db.SaveTable(t, "dbo.ErrorLog", {"ErrorLogID"})
                '提交事务
                db.CommitTransaction()
                appendText(String.Format("DataTable has been saved, Record count: {0}", Result))
            Catch ex As Exception
                '如果出错，回滚事务
                db.RollbackTransaction()
                appendText("Save DataTable Error: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub btnWriteTo_Click(sender As Object, e As EventArgs) Handles btnWriteTo.Click
        'WriteTo方法可将数据从一个数据库复制到另一个数据库，两个数据库可以是不同类型的数据库
        '创建源数据库的DBHelper实例
        Using dbRead As DBHelper = DBFactory.CreateDBHelper
            '从源数据库中创建一个DataReader对象，用于读取数据
            Dim dr As DbDataReader = dbRead.ExecuteReader("select top 10 'WriteTo' as UserName,ErrorNumber,ErrorMessage from dbo.errorlog where username='SaveList'")
            '创建目标数据库的DBHelper实例，此例采用同一个数据库，实际上可以与源数据库不同，甚至是异构数据库
            Using dbSave As DBHelper = DBFactory.CreateDBHelper
                '开启事务
                dbSave.BeginTransaction()
                Try
                    '将源数据保存到目标数据库中
                    Dim Result As Integer = dbRead.WriteToDb(dr, dbSave, "dbo.ErrorLog", {"ErrorLogID"})
                    '提交事务
                    dbSave.CommitTransaction()
                    appendText(String.Format("DataTable has been saved, Record count: {0}", Result))
                Catch ex As Exception
                    '如果出错，回滚事务
                    dbSave.RollbackTransaction()
                    appendText("Save DataTable Error: " & ex.Message)
                End Try
            End Using
        End Using
    End Sub
End Class
