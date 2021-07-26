<AttributeUsage(AttributeTargets.Property, Inherited:=True)>
Public Class PropertyAttribute
    Inherits Attribute

    Property DbName As String

    Sub New(ByVal DbName As String)
        Me.DbName = DbName
    End Sub
End Class
