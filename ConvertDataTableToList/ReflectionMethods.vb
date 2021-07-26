Imports System.Reflection

Public Class ReflectionMethods
    Public Function ConvertirDataTableToList(Of T)(ByVal dTable As DataTable) As List(Of T)

        Dim lst As New List(Of T)

        'Obtenemos las propiedades de la clase generica.
        Dim fields = GetType(T).GetProperties().Where(Function(s) s.IsDefined(GetType(PropertyAttribute)))

        'Recorremos la tabla.
        For Each row As DataRow In dTable.Rows

            'Creamos una instancia de la clase generica.
            Dim objt = Activator.CreateInstance(Of T)()

            Try
                For Each FieldInfo In fields

                    For Each dColumn As DataColumn In dTable.Columns
                        Dim MyAttribute As PropertyAttribute = Nothing

                        'Verificamos si se declararon los atributos.
                        Try
                            MyAttribute = CType(Attribute.GetCustomAttribute(FieldInfo, GetType(PropertyAttribute)), PropertyAttribute)
                        Catch ex As Exception
                            Throw ex
                        End Try

                        Try
                            If MyAttribute Is Nothing Then
                                If FieldInfo.Name = dColumn.ColumnName Then
                                    Dim value = row(dColumn.ColumnName)
                                    FieldInfo.SetValue(objt, value)
                                    Exit For
                                End If
                            Else
                                If MyAttribute.DbName = dColumn.ColumnName Then
                                    Dim value = row(dColumn.ColumnName)
                                    FieldInfo.SetValue(objt, value)
                                    Exit For
                                End If
                            End If
                        Catch ex As Exception
                            Throw ex
                        End Try
                    Next
                Next
            Catch ex As Exception
                Throw ex
            End Try
            'Agregamos el registro a la lista.
            lst.Add(objt)
        Next
        Return lst
    End Function

    Public Function ListToDataSet(Of T)(ByVal collection As IEnumerable(Of T), ByVal strName As String) As DataSet
        If collection Is Nothing Then
            Throw New ArgumentNullException("source")
        End If

        If String.IsNullOrEmpty(strName) Then
            Throw New ArgumentNullException("name")
        End If

        Dim ds As New DataSet(strName)
        ds.Tables.Add(NewTable(Of T)(strName, collection))
        Return ds

    End Function

    Private Function NewTable(Of T)(ByVal strName As String, ByVal collection As IEnumerable(Of T)) As DataTable
        Dim PropertyInfo() As PropertyInfo
        Dim dTable As DataTable = Nothing

        Try
            PropertyInfo = GetType(T).GetProperties()
            dTable = Table(Of T)(strName, collection, PropertyInfo)

            Dim enumerator As IEnumerator(Of T) = collection.GetEnumerator()
            While enumerator.MoveNext()
                dTable.Rows.Add(CreateRow(Of T)(dTable.NewRow(), enumerator.Current, PropertyInfo))
            End While
        Catch ex As Exception
            Throw ex
        End Try
        Return dTable
    End Function

    Private Function CreateRow(Of T)(ByVal row As DataRow, ByVal listItem As T, ByVal pi() As PropertyInfo) As DataRow
        Dim dDataRow As DataRow = row
        Try
            For Each PropInfo As PropertyInfo In pi
                dDataRow(PropInfo.Name.ToString()) = PropInfo.GetValue(listItem, Nothing)
            Next
        Catch ex As Exception
            Throw ex
        End Try
        Return dDataRow
    End Function

    Private Function Table(Of T)(ByVal strName As String, ByVal collection As IEnumerable(Of T), ByVal pi() As PropertyInfo)
        Dim dtable As DataTable = New DataTable(strName)
        Try
            For Each PropInfo As PropertyInfo In pi
                dtable.Columns.Add(PropInfo.Name, PropInfo.PropertyType)
            Next
        Catch ex As Exception
            Throw ex
        End Try
        Return dtable
    End Function
End Class
