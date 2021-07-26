Imports System.Runtime.CompilerServices

Module CustomModules
    <Extension()>
    Public Function DataTableToList(Of T)(ByVal aDataTable As DataTable) As List(Of T)
        Dim ref As New ReflectionMethods
        Return ref.ConvertirDataTableToList(Of T)(aDataTable)
    End Function

    <Extension()>
    Public Function ListToDataSet(Of T)(ByVal collection As IEnumerable(Of T), ByVal name As String) As DataSet
        Dim ref As New ReflectionMethods
        Return ref.ListToDataSet(Of T)(collection, name)
    End Function

End Module
