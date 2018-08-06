<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnExecScalar = New System.Windows.Forms.Button()
        Me.txtResult = New System.Windows.Forms.TextBox()
        Me.btnExecReader = New System.Windows.Forms.Button()
        Me.btnExecNonQuery = New System.Windows.Forms.Button()
        Me.btnLoad = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnRemove = New System.Windows.Forms.Button()
        Me.btnReadEntity = New System.Windows.Forms.Button()
        Me.btnReadList = New System.Windows.Forms.Button()
        Me.btnReadTable = New System.Windows.Forms.Button()
        Me.btnSaveList = New System.Windows.Forms.Button()
        Me.btnSaveTable = New System.Windows.Forms.Button()
        Me.btnWriteTo = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnExecScalar
        '
        Me.btnExecScalar.Location = New System.Drawing.Point(12, 12)
        Me.btnExecScalar.Name = "btnExecScalar"
        Me.btnExecScalar.Size = New System.Drawing.Size(75, 23)
        Me.btnExecScalar.TabIndex = 0
        Me.btnExecScalar.Text = "ExecScalar"
        Me.btnExecScalar.UseVisualStyleBackColor = True
        '
        'txtResult
        '
        Me.txtResult.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtResult.Location = New System.Drawing.Point(12, 41)
        Me.txtResult.Multiline = True
        Me.txtResult.Name = "txtResult"
        Me.txtResult.Size = New System.Drawing.Size(965, 288)
        Me.txtResult.TabIndex = 1
        '
        'btnExecReader
        '
        Me.btnExecReader.Location = New System.Drawing.Point(93, 12)
        Me.btnExecReader.Name = "btnExecReader"
        Me.btnExecReader.Size = New System.Drawing.Size(75, 23)
        Me.btnExecReader.TabIndex = 2
        Me.btnExecReader.Text = "ExecReader"
        Me.btnExecReader.UseVisualStyleBackColor = True
        '
        'btnExecNonQuery
        '
        Me.btnExecNonQuery.Location = New System.Drawing.Point(174, 12)
        Me.btnExecNonQuery.Name = "btnExecNonQuery"
        Me.btnExecNonQuery.Size = New System.Drawing.Size(75, 23)
        Me.btnExecNonQuery.TabIndex = 3
        Me.btnExecNonQuery.Text = "ExecNonQuery"
        Me.btnExecNonQuery.UseVisualStyleBackColor = True
        '
        'btnLoad
        '
        Me.btnLoad.Location = New System.Drawing.Point(255, 12)
        Me.btnLoad.Name = "btnLoad"
        Me.btnLoad.Size = New System.Drawing.Size(75, 23)
        Me.btnLoad.TabIndex = 4
        Me.btnLoad.Text = "Load"
        Me.btnLoad.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(336, 12)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(75, 23)
        Me.btnSave.TabIndex = 5
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnRemove
        '
        Me.btnRemove.Location = New System.Drawing.Point(417, 12)
        Me.btnRemove.Name = "btnRemove"
        Me.btnRemove.Size = New System.Drawing.Size(75, 23)
        Me.btnRemove.TabIndex = 6
        Me.btnRemove.Text = "Remove"
        Me.btnRemove.UseVisualStyleBackColor = True
        '
        'btnReadEntity
        '
        Me.btnReadEntity.Location = New System.Drawing.Point(498, 12)
        Me.btnReadEntity.Name = "btnReadEntity"
        Me.btnReadEntity.Size = New System.Drawing.Size(75, 23)
        Me.btnReadEntity.TabIndex = 7
        Me.btnReadEntity.Text = "ReadEntity"
        Me.btnReadEntity.UseVisualStyleBackColor = True
        '
        'btnReadList
        '
        Me.btnReadList.Location = New System.Drawing.Point(579, 12)
        Me.btnReadList.Name = "btnReadList"
        Me.btnReadList.Size = New System.Drawing.Size(75, 23)
        Me.btnReadList.TabIndex = 8
        Me.btnReadList.Text = "ReadList"
        Me.btnReadList.UseVisualStyleBackColor = True
        '
        'btnReadTable
        '
        Me.btnReadTable.Location = New System.Drawing.Point(660, 12)
        Me.btnReadTable.Name = "btnReadTable"
        Me.btnReadTable.Size = New System.Drawing.Size(75, 23)
        Me.btnReadTable.TabIndex = 9
        Me.btnReadTable.Text = "ReadTable"
        Me.btnReadTable.UseVisualStyleBackColor = True
        '
        'btnSaveList
        '
        Me.btnSaveList.Location = New System.Drawing.Point(741, 12)
        Me.btnSaveList.Name = "btnSaveList"
        Me.btnSaveList.Size = New System.Drawing.Size(75, 23)
        Me.btnSaveList.TabIndex = 10
        Me.btnSaveList.Text = "SaveList"
        Me.btnSaveList.UseVisualStyleBackColor = True
        '
        'btnSaveTable
        '
        Me.btnSaveTable.Location = New System.Drawing.Point(822, 12)
        Me.btnSaveTable.Name = "btnSaveTable"
        Me.btnSaveTable.Size = New System.Drawing.Size(75, 23)
        Me.btnSaveTable.TabIndex = 11
        Me.btnSaveTable.Text = "SaveTable"
        Me.btnSaveTable.UseVisualStyleBackColor = True
        '
        'btnWriteTo
        '
        Me.btnWriteTo.Location = New System.Drawing.Point(903, 12)
        Me.btnWriteTo.Name = "btnWriteTo"
        Me.btnWriteTo.Size = New System.Drawing.Size(75, 23)
        Me.btnWriteTo.TabIndex = 12
        Me.btnWriteTo.Text = "WriteTo"
        Me.btnWriteTo.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(989, 341)
        Me.Controls.Add(Me.btnWriteTo)
        Me.Controls.Add(Me.btnSaveTable)
        Me.Controls.Add(Me.btnSaveList)
        Me.Controls.Add(Me.btnReadTable)
        Me.Controls.Add(Me.btnReadList)
        Me.Controls.Add(Me.btnReadEntity)
        Me.Controls.Add(Me.btnRemove)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnLoad)
        Me.Controls.Add(Me.btnExecNonQuery)
        Me.Controls.Add(Me.btnExecReader)
        Me.Controls.Add(Me.txtResult)
        Me.Controls.Add(Me.btnExecScalar)
        Me.Name = "Form1"
        Me.Text = "DBHelper2Demo"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnExecScalar As Button
    Friend WithEvents txtResult As TextBox
    Friend WithEvents btnExecReader As Button
    Friend WithEvents btnExecNonQuery As Button
    Friend WithEvents btnLoad As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents btnRemove As Button
    Friend WithEvents btnReadEntity As Button
    Friend WithEvents btnReadList As Button
    Friend WithEvents btnReadTable As Button
    Friend WithEvents btnSaveList As Button
    Friend WithEvents btnSaveTable As Button
    Friend WithEvents btnWriteTo As Button
End Class
